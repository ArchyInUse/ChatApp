using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg = Console.ReadLine() + "<EOT>";

            int port = 60000;
            IPAddress addr = IPAddress.Parse("89.139.220.217");

            Socket s = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            byte[] Bytes = new byte[1024];

            try
            {
                s.Connect(new IPEndPoint(addr, port));

                Console.WriteLine($"Socket connected to {s.RemoteEndPoint.ToString()}");

                byte[] EncodedMsg = Encoding.ASCII.GetBytes(msg);

                int bytesSent = s.Send(EncodedMsg);

                int bytesRec = s.Receive(Bytes);

                Console.WriteLine($"Recieved:{Encoding.ASCII.GetString(Bytes, 0, bytesRec)}");
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();

                Console.WriteLine("Resetting...");
            }

            Console.ReadLine();
        }
    }
}
