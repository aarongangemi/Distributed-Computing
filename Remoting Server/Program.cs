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
            ServiceHost host; //Service host in OS
            NetTcpBinding tcp = new NetTcpBinding();  //Create .NET TCP port
            host = new ServiceHost(typeof(DataServer));  //Host Implementation class
            host.AddServiceEndpoint(typeof(IDataServerInterface.IDataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            host.Open();
            Console.WriteLine("System online");
            Console.ReadLine();
            host.Close();
        }
    }
}
