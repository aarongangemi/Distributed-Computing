using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    public class Client
    {
        public string IpAddress;
        public string port;
        public int jobsCompleted;
        public int clientId;
        public Client(string IpAddress, string port, int clientId)
        {
            this.IpAddress = IpAddress;
            this.port = port;
            jobsCompleted = 0;
            this.clientId = clientId;
        }
    }
}
