using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using LowLevelDesign.Hexify;

namespace eve_probe
{
    public class Packet
    {
        public int nr { get; set; }
        public string direction { get; set; }
        public string type { get; set; }
        public DateTime timestamp { get; set; }

        public string rawData { set; get; }
        public string cryptedData { set; get; }
    }

    public partial class MainWindow : Window
    {
        private MainWindowModel viewModel;
        private int packets = 0;
        private bool appClosing = false;
        private Socket client;

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
                            var dec = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                            // check for "header"
                            if (bytesRec > 0 && (dec[0] == 'e' || dec[0] == 'd'))
                            {
                                var outgoing = dec[0] == 'e';

                                var packet = new Packet()
                                {
                                    nr = packets++,
                                    direction = outgoing ? "Out" : "In",
                                    type = "Unknown",
                                    timestamp = DateTime.Now,
                                    rawData = "",
                                    cryptedData = "",
                                };

                                // unpack first part of data
                                var data_start = 5;
                                var data_length = BitConverter.ToInt32(bytes, 1);
                                
                                if (data_length > 0)
                                {
                                    if (outgoing)
                                        packet.rawData = dec.Substring(data_start, data_length);
                                    else
                                        packet.cryptedData = dec.Substring(data_start, data_length);
                                }

                                // unpack second part of data
                                data_start += data_length + 4;
                                data_length = BitConverter.ToInt32(bytes, data_start - 4);

                                if (data_length > 0)
                                {
                                    if (outgoing)
                                        packet.cryptedData = dec.Substring(data_start, data_length);
                                    else
                                        packet.rawData = dec.Substring(data_start, data_length);
                                }

                                // send to UI
                                if (!appClosing)
                                    inMain(() => viewModel.packets.Add(packet));
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

                viewModel.rawHex = Hex.PrettyPrint(Encoding.ASCII.GetBytes(packet.rawData));
                viewModel.cryptedHex = Hex.PrettyPrint(Encoding.ASCII.GetBytes(packet.cryptedData));
            }
        }
    }
}
