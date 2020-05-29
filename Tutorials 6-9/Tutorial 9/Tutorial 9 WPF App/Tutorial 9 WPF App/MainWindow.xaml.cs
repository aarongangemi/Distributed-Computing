using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Tutorial_9_Blockchain_Library;

namespace Tutorial_9_WPF_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RestClient client;
        private string URL;
        private int portNumber;
        private List<Client> listOfClients;
        public static Hashtable ht;
        private int clientId;
        public static Mutex mutex = new Mutex();
        public bool IsClosed { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44326/";
            client = new RestClient(URL);
            listOfClients = getClientList();
            ht = new Hashtable();
            portNumber = PortCounter.CurrentPort;
            Blockchain.generateGenesisBlock();
            Thread ServerThread = new Thread(new ThreadStart(BlockchainThread));
            ServerThread.Start();
            Thread NetworkingThread = new Thread(new ThreadStart(MiningThread));
            NetworkingThread.Start();
            JobsCompleted.Text = PortCounter.JobCounter.ToString();
            NoOfBlocks.Text = Blockchain.BlockChain.Count.ToString();
            Thread JobListThread = new Thread(new ThreadStart(UpdateList));
            JobListThread.Start();
        }
        private void BlockchainThread()
        {
            bool ServerCreated = false;
            ServiceHost host;

            while (!ServerCreated)
            {
                try
                {
                    host = new ServiceHost(typeof(BlockchainHost)); //Service host in OS
                    NetTcpBinding tcp = new NetTcpBinding();  //Create .NET TCP port
                    host.AddServiceEndpoint(typeof(IBlockchain), tcp, "net.tcp://127.0.0.1:" + portNumber + "/BlockchainServerHost");
                    host.Open();
                    RestRequest request = new RestRequest("api/Client/Register/");
                    PortCounter.ClientCounter++;
                    clientId = PortCounter.ClientCounter;
                    request.AddJsonBody(new Client("127.0.0.1", portNumber.ToString(), clientId));
                    client.Post(request);
                    Debug.WriteLine(portNumber.ToString());
                    ServerCreated = true;
                    while (!IsClosed)
                    { }
                    host.Close();
                }
                catch (AddressAlreadyInUseException)
                {
                    PortCounter.CurrentPort++;
                    portNumber = PortCounter.CurrentPort;
                    PortCounter.ClientCounter++;
                    clientId = PortCounter.ClientCounter;
                    host = new ServiceHost(typeof(BlockchainHost));
                }
            }
        }


        private void Upload_Python_File(object sender, RoutedEventArgs e)
        {
            mutex.WaitOne();
            if (!String.IsNullOrEmpty(PythonScriptText.Text) && PythonScriptText.Text.StartsWith("def main():"))
            {

                NetTcpBinding tcp = new NetTcpBinding();
                string URL;
                ChannelFactory<IBlockchain> foobFactory;
                IBlockchain foob;
                listOfClients = getClientList();
                Transaction transaction = null;
                for (int i = 0; i < listOfClients.Count; i++)
                {
                    URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                    foobFactory = new ChannelFactory<IBlockchain>(tcp, URL);
                    foob = foobFactory.CreateChannel();
                    byte[] textBytes = Encoding.UTF8.GetBytes(PythonScriptText.Text);
                    string base64String = Convert.ToBase64String(textBytes);
                    transaction = new Transaction(base64String,"", int.Parse(listOfClients.ElementAt(i).port));
                    foob.RecieveNewTransaction(transaction);
                }
                TransactionStorage.CompletedTransactions.Add(transaction);
            }
            else
            {
                MessageBox.Show("Python string must start with 'def main():' ");
            }
            mutex.ReleaseMutex();
        }

        private void MiningThread()
        {
            ChannelFactory<IBlockchain> foobFactory;
            IBlockchain foob;
            int max;
            string maxHash = "";
            while (true)
            {
                try
                {
                    listOfClients = getClientList();
                    if (TransactionStorage.TransactionQueue.Count == 5)
                    {
                        List<Transaction> transactionsList = TransactionStorage.TransactionQueue.OrderBy(key => key.PythonSrc).ToList();
                        uint offset = 0;
                        string prevBlockHash;
                        string hashString = "";
                        SHA256 sha256 = SHA256.Create();
                        prevBlockHash = Blockchain.BlockChain.Last().blockHash;
                        Block block = new Block(offset, prevBlockHash, hashString);
                        Dispatcher.Invoke(() =>
                        {
                            try
                            {

                                for (int j = 0; j < 5; j++)
                                {
                                    Transaction t = TransactionStorage.TransactionQueue.Dequeue();
                                    t = transactionsList.ElementAt(j);
                                    if (!String.IsNullOrEmpty(t.PythonSrc))
                                    {
                                        byte[] decodedBytes = Convert.FromBase64String(t.PythonSrc);
                                        t.PythonSrc = Encoding.UTF8.GetString(decodedBytes);
                                        JobStatusLabel.Content = "Running Python Job";
                                        ScriptEngine engine = Python.CreateEngine();
                                        ScriptScope scope = engine.CreateScope();
                                        engine.Execute(t.PythonSrc, scope);
                                        dynamic PyFunc = scope.GetVariable("main");
                                        var result = PyFunc();
                                        PyResult.Content = result;
                                        t.PythonResult = PyResult.Content.ToString();
                                        Debug.WriteLine("Python Script executed successfully - Result = " + t.PythonResult);
                                        byte[] resultBytes = Encoding.UTF8.GetBytes(t.PythonResult);
                                        string base64String = Convert.ToBase64String(resultBytes);
                                        block.AddPythonTransaction(t.PythonSrc, t.PythonResult);
                                        JobStatusLabel.Content = "Python Job Complete";
                                        PortCounter.JobCounter++;
                                        JobsCompleted.Text = PortCounter.JobCounter.ToString();
                                    }
                                }
                                while (!hashString.StartsWith("12345"))
                                {
                                    offset++;
                                    string blockString = block.blockID + block.TransactionDetailsList + offset + prevBlockHash;
                                    byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                                    byte[] hashedData = sha256.ComputeHash(textBytes);
                                    hashString = BitConverter.ToUInt64(hashedData, 0).ToString();
                                }
                                block.blockHash = hashString;
                                block.blockOffset = offset;
                                if (Blockchain.ValidateBlock(block))
                                {
                                    Blockchain.AddBlock(block);
                                    Debug.WriteLine("Block successfully added");
                                    Debug.WriteLine("Block offset = " + offset);
                                    Debug.WriteLine("Block hash = " + block.blockHash);
                                    Debug.WriteLine("Block previous hash = " + prevBlockHash);
                                    Debug.WriteLine(Blockchain.BlockChain.Count + "..............................................");
                                    Dispatcher.Invoke(() => { NoOfBlocks.Text = (Blockchain.BlockChain.Count).ToString(); });
                                }
                                else
                                {
                                    Debug.WriteLine("Validation for block failed, Trying again");
                                }
                            }
                            catch (SyntaxErrorException)
                            {
                                MessageBox.Show("Invalid Python script, please ensure python body is valid and proper indentation is used for variables");
                            }
                            catch (UnboundNameException)
                            {
                                MessageBox.Show("Invalid variables found in python body, please try again");
                            }
                            catch (NullReferenceException)
                            {
                                MessageBox.Show("No return type found in python script, please return something");
                            }
                        });
                    }
                    for (int i = 0; i < listOfClients.Count; i++)
                    {
                        NetTcpBinding tcp = new NetTcpBinding();
                        string clientURL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                        foobFactory = new ChannelFactory<IBlockchain>(tcp, clientURL);
                        foob = foobFactory.CreateChannel();
                        if (ht.ContainsKey(foob.GetCurrentBlock().blockHash))
                        {
                            ht[foob.GetCurrentBlock().blockHash] = (int)ht[foob.GetCurrentBlock().blockHash] + 1;
                        }
                        else
                        {
                            ht.Add(foob.GetCurrentBlock().blockHash, 1);
                        }
                    }
                    foreach (DictionaryEntry entry in ht)
                    {
                        max = (int)entry.Value;
                        maxHash = (string)entry.Key;
                        if ((int)entry.Value > max)
                        {
                            max = (int)entry.Value;
                            maxHash = (string)entry.Key;
                        }
                    }
                    if (ht.Count > 0)
                    {
                        if (maxHash != Blockchain.BlockChain.Last().blockHash)
                        {
                            bool chainChange = false;
                            for (int i = 0; i < listOfClients.Count; i++)
                            {
                                NetTcpBinding tcp = new NetTcpBinding();
                                string clientURL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                                foobFactory = new ChannelFactory<IBlockchain>(tcp, clientURL);
                                foob = foobFactory.CreateChannel();
                                foreach (Block block in foob.GetCurrentBlockchain())
                                {
                                    if ((block.blockHash == maxHash) && (foob.GetCurrentBlockchain().Count != Blockchain.BlockChain.Count))
                                    {
                                        Debug.WriteLine("Chain changed successfully");
                                        chainChange = true;
                                        Blockchain.BlockChain = foob.GetCurrentBlockchain();
                                        Dispatcher.Invoke(() => { JobsCompleted.Text = PortCounter.JobCounter.ToString();
                                            NoOfBlocks.Text = foob.GetCurrentBlockchain().Count.ToString();
                                        });
                                        break;
                                    }
                                }
                                if (chainChange)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (EndpointNotFoundException)
                {
                    if (IsClosed)
                    {
                        Debug.WriteLine(portNumber.ToString());
                        RestRequest removeRequest = new RestRequest("api/Client/Remove/" + portNumber.ToString());
                        client.Get(removeRequest);
                        listOfClients = getClientList();
                    }
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("Dispatcher had to cancel, trying again");
                }
            }
        }
        private void UpdateList()
        {
            while(true)
            {
                try
                {
                    Thread.Sleep(10000);
                    Dispatcher.Invoke(() =>
                    {
                        JobListBox.Items.Clear();
                        foreach (Block block in Blockchain.BlockChain)
                        {
                            foreach (string[] jsonTransaction in JsonConvert.DeserializeObject<List<string[]>>(block.TransactionDetailsList))
                            {
                                foreach (Transaction transaction in TransactionStorage.CompletedTransactions)
                                {
                                    if (Encoding.UTF8.GetString(Convert.FromBase64String(transaction.PythonSrc)) == jsonTransaction[0])
                                    {
                                        JobListBox.Items.Add("Client " + transaction.TransactionId + " submitted script with result: " + jsonTransaction[1]);
                                    }
                                }
                            }
                        }
                    });
                    
                }
                catch (TaskCanceledException)
                {
                    RestRequest removeRequest = new RestRequest("api/Client/Remove/" + portNumber.ToString());
                    client.Get(removeRequest);
                    listOfClients = getClientList();
                    Console.WriteLine("Dispatcher had to reload due to losing client connection");
                }
            }
        }
        

        private List<Client> getClientList()
        {
            RestRequest request = new RestRequest("api/Client/GetClientList");
            IRestResponse clientList = client.Get(request);
            List<Client> listOfClients = JsonConvert.DeserializeObject<List<Client>>(clientList.Content);
            return listOfClients;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RestRequest removeRequest = new RestRequest("api/Client/Remove/" + portNumber.ToString());
            client.Get(removeRequest);
            listOfClients = getClientList();
            IsClosed = true;
        }
    }
}