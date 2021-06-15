using System;

namespace Server56955
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerHandleNetworkData.InitNetworkPackages();
            TCPServer.SetupServer();
            Console.WriteLine("Welcome to Mastermind Server");
            Console.WriteLine("Server listening on : " + TCPServer._Socket.LocalEndPoint.ToString());
            Console.ReadLine();
        }
    }
}
