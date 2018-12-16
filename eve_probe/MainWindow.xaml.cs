﻿using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using EasyHook;
using System.IO;
using ScintillaNET;
using System.Drawing;
using System.Text.RegularExpressions;
using Be.Windows.Forms;
using System.Runtime.Remoting;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace eve_probe
{
    public class HookInterface : MarshalByRefObject
    {
        public void IsInstalled(Int32 InClientPID)
        {
            Log.log("Hook has been installed in target " + InClientPID + ".");
        }

        public void Enqueue(Tuple<bool, byte[], byte[]> message)
        {
            MainWindow.Queue.Enqueue(message);
        }

        public byte[] PollInjectionQueue()
        {
            byte[] pck = null;
            if (MainWindow.InjectQueue.TryDequeue(out pck))
            {
                return pck;
            }

            return null;
        }

        public void ReportException(Exception InInfo)
        {
            Log.log("-- error in target --");
            Log.log(InInfo.ToString());
            Log.log("-- /error in target --");
        }

        public void log(string txt)
        {
            Log.log(txt);
        }

        public void Ping()
        {
        }
    }

    public static class Zlib
    {
        public static byte[] ReadAllBytes(this Stream source)
        {
            var readBuffer = new byte[4096];

            int totalBytesRead = 0;
            int bytesRead;
            try
            {
                while ((bytesRead = source.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = source.ReadByte();
                        if (nextByte != -1)
                        {
                            var temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static byte[] Decompress(byte[] input)
        {
            if (input.Length < 3)
            {
                return null;
            }
            // two bytes shaved off (zlib header)
            var sourceStream = new MemoryStream(input, 2, input.Length - 2);
            var stream = new DeflateStream(sourceStream, CompressionMode.Decompress);
            if (stream == null)
            {
                return null;
            }
            return stream.ReadAllBytes();
        }
    }

    public class Packet
    {
        public int nr { get; set; }
        public string direction { get; set; }
        public string type { get; set; }
        public string method { get; set; }
        //public DateTime timestamp { get; set; }

        public byte[] rawData { set; get; }
        public byte[] cryptedData { set; get; }

        //public PyRep PyObject { set; get; }
        //public PyPacket PyPacket { set; get; }
        public string objectText { set; get; }
    }

    public partial class MainWindow : Window
    {
        private MainWindowModel viewModel;
        private int packets = 0;
        private bool appClosing = false;
        private bool pythonLoaded = false;
        public static ConcurrentQueue<Tuple<bool, byte[], byte[]>> Queue = new ConcurrentQueue<Tuple<bool, byte[], byte[]>>();
        public static ConcurrentQueue<string> EncodeQueue = new ConcurrentQueue<string>();
        public static ConcurrentQueue<byte[]> InjectQueue = new ConcurrentQueue<byte[]>();

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
            objectView.Lexer = Lexer.Python;
            objectView.StyleResetDefault();
            objectView.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            objectView.Styles[ScintillaNET.Style.Default].Weight = 900;
            objectView.Styles[ScintillaNET.Style.Default].Size = 10;
            objectView.StyleClearAll();
            objectView.Styles[ScintillaNET.Style.Python.Default].ForeColor = Color.FromArgb(0x80, 0x80, 0x80);
            objectView.Styles[ScintillaNET.Style.Python.CommentLine].ForeColor = Color.FromArgb(0x00, 0x7F, 0x00);
            objectView.Styles[ScintillaNET.Style.Python.CommentLine].Italic = true;
            objectView.Styles[ScintillaNET.Style.Python.Number].ForeColor = Color.FromArgb(0x00, 0x7F, 0x7F);
            objectView.Styles[ScintillaNET.Style.Python.String].ForeColor = Color.FromArgb(0x7F, 0x00, 0x7F);
            objectView.Styles[ScintillaNET.Style.Python.Character].ForeColor = Color.FromArgb(0x7F, 0x00, 0x7F);
            objectView.Styles[ScintillaNET.Style.Python.Word].ForeColor = Color.FromArgb(0x00, 0x00, 0x7F);
            objectView.Styles[ScintillaNET.Style.Python.Word].Bold = true;
            objectView.Styles[ScintillaNET.Style.Python.Triple].ForeColor = Color.FromArgb(0x7F, 0x00, 0x00);
            objectView.Styles[ScintillaNET.Style.Python.TripleDouble].ForeColor = Color.FromArgb(0x7F, 0x00, 0x00);
            objectView.Styles[ScintillaNET.Style.Python.ClassName].ForeColor = Color.FromArgb(0x00, 0x00, 0xFF);
            objectView.Styles[ScintillaNET.Style.Python.ClassName].Bold = true;
            objectView.Styles[ScintillaNET.Style.Python.DefName].ForeColor = Color.FromArgb(0x00, 0x7F, 0x7F);
            objectView.Styles[ScintillaNET.Style.Python.DefName].Bold = true;
            objectView.Styles[ScintillaNET.Style.Python.Operator].Bold = true;
            objectView.Styles[ScintillaNET.Style.Python.CommentBlock].ForeColor = Color.FromArgb(0x7F, 0x7F, 0x7F);
            objectView.Styles[ScintillaNET.Style.Python.CommentBlock].Italic = true;
            objectView.Styles[ScintillaNET.Style.Python.StringEol].ForeColor = Color.FromArgb(0x00, 0x00, 0x00);
            objectView.Styles[ScintillaNET.Style.Python.StringEol].BackColor = Color.FromArgb(0xE0, 0xC0, 0xE0);
            objectView.Styles[ScintillaNET.Style.Python.StringEol].FillLine = true;
            objectView.Styles[ScintillaNET.Style.Python.Word2].ForeColor = Color.FromArgb(0x40, 0x70, 0x90);
            objectView.Styles[ScintillaNET.Style.Python.Decorator].ForeColor = Color.FromArgb(0x80, 0x50, 0x00);
            injectorJSONView.Lexer = Lexer.Python;
            injectorJSONView.StyleResetDefault();
            injectorJSONView.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            injectorJSONView.Styles[ScintillaNET.Style.Default].Weight = 900;
            injectorJSONView.Styles[ScintillaNET.Style.Default].Size = 10;
            injectorJSONView.StyleClearAll();
            injectorJSONView.Styles[ScintillaNET.Style.Python.Default].ForeColor = Color.FromArgb(0x80, 0x80, 0x80);
            injectorJSONView.Styles[ScintillaNET.Style.Python.CommentLine].ForeColor = Color.FromArgb(0x00, 0x7F, 0x00);
            injectorJSONView.Styles[ScintillaNET.Style.Python.CommentLine].Italic = true;
            injectorJSONView.Styles[ScintillaNET.Style.Python.Number].ForeColor = Color.FromArgb(0x00, 0x7F, 0x7F);
            injectorJSONView.Styles[ScintillaNET.Style.Python.String].ForeColor = Color.FromArgb(0x7F, 0x00, 0x7F);
            injectorJSONView.Styles[ScintillaNET.Style.Python.Character].ForeColor = Color.FromArgb(0x7F, 0x00, 0x7F);
            injectorJSONView.Styles[ScintillaNET.Style.Python.Word].ForeColor = Color.FromArgb(0x00, 0x00, 0x7F);
            injectorJSONView.Styles[ScintillaNET.Style.Python.Word].Bold = true;
            injectorJSONView.Styles[ScintillaNET.Style.Python.Triple].ForeColor = Color.FromArgb(0x7F, 0x00, 0x00);
            injectorJSONView.Styles[ScintillaNET.Style.Python.TripleDouble].ForeColor = Color.FromArgb(0x7F, 0x00, 0x00);
            injectorJSONView.Styles[ScintillaNET.Style.Python.ClassName].ForeColor = Color.FromArgb(0x00, 0x00, 0xFF);
            injectorJSONView.Styles[ScintillaNET.Style.Python.ClassName].Bold = true;
            injectorJSONView.Styles[ScintillaNET.Style.Python.DefName].ForeColor = Color.FromArgb(0x00, 0x7F, 0x7F);
            injectorJSONView.Styles[ScintillaNET.Style.Python.DefName].Bold = true;
            injectorJSONView.Styles[ScintillaNET.Style.Python.Operator].Bold = true;
            injectorJSONView.Styles[ScintillaNET.Style.Python.CommentBlock].ForeColor = Color.FromArgb(0x7F, 0x7F, 0x7F);
            injectorJSONView.Styles[ScintillaNET.Style.Python.CommentBlock].Italic = true;
            injectorJSONView.Styles[ScintillaNET.Style.Python.StringEol].ForeColor = Color.FromArgb(0x00, 0x00, 0x00);
            injectorJSONView.Styles[ScintillaNET.Style.Python.StringEol].BackColor = Color.FromArgb(0xE0, 0xC0, 0xE0);
            injectorJSONView.Styles[ScintillaNET.Style.Python.StringEol].FillLine = true;
            injectorJSONView.Styles[ScintillaNET.Style.Python.Word2].ForeColor = Color.FromArgb(0x40, 0x70, 0x90);
            injectorJSONView.Styles[ScintillaNET.Style.Python.Decorator].ForeColor = Color.FromArgb(0x80, 0x50, 0x00);

            // allow hex free-edit @ injector
            injectorHexView.ByteProvider = new DynamicByteProvider(new byte[0]);

            // set log window
            Log.vm = viewModel;
            
            // boot data processing thread
            new Thread(SniffSniff).Start();

            Log.log("-- init complete --");
        }

        public void processRawPacket(bool outgoing, byte[] raw, byte[] crypted)
        {
            //MessageBox.Show("Got it! P " + viewModel.isPaused);

            if (!viewModel.isPaused)
            {
                var packet = new Packet()
                {
                    nr = packets++,
                    direction = outgoing ? "Out" : "In",
                    type = "Unknown",
                    method = "",
                    //timestamp = DateTime.Now,
                    objectText = "",
                    rawData = raw,
                    cryptedData = crypted,
                };

                // dump data
                //File.WriteAllBytes("dump\\" + packet.nr, packet.rawData);

                // unmarshal data
                //new Thread(() => unmarshal(packet)).Start();
                unmarshal(packet);
            }
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
            while (!pythonLoaded && !appClosing)
            {
                Thread.Sleep(500);
            }

            if (!appClosing)
            {
                Py_Initialize();
                //var gil = PyGILState_Ensure();
                var marshler = File.ReadAllText("marshal.py");
                PyRun_SimpleString(marshler);
                //PyGILState_Release(gil);
            }

            while (!appClosing)
            {
                Tuple<bool, byte[], byte[]> item = null;
                while (Queue.TryDequeue(out item))
                {
                    processRawPacket(item.Item1, item.Item2, item.Item3);
                }

                string py_obj_str = null;
                if (EncodeQueue.TryDequeue(out py_obj_str))
                {
                    marshal(py_obj_str);
                }

                Thread.Sleep(500);
            }
        }

        private byte[] marshal(string objTxt)
        {
            if (objTxt.Length > 0 && pythonLoaded)
            {
                //var gil = PyGILState_Ensure();
                // Remarshal
                var main = PyImport_AddModule("__main__");
                var save = PyObject_GetAttrString(main, "save");
                //var bText = objTxt.ToCharArray();
                //var uText = Marshal.AllocHGlobal(bText.Length);
                //Marshal.Copy(bText, 0, uText, bText.Length);
                var data = PyObject_CallFunction(save, "s#", __arglist(Marshal.StringToHGlobalAnsi(objTxt), objTxt.Length));
                //Marshal.FreeHGlobal(uText);

                if (main == IntPtr.Zero) MessageBox.Show("main null");
                if (save == IntPtr.Zero) MessageBox.Show("save null");
                if (data == IntPtr.Zero) MessageBox.Show("data null");

                //var uData = PyString_AsString(data);
                //var size = PyString_Size(data);

                //if (uData == IntPtr.Zero) MessageBox.Show("uData null");
                //if (size == -1) MessageBox.Show("size -1");

                //*
                var uData = PyString_AsString(data);
                var size = PyString_Size(data);
                var encoded = new byte[size];
                Marshal.Copy(uData, encoded, 0, size);

                inMain(() =>
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var b = new BinaryWriter(ms))
                        {
                            b.Write(encoded);
                            injectorHexView.ByteProvider = new DynamicByteProvider(ms.ToArray());
                        }
                    }
                });
                //*/
                //PyGILState_Release(gil);
            }

            return null;
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

            if (data[0] != HeaderByte)
            {
                if (data[0] == PythonMarker)
                    packet.objectText = "Python file";
            }
            else
            {
                if (pythonLoaded)
                {
                    // Demarshal
                    //var gil = PyGILState_Ensure();
                    var main = PyImport_AddModule("__main__");
                    var load = PyObject_GetAttrString(main, "load");
                    IntPtr uData = Marshal.AllocHGlobal(data.Length);
                    Marshal.Copy(data, 0, uData, data.Length);
                    var text = PyObject_CallFunction(load, "s#", __arglist(uData, data.Length));
                    Marshal.FreeHGlobal(uData);
                    packet.objectText = Marshal.PtrToStringAnsi(PyString_AsString(text));
                    //PyGILState_Release(gil);
                }
            }

            // send to UI
            if (!appClosing)
                inMain(() => viewModel.packets.Add(packet));
        }

        // cleanup before close
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            appClosing = true;
        }

        // click packet
        private void packetList_SelectionChanged(object sender, EventArgs e)
        {
            if (packetList.SelectedItem != null)
            {
                var packet = (Packet)packetList.SelectedItem;

                // go Hex! go!
                if(packet.rawData != null)
                    rawHexView.ByteProvider = new DynamicByteProvider(packet.rawData);
                if (packet.cryptedData != null)
                    cryptedHexView.ByteProvider = new DynamicByteProvider(packet.cryptedData);

                // what the hacks scyntilla?
                objectView.ReadOnly = false;
                objectView.Text = packet.objectText;
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

                injectorHexView.ByteProvider = null;
                injectorJSONView.Text = objectView.Text;
                tabControl.SelectedIndex = 1;
            }
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            viewModel.isPaused = !viewModel.isPaused;
            viewModel.pauseText = viewModel.isPaused ? "Unpause" : "Pause";
        }

        private void log_TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null)
            {
                return;
            }

            // autoscroll
            tb.SelectionStart = int.MaxValue;
            tb.SelectionLength = 0;
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
            EncodeQueue.Enqueue(injectorJSONView.Text);
        }

        private void inject_Click(object sender, RoutedEventArgs e)
        {
            InjectQueue.Enqueue(((DynamicByteProvider)injectorHexView.ByteProvider).Bytes.ToArray());
        }

        private void dump_Click(object sender, RoutedEventArgs e)
        {
            if (packetList.SelectedItem != null)
            {
                var packet = (Packet)packetList.SelectedItem;
                File.WriteAllBytes("dump.bin", packet.rawData);
            }
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

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("python27", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static extern void Py_Initialize();

        [DllImport("python27", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static extern Int32 PyRun_SimpleString([MarshalAs(UnmanagedType.LPStr)]string str);

        [DllImport("python27", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr PyImport_AddModule([MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport("python27", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr PyObject_GetAttrString(IntPtr module, [MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport("python27", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr PyObject_CallFunction(IntPtr callable, [MarshalAs(UnmanagedType.LPStr)]string format, __arglist);

        [DllImport("python27", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr PyString_AsString(IntPtr pyStr);

        [DllImport("python27", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static extern int PyString_Size(IntPtr pyStr);

        //[DllImport("python27", CallingConvention = CallingConvention.Cdecl)]
        //static extern IntPtr PyGILState_Ensure();

        //[DllImport("python27", CallingConvention = CallingConvention.Cdecl)]
        //static extern void PyGILState_Release(IntPtr gstate);

        private void hook_Click(object sender, RoutedEventArgs e)
        {
            String ChannelName = null;
            Int32 TargetPID = 0;

            Process[] procs = Process.GetProcessesByName("exefile");
            if (procs.Length == 0)
            {
                MessageBox.Show("Could not find EVE (exefile.exe)");
                return;
            }

            try
            {
                TargetPID = procs[0].Id;
                RemoteHooking.IpcCreateServer<HookInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);
                string injectionLibrary = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "HookInject.dll");

                RemoteHooking.Inject(
                    TargetPID,
                    injectionLibrary,
                    injectionLibrary,
                    ChannelName);

                //MessageBox.Show("Injected to process " + TargetPID);

                // SharedCache\tq
                var path = Path.GetDirectoryName(Path.GetDirectoryName(procs[0].MainModule.FileName));
                Environment.SetEnvironmentVariable("PYTHONPATH", path + "\\code.ccp;" + path + "\\bin");

                if (LoadLibrary(path + "\\bin\\python27.dll") != IntPtr.Zero)
                {
                    pythonLoaded = true;
                }
                else
                {
                    MessageBox.Show("Failed to load " + path + "\\bin\\python27.dll");
                }
            }
            catch (Exception ExtInfo)
            {
                MessageBox.Show("There was an error while connecting to target:\r\n" + ExtInfo.ToString());
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
