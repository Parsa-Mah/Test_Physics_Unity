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
        ulong packetSize = 1024 * 1024 * 100;
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


        private void ListeningOnServerMessages()
        {
            try
            {

                socketConnection = new TcpClient();
                socketConnection.Connect(address, port);
                Byte[] bytes = new Byte[packetSize];
                while (true)
                {

                    if (!socketConnection.Client.Connected)
                        continue;
                    else if (!isConnected)
                    {
                        isConnected = true;
                        if (OnConnected != null)
                            OnConnected();
                    }

                    // Get a stream object for reading 				
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);

                            // Convert byte array to string message. 						
                            string serverMessage = Encoding.ASCII.GetString(incommingData);
                            if (OnPacketRecieve != null)
                                OnPacketRecieve(serverMessage);
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }


        public void sendPacket(string message)
        {
            if (socketConnection == null)
                return;

            try
            {
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