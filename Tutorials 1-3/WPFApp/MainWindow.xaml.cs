using Bis_GUI;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
        private static System.Timers.Timer timer;
        private bool timerEnded;
        private LogData log;
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
            timerEnded = false;
            timer = new System.Timers.Timer();
            log = new LogData();
            log.setPath(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "LogFiles/log.txt"));
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
                    RestRequest request = new RestRequest("api/GetValues/" + idx.ToString());
                    IRestResponse response = await client.ExecuteGetAsync(request);
                    DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                    fnameField.Text = dataInter.fname;
                    lnameField.Text = dataInter.lname;
                    balanceField.Text = dataInter.bal.ToString();
                    acntNoField.Text = dataInter.acct.ToString();
                    pinField.Text = dataInter.pin.ToString("D4");
                    BitmapImage image = new BitmapImage(new Uri(dataInter.filePath));
                    img.Source = image;
                }
                else
                {
                    throw new FormatException("Index out of range");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid Index was found, please try again");
                fnameField.Text = "";
                lnameField.Text = "";
                balanceField.Text = "";
                IndexVal.Text = "Enter Index";
                acntNoField.Text = "";
                pinField.Text = "";
                log.errorLogMessage("Invalid Index was entered, please try again");
            }
            catch (JsonReaderException)
            {
                MessageBox.Show("Please check that you have specified a valid URL for client and try again");
                fnameField.Text = "";
                lnameField.Text = "";
                balanceField.Text = "";
                IndexVal.Text = "Enter Index";
                acntNoField.Text = "";
                pinField.Text = "";
                log.errorLogMessage("User attempted to search index using an invalid client URL");
            }
            catch(NullReferenceException)
            {
                MessageBox.Show("Received invalid JSON Object response, please check the URL entered and the data entered.");
                fnameField.Text = "";
                lnameField.Text = "";
                balanceField.Text = "";
                IndexVal.Text = "Enter Index";
                acntNoField.Text = "";
                pinField.Text = "";
                log.errorLogMessage("User attempted to search index using an invalid client URL");
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
                log.errorLogMessage("User has entered an invalid index on image upload");
            }
            catch(JsonReaderException)
            {
                MessageBox.Show("Please check that you have specified a valid URL for client and try again");
                log.errorLogMessage("User attempted to upload image using an invalid client URL");
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
                    throw new FormatException("Cannot allow empty string as last name, try again");
                }
                var regex = new Regex("^[a-zA-Z]*$");
                if(!regex.IsMatch(searchTxt.Text))
                {
                    throw new FormatException("Cannot allow illegal characters");
                }
                found = false;
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
                    SearchBtn.IsEnabled = true;
                }
                else
                {
                    throw new NullReferenceException("Unable to find an entry with that lastname in the database");
                }
            }
            catch(FormatException)
            {
                log.errorLogMessage("Invalid last name was entered for search");
                MessageBox.Show("Please enter a valid last name and try again");
                SearchLabel.Content = "Invalid last name entered, please try again";
                ProgBar.Value = 100;
                searchTxt.Text = "Enter Last Name";
                SearchBtn.IsEnabled = true;
            }
            catch(NullReferenceException)
            {
                found = true;
                ProgBar.Value = 100;
                MessageBox.Show("Was unable to find last name for entry, please try again");
                SearchLabel.Content = "Invalid last name entered, please try again";
                SearchLabel.Content = "Search Complete";
                searchTxt.Text = "Enter Last Name";
                SearchBtn.IsEnabled = true;
                log.errorLogMessage("No last name was found for entry: " + searchTxt.Text);
            }
            catch(JsonReaderException)
            {
                log.errorLogMessage("User attempted to search for last name with invalid URL as client");
                MessageBox.Show("Please check that you have specified a valid URL for client and try again");
                found = true;
                SearchBtn.IsEnabled = true;
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
            timer.Interval = 150000;
            timer.Elapsed += OnTimerEnd;
            timer.AutoReset = true;
            timer.Enabled = true;
            progress.Report(0);
            for (int i = 0; i != 100; i++)
            {
                Thread.Sleep(150000/100);
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
            log.logTimerEnd();
        }

        private void Click_Update_User(object sender, RoutedEventArgs e)
        {
            try
            {
                RestRequest request = new RestRequest("api/webapi/");
                DataIntermed dataIm = new DataIntermed();
                dataIm.index = Int32.Parse(IndexVal.Text);
                if (validateTextFields() && validateRegex())
                {
                    dataIm.fname = fnameField.Text;
                    dataIm.lname = lnameField.Text;
                    dataIm.acct = Convert.ToUInt32(acntNoField.Text);
                    dataIm.bal = Convert.ToInt32(balanceField.Text);
                    dataIm.pin = Convert.ToUInt32(pinField.Text);
                    request.AddJsonBody(dataIm);
                    IRestResponse response = client.Put(request);
                    DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                    fnameField.Text = dataInter.fname;
                    lnameField.Text = dataInter.lname;
                    acntNoField.Text = dataInter.acct.ToString();
                    balanceField.Text = dataInter.bal.ToString();
                    pinField.Text = dataInter.pin.ToString();
                    AccountStatusLabel.Content = "Account Updated Successfully";
                }
                else
                {
                    throw new FormatException("invalid data entered into user fields");
                }
            }
            catch(FormatException)
            {
                MessageBox.Show("Invalid data was entered into fields, please check user fields and index");
                log.errorLogMessage("Invalid data was entered in fields or index for update user");
            }
            catch(JsonReaderException)
            {
                MessageBox.Show("Please check that you have specified a valid URL for client and try again");
                log.errorLogMessage("User attempted to update user with invalid Base URL");
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Received invalid JSON Object response, please check the URL entered and the data entered.");
                fnameField.Text = "";
                lnameField.Text = "";
                balanceField.Text = "";
                IndexVal.Text = "Enter Index";
                acntNoField.Text = "";
                pinField.Text = "";
                log.errorLogMessage("User attempted to search index using an invalid client URL");
            }
        }

        private bool validateTextFields()
        {
            try
            {
                if (Int32.Parse(IndexVal.Text) >= 0 && Int32.Parse(IndexVal.Text) < Int32.Parse(NoOfItems.Content.ToString())
                    && !fnameField.Text.Equals("") && !lnameField.Text.Equals("") && !pinField.Text.Equals("") &&
                    !balanceField.Text.Equals("") && !acntNoField.Text.Equals("") && pinField.Text.Length == 4
                    && Convert.ToUInt32(balanceField.Text) > 0)
                {
                    return true;
                }
                if (pinField.Text.Length != 4)
                {
                    MessageBox.Show("Pin must be 4 digits to proceed");
                    log.errorLogMessage("invalid pin was entered by user");
                }
                return false;
            }
            catch(OverflowException)
            {
                log.errorLogMessage("User entered negative value and could not be processed");
                return false;
            }
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
                    log.errorLogMessage("Invalid data was entered in fields account number, pin or balance");
                    fieldsTrue = false;
                }
            }
            else
            {
                fieldsTrue = false;
                log.errorLogMessage("Invalid data was entered in first name or last name field");
            }
            return fieldsTrue;
        }

        private void Click_Url_Btn(object sender, RoutedEventArgs e)
        {
            if(Uri.IsWellFormedUriString(UrlText.Text, UriKind.Absolute))
            {
                URL = UrlText.Text;
                client = new RestClient(URL);
                URLStatus.Content = "URL Successfully changed";
                log.logUrlChange(UrlText.Text);
            }
            else
            {
                URLStatus.Content = "Please enter a valid URL";
                MessageBox.Show("Invalid URL used, please try again");
                log.errorLogMessage("Invalid URL was entered by user");
            }
        }
    }
}

