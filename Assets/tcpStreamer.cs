using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


namespace gamingCloud.Network.tcp
{
    public class tcpStreamer
    {
        #region private members 	
        private TcpClient socketConnection;
        private Thread clientReceiveThread;
        private int port;
        private string address;
        private bool isConnected;
        #endregion

        #region Listeners
        public event Action<string> OnPacketRecieve;
        public event Action OnConnected;
        ulong packetSize = 1024;
        #endregion


        private void initialize(string address, int port, ulong packetSize)
        {
            this.address = address;
            this.port = port;
            this.packetSize = packetSize;

            try
            {
                clientReceiveThread = new Thread(ListeningOnServerMessages) { IsBackground = true };
                clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                Debug.Log("On client connect exception " + e);
            }
        }

        public tcpStreamer(string address, int port)
        {
            initialize(address, port, packetSize);
        }

        public tcpStreamer(string address, int port, ulong packetSize)
        {
            initialize(address, port, packetSize);
        }


        private async void ListeningOnServerMessages()
        {
            var stream = socketConnection.GetStream();
            int i;
            byte[] bytes = new byte[] { };
            string recvData = String.Empty;
            try
            {
                Console.WriteLine("New Client.");
                while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                {
                    string currentParse = Encoding.UTF8.GetString(bytes, 0, i);
                    if (currentParse.Contains("<EOF>"))
                    {
                        string[] split = currentParse.Split(new string[]{ "<EOF>" }, StringSplitOptions.None);
                        recvData += split[0];

                        // packet is ok
                        OnPacketRecieve(recvData);

                        if (split.Length > 1 && split[1].Trim().Length > 0)
                            recvData = split[1];
                        else
                            recvData = String.Empty;
                    }
                    else recvData += currentParse;
                }

            }
            catch
            {
                // Client Disconnected
                socketConnection.Close();
            }
        }


        public void sendPacket(string message)
        {
            if (socketConnection == null)
                return;

            try
            {

                message = message + "<EOF>";

                // Get a stream object for writing. 			
                NetworkStream stream = socketConnection.GetStream();
                if (stream.CanWrite)
                {
                    // Convert string message to byte array.                 
                    byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);

                    // Write byte array to socketConnection stream.                 
                    stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }


        public void Disconnect()
        {
            clientReceiveThread.Abort();
            this.socketConnection = null;
        }

    }
}