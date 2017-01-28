using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using LowLevelDesign.Hexify;
using eveMarshal;
using System.Linq;
using System.IO;
using ScintillaNET;
using System.Drawing;
using System.Text.RegularExpressions;

namespace eve_probe
{
    public class Packet
    {
        public int nr { get; set; }
        public string direction { get; set; }
        public string type { get; set; }
        public DateTime timestamp { get; set; }

        public byte[] rawData { set; get; }
        public byte[] cryptedData { set; get; }

        public PyRep PyObject { set; get; }
        public string objectText { set; get; }
    }

    public partial class MainWindow : Window
    {
        private MainWindowModel viewModel;
        private int packets = 0;
        private bool appClosing = false;
        private Socket client;

        public const byte HeaderByte = 0x7E;
        // not a real magic since zlib just doesn't include one..
        public const byte ZlibMarker = 0x78;
        public const byte PythonMarker = 0x03;

        public MainWindow()
        {
            // bind stuff
            viewModel = new MainWindowModel()
            {
                packets = new ObservableCollection<Packet>(),
            };

            // boom?
            DataContext = viewModel;
            InitializeComponent();

            // scintilla
            objectView.Lexer = Lexer.Json;
            objectView.StyleResetDefault();
            objectView.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            objectView.Styles[ScintillaNET.Style.Default].Weight = 900;
            objectView.Styles[ScintillaNET.Style.Default].Size = 10;
            objectView.StyleClearAll();
            objectView.Styles[ScintillaNET.Style.Json.Number].ForeColor = ColorTranslator.FromHtml("#204070");
            objectView.Styles[ScintillaNET.Style.Json.Operator].ForeColor = ColorTranslator.FromHtml("#006633");
            objectView.Styles[ScintillaNET.Style.Json.PropertyName].ForeColor = ColorTranslator.FromHtml("#007050");
            objectView.Styles[ScintillaNET.Style.Json.String].ForeColor = ColorTranslator.FromHtml("#0050b0");
            injectorJSONView.Lexer = Lexer.Json;
            injectorJSONView.StyleResetDefault();
            injectorJSONView.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            injectorJSONView.Styles[ScintillaNET.Style.Default].Weight = 900;
            injectorJSONView.Styles[ScintillaNET.Style.Default].Size = 10;
            injectorJSONView.StyleClearAll();
            injectorJSONView.Styles[ScintillaNET.Style.Json.Number].ForeColor = ColorTranslator.FromHtml("#204070");
            injectorJSONView.Styles[ScintillaNET.Style.Json.Operator].ForeColor = ColorTranslator.FromHtml("#006633");
            injectorJSONView.Styles[ScintillaNET.Style.Json.PropertyName].ForeColor = ColorTranslator.FromHtml("#007050");
            injectorJSONView.Styles[ScintillaNET.Style.Json.String].ForeColor = ColorTranslator.FromHtml("#0050b0");

            new Thread(SniffSniff).Start();
        }

