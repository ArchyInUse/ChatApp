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
        public const string LISTEN_IP = "xx.xxx.xxx.xxx";
        public Socket _mainSocket;

        private IPAddress Addr = IPAddress.Parse(LISTEN_IP);
        private List<User> ConnectedUsers { get; }

        public ServerWrapper()
        {
            ConnectedUsers = new List<User>();

            _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var localIp = new IPEndPoint(IPAddress.Any, Port);

            _mainSocket.BeginAccept(OnClientConnect, null);
        }

        private void OnClientConnect(IAsyncResult asyncResult)
        {
            var socket = _mainSocket.EndAccept(asyncResult);

            Console.WriteLine($"Acceted connection from {socket.RemoteEndPoint}");
            ConnectedUsers.Add(new User(socket, this));

            _mainSocket.BeginAccept(OnClientConnect, null);
        }

        public async Task Log(byte[] bytes)
        {
            foreach(User u in ConnectedUsers)
            {
                Console.WriteLine($"Sending {u._socket.RemoteEndPoint} data...");
                u.Send(bytes);
            }
        }
    }
}