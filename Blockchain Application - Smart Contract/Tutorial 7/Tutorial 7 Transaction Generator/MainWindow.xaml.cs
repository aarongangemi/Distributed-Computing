using BlockchainLibrary;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;


namespace Tutorial_7_Transaction_Generator
{
    /// <summary>
    /// Purpose: Interaction logic for MainWindow.xaml. 
    /// The transaction generator allows the user to submit transactions and retrieve an account balance. 
    /// It connects to the miner and server to do this.
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    public partial class MainWindow : Window
    {
        private RestClient ServerClient;
        private RestClient MinerClient;
        private string ServerURL;
        private string MinerURL;
        public static Mutex mutex = new Mutex();
        /// <summary>
        /// Purpose: To display the main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ServerURL = "https://localhost:44353/";
            // connect to server
            MinerURL = "https://localhost:44317/";
            // connect to miner
            ServerClient = new RestClient(ServerURL);
            MinerClient = new RestClient(MinerURL);
            if(GetNoOfBlocks() == 0)
            {
                RestRequest GenesisRequest = new RestRequest("api/Server/GenerateGenesisBlock");
                ServerClient.Post(GenesisRequest);
            }
            NoOfBlocks.Content = GetNoOfBlocks().ToString();

            // display no of blocks which should be 1 because of genesis block
        }

        /// <summary>
        /// Purpose: When the user clicks to submit a transaction, the transaction should be validated and created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Transaction(object sender, RoutedEventArgs e)
        {
            mutex.WaitOne();
            // use mutex to only allow one transaction at a time
            try
            {
                if (!string.IsNullOrEmpty(IdFrom.Text) && !string.IsNullOrEmpty(IdTo.Text) && !string.IsNullOrEmpty(Amount.Text))
                {
                    // check if any fields are null or empty
                    if(IdFrom.Text == IdTo.Text)
                    {
                        MessageBox.Show("You cannot send money to yourself");
                        Debug.WriteLine("User tried to send money to self");
                        // disallow user to send funds to themselves
                    }
                    else
                    {
                        Transaction transaction = new Transaction();
                        transaction.amount = float.Parse(Amount.Text);
                        transaction.walletIdFrom = uint.Parse(IdFrom.Text);
                        transaction.walletIdTo = uint.Parse(IdTo.Text);
                        // use input to generate new transaction
                        RestRequest TransactionRequest = new RestRequest("api/Miner/AddTransaction/");
                        TransactionRequest.AddJsonBody(transaction);
                        // send transaction to miner for processing
                        MinerClient.Post(TransactionRequest);
                    }
                }
                else
                {
                    // if fields are empty then disallow transaction
                    MessageBox.Show("Please ensure all transaction fields are not empty and try again");
                    Debug.WriteLine("error: Unable to complete transaction, fields empty");
                    Debug.WriteLine(".................................................................");
                }
            }
            catch (JsonReaderException)
            {
                // if data could not be read in by JSON
                MessageBox.Show("Invalid data entered in transaction fields, please try again");
                Debug.WriteLine("error: transaction fields contain invalid data");
                Debug.WriteLine(".................................................................");
            }
            catch(FormatException)
            {
                // If the values could not be converted, then invalid data type has been entered
                MessageBox.Show("Unable to parse values. Please ensure amount, sender and receiver are correct data types");
                Debug.WriteLine("Unable to parse values, format exception caught");
            }
            catch (OverflowException)
            {
                // if user enters a negative value
                Debug.WriteLine("Negative value entered, try again");
            }
            // Refresh no of items
            NoOfBlocks.Content = GetNoOfBlocks().ToString();
            // release mutex so other client can submit transaction
            mutex.ReleaseMutex();

        }

        public int GetNoOfBlocks()
        {
            RestRequest NoOfBlocksRequest = new RestRequest("api/Server/GetNoOfBlocks");
            IRestResponse NoOfBlocksResponse = ServerClient.Get(NoOfBlocksRequest);
            return JsonConvert.DeserializeObject<int>(NoOfBlocksResponse.Content);
        }

        /// <summary>
        /// Purpose: If the user clicks "Get Account Balance", then successfully retrieve the account balance from the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_Account_Balance(object sender, RoutedEventArgs e)
        {
            try
            {
                // account balance cannot be empty
                if (!string.IsNullOrEmpty(AcntNo.Text))
                {
                    RestRequest BalanceRequest = new RestRequest("api/Server/GetBalance/" + AcntNo.Text);
                    IRestResponse BalanceRespose = ServerClient.Get(BalanceRequest);
                    // get request to server
                    float Balance = JsonConvert.DeserializeObject<float>(BalanceRespose.Content);
                    AcntBalance.Content = Balance.ToString();
                    // display balance
                    Debug.WriteLine("Account balance retrieved successfully for account " + AcntNo.Text + ". Balance = " + Balance);
                    Debug.WriteLine(".................................................................");
                    // output to console
                }
                else
                {
                    // display alert that field was empty
                    MessageBox.Show("Account ID Field cannot be null or empty");
                    Debug.WriteLine("Error: Account ID field was null or empty");
                    Debug.WriteLine(".................................................................");
                }
            }
            catch (JsonReaderException)
            {
                // if invalid data type was entered then throw error
                MessageBox.Show("Invalid data entered for account ID field, please try again");
                Debug.WriteLine("Account fields contains invalid data");
                Debug.WriteLine(".................................................................");
            }
            catch (OverflowException)
            {
                // account ID cannot be negative
                Debug.WriteLine("Negative value entered, try again");
            }
        }
    }
}
