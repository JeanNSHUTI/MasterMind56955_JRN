using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bindings;

namespace MasterMind56955_JRN
{
    public class ClientHandleNetworkData
    {
        public delegate void ReceiveDataFromServerHandler(string message);

        //public event ReceiveDataFromServerHandler serverPrintOut;

        //Listen to network packets at connection
        //private int index;
        private delegate void Packet_(byte[] data, TCPClient client);
        private Dictionary<int, Packet_> Packets;

        public ClientHandleNetworkData()
        {
            InitNetworkPackages();
        }

        public void InitNetworkPackages()
        {
            //Initialise dictionary that links key in packet to a specific handle method
            Packets = new Dictionary<int, Packet_>
            {
                {(int)ServerPackets.SConnectionOK, HandleConnectionOK}
            };
            
        }

        public void HandleNetworkInformation(byte[] data, TCPClient client)
        {
            int packetnum;
            PacketBuffer buffer = new PacketBuffer();
            //Copy data received to buffer
            buffer.WriteBytes(data);
            packetnum = buffer.ReadInteger();
            buffer.Dispose();
            //Test if a handle method has been defined to the key read in packet,
            //if so, invoke corresponding method
            if(Packets.TryGetValue(packetnum, out Packet_ Packet))
            {
                Packet.Invoke(data, client);
            }
        }

        private void HandleConnectionOK(byte[] data, TCPClient client)
        {
            //Instatiate delagate that will fire off method used to print data
            //OnDataReceived fires off an event for the form used by client
            ReceiveDataFromServerHandler receiveDFS = client.OnDataReceived;
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();

            receiveDFS.Invoke(msg);

        }
    }
}
