using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
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
        /// <summary>
        /// Purpose: The Client constructor sets the IpAddress and port of the client
        /// </summary>
        /// <param name="IpAddress"></param>
        /// <param name="port"></param>
        public Client(string IpAddress, string port)
        {
            this.IpAddress = IpAddress;
            this.port = port;
            this.jobsCompleted = 0;
        }
    }
}
