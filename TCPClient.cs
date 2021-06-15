using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Bindings;

namespace MasterMind56955_JRN
{
    public class TCPClient
    {
        public delegate void ClientConnectedHandler(TCPClient client);
        public delegate void ConnectionRefusedHandler(TCPClient client, string message);
        public delegate void ClientDisconnectedHandler(TCPClient client, string message);
        public delegate void DataReceivedHandler(string msg);


        public event ClientConnectedHandler ClientConnected;
        public event ConnectionRefusedHandler ConnectionRefused;
        public event ClientDisconnectedHandler ClientDisconnected;
        public event DataReceivedHandler DataReceived;

        private ClientHandleNetworkData clientNetworkHandle = new ClientHandleNetworkData();
        private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int totalPoints = 0;
        private bool connected = false;


        //Accessors
        public Socket ClientSocket { get { return _clientSocket; } set { _clientSocket = value; } }
        public int TotalPoints { get { return totalPoints; } set { totalPoints = value; } }
        public bool Connected { get { return connected; } set { connected = value; } }
        public ClientHandleNetworkData ClientNetworkHandle { get { return clientNetworkHandle; } set { clientNetworkHandle = value; } }


        //Methods
        public void ConnectToServer()
        {
            if (!_clientSocket.Connected)
            {
                _clientSocket.BeginConnect(Constants.IP_ADDRESS, Constants.PORT, new AsyncCallback(ConnectCallback), _clientSocket);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _clientSocket.EndConnect(ar);
            }
            catch (SocketException e)
            {
                onConnectionRefused(e.Message);
            }
            if(_clientSocket.Connected)
            {
                //onClientConnected(this);                        //event - delegate method to print to form
                while (_clientSocket.Connected)
                {
                    OnReceive();
                    //onClientConnected(this);                        //event - delegate method to print to form
                }
                //onClientConnected(this);                        //event - delegate method to print to form
            }
        }

        private void OnReceive()
        {
            byte[] _sizeinfo = new byte[4];
            byte[] _receivedbuffer = new byte[1024];

            int totalread = 0, currentread = 0;

            try
            {
                currentread = totalread = _clientSocket.Receive(_sizeinfo);
                 //If disconnected from server
                if(totalread <= 0)
                {
                    onClientDisconnected("Connection Refused: Lost connection to the server.");           
                }
                else
                {
                    // Still waiting to receive total package
                    while(totalread < _sizeinfo.Length && currentread > 0)
                    {
                        currentread = _clientSocket.Receive(_sizeinfo, totalread, _sizeinfo.Length - totalread, SocketFlags.None);
                        totalread += currentread;
                    }

                    int messageSize = 0;
                    messageSize |= _sizeinfo[0];
                    messageSize |= (_sizeinfo[1] << 8);
                    messageSize |= (_sizeinfo[2] << 16);
                    messageSize |= (_sizeinfo[3] << 24);

                    byte[] data = new byte[messageSize];

                    totalread = 0;
                    currentread = totalread = _clientSocket.Receive(data, totalread, data.Length - totalread, SocketFlags.None);

                    //In order to not lose the package
                    while(totalread < messageSize && currentread > 0)
                    {
                        currentread = _clientSocket.Receive(data, totalread, data.Length - totalread, SocketFlags.None);
                        totalread += currentread;
                    }


                    //Handle Network information
                    //ClientHandleNetworkData.HandleNetworkInformation(data);
                    clientNetworkHandle.HandleNetworkInformation(data, this);
                }
            }
            catch
            {
                onClientDisconnected("Error Receiving data: Lost connection to the server.");
            }
        }

        public void SendData(byte[] data)
        {
            //_clientSocket.Send(data);
            try
            {
                _clientSocket.Send(data);
            }
            catch (Exception e)
            {
                if (!_clientSocket.Connected)
                {
                    onClientDisconnected(e.Message);
                }
            }
            
        }

        public void Disconnect()
        {
            if (ClientSocket.Connected)
            {
                _clientSocket.Close();
            }
        }

        public void AcknowledgeServer()
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int)ClientPackets.CAcknowledge);
            buffer.WriteString("ACK");
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        #region Fire off event handlers
        public void onClientConnected(TCPClient asyncClient)
        {
            if (ClientConnected != null)
            {
                if (ClientConnected.Target is System.Windows.Forms.Control)
                {
                    ((System.Windows.Forms.Control)ClientConnected.Target).Invoke(ClientConnected, this);
                }
                else
                {
                    ClientConnected(this);
                }
            }
        }
        public void onConnectionRefused(string message)
        {
            if (ConnectionRefused.Target is System.Windows.Forms.Control)
            {
                ((System.Windows.Forms.Control)ConnectionRefused.Target).Invoke(ConnectionRefused, this, message);
            }
            else
            {
                ConnectionRefused(this, message);
            }
        }
        private void onClientDisconnected(string message)
        {
            if (ClientDisconnected != null)
            {
                if (ClientDisconnected.Target is System.Windows.Forms.Control)
                {
                    ((System.Windows.Forms.Control)ClientDisconnected.Target).Invoke(ClientDisconnected, this, message);
                }
                else
                {
                    ClientDisconnected(this, message);
                }
            }
        }
        public void OnDataReceived(string message)
        {
            if (DataReceived != null)
            {
                if (DataReceived.Target is System.Windows.Forms.Control)
                {
                    ((System.Windows.Forms.Control)ClientDisconnected.Target).Invoke(DataReceived, message);
                }
                else
                {
                    //Connected = true;
                    DataReceived(message);
                    //onClientConnected(this);                        //event - delegate method to print to form
                }
            }
        }
        #endregion

        public void AddBonus(int level)
        {
            switch (level)
            {
                case 1:
                    this.TotalPoints += 100;
                    break;
                case 2:
                    this.TotalPoints += 60;
                    break;
                case 3:
                    this.TotalPoints += 45;
                    break;
                case 4:
                    this.TotalPoints += 30;
                    break;
                case 5:
                    this.TotalPoints += 20;
                    break;
                case 6:
                    this.TotalPoints += 10;
                    break;
                default:
                    this.TotalPoints += 0;
                    break;
            }
        }

        public void SendPoints(bool win)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int)ClientPackets.CAcknowledge);
            if (win)
            {
                //buffer.WriteString("Game won");
                buffer.WriteString("Game won ! With " + this.TotalPoints + "points. Client: ");
                SendData(buffer.ToArray());
                buffer.Dispose();
            }
            else
            {
                //buffer.WriteString("Game won");
                buffer.WriteString("Game lost ! By Client: ");
                SendData(buffer.ToArray());
                buffer.Dispose();
            }
        }

    }
}
