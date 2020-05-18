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
        private List<Client> listOfClients;
        public static int JobsCompletedCount;
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
            JobsCompletedCount = 0;
            JobsCompleted.Text = JobsCompletedCount.ToString();
            portNumber = 8100;
        }
        private void RunServer()
        {
            ServiceHost host;
            bool ServerStarted = false;
            while(!ServerStarted)
            try
            {
                host = new ServiceHost(typeof(JobHost)); //Service host in OS
                NetTcpBinding tcp = new NetTcpBinding();  //Create .NET TCP port
                host.AddServiceEndpoint(typeof(IClient), tcp, "net.tcp://127.0.0.1:" + portNumber + "/JobServer");
                host.Open();
                RegisterClient();
                Console.WriteLine("Server Thread is running on port: " + portNumber.ToString());
                ServerStarted = true;
                while (!IsClosed)
                { }
                host.Close();
            }
            catch(AddressAlreadyInUseException)
            {
                portNumber++;
                host = new ServiceHost(typeof(JobHost));
            }
        }

        private void NetworkingThreadFunction()
        {
            SHA256 hashObj = SHA256.Create();
            NetTcpBinding tcp = new NetTcpBinding();
            string URL;
            ChannelFactory<IClient> foobFactory;
            IClient foob;
            while (true)
            {
                listOfClients = getClientList();
                for (int i = 0; i < listOfClients.Count; i++)
                {
                    try
                    {
                        if (portNumber.ToString() != listOfClients.ElementAt(i).port.ToString())
                        {
                            URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/JobServer";
                            foobFactory = new ChannelFactory<IClient>(tcp, URL);
                            foob = foobFactory.CreateChannel();
                                Job job = foob.RequestJob();
                                if (!String.IsNullOrEmpty(job.PythonSrc))
                                {
                                    byte[] decodedBytes = Convert.FromBase64String(job.PythonSrc);
                                    string pySource = Encoding.UTF8.GetString(decodedBytes);
                                    byte[] hashArray = job.hash;
                                    byte[] RecievedHash = hashObj.ComputeHash(Encoding.UTF8.GetBytes(job.PythonSrc));
                                    if (RecievedHash.SequenceEqual(hashArray))
                                    {
                                        job.PythonSrc = pySource;
                                        Dispatcher.Invoke(() =>
                                        {
                                            JobStatusLabel.Content = "Running Python Job";
                                        });
                                        Dispatcher.Invoke(() =>
                                        {
                                            ScriptEngine engine = Python.CreateEngine();
                                            ScriptScope scope = engine.CreateScope();
                                            engine.Execute(job.PythonSrc, scope);
                                            dynamic PyFunc = scope.GetVariable("main");
                                            var result = PyFunc();
                                            PyResult.Content = result;
                                            job.PythonResult = PyResult.Content.ToString();
                                            JobsCompletedCount++;
                                            JobsCompleted.Text = JobsCompletedCount.ToString();
                                        });
                                        UpdateCount(i);
                                        foob.UploadJobSolution(job.PythonResult, job.jobNumber); //Return the result of the script
                                        Dispatcher.Invoke(() =>
                                        {
                                            Thread.Sleep(500);
                                            JobStatusLabel.Content = "Python Job Complete";
                                        });
                                    }

                                }
                            
                        }
                    }
                    catch (EndpointNotFoundException)
                    {
                        MessageBox.Show("Client has been disconncted. Client " + i + " will be removed");
                        RestRequest request = new RestRequest("api/Client/Remove/" + i.ToString());
                        client.Post(request);
                        listOfClients = getClientList();
                        i = 0;
                    }
                    catch(FaultException)
                    {
                        MessageBox.Show("Closing client");
                    }
                }
            }
        }

        private void Upload_Python_File(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(PythonScriptText.Text))
            {
                SHA256 hash = SHA256.Create();
                byte[] textBytes = Encoding.UTF8.GetBytes(PythonScriptText.Text);
                string base64String = Convert.ToBase64String(textBytes);
                byte[] hashBytes = Encoding.UTF8.GetBytes(base64String);
                byte[] hashedData = hash.ComputeHash(hashBytes);
                Job job = new Job();
                job.setHash(hashedData);
                job.setPythonSrc(base64String);
                job.setJobNumber(JobList.ListOfJobs.Count + 1);
                JobList.ListOfJobs.Add(job);
            }
        } 

        private List<Client> getClientList()
        {
            RestRequest request = new RestRequest("api/Client/GetClientList");
            IRestResponse clientList = client.Get(request);
            List<Client> listOfClients = JsonConvert.DeserializeObject<List<Client>>(clientList.Content);
            return listOfClients;
        }

        private void UpdateCount(int idx)
        {
            RestRequest request = new RestRequest("api/Client/UpdateCount/" + idx.ToString());
            client.Put(request);
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
