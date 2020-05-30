using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    /// <summary>
    /// Purpose: The client class is used to create a client object which represents an existing client
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public class Client
    {
        public string IpAddress;
        public string port;
        public int jobsCompleted;
        public int clientId;

        /// <summary>
        /// Purpose: The Client constructor sets the IpAddress and port of the client
        /// </summary>
        /// <param name="IpAddress"></param>
        /// <param name="port"></param>
        public Client(string IpAddress, string port, int clientId)
        {
            this.IpAddress = IpAddress;
            this.port = port;
            jobsCompleted = 0;
            this.clientId = clientId;
        }
    }
}
