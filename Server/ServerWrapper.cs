using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

namespace Server
{
    class ServerWrapper
    {
        public const int Port = 60000;
        public const string LISTEN_IP = "10.100.102.8";
        public Socket _mainSocket;

        private IPAddress Addr = IPAddress.Parse(LISTEN_IP);
        public static List<User> ConnectedUsers { get; set; }

        public ServerWrapper()
        {
            ConnectedUsers = new List<User>();

            _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var localIp = new IPEndPoint(IPAddress.Any, Port);

            _mainSocket.Bind(localIp);

            _mainSocket.Listen(10);

            _mainSocket.BeginAccept(OnClientConnect, null);
        }

        private void OnClientConnect(IAsyncResult asyncResult)
        {
            var socket = _mainSocket.EndAccept(asyncResult);

            Console.WriteLine($"Client connected: {socket.RemoteEndPoint}");
            ConnectedUsers.Add(new User(socket));

            _mainSocket.BeginAccept(OnClientConnect, null);
        }

        public static async Task Log(byte[] bytes)
        {
            string msg = RemoveWhiteSpace(bytes);
            foreach (User u in ConnectedUsers)
            {
                u.Parse(msg);
            }
        }

        public static async Task Log(string msg) => Log(Encoding.ASCII.GetBytes(msg));

        public static string RemoveWhiteSpace(byte[] arr)
        {
            int b = Array.FindLastIndex(arr, arr.Length - 1, x => x != 0);

            Array.Resize(ref arr, b + 1);

            return Encoding.ASCII.GetString(arr);
        }
    }
}