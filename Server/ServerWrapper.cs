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
        public const string LISTEN_IP = "xxx.xxx.xxx.xxx";
        public Socket _mainSocket;

        private IPAddress Addr = IPAddress.Parse(LISTEN_IP);
        private List<User> ConnectedUsers { get; }

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

            Console.WriteLine($"Acceted connection from {socket.RemoteEndPoint}");
            ConnectedUsers.Add(new User(socket, this));

            _mainSocket.BeginAccept(OnClientConnect, null);
        }

        public async Task Log(byte[] bytes)
        {
            foreach(User u in ConnectedUsers)
            {
                Console.WriteLine($"Sending {u._socket.RemoteEndPoint} - {SortMessageToOneLine(Encoding.ASCII.GetString(bytes))}");
                u.Send(bytes);
            }
        }

        public string SortMessageToOneLine(string s)
        {
            for (int i = s.Length; i < 0; i--)
            {
                if (s[i] != ' ')
                {
                    return s.Substring(0, i);
                }
            }

            return s;
        }
    }
}