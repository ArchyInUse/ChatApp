using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    /// <summary>
    /// Class that contains all info on a user (name, ip etc)
    /// </summary>
    class User
    {
        public byte[] Data { get; set; } = new byte[1024];
        public Socket _socket;
        public string Name;

        public User(Socket s, string name = null)
        {
            _socket = s;
            Name = s.RemoteEndPoint.ToString().Substring(0, s.RemoteEndPoint.ToString().Length - 6);

            ListenForData();
        }

        public void Send(string data)
        {
            Send(Encoding.ASCII.GetBytes(data));
        }
        
        public void Send(byte[] data)
        {
            try
            {
                _socket.BeginSend(data, 0, data.Length, SocketFlags.None, OnSendComplete, null);
            }
            catch(SocketException)
            {
                Disconnect();
            }
            catch(ObjectDisposedException)
            {
                Disconnect();
            }
        }

        private void OnSendComplete(IAsyncResult ar)
        {
            try
            {
                _socket.EndSend(ar);
            }
            catch(SocketException)
            {
                Disconnect();
            }
        }

        public void ListenForData()
        {
            try
            {
                _socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, OnDataRecieved, null);
            }
            catch(SocketException)
            {
                Disconnect();
            }
        }

        private void OnDataRecieved(IAsyncResult ar)
        {
            try
            {
                if (!_socket.Connected) return;

                var MessageLength = _socket.EndReceive(ar);

                string strdata = Encoding.ASCII.GetString(Data, 0, Data.Length);
                Data = new byte[1024];

                string sorteddata = $"{Name}:{strdata}";

                if (MessageLength == 0) Disconnect();

                Parse(sorteddata);

                ListenForData();
            }
            catch(SocketException)
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (!_socket.Connected) return;

            Console.WriteLine($"Disconnecting {_socket.RemoteEndPoint}");

            ServerWrapper.Log(Encoding.ASCII.GetBytes($"{_socket.RemoteEndPoint} disconnected."));

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();


        }

        public void Parse(string str)
        {
            if (str.StartsWith($"{Name}:/nick "))
            {
                str = ServerWrapper.RemoveWhiteSpace(Encoding.ASCII.GetBytes(str));
                ServerWrapper.Log($"{Name} has changed their name to {str.Substring($"{Name}:/nick ".Length)}");
                Console.WriteLine($"{Name} has changed their name to {str.Substring($"{Name}:/nick ".Length)}");
                Name = str.Substring($"{Name}:/nick ".Length);
            }
            else if(str.Trim() == "/disconnect")
            {
                ServerWrapper.Log($"{Name} has disconnected.");
                Console.WriteLine($"{Name} has disconnected.");
                Disconnect();
                ServerWrapper.ConnectedUsers.Remove(this);
            }
            else
            {
                Console.WriteLine(ServerWrapper.RemoveWhiteSpace(Encoding.ASCII.GetBytes(str)));
                ServerWrapper.Log(ServerWrapper.RemoveWhiteSpace(Encoding.ASCII.GetBytes(str)));
            }
        }
    }
}
