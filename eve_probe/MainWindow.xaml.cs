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
                            if (bytesRec > 0 && (bytes[0] == 'e' || bytes[0] == 'd'))
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
                    /* // TODO: implement PyPacket encoder
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
                    packet.objectText = PrettyPrinter.Print(obj);
                }
                catch
                {
                    packet.objectText = "Error while decoding";
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
                viewModel.objectText = packet.objectText;
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
                        packet.PyObject.Encode(b);
                        viewModel.injectorHex = Hex.PrettyPrint(ms.ToArray());
                    }
                }
            }
        }
    }
}
