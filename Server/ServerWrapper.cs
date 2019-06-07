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
        public const int SEND_PORT = 61000;
        public const int REC_PORT = 60000;
        public const string LISTEN_IP = "10.100.102.8";

        private IPAddress Addr = IPAddress.Parse(LISTEN_IP);
        private List<User> ConnectedUsers { get; }

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
                    // have to close socket so that there isn't 2 sockets (causing an exception)
                    s.Close();

                    // Listen for Connection
                    while(true)
                    {
                        int bytesRec = handle.Receive(bytes);
                        Data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (Data.IndexOf("<EOT>") > -1)
                            break;
                    }

                    Console.WriteLine($"Text recieved {Data}");

                    // HANDLE QUIT REQUEST
                    if (Data == "<JRQ><EOT>")
                    {
                        Console.WriteLine($"{handle.RemoteEndPoint.ToString()} joined.");
                        ConnectedUsers.Add(new User(handle.RemoteEndPoint));
                        handle.Send(Encoding.ASCII.GetBytes("<JRA><EOT>"));
                        SendMessage($"{handle.RemoteEndPoint.ToString()} joined.", handle.RemoteEndPoint);
                    }
                    // QUIT REQUEST PENDING
                    else if (Data == "<QRP><EOT>")
                    {
                        Console.WriteLine($"{handle.RemoteEndPoint} quit.<EOT>");
                        ConnectedUsers.Remove(ConnectedUsers.Find(x => x == handle.RemoteEndPoint));
                        SendMessage($"{handle.RemoteEndPoint} quit.<EOT>", handle.RemoteEndPoint);
                    }
                    // NAME CHANGE REQUEST
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

        public void SendMessage(string message, EndPoint except)
        {
            Socket socket = new Socket(Addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            foreach(User user in ConnectedUsers)
            {
                //if (user != except)
                //{
                    socket.Connect(user.EP);
                    socket.Send(Encoding.ASCII.GetBytes(message));
                //}
            }
        }
    }
}