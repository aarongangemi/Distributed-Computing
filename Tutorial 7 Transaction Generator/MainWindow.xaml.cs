using BlockchainIntermed;
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
        private string URL;
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44353/";
            ServerClient = new RestClient(URL);
            NoOfBlocks.Content = GetNoOfBlocks().ToString();
            RegisterAccounts();
            List<Account> acntList = GetAccountList();
            foreach(Account a in acntList)
            {
                AcntField.Text = AcntField.Text + "\n" + "Account ID: " + a.accountID + ", Amount = $" + a.accountAmount.ToString();
            }
            
        }
        private void Submit_Transaction(object sender, RoutedEventArgs e)
        {

        }

        public int GetNoOfBlocks() 
        {
            RestRequest NoOfBlocksRequest = new RestRequest("api/Server/GetNoOfBlocks");
            IRestResponse NoOfBlocksResponse = ServerClient.Get(NoOfBlocksRequest);
            return JsonConvert.DeserializeObject<int>(NoOfBlocksResponse.Content);
        }

        public List<Account> GetAccountList()
        {
            RestRequest AcntRequest = new RestRequest("api/Server/GetAccounts");
            IRestResponse AcntResponse = ServerClient.Get(AcntRequest);
            return JsonConvert.DeserializeObject<List<Account>>(AcntResponse.Content);
        }

        public void RegisterAccounts()
        {
            for (int i = 0; i < 15; i++)
            {
                RestRequest AcntRequest = new RestRequest("api/Server/AddAcnt");
                ServerClient.Post(AcntRequest);
            }
        }

    }
}
