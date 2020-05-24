using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_2
{
    /// <summary>
    /// Purpose: Used to create business server console
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
            host = new ServiceHost(typeof(BusinessServer));
            // Create host
            tcp.OpenTimeout = new TimeSpan(0, 30, 0);
            tcp.CloseTimeout = new TimeSpan(0, 30, 0);
            tcp.SendTimeout = new TimeSpan(0, 30, 0);
            tcp.ReceiveTimeout = new TimeSpan(0, 30, 0);
            // Set time span for longer so no timeout occurs
            host.AddServiceEndpoint(typeof(BusinessServerInterface), tcp, "net.tcp://0.0.0.0:8200/BusinessService");
            // Add service endpoint
            host.Open();
            Console.WriteLine("System online");
            Console.ReadLine();
            // Keep console open
            host.Close();
            // Close port
        }
    }
}
