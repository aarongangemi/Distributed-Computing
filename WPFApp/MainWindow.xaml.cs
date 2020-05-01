using Bis_GUI;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    public delegate int SearchOperation(string str);

    public partial class MainWindow : Window
    {
        private string URL;
        private RestClient client;
        private bool found;
        private StreamWriter writer;
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44383/";
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/WebApi");
            IRestResponse numOfItems = client.Get(request);
            NoOfItems.Content = numOfItems.Content;
            found = false;
            
        }

        private async void Click_Go(object sender, RoutedEventArgs e)
        {
            int idx;
            try
            {
                idx = Int32.Parse(IndexVal.Text);
                RestRequest request = new RestRequest("api/webapi/" + idx.ToString());
                IRestResponse response = await client.ExecuteGetAsync(request);
                DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                Fname.Content = "First Name: " + dataInter.fname;
                Lname.Content = "Last Name: " + dataInter.lname;
                Balance.Content = "Balance: " + dataInter.bal.ToString("C");
                AcntNo.Content = "Account No: " + dataInter.acct;
                PIN.Content = "PIN: " + dataInter.pin.ToString("D4");
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/WPFApp/Log file.txt", append: true);
                writer.WriteLine("Log file function: Get Account Details by index");
                writer.WriteLine("Account " + dataInter.acct + " retrieved for: " + dataInter.fname + " " + dataInter.lname + " at index: " + idx.ToString());
                writer.Close();

            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid Data was found, please try again");
                Fname.Content = "First Name:";
                Lname.Content = "Last Name: ";
                Balance.Content = "Balance: ";
                AcntNo.Content = "Account Number";
                PIN.Content = "PIN: ";
                writer.WriteLine("Formatting data error - Error Attempt");
                writer.Close();
            }


        }

        private void Click_Upload_Image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp;)|*.jpg; *.jpeg; *.gif; *.bmp;";
            bool? result = open.ShowDialog();
            if (result == true)
            {
                string filePath = open.FileName;
                BitmapImage image = new BitmapImage(new Uri(filePath));
                img.Source = image;
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/WPFApp/Log file.txt", append: true);
                writer.WriteLine("Log File Function: Upload profile image");
                writer.WriteLine("Image upload from File Path: " + filePath);
                writer.Close();
            }

        }

        //The Search Button
        private async void Click_Search_btn(object sender, RoutedEventArgs e)
        {
            progressProcessingAsync();
            SearchData mySearch = new SearchData();
            mySearch.searchStr = searchTxt.Text;
            RestRequest request = new RestRequest("api/Search/");
            request.AddJsonBody(mySearch);
            IRestResponse response = await client.ExecutePostAsync(request);
            DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
            if (response.IsSuccessful)
            {
                found = true;
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/WPFApp/Log file.txt", append: true);
                writer.WriteLine("Log file function: Search by lastname");
                writer.WriteLine("Account: " + dataInter.acct +
                    " has searched for last name: " + dataInter.lname
                    + " and found a result for: " + dataInter.fname + " " +
                    dataInter.lname);
                writer.Close();
            }
            Fname.Content = "First Name: " + dataInter.fname;
            Lname.Content = "Last Name: " + dataInter.lname;
            Balance.Content = "Balance: " + dataInter.bal.ToString("C");
            AcntNo.Content = "Account Number: " + dataInter.acct.ToString();
            PIN.Content = "PIN: " + dataInter.pin.ToString("D4");

        }



        private async Task progressProcessingAsync()
        {
            ProgBar.Value = 0;
          
            var progress = new Progress<int>(percent =>
            {
                ProgBar.Value = percent;
                ProgLabel.Content = percent.ToString() + "%";
            });
            await Task.Run(() => processProgress(progress));
        }

        public void processProgress(IProgress<int> progress)
        {
            progress.Report(0);
            for (int i = 0; i != 100; i++)
            {
                Thread.Sleep(100);
                if (progress != null)
                {
                    progress.Report(i);
                }
                if(found)
                {
                    progress.Report(100);
                    found = false;
                    break;
                }
            }
        }
    }
}

