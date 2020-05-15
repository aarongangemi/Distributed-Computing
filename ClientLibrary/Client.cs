using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class Client
    {
        public string IpAddress;
        public string port;

        public Client(string IpAddress, string port)
        {
            this.IpAddress = IpAddress;
            this.port = port;
        }
    }
}
