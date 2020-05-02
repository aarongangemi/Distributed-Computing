using Bis_GUI;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
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
        private LogData log;
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44383/";
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/WebApi");
            IRestResponse numOfItems = client.Get(request);
            NoOfItems.Content = numOfItems.Content;
            found = false;
            log = new LogData();
        }

        private async void Click_Go(object sender, RoutedEventArgs e)
        {
            int idx;
            try
            {
                idx = Int32.Parse(IndexVal.Text);
                if (idx >= 0  && idx < Int32.Parse(NoOfItems.Content.ToString()))
                {
                    RestRequest request = new RestRequest("api/webapi/" + idx.ToString());
                    IRestResponse response = await client.ExecuteGetAsync(request);
                    DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                    Fname.Content = "First Name: " + dataInter.fname;
                    Lname.Content = "Last Name: " + dataInter.lname;
                    Balance.Content = "Balance: " + dataInter.bal.ToString("C");
                    AcntNo.Content = "Account No: " + dataInter.acct;
                    PIN.Content = "PIN: " + dataInter.pin.ToString("D4");
                    BitmapImage image = new BitmapImage(new Uri(dataInter.filePath));
                    img.Source = image;
                    log.logIndexSearch(dataInter);

                }
                else
                {
                    throw new FormatException("Index out of range");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid Data was found, please try again");
                Fname.Content = "First Name:";
                Lname.Content = "Last Name: ";
                Balance.Content = "Balance: ";
                IndexVal.Text = "Enter Index";
                AcntNo.Content = "Account Number";
                PIN.Content = "PIN: ";
            }
            catch (HttpException)
            {
                MessageBox.Show("Http Exception raised, please try again");
            }


        }

        private void Click_Upload_Image(object sender, RoutedEventArgs e)
        {
            try
            { 
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp;)|*.jpg; *.jpeg; *.gif; *.bmp;";
                bool? result = open.ShowDialog();
                if (result == true)
                {
                    int idx = Int32.Parse(IndexVal.Text);
                    if (idx >= 0 && idx < Int32.Parse(NoOfItems.Content.ToString()))
                    {
                        FilePathData file = new FilePathData();
                        string filePath = Path.GetFullPath(open.FileName);
                        file.filePath = filePath;
                        file.indexToUpdate = idx;
                        RestRequest request = new RestRequest("api/webapi/");
                        request.AddJsonBody(file);
                        IRestResponse response = client.Post(request);
                        DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                        Fname.Content = "First Name: " + dataInter.fname;
                        Lname.Content = "Last Name: " + dataInter.lname;
                        Balance.Content = "Balance: " + dataInter.bal.ToString("C");
                        AcntNo.Content = "Account No: " + dataInter.acct;
                        PIN.Content = "PIN: " + dataInter.pin.ToString("D4");
                        BitmapImage image = new BitmapImage(new Uri(dataInter.filePath));
                        img.Source = image;
                        log.logImageUpload(filePath);
                    }
                    else
                    {
                        throw new FormatException("Index out of range, please try again");
                    }
                }
            }
            catch(FormatException)
            {
                MessageBox.Show("Invalid index entered, please try again");
            }

        }

        //The Search Button
        private async void Click_Search_btn(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchLabel.Content = "Searching 100000 results for Last name";
                if (searchTxt.Text.Length == 0)
                {
                    throw new FormatException("Cannot allow empty string as last name, try again");
                }
                var regex = new Regex("^[a-zA-Z]*$");
                if(!regex.IsMatch(searchTxt.Text))
                {
                    throw new FormatException("Cannot allow illegal characters");
                }
                progressProcessingAsync();
                SearchData mySearch = new SearchData();
                mySearch.searchStr = searchTxt.Text;
                RestRequest request = new RestRequest("api/Search/");
                request.AddJsonBody(mySearch);
                IRestResponse response = await client.ExecutePostAsync(request);
                DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                if (dataInter != null)
                {
                    SearchLabel.Content = "Search Complete";
                    found = true;
                    Fname.Content = "First Name: " + dataInter.fname;
                    Lname.Content = "Last Name: " + dataInter.lname;
                    Balance.Content = "Balance: " + dataInter.bal.ToString("C");
                    AcntNo.Content = "Account Number: " + dataInter.acct.ToString();
                    PIN.Content = "PIN: " + dataInter.pin.ToString("D4");
                    img.Source = new BitmapImage(new Uri(dataInter.filePath));
                    log.logSearch(dataInter);
                }
                else
                {
                    ProgBar.Value = 100;
                    SearchLabel.Content = "Search Complete";
                    throw new FormatException("Unable to find last name provided, try again");
                }

            }
            catch(FormatException)
            {
                MessageBox.Show("Please enter a valid last name and try again");
                ProgBar.Value = 100;
                searchTxt.Text = "Enter Last Name";
            }
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
                Thread.Sleep(1500);
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

