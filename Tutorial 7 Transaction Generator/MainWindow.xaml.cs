using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            RestRequest TransactionRequest = new RestRequest("api/Server/GenerateGenesisBlock");
            ServerClient.Post(TransactionRequest);
            NoOfBlocks.Content = GetNoOfBlocks().ToString();
            
        }
        private void Submit_Transaction(object sender, RoutedEventArgs e)
        {
            RestRequest TransactionRequest = new RestRequest("api/Blockchain/AddTransaction/" + IdFrom.Text + "/" + IdTo.Text + "/" + Amount.Text);
            IRestResponse TransactionResponse = MinerClient.Post(TransactionRequest);
            bool TransactionProcessed = JsonConvert.DeserializeObject<bool>(TransactionResponse.Content);
            if(TransactionProcessed)
            {
                MessageBox.Show("Transaction was successful");
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
            if(!string.IsNullOrEmpty(AcntNo.Text))
            {
                RestRequest BalanceRequest = new RestRequest("api/Server/GetBalance/" + AcntNo.Text);
                IRestResponse BalanceRespose = ServerClient.Get(BalanceRequest);
                float Balance = JsonConvert.DeserializeObject<float>(BalanceRespose.Content);
                AcntBalance.Content = Balance.ToString();
            }
            else
            {
                MessageBox.Show("Account ID Field cannot be null or empty");
            }
        }
    }
}
