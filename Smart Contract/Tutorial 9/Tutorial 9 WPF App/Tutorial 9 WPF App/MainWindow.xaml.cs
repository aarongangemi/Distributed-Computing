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
    /// Purpose: Interaction logic for MainWindow.xaml. Contains any functionality that is implemented in the GUI and includes
    /// the mining thread for processing python code and the .NET remoting server thread.
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
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

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44326/";
            // connect to web server using URL
            PythonScriptText.AcceptsTab = true;
            PythonScriptText.AcceptsReturn = true;
            client = new RestClient(URL);
            // get list of clients
            listOfClients = getClientList();
            // hash table to store hash and quantity
            ht = new Hashtable();
            // get port number
            portNumber = Counter.CurrentPort;
            // create genesis block
            Blockchain.generateGenesisBlock();
            Thread ServerThread = new Thread(new ThreadStart(BlockchainThread));
            ServerThread.Start();
            // start server thread
            Thread NetworkingThread = new Thread(new ThreadStart(MiningThread));
            NetworkingThread.Start();
            // start networking thread
            JobsCompleted.Text = Counter.JobCounter.ToString();
            NoOfBlocks.Text = Blockchain.BlockChain.Count.ToString();
            // set no of blocks in GUI
            Thread JobListThread = new Thread(new ThreadStart(UpdateList));
            JobListThread.Start();

            // start thread to consistently update list
        }

        /// <summary>
        /// Purpose: The blockchain thread is responsible for starting up the .NET remoting server thread and
        /// connecting a client
        /// </summary>
        private void BlockchainThread()
        {
            bool ServerCreated = false;
            ServiceHost host;
            // run loop until client is successfully connected
            while (!ServerCreated)
            {
                try
                {
                    host = new ServiceHost(typeof(BlockchainHost));
                    NetTcpBinding tcp = new NetTcpBinding(); 
                    // create service host
                    host.AddServiceEndpoint(typeof(IBlockchain), tcp, "net.tcp://127.0.0.1:" + portNumber + "/BlockchainServerHost");
                    // add endpoint for client
                    host.Open();
                    // attempt to open client - may throw exception if port number is already in use
                    RestRequest request = new RestRequest("api/Client/Register/");
                    Counter.ClientCounter++;
                    // increment client number
                    clientId = Counter.ClientCounter;
                    // set client ID
                    request.AddJsonBody(new Client("127.0.0.1", portNumber.ToString(), clientId));
                    client.Post(request);
                    // register client
                    ServerCreated = true;
                    Debug.WriteLine("Successfully connected to port number: " + portNumber);
                    // server was successfully created so can exit loop
                    while (!IsClosed)
                    { }
                    // loop remains open to keep client host open until window is closed. If window closes, then loop will close
                    host.Close();
                }
                catch (AddressAlreadyInUseException)
                {
                    // If the port number the client attempts to use is already in use, then increment the client number and port
                    // and try again
                    Counter.CurrentPort++;
                    portNumber = Counter.CurrentPort;
                    // increment port and client no
                    Counter.ClientCounter++;
                    clientId = Counter.ClientCounter;
                    host = new ServiceHost(typeof(BlockchainHost));
                    Debug.WriteLine("Unable to connect to port number, trying " + portNumber);
                }
            }
        }

        /// <summary>
        /// Purpose: if the user clicks the button to upload a python file, then submit the transaction to the transaction queue and
        /// list of submitted transactions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Upload_Python_File(object sender, RoutedEventArgs e)
        {
            // use the mutex to only allow one file at a time
            mutex.WaitOne();
            if (!String.IsNullOrEmpty(PythonScriptText.Text) && PythonScriptText.Text.StartsWith("def main():") && PythonScriptText.Text.Contains("return"))
            {
                // must check if string field is not null, empty and starts with main()
                NetTcpBinding tcp = new NetTcpBinding();
                string URL;
                ChannelFactory<IBlockchain> foobFactory;
                IBlockchain foob;
                listOfClients = getClientList();
                // get the latest client list
                Transaction transaction = null;
                for (int i = 0; i < listOfClients.Count; i++)
                {
                    // loop through list of clients and connect to each client using IP and port number
                    URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                    foobFactory = new ChannelFactory<IBlockchain>(tcp, URL);
                    foob = foobFactory.CreateChannel();
                    // encode the python script using base 64 encoding
                    byte[] textBytes = Encoding.UTF8.GetBytes(PythonScriptText.Text);
                    string base64String = Convert.ToBase64String(textBytes);
                    transaction = new Transaction(base64String,"", int.Parse(listOfClients.ElementAt(i).port));
                    // create a transaction and add to queue - no result has been calculate at this point so 
                    // empty string is passed into transaction.
                    foob.RecieveNewTransaction(transaction);
                }
                TransactionStorage.CompletedTransactions.Add(transaction);
                // add transaction to list of submitted transactions
            }
            else
            {
                // if the script is null or the start does not def main();
                MessageBox.Show("Python string must start with 'def main():' and have a return ");
                // log error
                Debug.WriteLine("Python string must start with 'def main():' and have a return");
            }
            // release the mutex for another client to use
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Purpose: The mining thread is used to process transactions/python script and then add a valid
        /// transaction to the block chain with the corresponding result.
        /// </summary>
        private void MiningThread()
        {
            string maxHash = "";
            while (true)
            {
                // infinitely loop mining thread so it can be used for all active clients
                try
                {
                    listOfClients = getClientList();
                    // retrieve latest client list so each list will account for newly added clients
                    if (TransactionStorage.TransactionQueue.Count == 5)
                    {
                        // only execute if transaction count is 5
                        List<Transaction> transactionsList = TransactionStorage.TransactionQueue.OrderBy(key => key.PythonSrc).ToList();
                        // Convert queue to list for alphabetical sorting
                        uint offset = 0;
                        string prevBlockHash;
                        string hashString = "";
                        prevBlockHash = Blockchain.BlockChain.Last().blockHash;
                        // set previous block hash field to last blockhash in chain
                        Block block = new Block(offset, prevBlockHash, hashString);
                        // create a new block
                        Dispatcher.Invoke(() =>
                        // use dispatcher to access GUI thread
                        {
                            try
                            {
                                for (int j = 0; j < 5; j++)
                                {
                                    // loop through 5 transactions
                                    Transaction t = TransactionStorage.TransactionQueue.Dequeue();
                                    // dequeue transaction from queue
                                    t = transactionsList.ElementAt(j);
                                    // get alphabetical transaction from list
                                    if (!String.IsNullOrEmpty(t.PythonSrc))
                                    {
                                        // decode python code as it is in base64 encoding
                                        byte[] decodedBytes = Convert.FromBase64String(t.PythonSrc);
                                        t.PythonSrc = Encoding.UTF8.GetString(decodedBytes);
                                        JobStatusLabel.Content = "Running Python Job";
                                        // change label to indicate python job is running
                                        ScriptEngine engine = Python.CreateEngine();
                                        ScriptScope scope = engine.CreateScope();
                                        // create engine and scope using iron python to execute script
                                        engine.Execute(t.PythonSrc, scope);
                                        dynamic PyFunc = scope.GetVariable("main");
                                        // look for function main
                                        var result = PyFunc();
                                        ExecutedList.Items.Add("Client " + portNumber + ", result = " + result);
                                        string resultString = Convert.ToString(result);
                                        // set python result in GUI and transaction result to python result
                                        Debug.WriteLine("Python Script executed successfully - Result = " + resultString);
                                        byte[] resultBytes = Encoding.UTF8.GetBytes(resultString);
                                        string base64String = Convert.ToBase64String(resultBytes);
                                        // convert the result to base 64 encoding
                                        block.AddPythonTransaction(t.PythonSrc, base64String);
                                        // add the transaction to the serialized JSON transaction list in the block.
                                        // method will deserialize into List<String[]>
                                        JobStatusLabel.Content = "Python Job Complete";
                                        // change status
                                        Counter.JobCounter++;
                                        // increment counter for number of jobs completed and display count below
                                        JobsCompleted.Text = Counter.JobCounter.ToString();
                                    }
                                }
                                block.blockHash = BruteForceHash(out offset, block, prevBlockHash);
                                // generate a block hash that begins with "12345"
                                block.blockOffset = offset;
                                // set the block offset
                                if (Blockchain.ValidateBlock(block))
                                {
                                    // if the validation for the block is successful, then add the block to the chain
                                    Blockchain.AddBlock(block);
                                    // log the result
                                    Debug.WriteLine("Block successfully added");
                                    Debug.WriteLine("Block offset = " + offset);
                                    Debug.WriteLine("Block hash = " + block.blockHash);
                                    Debug.WriteLine("Block previous hash = " + prevBlockHash);
                                    Debug.WriteLine("..............................................");
                                    // use dispatcher to invoke GUI thread and update blockchain count for each client
                                    Dispatcher.Invoke(() => { NoOfBlocks.Text = Blockchain.BlockChain.Count.ToString(); });
                                }
                                else
                                {
                                    // Validation failed, so log error
                                    Debug.WriteLine("Validation for block failed, Trying again");
                                }
                            }
                            catch(FormatException)
                            {
                                TransactionStorage.TransactionQueue.Clear();
                                Debug.WriteLine("Transaction queue cleared due to invalid transaction");
                                JobStatusLabel.Content = "Python Job Complete";
                            }
                            catch (SyntaxErrorException)
                            {
                                // Syntax error occurs in python body- error caught, displayed and logged
                                Debug.WriteLine("Python script is invalid due to body, syntax error caught");
                                MessageBox.Show("Invalid Python script, please ensure python body is valid and proper indentation is used for variables");
                            }
                            catch (UnboundNameException)
                            {
                                // invalid variables found in script, - error caught, displayed and logged
                                Debug.WriteLine("Invalid variables found in python body");
                                MessageBox.Show("Invalid variables found in python body, please try again");
                            }
                            catch (NullReferenceException)
                            {
                                // no return was included in script
                                Debug.WriteLine("Invalid return type found, ensure script has return");
                                MessageBox.Show("No return type found in python script, please return something");
                            }
                        });
                    }
                    // used to update hash table with last block in chain
                    UpdateHashTable();
                    // generate max hash from updated hash table
                    maxHash = GenerateMaxHash();
                    // update the block chain using the previously calculated max hash
                    UpdateBlockchain(maxHash);
                }
                catch (EndpointNotFoundException)
                {
                    if (IsClosed)
                    {
                        // remove client if closed
                        RestRequest removeRequest = new RestRequest("api/Client/Remove/" + portNumber.ToString());
                        client.Get(removeRequest);
                        listOfClients = getClientList();
                    }
                    // log error
                    Debug.WriteLine("Endpoint exception occured, attempting to close client");
                }
                catch (TaskCanceledException)
                {
                    // Dispatcher had to abort due to inactive client, will try again
                    Debug.WriteLine("Dispatcher had to cancel, trying again");
                }
            }
        }

        /// <summary>
        /// Purpose:
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="block"></param>
        /// <param name="prevBlockHash"></param>
        /// <returns>The brute forced hashed string used for the block</returns>
        private string BruteForceHash(out uint offset, Block block, string prevBlockHash)
        {
            string hashString = "";
            SHA256 sha256 = SHA256.Create();
            uint val = 0;
            while (!hashString.StartsWith("12345"))
            {
                // continue running hash until hashed string starts with 12345
                val++;
                string blockString = block.blockID + block.TransactionDetailsList + val + prevBlockHash;
                byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                // hash string
                byte[] hashedData = sha256.ComputeHash(textBytes);
                hashString = BitConverter.ToUInt64(hashedData, 0).ToString();
                // get integer representation of hashed string, if hashed string is invalid, try again with
                // incremented offset
            }
            offset = val;
            // use out parameter to set offset
            return hashString;
        }

        /// <summary>
        /// Purpose: To generate the maximum hash in the hashtable. Used to test if chain is not in sync
        /// </summary>
        /// <returns>Max hash value</returns>
        private string GenerateMaxHash()
        {
            int max;
            string maxHash = "";
            foreach (DictionaryEntry entry in ht)
            {
                // loop through the hash table
                max = (int)entry.Value;
                maxHash = (string)entry.Key;
                // find max value in hash table and return hash string
                if ((int)entry.Value > max)
                {
                    max = (int)entry.Value;
                    maxHash = (string)entry.Key;
                }
            }
            return maxHash;
        }

        /// <summary>
        /// Purpose: The update hash table either appends the hash of the current block into the hash table, 
        /// or append the value to the table if it already exists. This is done for each client so they can
        /// have the same blockchain across all clients that are active
        /// </summary>
        private void UpdateHashTable()
        {
            try
            {
                ChannelFactory<IBlockchain> foobFactory;
                IBlockchain foob;
                listOfClients = getClientList();
                for (int i = 0; i < listOfClients.Count; i++)
                {
                    // loop through all clients and connect to them using Port and IP
                    NetTcpBinding tcp = new NetTcpBinding();
                    string clientURL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                    foobFactory = new ChannelFactory<IBlockchain>(tcp, clientURL);
                    foob = foobFactory.CreateChannel();
                    // if hash table contains client last block hash
                    if (ht.ContainsKey(foob.GetCurrentBlock().blockHash))
                    {
                        // add one to table for existing block hash to store count
                        ht[foob.GetCurrentBlock().blockHash] = (int)ht[foob.GetCurrentBlock().blockHash] + 1;
                    }
                    else
                    {
                        // add the hash and 1 to hash table to indicate existing
                        ht.Add(foob.GetCurrentBlock().blockHash, 1);
                    }
                }
            }
            catch(NullReferenceException)
            {
                Debug.WriteLine("Web server not running, please start the program again with the web server running first");
            }
        }

        /// <summary>
        /// Purpose: Update the blockchain if the chain is out of sync
        /// </summary>
        /// <param name="maxHash"></param>
        private void UpdateBlockchain(string maxHash)
        {
            ChannelFactory<IBlockchain> foobFactory;
            IBlockchain foob;
            if (ht.Count > 0)
            {
                // check hash table is not empty - no blocks
                if (maxHash != Blockchain.BlockChain.Last().blockHash)
                {
                    bool chainChange = false;
                    for (int i = 0; i < listOfClients.Count; i++)
                    {
                        // loop through list of clients and connect to them using port and ip
                        NetTcpBinding tcp = new NetTcpBinding();
                        string clientURL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                        foobFactory = new ChannelFactory<IBlockchain>(tcp, clientURL);
                        foob = foobFactory.CreateChannel();
                        // loop through each clients blockchain
                        foreach (Block block in foob.GetCurrentBlockchain())
                        {
                            if ((block.blockHash == maxHash) && (foob.GetCurrentBlockchain().Count != Blockchain.BlockChain.Count))
                            {
                                // if their blockchain has the max hash in their chain
                                Debug.WriteLine("Chain changed successfully");
                                chainChange = true;
                                Blockchain.BlockChain = foob.GetCurrentBlockchain();
                                // set the current blockchain to the latest clients chain who contains the max hash value
                                Dispatcher.Invoke(() => {
                                    // use dispatcher to access GUI elements in GUI thread
                                    JobsCompleted.Text = Counter.JobCounter.ToString();
                                    NoOfBlocks.Text = foob.GetCurrentBlockchain().Count.ToString();
                                });
                                // exit current loop
                                break;
                            }
                        }
                        if (chainChange)
                        {
                            // if chain has changed, exit loop and method
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Purpose: Create a thread which upates each clients job list every 10 seconds.
        /// </summary>
        private void UpdateList()
        {
            while(true)
            {
                // loop until program ends
                try
                {
                    // sleep for 10 seconds so it updates every 10 seconds
                    Thread.Sleep(10000);
                    Dispatcher.Invoke(() =>
                    {
                        // use dispatcher to invoke GUI thread
                        JobListBox.Items.Clear();
                        foreach (Block block in Blockchain.BlockChain)
                        {
                            // loop through blockchain
                            foreach (string[] jsonTransaction in JsonConvert.DeserializeObject<List<string[]>>(block.TransactionDetailsList))
                            {
                                // loop through JSON list of block
                                foreach (Transaction transaction in TransactionStorage.CompletedTransactions)
                                {
                                    // loop through list of transactions that are submitted
                                    if (Encoding.UTF8.GetString(Convert.FromBase64String(transaction.PythonSrc)) == jsonTransaction[0])
                                    {
                                        // display transaction in job list with client who submitted transaction
                                        JobListBox.Items.Add("Submitted script with result: " + jsonTransaction[1]);
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
                    Debug.WriteLine("Dispatcher had to reload due to losing client connection");
                }
            }
        }
        
        /// <summary>
        /// Purpose: retrieve the client list from the web server
        /// </summary>
        /// <returns>client list</returns>
        private List<Client> getClientList()
        {
            RestRequest request = new RestRequest("api/Client/GetClientList");
            IRestResponse clientList = client.Get(request);
            // use get request to retrieve client list from web server
            List<Client> listOfClients = JsonConvert.DeserializeObject<List<Client>>(clientList.Content);
            // deserialize list and return it
            return listOfClients;
        }

        /// <summary>
        /// Purpose: Remove the client if the window is closing and set the bool condition so client port is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RestRequest removeRequest = new RestRequest("api/Client/Remove/" + portNumber.ToString());
            client.Get(removeRequest);
            // remove client from web service and update client list
            listOfClients = getClientList();
            // close port
            IsClosed = true;
        }
    }
}