        // do stuff in UI thread
        public void inMain(Action stuff)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                stuff
            );
        }

        // read from advapi captures
        private void SniffSniff()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[4096];

            while (!appClosing)
            {
                // Connect to a remote device.
                try
                {
                    // Establish the remote endpoint for the socket.
                    IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 27010);

                    // Create a TCP/IP  socket.
                    client = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);

                    // Connect the socket to the remote endpoint. Catch any errors.
                    int bytesRec = 0;
                    client.Connect(remoteEP);

                    if (client.Connected)
                    {
                        do
                        {
                            // Receive the response from the remote device.
                            bytesRec = 0;
                            bytesRec = client.Receive(bytes);

                            // check for "header"
                            if (bytesRec > 0 && (bytes[0] == 'e' || bytes[0] == 'd') && !viewModel.isPaused)
                            {
                                var outgoing = bytes[0] == 'e';

                                var packet = new Packet()
                                {
                                    nr = packets++,
                                    direction = outgoing ? "Out" : "In",
                                    type = "Unknown",
                                    timestamp = DateTime.Now,
                                    objectText = "",
                                };

                                // unpack first part of data
                                var data_start = 5;
                                var data_length = BitConverter.ToInt32(bytes, 1);
                                
                                if (data_length > 0 && data_start + data_length <= bytes.Length)
                                {
                                    if (outgoing)
                                    {
                                        packet.rawData = new byte[data_length];
                                        Array.Copy(bytes, data_start, packet.rawData, 0, data_length);
                                    }
                                    else
                                    {
                                        packet.cryptedData = new byte[data_length];
                                        Array.Copy(bytes, data_start, packet.cryptedData, 0, data_length);
                                    }
                                }

                                // unpack second part of data
                                data_start += data_length + 4;
                                data_length = BitConverter.ToInt32(bytes, data_start - 4);

                                if (data_length > 0 && data_start + data_length <= bytes.Length)
                                {
                                    if (outgoing)
                                    {
                                        packet.cryptedData = new byte[data_length];
                                        Array.Copy(bytes, data_start, packet.cryptedData, 0, data_length);
                                    }
                                    else
                                    {
                                        packet.rawData = new byte[data_length];
                                        Array.Copy(bytes, data_start, packet.rawData, 0, data_length);
                                    }
                                }

                                // unmarshal data
                                new Thread(() => unmarshal(packet)).Start();
                            }
                        }
                        while (bytesRec > 0);
                    }

                    // something went wrong - cleanup socket, try again
                    if (client.Connected)
                        client.Close();
                }
                catch { }

                Thread.Sleep(500);
            }
        }

        // Decode EVE's packets
        private void unmarshal(Packet packet)
        {
            var data = packet.rawData;

            if (data == null || data.Length == 0)
            {
                // send to UI
                if (!appClosing)
                    inMain(() => viewModel.packets.Add(packet));
                return;
            }

            // Is this a compressed file?
            if (data[0] == ZlibMarker)
            {
                // Yes, decompress it.
                try
                {
                    data = Zlib.Decompress(data);
                }
                catch
                {
                    packet.objectText = "Zlib decompression failed";
                }

                if (data == null)
                {
                    // Decompress failed.
                    packet.objectText = "Zlib decompression failed";
                }
                else
                {
                    // Update raw view
                    packet.rawData = data;
                }
            }

            // Is this a proper python serial stream?
            if (data[0] != HeaderByte)
            {
                // No, is this a python file?
                // I have no idea what to do with it
                if (data[0] == PythonMarker)
                    packet.objectText = "Python file";
                else if(data[0] != ZlibMarker)
                    packet.objectText = "Unknown data type";
            }
            else
            {
                try
                {
                    Unmarshal un = new Unmarshal();
                    PyRep obj = un.Process(data);

                    // Attempt cast to PyPacket
                    //*
                    if (obj.Type == PyObjectType.ObjectData)
                    {
                        PyObject packetData = obj as PyObject;
                        try
                        {
                            obj = new PyPacket(packetData);
                        }
                        catch { }
                    }
                    //*/

                    packet.PyObject = obj;
                    //packet.objectText = PrettyPrinter.Print(obj);
                    packet.objectText = JSON_PrettyPrinter.Process(obj.dumpJSON());
                }
                catch (Exception e)
                {
                    packet.objectText = "Error while decoding\r\n\r\n" + e.Message;
                }
                
            }

            // send to UI
            if (!appClosing)
                inMain(() => viewModel.packets.Add(packet));
        }

        // cleanup before close
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Kill the socket, the thread should exit soon
            appClosing = true;
            client.Close();
        }

        // click packet
        private void packetList_SelectionChanged(object sender, EventArgs e)
        {
            if (packetList.SelectedItem != null)
            {
                var packet = (Packet)packetList.SelectedItem;

                viewModel.rawHex = (packet.rawData == null) ? "" : Hex.PrettyPrint(packet.rawData);
                viewModel.cryptedHex = (packet.cryptedData == null) ? "" : Hex.PrettyPrint(packet.cryptedData);

                // what the hacks scyntilla?
                objectView.ReadOnly = false;
                objectView.Text = packet.objectText;
                objectView.Colorize(0, objectView.TextLength);
                objectView.ReadOnly = true;

                //viewModel.copyEnabled = packet.direction == "Out";
                viewModel.copyEnabled = true;
                tabControl.SelectedIndex = 0;
            }
        }

        private void toInjector_Click(object sender, RoutedEventArgs e)
        {
            if (packetList.SelectedItem != null)
            {
                var packet = (Packet)packetList.SelectedItem;

                using (var ms = new MemoryStream())
                {
                    using (var b = new BinaryWriter(ms))
                    {
                        //packet.PyObject.Encode(b);
                        //viewModel.injectorHex = Hex.PrettyPrint(ms.ToArray());
                        injectorJSONView.Text = objectView.Text;
                        tabControl.SelectedIndex = 1;
                    }
                }
            }
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            viewModel.isPaused = !viewModel.isPaused;
            viewModel.pauseText = viewModel.isPaused ? "Unpause" : "Pause";
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = viewModel.packets.Count - 1; i >= 0; i--)
            {
                viewModel.packets.RemoveAt(i);
            }
        }

        private void encode_Click(object sender, RoutedEventArgs e)
        {
            /*
            var js = new JavaScriptSerializer();
            var obj = js.Deserialize<PyRep>(viewModel.injectorJSON);

            using (var ms = new MemoryStream())
            {
                using (var b = new BinaryWriter(ms))
                {
                    obj.Encode(b);
                    viewModel.injectorHex = Hex.PrettyPrint(ms.ToArray());
                    injectorTabs.SelectedIndex = 1;
                }
            }
            */
        }


        // -------------------------------------------------------------------------------------
        // Auto-tab
        // https://github.com/jacobslusser/ScintillaNET/issues/35
        private void injectorJSONView_CharAdded(object sender, CharAddedEventArgs e)
        {
            //The '}' char.
            if (e.Char == 125)
            {
                int curLine = injectorJSONView.LineFromPosition(injectorJSONView.CurrentPosition);

                if (injectorJSONView.Lines[curLine].Text.Trim() == "}")
                { //Check whether the bracket is the only thing on the line.. For cases like "if() { }".
                    SetIndent(injectorJSONView, curLine, GetIndent(injectorJSONView, curLine) - 4);
                }
            }
        }

        private void injectorJSONView_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            if ((e.Text.EndsWith("\n") || e.Text.EndsWith("\r")))
            {
                int startPos = injectorJSONView.Lines[injectorJSONView.LineFromPosition(injectorJSONView.CurrentPosition)].Position;
                int endPos = e.Position;
                string curLineText = injectorJSONView.GetTextRange(startPos, (endPos - startPos)); //Text until the caret so that the whitespace is always equal in every line.

                Match indent = Regex.Match(curLineText, "^[ \\t]*");
                e.Text = (e.Text + indent.Value);
                if (Regex.IsMatch(curLineText, "{\\s*$"))
                {
                    e.Text = (e.Text + "\t");
                }
            }
        }

        //Codes for the handling the Indention of the lines.
        //They are manually added here until they get officially added to the Scintilla control.
        #region "CodeIndent Handlers"
        const int SCI_SETLINEINDENTATION = 2126;
        const int SCI_GETLINEINDENTATION = 2127;
        private void SetIndent(ScintillaNET.Scintilla scin, int line, int indent)
        {
            scin.DirectMessage(SCI_SETLINEINDENTATION, new IntPtr(line), new IntPtr(indent));
        }
        private int GetIndent(ScintillaNET.Scintilla scin, int line)
        {
            return (scin.DirectMessage(SCI_GETLINEINDENTATION, new IntPtr(line), new IntPtr())).ToInt32();
        }
        #endregion
    }
}
