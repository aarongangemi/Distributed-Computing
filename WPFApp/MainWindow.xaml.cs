using Bis_GUI;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WPFApp
{
    public partial class MainWindow : Window
    {
        private string URL;
        private RestClient client;
        private bool found;
        private LogData log;
        private static System.Timers.Timer timer;
        private bool timerEnded;
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44383/";
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/WebApi");
            IRestResponse numOfItems = client.Get(request);
            NoOfItems.Content = numOfItems.Content;
            img.Source = new BitmapImage(new Uri(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Images/ProfileImage.jpg")));
            found = false;
            log = new LogData();
            timerEnded = false;
            timer = new System.Timers.Timer();
        }

        private async void Click_Go(object sender, RoutedEventArgs e)
        {
            int idx;
            try
            {
                idx = Int32.Parse(IndexVal.Text);
                if (idx >= 0  && idx < Int32.Parse(NoOfItems.Content.ToString()))
                {
                    AccountStatusLabel.Content = "";
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
                    log.errorLogMessage("Index could not be passed successfully, " +
                        "please ensure index is > 0 and < 100,000, and is a valid integer. " +
                        "Fields will be cleared");
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
                        log.errorLogMessage("Index could not be passed successfully, " +
                        "please ensure index is > 0 and < 100,000, and is a valid integer. " +
                        "Fields will be cleared");
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
                SearchBtn.IsEnabled = false;
                SearchLabel.Content = "Searching results for Last name";
                if (searchTxt.Text.Length == 0)
                {
                    log.errorLogMessage("Last name cannot be empty, user must enter valid last name");
                    throw new FormatException("Cannot allow empty string as last name, try again");
                }
                var regex = new Regex("^[a-zA-Z]*$");
                if(!regex.IsMatch(searchTxt.Text))
                {
                    log.errorLogMessage("Invalid last name was entered, last name consists of invalid characters. " +
                        "Ensure last name is only letters from the alphabet");
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
                    IndexVal.Text = dataInter.index.ToString();
                    fnameField.Text = dataInter.fname;
                    lnameField.Text = dataInter.lname;
                    balanceField.Text = dataInter.bal.ToString();
                    acntNoField.Text = dataInter.acct.ToString();
                    pinField.Text = dataInter.pin.ToString("D4");
                    img.Source = new BitmapImage(new Uri(dataInter.filePath));
                    log.logSearch(dataInter);
                    SearchBtn.IsEnabled = true;
                }
                else
                {
                    throw new NullReferenceException("Unable to find an entry with that lastname in the database");
                }
            }
            catch(FormatException x)
            {
                MessageBox.Show("Please enter a valid last name and try again");
                SearchLabel.Content = "Invalid last name entered, please try again";
                ProgBar.Value = 100;
                searchTxt.Text = "Enter Last Name";
                SearchBtn.IsEnabled = true;
                log.errorLogMessage(x.Message);
            }
            catch(NullReferenceException y)
            {
                MessageBox.Show("Was unable to find last name for entry, please try again");
                SearchLabel.Content = "Invalid last name entered, please try again";
                ProgBar.Value = 100;
                SearchLabel.Content = "Search Complete";
                searchTxt.Text = "Enter Last Name";
                SearchBtn.IsEnabled = true;
                log.errorLogMessage(y.Message);
            }
        }

        private void progressProcessingAsync()
        {
            ProgBar.Value = 0;
            timerEnded = false;
            var progress = new Progress<int>(percent =>
            {
                ProgBar.Value = percent;
                ProgLabel.Content = percent.ToString() + "%";
                if (timerEnded)
                {
                    SearchLabel.Content = "Account with last name searched does not exist";
                    SearchBtn.IsEnabled = true;
                    timerEnded = false;
                }
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
            timer.Interval = 120000;
            timer.Elapsed += OnTimerEnd;
            timer.AutoReset = true;
            timer.Enabled = true;
            progress.Report(0);
            for (int i = 0; i != 100; i++)
            {
                Thread.Sleep(120000/100);
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
                
                if(timerEnded)
                {
                    progress.Report(100);
                    found = false;
                    break;
                }
            }
        }

        private void OnTimerEnd(Object source, System.Timers.ElapsedEventArgs e)
        {
            timerEnded = true;
        }

        private void Click_Update_User(object sender, RoutedEventArgs e)
        {
            try
            {
                RestRequest request = new RestRequest("api/webapi/");
                UpdatedUser user = new UpdatedUser();
                user.index = Int32.Parse(IndexVal.Text);
                if (validateTextFields() && validateRegex())
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
                    throw new FormatException("invalid data entered into user fields");
                }
            }
            catch(FormatException x)
            {
                MessageBox.Show(x.Message);
                log.errorLogMessage(x.Message);
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

        private bool validateRegex()
        {
            bool fieldsTrue;
            var regex = new Regex("^[a-zA-Z]*$");
            var numRegex = new Regex("^[0-9]*$");
            if(regex.IsMatch(fnameField.Text) && regex.IsMatch(lnameField.Text))
            {
                if(numRegex.IsMatch(acntNoField.Text) && numRegex.IsMatch(pinField.Text) && numRegex.IsMatch(balanceField.Text))
                {
                    fieldsTrue = true;
                }
                else
                {
                    fieldsTrue = false;
                }
            }
            else
            {
                fieldsTrue = false;
            }
            return fieldsTrue;
        }
    }
}

