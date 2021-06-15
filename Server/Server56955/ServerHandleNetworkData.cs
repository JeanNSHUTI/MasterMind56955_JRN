using System;
using System.Collections.Generic;
using System.Text;
using Bindings;

namespace Server56955
{
    class ServerHandleNetworkData
    {

        //Listen to network packets at connection
        private delegate void Packet_(int index, byte[] data);
        private static Dictionary<int, Packet_> Packets;

        public static void InitNetworkPackages()
        {
            Packets = new Dictionary<int, Packet_>
            {
                {(int)ClientPackets.CAcknowledge, HandleGameOver}
            };

        }

        public static void HandleNetworkInformation(int index, byte[] data)
        {
            int packetnum;
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            packetnum = buffer.ReadInteger(true);
            buffer.Dispose();
            if (Packets.TryGetValue(packetnum, out Packet_ Packet))
            {
                Packet.Invoke(index, data);
            }
        }

        private static void HandleGameOver(int index, byte[] data)
        {
            //throw new NotImplementedException();
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            
            buffer.Dispose();

            Console.WriteLine(msg + TCPServer._clients[index].ip);
        }
    }
}
