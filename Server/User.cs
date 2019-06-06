using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    /// <summary>
    /// Class that contains all info on a user (name, ip etc)
    /// </summary>
    class User
    {
        public EndPoint EP { get; }
        public string Name { get; }

        public User(EndPoint ep, string name = null)
        {
            EP = ep;
            Name = name;
        }
    }
}
