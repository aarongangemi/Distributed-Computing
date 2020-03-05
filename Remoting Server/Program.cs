using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Remoting_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Aaron's Server");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            host.Open();
            Console.WriteLine("System online");
            Console.ReadLine();
            host.Close();
        }
    }
}
