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
using System.Threading.Tasks;

namespace Tutorial_6_Client_Application
{
    /// <summary>
    /// Purpose: This is the GUI class. Used to provide processing upon interaction
    /// with the GUI and display any resulting data
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public partial class MainWindow : Window
    {
        private RestClient client;
        private string URL;
        private int portNumber;
        private List<Client> listOfClients;
        private Log log;
        private int count = 0;
        public bool IsClosed { get; private set; }
        // a bool variable which determines if the window is closed or not
        public MainWindow()
        {
            InitializeComponent();
            // allow log of data
            log = new Log();
            log.logMessage("Client started");
            URL = "https://localhost:44369/";
            // web server URL
            // Create rest client
            client = new RestClient(URL);
            // start server and networking thread
            Thread ServerThread = new Thread(new ThreadStart(RunServer));
            ServerThread.Start();
            Thread NetworkingThread = new Thread(new ThreadStart(NetworkingThreadFunction));
            NetworkingThread.Start();
            // set current client port number
            portNumber = PortCounter.CurrentPort;
            // allow python to have new line and tab characters
            PythonScriptText.AcceptsReturn = true;
            PythonScriptText.AcceptsTab = true;
        }

        /// <summary>
        /// Purpose: This is the server thread. Used to register a client and host there port number and ip
        /// </summary>
        private void RunServer()
        {
            bool ServerCreated = false;
            // server not yet created
            ServiceHost host;
            listOfClients = getClientList();
            // retrieve the latest list of clients
            do
            {
                try
                {
                    host = new ServiceHost(typeof(JobHost)); 
                    NetTcpBinding tcp = new NetTcpBinding();  
                    host.AddServiceEndpoint(typeof(IClient), tcp, "net.tcp://127.0.0.1:" + portNumber + "/JobServer");
                    // specify client port number to open
                    // open client
                    host.Open();
                    RestRequest request = new RestRequest("api/Client/Register/");
                    request.AddJsonBody(new Client("127.0.0.1", portNumber.ToString()));
                    // Register client with web service
                    client.Post(request);
                    // post client request
                    ServerCreated = true;
                    log.logMessage("Client with port " + portNumber.ToString() + " was successfully updated");
                    log.logMessage("Server Thread is running on port: " + portNumber.ToString());
                    IsClosed = false;
                    // window is open
                    while (!IsClosed)
                    { }
                    // infinite while loop will run while window is open. 
                    // allows client to hang so host will no close.
                    // cannot use console.readLine() as there is not console
                    host.Close();
                }
                catch (AddressAlreadyInUseException)
                {
                    // if the client attempts to connect with a port number that is already in use,
                    // the clients port number will be incremented and will try again using the do-while loop
                    PortCounter.CurrentPort++;
                    portNumber = PortCounter.CurrentPort;
                    host = new ServiceHost(typeof(JobHost));
                }
            } while (!ServerCreated);
        }

        /// <summary>
        /// Purpose: This is the networking thread. This is used for any communication between multiple clients.
        /// Also used for processing of python code across multiple clients
        /// </summary>
        private void NetworkingThreadFunction()
        {
            SHA256 hashObj = SHA256.Create();
            NetTcpBinding tcp = new NetTcpBinding();
            string URL;
            ChannelFactory<IClient> foobFactory;
            IClient foob;
            // run loop infinitely
            while (true)
            {
                listOfClients = getClientList();
                // always get the lastest list of clients
                count = 0;
                // reset the count for total number of elements 
                for (int i = 0; i < listOfClients.Count; i++)
                {
                    try
                    {
                        count += listOfClients.ElementAt(i).jobsCompleted;
                        // get each clients list number of jobs completed
                        if ((i == listOfClients.Count - 1) && (!IsClosed))
                        {
                            // display total job count
                            Dispatcher.Invoke(() => { JobsCompleted.Text = count.ToString(); });
                        }
                        if (portNumber.ToString() != listOfClients.ElementAt(i).port.ToString())
                        {
                            // if statement disallows a client to process their own python code
                            URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/JobServer";
                            // connect to client IP and port number
                            foobFactory = new ChannelFactory<IClient>(tcp, URL);
                            foob = foobFactory.CreateChannel();
                            Job job = foob.RequestJob();
                            // request next available job from client
                            if (!String.IsNullOrEmpty(job.PythonSrc))
                            {
                                // check if python string is null or empty
                                log.logMessage("Connected to client: " + listOfClients.ElementAt(i).port.ToString());
                                // log that client X was connected
                                byte[] decodedBytes = Convert.FromBase64String(job.PythonSrc);
                                string pySource = Encoding.UTF8.GetString(decodedBytes);
                                // decode python string
                                byte[] hashArray = job.hash;
                                byte[] RecievedHash = hashObj.ComputeHash(Encoding.UTF8.GetBytes(job.PythonSrc));
                                // successfully retrieve hash for python string and compute a corresponding hash
                                if (RecievedHash.SequenceEqual(hashArray))
                                {
                                    // if hashes and base match, then execute python string
                                    job.PythonSrc = pySource;
                                    // set python source code to job
                                    Dispatcher.Invoke(() =>
                                    {
                                        try
                                        {
                                            JobStatusLabel.Content = "Running Python Job";
                                            // indicate job is executing
                                            log.logMessage("Attempting to execute python script");
                                            ScriptEngine engine = Python.CreateEngine();
                                            ScriptScope scope = engine.CreateScope();
                                            // Iron Python processing
                                            engine.Execute(job.PythonSrc, scope);
                                            // execute script provided by client
                                            dynamic PyFunc = scope.GetVariable("main");
                                            var result = PyFunc();
                                            // display result in client
                                            PyResult.Content = result;
                                            job.PythonResult = PyResult.Content.ToString();
                                            RestRequest request = new RestRequest("api/Client/UpdateCount/" + i.ToString());
                                            // update the client count using loop index
                                            client.Put(request);
                                            // put request sent and message logged in file
                                            log.logMessage("Count updated");

                                        }
                                        catch (SyntaxErrorException)
                                        {
                                            MessageBox.Show("Invalid Python script, please ensure python body is valid and proper indentation is used for variables");
                                            log.logError("Invalid Python script - invalid body");
                                            // checks for any invalid indentation in python body
                                        }
                                        catch (UnboundNameException)
                                        {
                                            MessageBox.Show("Invalid variables found in python body, please try again");
                                            log.logError("Invalid variables in python script");
                                            // check for invalid python body
                                        }
                                        catch (NullReferenceException)
                                        {
                                            MessageBox.Show("No return type found in python script, please return something");
                                            log.logError("no return found");
                                            // if python cannot return any data back to GUI, the catch error
                                        }
                                    });
                                    foob.UploadJobSolution(job.PythonResult, job.jobNumber);
                                    // Return the result of the script
                                    Dispatcher.Invoke(() =>
                                    {
                                        // display python result and log message of success
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
                        // if client has not been found - then remove them remove them if they have not been removed
                        if (IsClosed)
                        {
                            log.logError("Removing client");
                            RestRequest removeRequest = new RestRequest("api/Client/Remove/" + portNumber.ToString());
                            client.Get(removeRequest);
                            listOfClients = getClientList();
                            // update client list
                        }
                    }
                    catch (FaultException)
                    {
                        // if client has not been found - then remove them remove them if they have not been removed
                        if (IsClosed)
                        {
                            log.logError("Removing client");
                            RestRequest removeRequest = new RestRequest("api/Client/Remove/" + portNumber.ToString());
                            client.Get(removeRequest);
                            listOfClients = getClientList();
                            // update client list
                        }
                    }
                    catch (CommunicationException) 
                    {
                        // refresh the client list
                        log.logError("Communication exception - something went wrong, trying again");
                        listOfClients = getClientList();
                    }
                    catch (TaskCanceledException) 
                    { 
                        // refresh the client list
                        log.logError("Dispatcher had to cancel");
                        listOfClients = getClientList();
                    }
                }
                
            }
        }

        /// <summary>
        /// Purpose: If the user clicks on the upload python file button, then process the entered script into a job
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Upload_Python_File(object sender, RoutedEventArgs e)
        {
            // check the script starts with def main() and is not null
            if (!String.IsNullOrEmpty(PythonScriptText.Text) && PythonScriptText.Text.StartsWith("def main():") && PythonScriptText.Text.Contains("return"))
            {
                // create hash
                SHA256 hash = SHA256.Create();
                byte[] textBytes = Encoding.UTF8.GetBytes(PythonScriptText.Text);
                string base64String = Convert.ToBase64String(textBytes);
                // convert to base 64 encoding
                byte[] hashBytes = Encoding.UTF8.GetBytes(base64String);
                byte[] hashedData = hash.ComputeHash(hashBytes);
                // compute the hash
                Job job = new Job();
                job.setHash(hashedData);
                job.setPythonSrc(base64String);
                job.setJobNumber(JobList.ListOfJobs.Count + 1);
                // set the job details in a job object and add job to list
                JobList.ListOfJobs.Add(job);
                // log added job
                log.logMessage("Job " + job.jobNumber + " was added");
            }
            else
            {
                // something is wrong with the entered text - user needs to try again
                MessageBox.Show("Python string must start with 'def main():' and contain a return value");
                // log error
                log.logError("Invalid python script with no main function");
            }
        }

        /// <summary>
        /// Purpose: Used to retrieve the current client list
        /// </summary>
        /// <returns>list of clients</returns>
        private List<Client> getClientList()
        {
            // create and submit rest request to retrieve client list
            RestRequest request = new RestRequest("api/Client/GetClientList");
            IRestResponse clientList = client.Get(request);
            List<Client> listOfClients = JsonConvert.DeserializeObject<List<Client>>(clientList.Content);
            return listOfClients;
        }

        /// <summary>
        /// Purpose: Method to perform if the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsClosed = true;
            // used IsClosed to close client port
            RestRequest removeRequest = new RestRequest("api/Client/Remove/" + portNumber.ToString());
            client.Get(removeRequest);
            // remove current client from web service
        }
    }
}
