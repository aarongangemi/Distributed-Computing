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
            JobList.ListOfJobs = new List<Job>();
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
            {}
            host.Close();
            
        }

        private void NetworkingThreadFunction()
        {
            SHA256 hashObj = SHA256.Create();
            List<Client> listOfClients = getClientList();
            NetTcpBinding tcp = new NetTcpBinding();
            string URL;
            ChannelFactory<IClient> foobFactory;
            IClient foob;
            int idx, val = 0;
            while (true)
            {
                for (int i = 0; i < listOfClients.Count; i++)
                {
                    try
                    {
                        if (portNumber.ToString() != listOfClients.ElementAt(i).ToString())
                        {
                            URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/JobServer";
                            foobFactory = new ChannelFactory<IClient>(tcp, URL);
                            foob = foobFactory.CreateChannel();
                            if (JobList.ListOfJobs.Count > 0)
                            {
                                foob.RequestJob(out idx, JobList.ListOfJobs); //Get number of items
                                Random rand = new Random();
                                val = rand.Next(0, listOfClients.Count);
                                listOfClients.ElementAt(val).jobAssigned = JobList.ListOfJobs.ElementAt(idx);
                                byte[] decodedBytes = Convert.FromBase64String(listOfClients.ElementAt(val).jobAssigned.PythonSrc);
                                string PythonSrc = Encoding.UTF8.GetString(decodedBytes);
                                byte[] hashArray = listOfClients.ElementAt(val).jobAssigned.hash;
                                byte[] RecievedHash = hashObj.ComputeHash(Encoding.UTF8.GetBytes(listOfClients.ElementAt(val).jobAssigned.PythonSrc));
                                if (RecievedHash.SequenceEqual(hashArray))
                                {
                                    RunPythonCode(PythonSrc);
                                    updateCount();
                                    foob.UploadJobSolution(PythonSrc, idx, JobList.ListOfJobs); //Return the result of the script
                                    JobList.ListOfJobs.RemoveAt(idx);
                                }
                            }
                        }
                    }
                    catch (EndpointNotFoundException)
                    {
                        MessageBox.Show("Client has been disconncted. Client " + i + "will be removed");
                        listOfClients.RemoveAt(i);
                    }
                }
            }
        }

        private void RunPythonCode(string PythonSrc)
        {
            Dispatcher.Invoke(() =>
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                engine.Execute(PythonSrc, scope);
                dynamic PyFunc = scope.GetVariable("main");
                var result = PyFunc();
                Console.WriteLine("The result of the python script was: " + result);
            });

        }
        private void updateCount()
        {
            Dispatcher.Invoke(() =>
            {
                int count = 0;
                for(int i = 0; i < JobList.ListOfJobs.Count; i++)
                {
                    if(JobList.ListOfJobs.ElementAt(i).JobComplete)
                    {
                        count++;
                    }
                }
                JobCountField.Text = count.ToString();
            });

        }
        private void Upload_Python_File(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(PythonScriptText.Text))
            {
                SHA256 hash = SHA256.Create();
                byte[] textBytes = Encoding.UTF8.GetBytes(PythonScriptText.Text);
                string base64String = Convert.ToBase64String(textBytes);
                Console.WriteLine(base64String);
                byte[] hashBytes = Encoding.UTF8.GetBytes(base64String);
                Console.WriteLine(hashBytes);
                byte[] hashedData = hash.ComputeHash(hashBytes);
                addJob(base64String, hashedData);
                JobCountField.Text = JobList.ListOfJobs.Count.ToString();
            }
        } 

        private void addJob(string PythonSrc, byte[] hashedData)
        {
            JobList.ListOfJobs.Add(new Job(PythonSrc, hashedData));
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
