using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    //Key-Packet declarations
    //Key in packet defining format of data from server to client
    //Client has to listen to server packets
    public enum ServerPackets
    {
        SConnectionOK = 1,
    }


    //Get send from client to server
    //Server has to listen to client packets
    public enum ClientPackets
    {
        CAcknowledge = 1,
    }
}
