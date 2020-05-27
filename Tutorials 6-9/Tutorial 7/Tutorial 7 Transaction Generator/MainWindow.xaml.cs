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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RestClient ServerClient;
        private RestClient MinerClient;
        private string ServerURL;
        private string MinerURL;
        public MainWindow()
        {
            InitializeComponent();
            ServerURL = "https://localhost:44353/";
            MinerURL = "https://localhost:44317/";
            ServerClient = new RestClient(ServerURL);
            MinerClient = new RestClient(MinerURL);
            NoOfBlocks.Content = GetNoOfBlocks().ToString();
            Debug.WriteLine("Genesis block created");
            Debug.WriteLine(".................................................................");
        }

        private void Submit_Transaction(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(IdFrom.Text) && !string.IsNullOrEmpty(IdTo.Text) && !string.IsNullOrEmpty(Amount.Text))
                {
                    Transaction transaction = new Transaction();
                    transaction.amount = float.Parse(Amount.Text);
                    transaction.walletIdFrom = uint.Parse(IdFrom.Text);
                    transaction.walletIdTo = uint.Parse(IdTo.Text);
                    RestRequest TransactionRequest = new RestRequest("api/Miner/AddTransaction/");
                    TransactionRequest.AddJsonBody(transaction);
                    MinerClient.Post(TransactionRequest);
                }
                else
                {
                    MessageBox.Show("Please ensure all transaction fields are not empty and try again");
                    Debug.WriteLine("error: Unable to complete transaction, fields empty");
                    Debug.WriteLine(".................................................................");
                }
            }
            catch (JsonReaderException)
            {
                MessageBox.Show("Invalid data entered in transaction fields, please try again");
                Debug.WriteLine("error: transaction fields contain invalid data");
                Debug.WriteLine(".................................................................");
            }
            catch(FormatException)
            {
                MessageBox.Show("Unable to parse values. Please ensure amount, sender and receiver are correct data types");
                Debug.WriteLine("Unable to parse values, format exception caught");
            }
            catch (OverflowException)
            {
                Debug.WriteLine("Negative value entered, try again");
            }
            NoOfBlocks.Content = GetNoOfBlocks().ToString();

        }

        public int GetNoOfBlocks()
        {
            RestRequest NoOfBlocksRequest = new RestRequest("api/Server/GetNoOfBlocks");
            IRestResponse NoOfBlocksResponse = ServerClient.Get(NoOfBlocksRequest);
            return JsonConvert.DeserializeObject<int>(NoOfBlocksResponse.Content);
        }

        private void Get_Account_Balance(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(AcntNo.Text))
                {
                    RestRequest BalanceRequest = new RestRequest("api/Server/GetBalance/" + AcntNo.Text);
                    IRestResponse BalanceRespose = ServerClient.Get(BalanceRequest);
                    float Balance = JsonConvert.DeserializeObject<float>(BalanceRespose.Content);
                    AcntBalance.Content = Balance.ToString();
                    Debug.WriteLine("Account balance retrieved successfully for account " + AcntNo.Text + ". Balance = " + Balance);
                    Debug.WriteLine(".................................................................");
                }
                else
                {
                    MessageBox.Show("Account ID Field cannot be null or empty");
                    Debug.WriteLine("Error: Account ID field was null or empty");
                    Debug.WriteLine(".................................................................");
                }
            }
            catch (JsonReaderException)
            {
                MessageBox.Show("Invalid data entered for account ID field, please try again");
                Debug.WriteLine("Account fields contains invalid data");
                Debug.WriteLine(".................................................................");
            }
            catch (OverflowException)
            {
                Debug.WriteLine("Negative value entered, try again");
            }
        }
    }
}
