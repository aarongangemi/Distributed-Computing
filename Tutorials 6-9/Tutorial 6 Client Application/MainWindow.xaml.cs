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
using System.Diagnostics;
using Microsoft.Scripting;

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
        private int portNumber;
        public bool IsClosed { get; private set; }
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
            
            JobCountField.Text = JobList.ListOfJobs.Count.ToString();
            
        }
        private void RunServer()
        {
                    Random rand = new Random();
                    int port = rand.Next(8100, 9000);
                    ServiceHost host;
                    host = new ServiceHost(typeof(ClientHost)); ; //Service host in OS
                    NetTcpBinding tcp = new NetTcpBinding();  //Create .NET TCP port
                    portNumber = port;
                    host.AddServiceEndpoint(typeof(IClient), tcp, "net.tcp://127.0.0.1:" + port + "/JobServer");
                    Console.WriteLine("Server Thread is running on port: " + port.ToString());
                    host.Open();
                    RegisterClient();
                    while (!IsClosed)
                    {

                    }
                    host.Close();
            
        }

        private void NetworkingThreadFunction()
        {
            List<Client> listOfClients = getClientList();
            NetTcpBinding tcp = new NetTcpBinding();
            string URL;
            ChannelFactory<IClient> foobFactory;
            IClient foob;
            string src;
            int idx;
            Console.WriteLine("............................");
            Console.WriteLine(listOfClients.Count);
            while (true)
            {
                for (int i = 0; i < listOfClients.Count; i++)
                {
                    if(portNumber.ToString() != listOfClients.ElementAt(i).ToString())
                    {
                        URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/JobServer";
                        foobFactory = new ChannelFactory<IClient>(tcp, URL);
                        foob = foobFactory.CreateChannel();
                        if (JobList.ListOfJobs.Count > 0)
                        {

                            foob.RequestJob(out src, out idx); //Get number of items
                            RunPythonCode();
                            updateCount();
                            //foob.UploadJobSolution(); //Return the result of the script
                        }
                    }
                }
                
            }
        }

        private void RunPythonCode()
        {
            Dispatcher.Invoke(() =>
            {
                var py = Python.CreateEngine();
                var result = py.Execute("print('HIIIII')");
                Console.WriteLine(result);
            });

        }
        private void updateCount()
        {
            Dispatcher.Invoke(() =>
            {
                JobCountField.Text = JobList.ListOfJobs.Count.ToString();
            });

        }
        private void Upload_Python_File(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(PythonScriptText.Text))
            {
                byte[] textBytes = Encoding.UTF8.GetBytes(PythonScriptText.Text);
               // SHA256 hash = SHA256.Create();
               // byte[] hashedText = hash.ComputeHash(textBytes);
                string encodedText = Convert.ToBase64String(textBytes);
                addJob(encodedText);
                JobCountField.Text = JobList.ListOfJobs.Count.ToString();
            }
        } 

        private void addJob(string PythonSrc)
        {
            JobList.ListOfJobs.Add(new Job(PythonSrc));     
        }

        private List<Client> getClientList()
        {
            bool isSerialized = false;
            while(!isSerialized)
            { 
                try
                {
                    RestRequest request = new RestRequest("api/Client/GetClientList");
                    IRestResponse clientList = client.Get(request);
                    List<Client> listOfClients = JsonConvert.DeserializeObject<List<Client>>(clientList.Content);
                    isSerialized = true;
                    return listOfClients;
                }
                catch(Exception)
                {

                }
            }
            return null;
        }

        private void RegisterClient()
        {
            RestRequest request = new RestRequest("api/Client/Register/");
            request.AddJsonBody(new Client("127.0.0.1", portNumber.ToString()));
            client.Post(request);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }
    }
}
