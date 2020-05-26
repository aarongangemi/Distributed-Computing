using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using Tutorial_8_Blockchain_Library;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void ServerDel();
        private delegate void NetworkDel();
        private RestClient client;
        private string URL;
        private int portNumber;
        private List<Client> listOfClients;
        public bool IsClosed { get; private set; }
        public MainWindow()
        {
            URL = "https://localhost:44330/";
            client = new RestClient(URL);
            Thread ServerThread = new Thread(new ThreadStart(BlockchainThread));
            ServerThread.Start();
            Thread NetworkingThread = new Thread(new ThreadStart(MiningThread));
            NetworkingThread.Start();
            portNumber = PortCounter.CurrentPort;
            InitializeComponent();
        }

        private void BlockchainThread()
        {
            bool ServerCreated = false;
            ServiceHost host;
            listOfClients = getClientList();
            while (!ServerCreated)
            {
                try
                {
                    host = new ServiceHost(typeof(BlockchainHost)); //Service host in OS
                    NetTcpBinding tcp = new NetTcpBinding();  //Create .NET TCP port
                    host.AddServiceEndpoint(typeof(IBlockchain), tcp, "net.tcp://127.0.0.1:" + portNumber + "/JobServer");
                    host.Open();
                    RestRequest request = new RestRequest("api/Client/Register/");
                    request.AddJsonBody(new Client("127.0.0.1", portNumber.ToString()));
                    client.Post(request);
                    ServerCreated = true;
                    while (!IsClosed)
                    { }
                    host.Close();
                }
                catch (AddressAlreadyInUseException)
                {
                    PortCounter.CurrentPort++;
                    portNumber = PortCounter.CurrentPort;
                    host = new ServiceHost(typeof(BlockchainHost));
                }
            }
        }

        private void MiningThread()
        {

        }

        private List<Client> getClientList()
        {
            RestRequest request = new RestRequest("api/Client/GetClientList");
            IRestResponse clientList = client.Get(request);
            List<Client> listOfClients = JsonConvert.DeserializeObject<List<Client>>(clientList.Content);
            return listOfClients;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }
    }
}
