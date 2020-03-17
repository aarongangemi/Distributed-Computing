﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Aaron's Server");
            ServiceHost host; //Service host in OS
            NetTcpBinding tcp = new NetTcpBinding();  //Create .NET TCP port
            host = new ServiceHost(typeof(BusinessServer));
            tcp.OpenTimeout = new TimeSpan(0, 30, 0);
            tcp.CloseTimeout = new TimeSpan(0, 30, 0);
            tcp.SendTimeout = new TimeSpan(0, 30, 0);
            tcp.ReceiveTimeout = new TimeSpan(0, 30, 0);
            host.AddServiceEndpoint(typeof(BusinessServerInterface), tcp, "net.tcp://0.0.0.0:8200/BusinessService");
            host.Open();
            Console.WriteLine("System online");
            Console.ReadLine();
            host.Close();
        }
    }
}
