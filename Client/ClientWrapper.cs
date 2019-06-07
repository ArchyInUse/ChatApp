﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class ClientWrapper
    {
        public IPAddress addr { get; set; } = IPAddress.Parse("89.139.220.217");
        public ClientWrapper()
        {

        }
        
        public void Connect()
        {
            int port = 60000;
            IPAddress addr = IPAddress.Parse("89.139.220.217");

            byte[] Bytes = new byte[1024];

            while (true)
            {
                try
                {
                    Socket s = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    string msg = Console.ReadLine() + "<EOT>";
                    s.Connect(new IPEndPoint(addr, port));

                    if (msg == "//Quit<EOT>")
                    {
                        // quit request pending
                        byte[] EncodedQuit = Encoding.ASCII.GetBytes("<QRP><EOT>");
                        Bytes = new byte[1024];
                        Console.WriteLine("Finished preparation for socket variables and data buffer.");
                        s.Send(EncodedQuit);
                        Console.WriteLine("Sent data...");
                        int BytesRec = s.Receive(Bytes);
                        // accepted quit reqest
                        if (Encoding.ASCII.GetString(Bytes, 0, BytesRec) == "<AQR>")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("---------------------");
                            Console.WriteLine("Accepted quit request");
                            Console.WriteLine("---------------------");
                            Console.ResetColor();
                            break;
                        }

                        s.Shutdown(SocketShutdown.Both);
                        s.Close();
                    }
                    else
                    {

                        Console.WriteLine($"Socket connected to {s.RemoteEndPoint.ToString()}");

                        byte[] EncodedMsg = Encoding.ASCII.GetBytes(msg);
                        int bytesSent = s.Send(EncodedMsg);
                        int bytesRec = s.Receive(Bytes);
                        Console.WriteLine($"{Encoding.ASCII.GetString(Bytes, 0, bytesRec)}");

                        s.Shutdown(SocketShutdown.Both);
                        s.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();

                    Console.WriteLine("Resetting...");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void FirstConnect()
        {
            try
            {
                byte[] RecBuffer = new byte[1024];
                Socket s = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(new IPEndPoint(addr, 60000));

                s.Send(Encoding.ASCII.GetBytes("<JRQ><EOT>"));
                var bytesrecieved = s.Receive(RecBuffer);

                // Join request accepted
                if (Encoding.ASCII.GetString(RecBuffer, 0, bytesrecieved) == "<JRA><EOT>")
                {
                    Console.WriteLine($"Connected successfully ({Encoding.ASCII.GetString(RecBuffer, 0, bytesrecieved)})");
                }
                else
                {
                    Console.WriteLine("Unsuccessful connection");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Connection failed : {e}");
                Console.WriteLine("Press any key...");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }
    }
}