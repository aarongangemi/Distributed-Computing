using Microsoft.Win32;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tutorial_6_Web_Server.Models;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Threading;
using System.ServiceModel;
using Tutorial_6_Client_Application.ClientGUIClass;
using Newtonsoft.Json;

namespace Tutorial_6_Client_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public delegate void ClientDel();
        private RestClient client;
        private string URL;
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44369/";
            client = new RestClient(URL);
            Thread ServerThread = new Thread(new ThreadStart(ServerThreadFunction));
            ServerThread.Start();
            Thread NetworkingThread = new Thread(new ThreadStart(NetworkingThreadFunction));
            NetworkingThread.Start();
        }

        private void ServerThreadFunction()
        {
            Console.WriteLine("Server Thread is running now...");
            ServiceHost host; //Service host in OS
            NetTcpBinding tcp = new NetTcpBinding();  //Create .NET TCP port
            host = new ServiceHost(typeof(ClientHost));  //Host Implementation class
            host.AddServiceEndpoint(typeof(IClient), tcp, "net.tcp://0.0.0.0:8100/ServerThread");
            host.Open();
            Console.WriteLine("System online");
            Console.ReadLine();
            host.Close();
        }

        private void NetworkingThreadFunction()
        {
            RestRequest request = new RestRequest("api/Client/GetClientList");
            IRestResponse clientList = client.Get(request);
            List<Client> listOfClients = JsonConvert.DeserializeObject<List<Client>>(clientList.Content);
            NetTcpBinding tcp = new NetTcpBinding();
            string URL;
            ChannelFactory<IClient> foobFactory;
            IClient foob;
            for(int i = 0; i < listOfClients.Count; i++)
            {
                URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress + ":" + listOfClients.ElementAt(i).port + "/NetworkingThread";
                foobFactory = new ChannelFactory<IClient>(tcp, URL);
                foob = foobFactory.CreateChannel();
                foob.RequestJob();
                foob.DownloadJob();
                foob.UploadJobSolution();
            }
        }

        private void Upload_Python_File(object sender, RoutedEventArgs e)
        { 
        } 
    }
}
