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
        public byte[] Data { get; } = new byte[1024];
        public Socket _socket;
        public string Name;
        public ServerWrapper _wrapper { get; }

        public User(Socket s, ServerWrapper sw, string name = null)
        {
            _socket = s;
            _wrapper = sw;
            Name = name;

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
                Console.WriteLine($"Sent message to {_socket.RemoteEndPoint}");
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

                string sorteddata = $"{_socket.RemoteEndPoint}: {strdata}";

                if (MessageLength == 0) Disconnect();

                _wrapper.Log(Encoding.ASCII.GetBytes(sorteddata));

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

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();


        }
    }
}
