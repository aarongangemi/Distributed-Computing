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
        public static Hashtable ht;
        private int clientId;
        public bool IsClosed { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44330/";
            client = new RestClient(URL);
            listOfClients = getClientList();
            Thread ServerThread = new Thread(new ThreadStart(BlockchainThread));
            ServerThread.Start();
            Thread NetworkingThread = new Thread(new ThreadStart(MiningThread));
            NetworkingThread.Start();
            portNumber = PortCounter.CurrentPort;
            ht = new Hashtable();
            Blockchain.generateGenesisBlock();
            NoOfBlocks.Content = Blockchain.GetChainCount().ToString();
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
                    Dispatcher.Invoke(() => { ClientID.Content = clientId.ToString(); });
                    request.AddJsonBody(new Client("127.0.0.1", portNumber.ToString(),clientId));
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

        private void Submit_Transaction(object sender, RoutedEventArgs e)
        {
            try
            {
                float amount = float.Parse(Amount.Text);
                uint senderId = uint.Parse(IdFrom.Text);
                uint recieverId = uint.Parse(IdTo.Text);
                if(Blockchain.GetAccountBalance(uint.Parse(IdFrom.Text)) < amount)
                {
                    MessageBox.Show("Insufficient funds, please try again");
                    Debug.WriteLine("Insufficient funds entered");
                }
                else if(amount < 0 || senderId < 0 || recieverId < 0)
                {
                    MessageBox.Show("Invalid data was entered, please try again");
                    Debug.WriteLine("Invalid data was entered");
                }
                else if(clientId != senderId)
                {
                    MessageBox.Show("Cannot send funds, not your wallet. Enter your wallet ID");
                    Debug.WriteLine("Invalid ID entered");
                }
                else
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
                        Transaction transaction = new Transaction();
                        transaction.amount = float.Parse(Amount.Text);
                        transaction.walletIdFrom = uint.Parse(IdFrom.Text);
                        transaction.walletIdTo = uint.Parse(IdTo.Text);
                        foob.RecieveNewTransaction(transaction);
                    }
                }
 
            }
            catch(FormatException)
            {
                MessageBox.Show("Unable to parse data, please try again");
                Debug.WriteLine("Unable to parse data, please try again");
            }
            catch(OverflowException)
            {
                MessageBox.Show("Negative value entered, try again");
                Debug.WriteLine("Negative value entered, try again");
            }
        }

        private void Get_Account_Balance(object sender, RoutedEventArgs e)
        {
            try
            {
                AcntBalance.Content = Blockchain.GetAccountBalance(uint.Parse(AcntNo.Text)).ToString();
            }
            catch(FormatException)
            {
                Debug.WriteLine("Unable to parse data, please try again");
            }
            catch (OverflowException)
            {
                Debug.WriteLine("Negative value entered, try again");
            }
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
                    if (TransactionStorage.TransactionQueue.Count > 0)
                    {
                        uint offset = 0;
                        string prevBlockHash;
                        Transaction transaction = TransactionStorage.TransactionQueue.Dequeue();
                        string hashString = "";
                        float acntBalance = Blockchain.GetAccountBalance(transaction.walletIdFrom);
                        if (transaction.amount > 0 && transaction.walletIdFrom >= 0 && transaction.walletIdTo >= 0 && transaction.amount <= acntBalance)
                        {
                            Debug.WriteLine("Adding new block to blockchain");
                            Debug.WriteLine("Wallet ID from = " + transaction.walletIdFrom);
                            Debug.WriteLine("Wallet ID to = " + transaction.walletIdTo);
                            Debug.WriteLine("Transaction amount = " + transaction.amount);
                            SHA256 sha256 = SHA256.Create();
                            prevBlockHash = Blockchain.BlockChain.Last().blockHash;
                            while (!hashString.StartsWith("12345"))
                            {
                                offset++;
                                string blockString = transaction.walletIdFrom.ToString() + transaction.walletIdTo.ToString() + transaction.amount.ToString() + offset + prevBlockHash;
                                byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                                byte[] hashedData = sha256.ComputeHash(textBytes);
                                hashString = BitConverter.ToUInt64(hashedData, 0).ToString();
                            }
                            Debug.WriteLine("Offset = " + offset);
                            Debug.WriteLine("Block hash = " + hashString);
                            Block block = new Block(transaction.walletIdFrom, transaction.walletIdTo, transaction.amount, offset, prevBlockHash, hashString);
                            if (Blockchain.ValidateBlock(block))
                            {
                                Blockchain.AddBlock(block);
                                Debug.WriteLine("Block successfully added");
                                Debug.WriteLine("..............................................");
                                Dispatcher.Invoke(() => { NoOfBlocks.Content = Blockchain.GetChainCount(); });
                            }
                            else
                            {
                                Debug.WriteLine("Validation for block failed, Trying again");
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
                                        Dispatcher.Invoke(() => { NoOfBlocks.Content = Blockchain.GetChainCount().ToString(); });
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
                catch (CommunicationException) 
                {
                    Debug.WriteLine("Re-establishing communication, one sec");
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
