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
using Tutorial_8_Blockchain_Library;

namespace ClientApp
{
    /// <summary>
    /// Purpose: Interaction logic for MainWindow.xaml. Used to start the server and mining thread.
    /// Also used to retrieve the balance and submit a new transaction.
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void ServerDel();
        private delegate void NetworkDel();
        private RestClient client;
        private string URL;
        private int portNumber;
        private List<Client> listOfClients;
        public static Hashtable ht;
        private int clientId;
        public static Mutex mutex = new Mutex();
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Purpose: To start up the main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44330/";
            client = new RestClient(URL);
            listOfClients = getClientList();
            // start server thread
            Thread ServerThread = new Thread(new ThreadStart(BlockchainThread));
            ServerThread.Start();
            // start mining thread
            Thread NetworkingThread = new Thread(new ThreadStart(MiningThread));
            NetworkingThread.Start();
            // set current client port
            portNumber = PortCounter.CurrentPort;
            ht = new Hashtable();
            // generate genesis block on start up
            Blockchain.generateGenesisBlock();
            // display number of blocks in chain
            NoOfBlocks.Content = Blockchain.GetChainCount().ToString();
        }

        /// <summary>
        /// Purpose: The server thread is used to start a .NET remoting server for each client
        /// </summary>
        private void BlockchainThread()
        {
            // bool to indicate if server exists
            bool ServerCreated = false;
            ServiceHost host;
            while (!ServerCreated)
            {
                try
                {
                    host = new ServiceHost(typeof(BlockchainHost)); //Service host in OS
                    NetTcpBinding tcp = new NetTcpBinding();  //Create .NET TCP port
                    host.AddServiceEndpoint(typeof(IBlockchain), tcp, "net.tcp://127.0.0.1:" + portNumber + "/BlockchainServerHost");
                    // add client endpoint using port number
                    host.Open();
                    RestRequest request = new RestRequest("api/Client/Register/");
                    PortCounter.ClientCounter++;
                    clientId = PortCounter.ClientCounter;
                    // register client
                    Dispatcher.Invoke(() => { ClientID.Content = clientId.ToString(); });
                    // display client ID
                    request.AddJsonBody(new Client("127.0.0.1", portNumber.ToString(),clientId));
                    client.Post(request);
                    // register client with web server
                    Debug.WriteLine(portNumber.ToString());
                    ServerCreated = true;
                    // indicate server is created
                    while (!IsClosed)
                    { }
                    // use empty while loop to leave server open, will close when window closes which is
                    // indicate by bool condition
                    host.Close();
                }
                catch (AddressAlreadyInUseException)
                {
                    // if address is already in use, increment client port number and try again
                    PortCounter.CurrentPort++;
                    portNumber = PortCounter.CurrentPort;
                    PortCounter.ClientCounter++;
                    clientId = PortCounter.ClientCounter;
                    // increment client ID
                    host = new ServiceHost(typeof(BlockchainHost));
                }
            }
        }

        /// <summary>
        /// Purpose: If the user clicks the submit transaction button, then process their input and try create a new transaction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Transaction(object sender, RoutedEventArgs e)
        {
            // Hold mutex so only one transaction can be processed at a time
            mutex.WaitOne();
            try
            {
                float amount = float.Parse(Amount.Text);
                uint senderId = uint.Parse(IdFrom.Text);
                uint recieverId = uint.Parse(IdTo.Text);
                // parse all values to correct data types
                if(Blockchain.GetAccountBalance(uint.Parse(IdFrom.Text)) < amount)
                {
                    // check sender has sufficient funds
                    MessageBox.Show("Insufficient funds, please try again");
                    Debug.WriteLine("Insufficient funds entered");
                }
                else if(amount < 0 || senderId < 0 || recieverId < 0)
                {
                    // check no values are negative
                    MessageBox.Show("Invalid data was entered, please try again");
                    Debug.WriteLine("Invalid data was entered");
                }
                else if(senderId == recieverId)
                {
                    // check the sender does not send to themselves
                    MessageBox.Show("Sender ID and reciever ID cannot be the same");
                    Debug.WriteLine("Sender ID and reciever ID cannot be the same");
                }
                else if(clientId != senderId)
                {
                    // only client can send from their own account
                    MessageBox.Show("Cannot send funds, not your wallet. Enter your wallet ID");
                    Debug.WriteLine("Invalid ID entered");
                }
                else
                {
                    // if all above conditions are false, the process transaction
                    NetTcpBinding tcp = new NetTcpBinding();
                    string URL;
                    ChannelFactory<IBlockchain> foobFactory;
                    IBlockchain foob;
                    listOfClients = getClientList();
                    // refresh client list
                    for (int i = 0; i < listOfClients.Count; i++)
                    {
                        // loop through client list and connect to client
                        URL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                        foobFactory = new ChannelFactory<IBlockchain>(tcp, URL);
                        foob = foobFactory.CreateChannel();
                        // create new transaction and set details
                        Transaction transaction = new Transaction();
                        transaction.amount = float.Parse(Amount.Text);
                        transaction.walletIdFrom = uint.Parse(IdFrom.Text);
                        transaction.walletIdTo = uint.Parse(IdTo.Text);
                        // add transaction to queue
                        foob.RecieveNewTransaction(transaction);
                    }
                }
            }
            catch(FormatException)
            {
                // catch format exception if the values being parsed are not of correct data type
                MessageBox.Show("Unable to parse data, please try again");
                Debug.WriteLine("Unable to parse data, please try again");
            }
            catch(OverflowException)
            {
                // if any negative values are entered
                MessageBox.Show("Negative value entered, try again");
                Debug.WriteLine("Negative value entered, try again");
            }
            // allow other transaction to be processed by releasing mutex
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Purpose: To retrieve the account balance using the parsed in value. Used when user clicks to retrieve account balance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_Account_Balance(object sender, RoutedEventArgs e)
        {
            try
            {
                AcntBalance.Content = Blockchain.GetAccountBalance(uint.Parse(AcntNo.Text)).ToString();
                // get account balance
            }
            catch(FormatException)
            {
                // if values could not be parsed, catch format exception
                MessageBox.Show("Unable to parse value, incorrect data type, try again");
                Debug.WriteLine("Unable to parse data, please try again");
            }
            catch (OverflowException)
            {
                // If account ID entered is negative value
                Debug.WriteLine("Negative value entered, try again");
            }
        }

        /// <summary>
        /// Purpose: This is the mining thread, used to mine blocks and process submitted transactions
        /// </summary>
        private void MiningThread()
        {
            while (true)
            {
                // leave loop running forever
                try
                {
                    // get latest list of clients on every iteration
                    listOfClients = getClientList();
                    // check if transaction exists to process
                    if (TransactionStorage.TransactionQueue.Count > 0)
                    {
                        uint offset = 0;
                        string prevBlockHash;
                        Transaction transaction = TransactionStorage.TransactionQueue.Dequeue();
                        string hashString = "";
                        // get account balance to do validation of account input
                        float acntBalance = Blockchain.GetAccountBalance(transaction.walletIdFrom);
                        if (transaction.amount > 0 && transaction.walletIdFrom >= 0 && transaction.walletIdTo >= 0 && transaction.amount <= acntBalance)
                        {
                            // check values are positive and sender has sufficient funds
                            Debug.WriteLine("Adding new block to blockchain");
                            Debug.WriteLine("Wallet ID from = " + transaction.walletIdFrom);
                            Debug.WriteLine("Wallet ID to = " + transaction.walletIdTo);
                            Debug.WriteLine("Transaction amount = " + transaction.amount);
                            // log data to console
                            SHA256 sha256 = SHA256.Create();
                            prevBlockHash = Blockchain.BlockChain.Last().blockHash;
                            // set last block hash
                            while (!hashString.StartsWith("12345"))
                            {
                                // brute force a hash to start with 12345
                                offset++;
                                string blockString = transaction.walletIdFrom.ToString() + transaction.walletIdTo.ToString() + transaction.amount.ToString() + offset + prevBlockHash;
                                byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                                byte[] hashedData = sha256.ComputeHash(textBytes);
                                // get integer representation of a hash
                                hashString = BitConverter.ToUInt64(hashedData, 0).ToString();
                                // if hash is invalid, increment offset and try again
                            }
                            Debug.WriteLine("Offset = " + offset);
                            Debug.WriteLine("Block hash = " + hashString);
                            // log console values
                            Block block = new Block(transaction.walletIdFrom, transaction.walletIdTo, transaction.amount, offset, prevBlockHash, hashString);
                            // create block after validation
                            if (Blockchain.ValidateBlock(block))
                            {
                                // validate block, if valid then add to blockchain
                                Blockchain.AddBlock(block);
                                Debug.WriteLine("Block successfully added");
                                Debug.WriteLine("..............................................");
                                Dispatcher.Invoke(() => { NoOfBlocks.Content = Blockchain.GetChainCount(); });
                                // update chain count
                            }
                            else
                            {
                                // if validation failed, then display error
                                Debug.WriteLine("Validation for block failed, Trying again");
                            }
                        }
                    }
                    UpdateHashTable();
                    // add block to hash table, or update existing block
                    string maxHash = GetMaxHash();
                    // retrieve the max hash from the hash table
                    if (ht.Count > 0)
                    {
                        if (maxHash != Blockchain.BlockChain.Last().blockHash)
                        {
                            // if max hash doesnt equal the last block in the chain, then update the blockchain
                            // to client chain that does have hash
                            UpdateChain(maxHash);
                        }
                    }
                }
                catch (EndpointNotFoundException)
                {
                    // if client is closed, then remove them from the list
                    if (IsClosed)
                    {
                        Debug.WriteLine(portNumber.ToString());
                        RestRequest removeRequest = new RestRequest("api/Client/Remove/" + portNumber.ToString());
                        client.Get(removeRequest);
                        // remove client and refresh list
                        listOfClients = getClientList();
                    }
                }
                catch (TaskCanceledException) 
                {
                    // if dispatcher has to abort, then try again
                    Debug.WriteLine("Dispatcher had to cancel, trying again");
                }
                catch (CommunicationException) 
                {
                    // if communication fails, then it will try again with another client to connect
                    Debug.WriteLine("Re-establishing communication, one sec");
                }
                
            }
        }

        /// <summary>
        /// Purpose: To update the hash tabl using the last block in the chain
        /// </summary>
        private void UpdateHashTable()
        {
            // loop through clients
            ChannelFactory<IBlockchain> foobFactory;
            IBlockchain foob;
            for (int i = 0; i < listOfClients.Count; i++)
            {
                NetTcpBinding tcp = new NetTcpBinding();
                // connect to client using port and ip
                string clientURL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                foobFactory = new ChannelFactory<IBlockchain>(tcp, clientURL);
                foob = foobFactory.CreateChannel();
                // check if the last block is in the hash table
                if (ht.ContainsKey(foob.GetCurrentBlock().blockHash))
                {
                    // if block is in table, then increment value by 1
                    ht[foob.GetCurrentBlock().blockHash] = (int)ht[foob.GetCurrentBlock().blockHash] + 1;
                }
                else
                {
                    // if not in table, then add block to table with value of 1
                    ht.Add(foob.GetCurrentBlock().blockHash, 1);
                }
            }
        }
        
        /// <summary>
        /// Purpose: If the max hash that was calculated is not in the current chain, then get a client chain and reset blockchain
        /// </summary>
        /// <param name="maxHash"></param>
        private void UpdateChain(string maxHash)
        {
            ChannelFactory<IBlockchain> foobFactory;
            IBlockchain foob;
            bool chainChange = false;
            // loop through clients
            for (int i = 0; i < listOfClients.Count; i++)
            {
                NetTcpBinding tcp = new NetTcpBinding();
                // connect to client using port and ip
                string clientURL = "net.tcp://" + listOfClients.ElementAt(i).IpAddress.ToString() + ":" + listOfClients.ElementAt(i).port.ToString() + "/BlockchainServerHost";
                foobFactory = new ChannelFactory<IBlockchain>(tcp, clientURL);
                foob = foobFactory.CreateChannel();
                // loop through blocks in blockchain
                foreach (Block block in foob.GetCurrentBlockchain())
                {
                    // check max hash exists in blockchain and number of blocks is different between client and server chain
                    if ((block.blockHash == maxHash) && (foob.GetCurrentBlockchain().Count != Blockchain.BlockChain.Count))
                    {
                        Debug.WriteLine("Chain changed successfully");
                        // update chain
                        chainChange = true;
                        Blockchain.BlockChain = foob.GetCurrentBlockchain();
                        // display new count
                        Dispatcher.Invoke(() => { NoOfBlocks.Content = Blockchain.GetChainCount().ToString(); });
                        break;
                    }
                }
                if (chainChange)
                {
                    // if the chain has change, end all loops
                    break;
                }
            }
        }

        /// <summary>
        /// Purpose: retrieve the max hash from the hash table
        /// </summary>
        /// <returns>Max hash in table</returns>
        private string GetMaxHash()
        {
            int max;
            string maxHash = "";
            foreach (DictionaryEntry entry in ht)
            {
                // loop through hash table
                max = (int)entry.Value;
                maxHash = (string)entry.Key;
                // get max hash value
                if ((int)entry.Value > max)
                {
                    // if max is greater than previous max, then update max and max hash
                    max = (int)entry.Value;
                    maxHash = (string)entry.Key;
                }
            }
            return maxHash;
        }

        /// <summary>
        /// Purpose: to retrieve the last client list from the client controller
        /// </summary>
        /// <returns>Client list</returns>
        private List<Client> getClientList()
        {
            RestRequest request = new RestRequest("api/Client/GetClientList");
            IRestResponse clientList = client.Get(request);
            List<Client> listOfClients = JsonConvert.DeserializeObject<List<Client>>(clientList.Content);
            // get client list
            return listOfClients;
        }

        /// <summary>
        /// Purpose: set bool condition to close client port if window is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsClosed = true;
        }
    }
}
