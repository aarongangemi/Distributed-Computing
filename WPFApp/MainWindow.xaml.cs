using Bis_GUI;
using java.math;
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
        private static System.Timers.Timer timer = new System.Timers.Timer();
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44383/";
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/WebApi");
            IRestResponse numOfItems = client.Get(request);
            NoOfItems.Content = numOfItems.Content;
            img.Source = new BitmapImage(new Uri("C:/WebStuff/ProfileImage.jpg"));
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
                    fnameField.Text = dataInter.fname;
                    lnameField.Text = dataInter.lname;
                    balanceField.Text = dataInter.bal.ToString();
                    acntNoField.Text = dataInter.acct.ToString();
                    pinField.Text = dataInter.pin.ToString("D4");
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
                fnameField.Text = "First Name";
                lnameField.Text = "Last Name";
                balanceField.Text = "Balance";
                IndexVal.Text = "Enter Index";
                acntNoField.Text = "Account Number";
                pinField.Text = "PIN";
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
                        fnameField.Text = dataInter.fname;
                        lnameField.Text = dataInter.lname;
                        balanceField.Text = dataInter.bal.ToString();
                        acntNoField.Text = dataInter.acct.ToString();
                        pinField.Text = dataInter.pin.ToString("D4");
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
                    fnameField.Text = dataInter.fname;
                    lnameField.Text = dataInter.lname;
                    balanceField.Text = dataInter.bal.ToString();
                    acntNoField.Text = dataInter.acct.ToString();
                    pinField.Text = dataInter.pin.ToString("D4");
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

        private void progressProcessingAsync()
        {
            ProgBar.Value = 0;
            var progress = new Progress<int>(percent =>
            {
                ProgBar.Value = percent;
                ProgLabel.Content = percent.ToString() + "%";
            });
            Task.Run(() => processProgress(progress));
        }

        /***************************************************************
         * Purpose: process the progress bar and add a timer to have a timeout if results aren't found
         * Reference for timer functionality: https://www.tutorialspoint.com/Timer-in-Chash
         * Author: Karthikeya Boyini
         * Date Accessed: 03/05/2020
         * ***************************************************************/
        public void processProgress(IProgress<int> progress)
        {
            timer.Interval = 30000;
            timer.Elapsed += OnTimerEnd;
            timer.AutoReset = true;
            timer.Enabled = true;
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

        private void OnTimerEnd(Object source, System.Timers.ElapsedEventArgs e)
        {
            found = true;
            ProgLabel.Content = "Unable to find search string in database, please try again.";
        }

        private void Click_Update_User(object sender, RoutedEventArgs e)
        {
            try
            {
                RestRequest request = new RestRequest("api/webapi/");
                UpdatedUser user = new UpdatedUser();
                user.index = Int32.Parse(IndexVal.Text);
                if (validateTextFields())
                {
                    user.fname = fnameField.Text;
                    user.lname = lnameField.Text;
                    user.acct = Convert.ToUInt32(acntNoField.Text);
                    user.bal = Convert.ToInt32(balanceField.Text);
                    user.pin = Convert.ToUInt32(pinField.Text);
                    request.AddJsonBody(user);
                    IRestResponse response = client.Put(request);
                    DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                    fnameField.Text = dataInter.fname;
                    lnameField.Text = dataInter.lname;
                    acntNoField.Text = dataInter.acct.ToString();
                    balanceField.Text = dataInter.bal.ToString();
                    pinField.Text = dataInter.pin.ToString();
                    AccountStatusLabel.Content = "Account Updated Successfully";
                    log.updateAccount(user);
                }
                else
                {
                    MessageBox.Show("Please ensure all user fields are not empty and all fields have correct data types");
                }
            }
            catch(FormatException)
            {
                MessageBox.Show("Invalid data entered, please ensure index and data fields are completed successfully");
            }

        }

        private bool validateTextFields()
        {
            if (Int32.Parse(IndexVal.Text) >= 0 && Int32.Parse(IndexVal.Text) < Int32.Parse(NoOfItems.Content.ToString())
                && !fnameField.Text.Equals("") && !lnameField.Text.Equals("") && !pinField.Text.Equals("") &&
                !balanceField.Text.Equals("") && !acntNoField.Text.Equals(""))
            {
                return true;
            }
            return false;
        }
    }
}

