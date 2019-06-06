using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Server...");

            ServerWrapper server = new ServerWrapper();

            server.Start();

            Console.Read();
        }
    }
}
