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

        public static bool operator==(User user1, User user2)
        {
            if(user1.EP.ToString() == user2.EP.ToString())
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(User user1, User user2)
        {
            if (user1.EP.ToString() != user2.EP.ToString())
            {
                return true;
            }
            return false;
        }
    }
}
