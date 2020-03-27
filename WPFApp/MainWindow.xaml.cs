using Bis_GUI;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Tutorial_2;
using Web_Service.Models;
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
        private BitmapImage i;
        
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44306/";
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/webapi");
            IRestResponse numOfItems = client.Get(request);
            totalTxt.Text = numOfItems.Content;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            i = new BitmapImage();
            int idx;
            try
            {
                idx = Int32.Parse(IndexVal.Text);
                RestRequest request = new RestRequest("api/webapi/" + idx.ToString());
                IRestResponse response = await client.ExecuteGetAsync(request);
                DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                if (img.Source == null)
                {
                    img.Source = i;
                }

                Fname.Text = dataInter.fname;
                LName.Text = dataInter.lname;
                Balance.Text = dataInter.bal.ToString("C");
                AcntNo.Text = dataInter.acct.ToString();
                PIN.Text = dataInter.pin.ToString("D4");
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid Data was found, please try again");
                Fname.Clear();
                LName.Clear();
                Balance.Clear();
                AcntNo.Clear();
                PIN.Clear();
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BitmapImage image = null;
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp;)|*.jpg; *.jpeg; *.gif; *.bmp;";
            bool? result = open.ShowDialog();
            if (result == true)
            {
                string filePath = open.FileName;
                image = new BitmapImage(new Uri(filePath));
                img.Source = image;
            }

        }

        //The Search Button
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await progressProcessingAsync();
            SearchData mySearch = new SearchData();
            mySearch.searchStr = searchTxt.Text;
            RestRequest request = new RestRequest("api/Search/");
            request.AddJsonBody(mySearch);
            IRestResponse response = await client.ExecutePostAsync(request);
            DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
            Fname.Text = dataInter.fname;
            LName.Text = dataInter.lname;
            Balance.Text = dataInter.bal.ToString("C");
            AcntNo.Text = dataInter.acct.ToString();
            PIN.Text = dataInter.pin.ToString("D4");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            WPFApp.AccessLog objSecondWindow = new WPFApp.AccessLog();
            this.Visibility = Visibility.Hidden;
            objSecondWindow.Show();
        }

        private async Task progressProcessingAsync()
        {
            var progress = new Progress<int>(percent =>
            {
                ProgBar.Value = percent;
                ProgLabel.Content = percent.ToString() + "%";
            });
            await Task.Run(() => processProgress(progress));
        }

        public void processProgress(IProgress<int> progress)
        {
            for (int i = 0; i != 100; i++)
            {
                Thread.Sleep(150);
                if (progress != null)
                {
                    progress.Report(i);
                }
            }
        }
    }
}

