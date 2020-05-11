using Microsoft.Win32;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Threading;
using System.ServiceModel;
using Newtonsoft.Json;
using ClientLibrary;
using System.Security.Cryptography;

namespace Tutorial_6_Client_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void ServerDel();
        public delegate void NetworkDel();
        private RestClient client;
        private string URL;
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44369/";
            client = new RestClient(URL);
            ServerDel serverDelegate;
            NetworkDel networkDelegate;
            serverDelegate = RunServer;
            networkDelegate = NetworkingThreadFunction;
            serverDelegate.BeginInvoke(null, null);
            networkDelegate.BeginInvoke(null, null);
            RegisterClient();
        }
        private void RunServer()
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
            List<Client> listOfClients = getClientList();
            NetTcpBinding tcp = new NetTcpBinding();
            string URL;
            ChannelFactory<IClient> foobFactory;
            IClient foob;
            for(int i = 0; i < listOfClients.Count; i++)
            {
                URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress + ":" + listOfClients.ElementAt(i).port + "/NetworkingThread";
                foobFactory = new ChannelFactory<IClient>(tcp, URL);
                foob = foobFactory.CreateChannel();
                foob.RequestJob(); //Get number of items
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                engine.Execute(PythonScriptText.Text, scope);
                foob.UploadJobSolution(); //Return the result of the script
            }
        }

        private void Upload_Python_File(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(PythonScriptText.Text))
            {
                Console.WriteLine("No python code entered");
                byte[] textBytes = Encoding.UTF8.GetBytes(PythonScriptText.Text);
                SHA256 hash = SHA256.Create();
                byte[] hashedText = hash.ComputeHash(textBytes);
                string encodedText = Convert.ToBase64String(hashedText);
            }
            //addJob();
        } 

        private void addJob(string inJob)
        {
            JobList.ListOfJobs.Add(new Job(inJob));     
        }

        private List<Client> getClientList()
        {
            RestRequest request = new RestRequest("api/Client/GetClientList");
            IRestResponse clientList = client.Get(request);
            List<Client> listOfClients = JsonConvert.DeserializeObject<List<Client>>(clientList.Content);
            return listOfClients;
        }

        private void RegisterClient()
        {
            Random rand = new Random();
            string port = rand.Next(8101, 9000).ToString();
            RestRequest request = new RestRequest("api/Client/Register/");
            request.AddJsonBody(new Client("127.0.0.1", port));
            client.Post(request);
        }
    }
}
