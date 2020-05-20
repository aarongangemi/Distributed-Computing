using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Threading;
using System.ServiceModel;
using Newtonsoft.Json;
using ClientLibrary;
using System.Security.Cryptography;
using JobLibrary;
using Microsoft.Scripting;
using IronPython.Runtime;

namespace Tutorial_6_Client_Application
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
        private static int JobsCompletedCount;
        private Log log;
        private bool ClientExists;
        public bool IsClosed { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            log = new Log();
            log.logMessage("Client started");
            URL = "https://localhost:44369/";
            client = new RestClient(URL);
            Thread ServerThread = new Thread(new ThreadStart(RunServer));
            ServerThread.Start();
            Thread NetworkingThread = new Thread(new ThreadStart(NetworkingThreadFunction));
            NetworkingThread.Start();
            JobsCompletedCount = 0;
            JobsCompleted.Text = JobsCompletedCount.ToString();
            portNumber = PortCounter.CurrentPort;
            PythonScriptText.AcceptsReturn = true;
            PythonScriptText.AcceptsTab = true;
        }
        private void RunServer()
        {
            ServiceHost host;
            listOfClients = getClientList();
            foreach(Client c in listOfClients)
            {
                if(portNumber.ToString() == c.port)
                {
                    PortCounter.CurrentPort++;
                    portNumber = PortCounter.CurrentPort;
                }
                else
                {
                    portNumber = PortCounter.CurrentPort;
                    break;
                }
            }
            host = new ServiceHost(typeof(JobHost)); //Service host in OS
            NetTcpBinding tcp = new NetTcpBinding();  //Create .NET TCP port
            host.AddServiceEndpoint(typeof(IClient), tcp, "net.tcp://127.0.0.1:" + portNumber + "/JobServer");
            host.Open();
            RestRequest request = new RestRequest("api/Client/Register/");
            request.AddJsonBody(new Client("127.0.0.1", portNumber.ToString()));
            client.Post(request);
            log.logMessage("Client with port " + portNumber.ToString() + " was successfully updated");
            log.logMessage("Server Thread is running on port: " + portNumber.ToString());
            while (!IsClosed)
            { }
            host.Close();
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
                                log.logMessage("Connected to client: " + listOfClients.ElementAt(i).port.ToString());
                                byte[] decodedBytes = Convert.FromBase64String(job.PythonSrc);
                                string pySource = Encoding.UTF8.GetString(decodedBytes);
                                byte[] hashArray = job.hash;
                                byte[] RecievedHash = hashObj.ComputeHash(Encoding.UTF8.GetBytes(job.PythonSrc));
                                if (RecievedHash.SequenceEqual(hashArray))
                                {
                                    job.PythonSrc = pySource;
                                    Dispatcher.Invoke(() =>
                                    {
                                        try
                                        {
                                            JobStatusLabel.Content = "Running Python Job";
                                            log.logMessage("Attempting to execute python script");
                                            ScriptEngine engine = Python.CreateEngine();
                                            ScriptScope scope = engine.CreateScope();
                                            engine.Execute(job.PythonSrc, scope);
                                            dynamic PyFunc = scope.GetVariable("main");
                                            var result = PyFunc();
                                            PyResult.Content = result;
                                            job.PythonResult = PyResult.Content.ToString();
                                            JobsCompletedCount++;
                                            JobsCompleted.Text = JobsCompletedCount.ToString();
                                            log.logMessage("Python script executed successfully: result is: " + PyResult.Content);
                                        }
                                        catch (SyntaxErrorException)
                                        {
                                            MessageBox.Show("Invalid Python script, please ensure python body is valid and proper indentation is used for variables");
                                            log.logError("Invalid Python script - invalid body");
                                        }
                                        catch(UnboundNameException)
                                        {
                                            MessageBox.Show("Invalid variables found in python body, please try again");
                                            log.logError("Invalid variables in python script");
                                        }
                                        catch(NullReferenceException)
                                        {
                                            MessageBox.Show("No return type found in python script, please return something");
                                            log.logError("no return found");
                                        }
                                    });
                                    RestRequest request = new RestRequest("api/Client/UpdateCount/" + i.ToString());
                                    client.Put(request);
                                    log.logMessage("Count updated");
                                    foob.UploadJobSolution(job.PythonResult, job.jobNumber); //Return the result of the script
                                    Dispatcher.Invoke(() =>
                                    {
                                        Thread.Sleep(500);
                                        JobStatusLabel.Content = "Python Job Complete";
                                        log.logMessage("Python result: " + job.PythonResult + " was successfully uploaded");
                                    });
                                }

                            }
                            
                        }
                    }
                    catch (EndpointNotFoundException)
                    {
                        RestRequest request = new RestRequest("api/Client/Remove/" + i.ToString());
                        log.logError("Client " + i.ToString() + " has successfully been disconnected");
                        client.Post(request);
                        listOfClients = getClientList();
                        i = 0;
                    }
                    catch(FaultException)
                    {
                        MessageBox.Show("Closing client");
                        log.logError("Client successfully closed");

                    }
                }
            }
        }

        private void Upload_Python_File(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(PythonScriptText.Text) && PythonScriptText.Text.StartsWith("def main():"))
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
                log.logMessage("Job " + job.jobNumber + " was added");
            }
            else
            {
                MessageBox.Show("Python string must start with 'def main():' ");
                log.logError("Invalid python script with no main function");
            }
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
