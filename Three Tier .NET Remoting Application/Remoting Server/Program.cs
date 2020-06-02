using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Remoting_Server
{
    /// <summary>
    /// Purpose: Start the console for the program
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Aaron's Server");
            ServiceHost host; 
            // Service host in OS
            NetTcpBinding tcp = new NetTcpBinding(); 
            // Create .NET TCP port
            host = new ServiceHost(typeof(DataServer));  
            // Host Implementation class
            host.AddServiceEndpoint(typeof(IDataServerInterface.IDataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            // open port
            host.Open();
            Console.WriteLine("System online");
            Console.ReadLine();
            host.Close();
            // close port
        }
    }
}
