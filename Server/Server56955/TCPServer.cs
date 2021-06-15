using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Bindings;

namespace Server56955
{
    class TCPServer
    {
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly byte[] _buffer = new byte[1024];

        public static Client[] _clients = new Client[Constants.MAX_PLAYERS];

        //Accessors
        public static Socket _Socket { get { return _serverSocket; } }


        //initialise server
        public static void SetupServer()
        {
            //populate server client array with empty clients
            for(int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                _clients[i] = new Client();
            }
            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(Constants.IP_ADDRESS), Constants.PORT));
            _serverSocket.Listen(0);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        //Called when we have a pending connection
        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);                                //Client connected to server
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);         //Allow other clients to connect
            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                // check if slot in array is open to add new player
                if(_clients[i].socket == null)
                {
                    //add player to slot
                    _clients[i].socket = socket;
                    _clients[i].index = i;
                    _clients[i].ip = socket.RemoteEndPoint.ToString();
                    _clients[i].TotalPoints = 0;
                    _clients[i].StartClient();
                    Console.WriteLine("Connection from '{0}' received", _clients[i].ip);
                    SendConnectionOK(i);
                    return;
                }
            }
        }

        //Method used to avoid data packet loss during client server communication by
        //adding a prefix to data sent specifying length of bytes in packet. Server must
        //wait until packet data fully received and decline other packets from other clients until reception complete
        public static void SendDataTo(int index, byte[]data)
        {
            byte[] sizeinfo = new byte[4];
            sizeinfo[0] = (byte)data.Length;
            sizeinfo[1] = (byte)(data.Length >> 8);
            sizeinfo[2] = (byte)(data.Length >> 16);
            sizeinfo[3] = (byte)(data.Length >> 24);

            _clients[index].socket.Send(sizeinfo);
            _clients[index].socket.Send(data);
        }

        //Method used to acknowledge to client that player has been accepted on server
        public static void SendConnectionOK(int index)
        {
            PacketBuffer buffer = new PacketBuffer();
            string cindex = index.ToString();
            //Give packet a number so client knows what code to execute when package received
            buffer.WriteInteger((int)ServerPackets.SConnectionOK);
            buffer.WriteString("Client: "+cindex);
            SendDataTo(index, buffer.ToArray());
            buffer.Dispose();

        }
    }

    class Client
    {
        public int index;
        public string ip;
        public Socket socket;
        public bool closing = false;
        private byte[] _buffer = new byte[1024];

        private int totalPoints = 0;

        //Accessors
        public int TotalPoints { get { return totalPoints; } set { totalPoints = value; } }

        public void StartClient()
        {
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            closing = false;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;

            //Avoid server crash when client disconnects from server
            try
            {
                int received = socket.EndReceive(ar);

                if(received <= 0)
                {
                    CloseClient(index);
                }
                else
                {
                    byte[] databuffer = new byte[received];
                    Array.Copy(_buffer, databuffer, received);

                    //Handle Network information
                    ServerHandleNetworkData.HandleNetworkInformation(index, databuffer);
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                }
            }
            catch
            {
                CloseClient(index);
            }
        }

        public void CloseClient(int index)
        {
            closing = true;
            Console.WriteLine("Connection from {0} has been terminated.", ip);

            //close socket when player exits game          
            socket.Close();
            socket.Dispose();
        }

    }
}
