using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

namespace Server
{
    /// <summary>
    /// Add Nick maybe?
    /// </summary>
    class ServerWrapper
    {
        public const int SEND_PORT = 11000;
        public const int REC_PORT =60000;
        public const string LISTEN_IP = "10.100.102.8";

        public IPAddress Addr = IPAddress.Parse(LISTEN_IP);
        public List<User> ConnectedUsers { get; }

        public ServerWrapper()
        {
            ConnectedUsers = new List<User>();
        }

        public void Start()
        {
            //Task L = Task.Run(() => Listen());
            //Task S = Task.Run(() => Send());

            Listen();
        }

        public void Listen()
        {
            string Data = null;
            byte[] bytes = new Byte[1024];

            IPEndPoint EP = new IPEndPoint(Addr, REC_PORT);
            while (true) {
                Socket s = new Socket(Addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    s.Bind(EP);
                    s.Listen(10);

                    Console.WriteLine("Listening for connection...");
                    Data = null;
                    Socket handle = s.Accept();

                    // Listen for Connection
                    while(true)
                    {
                        int bytesRec = handle.Receive(bytes);
                        Data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (Data.IndexOf("<EOT>") > -1)
                            break;
                    }
                    s.Close();

                    Console.WriteLine($"Text recieved {Data}");

                    // HANDLE QUIT REQUEST
                    if (Data == "<JRQ><EOT>")
                    {
                        Console.WriteLine($"Accepted join request from {handle.RemoteEndPoint.ToString()}");
                        Console.WriteLine($"Adding {handle.RemoteEndPoint.ToString()}");
                        ConnectedUsers.Add(new User(handle.RemoteEndPoint));
                    }
                    else if (Data == "<QRP><EOT>")
                    {
                        Console.WriteLine($"Accepted quit request from {handle.RemoteEndPoint.ToString()}");
                        Console.WriteLine($"Deleting {handle.RemoteEndPoint.ToString()} from IP List.");
                        //ConnectedUsers.Remove(ConnectedUsers.Find(x => x.EP.ToString() == x.));
                    }
                    else if (Data.StartsWith("<NCR>"))
                    {

                    }
                    else
                    {

                        // Echo message back
                        Console.WriteLine($"Echoing\n{handle.RemoteEndPoint.ToString()}:{ Data.Substring(0, Data.Length - 5)}");
                        byte[] msg = Encoding.ASCII.GetBytes($"{handle.RemoteEndPoint.ToString()}:{Data.Substring(0,Data.Length - 5)}");
                        handle.Send(msg);
                    }
                    // Shut down socket
                    handle.Shutdown(SocketShutdown.Both);
                    handle.Close();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();

                    Console.WriteLine("Quitting...");
                    break;
                }
            }
        }
    }
}