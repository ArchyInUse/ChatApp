using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class ClientWrapper
    {
        public Socket _socket { get; }
        public IPEndPoint _ipEp { get; } = new IPEndPoint(IPAddress.Parse("89.139.220.217"), 60000);
        public byte[] _dataBuffer { get; } = new byte[1024];

        public ClientWrapper()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_ipEp);

            Send();
            Listen();
        }

        // Fire and forget implementation
        public void Send()
        {
            string msg = Console.ReadLine();
            byte[] msgBytes = Encoding.ASCII.GetBytes(msg);

            try
            {
                _socket.BeginSend(msgBytes, 0, msgBytes.Length, SocketFlags.None, OnSendComplete, null);
            }
            catch(SocketException e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
                Disconnect();
            }
        }

        public void OnSendComplete(IAsyncResult ar)
        {
            Send();
        }

        public void Listen()
        {
            try
            {
                _socket.BeginReceive(_dataBuffer, 0, _dataBuffer.Length, SocketFlags.None, OnMessageRecieved, null);
            }
            catch(SocketException)
            {
                Disconnect();
            }
        }

        public void OnMessageRecieved(IAsyncResult ar)
        {
            int bytesrec = _socket.EndReceive(ar);

            string msg = Encoding.ASCII.GetString(_dataBuffer, 0, bytesrec);

            msg = SortMessageToOneLine(msg);

            Console.WriteLine(msg);

            Listen();
        }

        public void Disconnect()
        {
            Console.WriteLine($"Disconnect call recieved.");
            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                Console.WriteLine("Disconnected.");
            }
            else
            {
                Console.WriteLine("Server already disconnected.");
            }
        }

        public string SortMessageToOneLine(string s)
        {
            for(int i = s.Length; i < 0; i--)
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
