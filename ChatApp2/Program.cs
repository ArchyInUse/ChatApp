using System;

namespace ServerSide
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
