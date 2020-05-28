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
                for (int i = 0; i < listOfClients.Count; i++)
                {
                    URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                    foobFactory = new ChannelFactory<IBlockchain>(tcp, URL);
                    foob = foobFactory.CreateChannel();
                    byte[] textBytes = Encoding.UTF8.GetBytes(PythonScriptText.Text);
                    string base64String = Convert.ToBase64String(textBytes);
                    Transaction transaction = new Transaction(base64String,"", int.Parse(listOfClients.ElementAt(i).port));
                    foob.RecieveNewTransaction(transaction);
                }
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
                        for (int i = 0; i < transactionsList.Count; i++)
                        {
                            Transaction t = transactionsList.ElementAt(i);
                            TransactionStorage.TransactionQueue.Dequeue();
                            uint offset = 0;
                            string prevBlockHash;
                            string hashString = "";
                            SHA256 sha256 = SHA256.Create();
                            prevBlockHash = Blockchain.BlockChain.Last().blockHash;
                            foreach (Client c in listOfClients)
                            {
                                if (c.port != portNumber.ToString())
                                {
                                    if (!String.IsNullOrEmpty(t.PythonSrc))
                                    {
                                        byte[] decodedBytes = Convert.FromBase64String(t.PythonSrc);
                                        t.PythonSrc = Encoding.UTF8.GetString(decodedBytes);
                                        Dispatcher.Invoke(() =>
                                        {
                                            try
                                            {
                                                JobStatusLabel.Content = "Running Python Job";
                                                ScriptEngine engine = Python.CreateEngine();
                                                ScriptScope scope = engine.CreateScope();
                                                engine.Execute(t.PythonSrc, scope);
                                                dynamic PyFunc = scope.GetVariable("main");
                                                var result = PyFunc();
                                                PyResult.Content = result;
                                                t.PythonResult = PyResult.Content.ToString();
                                                Debug.WriteLine("Python Script executed successfully - Result = " + t.PythonResult);
                                                TransactionStorage.CompletedTransactions.Add(t);
                                                byte[] resultBytes = Encoding.UTF8.GetBytes(t.PythonResult);
                                                string base64String = Convert.ToBase64String(resultBytes);
                                                Block block = new Block(offset, prevBlockHash, hashString);
                                                block.AddPythonTransaction(t.PythonSrc, base64String);
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
                                                    Debug.WriteLine("..............................................");
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
                                }
                            }
                        }
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
                if (Blockchain.BlockChain.Count != JobListBox.Items.Count && Blockchain.BlockChain.Count > 1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Debug.WriteLine(Blockchain.BlockChain.Count);
                        JobListBox.Items.Clear();
                        for (int i = 0; i < TransactionStorage.CompletedTransactions.Count; i++)
                        {
                            if(portNumber.ToString() == TransactionStorage.CompletedTransactions.ElementAt(i).TransactionId.ToString())
                            {
                                JobListBox.Items.Add("Client " + TransactionStorage.CompletedTransactions.ElementAt(i).TransactionId + 
                                    " executed: " + TransactionStorage.CompletedTransactions.ElementAt(i).PythonResult);
                            }
                        }
                    });
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
            IsClosed = true;
        }
    }
}