using BlockchainLibrary;
using Newtonsoft.Json;
using RestSharp;
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
        private static Mutex mutex = new Mutex();
        private LogBlockchain log;
        public MainWindow()
        {
            InitializeComponent();
            log = new LogBlockchain();
            ServerURL = "https://localhost:44353/";
            MinerURL = "https://localhost:44317/";
            ServerClient = new RestClient(ServerURL);
            MinerClient = new RestClient(MinerURL);
            RestRequest TransactionRequest = new RestRequest("api/Server/GenerateGenesisBlock");
            ServerClient.Post(TransactionRequest);
            NoOfBlocks.Content = GetNoOfBlocks().ToString();
            log.logData("Genesis block created");
            log.logData(".................................................................");
        }
        private async void Submit_Transaction(object sender, RoutedEventArgs e)
        {
            mutex.WaitOne();
            try
            {
                if (!string.IsNullOrEmpty(IdFrom.Text) && !string.IsNullOrEmpty(IdTo.Text) && !string.IsNullOrEmpty(Amount.Text))
                {
                    RestRequest TransactionRequest = new RestRequest("api/Blockchain/AddTransaction/" + IdFrom.Text + "/" + IdTo.Text + "/" + Amount.Text);
                    IRestResponse TransactionResponse = await MinerClient.ExecutePostAsync(TransactionRequest);
                    bool TransactionProcessed = JsonConvert.DeserializeObject<bool>(TransactionResponse.Content);
                    if (TransactionProcessed)
                    {
                        MessageBox.Show("Transaction was successful");
                        NoOfBlocks.Content = GetNoOfBlocks().ToString();   
                    }
                    else
                    {
                        MessageBox.Show("Transaction was unsuccessful. Please check: \n" +
                                        "1. The amount is less than or equal to the senders balance \n" +
                                        "2. The amount being sent is greater than 0 \n" +
                                        "3. The wallet ID From and wallet ID To are greater than 0 \n" +
                                        "4. The amount is greater than 0 \n" +
                                        "5. The wallet ID from is greater than 0 \n");
                        log.logData("Transaction unsuccessful - Insufficient funds, amount or ID's entered");
                        log.logData(".................................................................");
                    }
                }
                else
                {
                    MessageBox.Show("Please ensure all transaction fields are not empty and try again");
                    log.logData("error: Unable to complete transaction, fields empty");
                    log.logData(".................................................................");
                }
            }
            catch(JsonReaderException)
            {
                MessageBox.Show("Invalid data entered in transaction fields, please try again");
                log.logData("error: transaction fields contain invalid data");
                log.logData(".................................................................");
            }
            mutex.ReleaseMutex();
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
                    log.logData("Account balance retrieved successfully for account " + AcntNo.Text + ". Balance = " + Balance);
                    log.logData(".................................................................");
                }
                else
                {
                    MessageBox.Show("Account ID Field cannot be null or empty");
                    log.logData("Error: Account ID field was null or empty");
                    log.logData(".................................................................");
                }
            }
            catch(JsonReaderException)
            {
                MessageBox.Show("Invalid data entered for account ID field, please try again");
                log.logData("Account fields contains invalid data");
                log.logData(".................................................................");
            }
        }
    }
}
