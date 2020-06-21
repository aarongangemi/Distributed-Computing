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
    /// <summary>
    /// Purpose: The GUI class which is used to displayed elements
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public partial class MainWindow : Window
    {
        private string URL;
        private RestClient client;
        private bool found;
        private static System.Timers.Timer timer;
        private bool timerEnded;
        private LogData log;

        /// <summary>
        /// Purpose: Main window to initialize the GUI
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            URL = "https://localhost:44383/";
            // URL for web server
            client = new RestClient(URL);
            // Create client
            RestRequest request = new RestRequest("api/WebApi");
            IRestResponse numOfItems = client.Get(request);
            // Get number of items from WebApi
            NoOfItems.Content = numOfItems.Content;
            img.Source = new BitmapImage(new Uri(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Images/ProfileImage.jpg")));
            // Set default image
            found = false;
            timerEnded = false;
            // Set timer ended to false and no last name found to false
            timer = new System.Timers.Timer();
            log = new LogData();
            log.setPath(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "LogFiles/log.txt"));
            // Set the path to log data
        }

        /// <summary>
        /// Purpose: When the user clicks go to get an account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Click_Go(object sender, RoutedEventArgs e)
        {
            try
            {
                int idx = Int32.Parse(IndexVal.Text);
                // Convert entered index from string to number
                // Check if invalid is between 0 and 100000
                if ((idx >= 0)  && (idx < Int32.Parse(NoOfItems.Content.ToString())))
                {
                    AccountStatusLabel.Content = "";
                    // Set no account label
                    RestRequest request = new RestRequest("api/GetValues/" + idx.ToString());
                    IRestResponse response = await client.ExecuteGetAsync(request);
                    // Execute get request in using async
                    DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                    // Gets the values for the associated account for the index
                    fnameField.Text = dataInter.fname;
                    lnameField.Text = dataInter.lname;
                    balanceField.Text = dataInter.bal.ToString();
                    acntNoField.Text = dataInter.acct.ToString();
                    pinField.Text = dataInter.pin.ToString("D4");
                    BitmapImage image = new BitmapImage(new Uri(dataInter.filePath));
                    img.Source = image;
                    //Set user field using data intermed object retrieved from account
                }
                else
                {
                    throw new FormatException("Index out of range");
                    // Throw exception if index is out of range
                }
            }
            catch (FormatException)
            {
                // Error if index is out of range
                MessageBox.Show("Invalid Index was found, please try again");
                // Display error message in window
                // Reset fields
                fnameField.Text = "";
                lnameField.Text = "";
                balanceField.Text = "";
                IndexVal.Text = "Enter Index";
                acntNoField.Text = "";
                pinField.Text = "";
                // Log error message
                log.errorLogMessage("Invalid Index was entered, please try again");
            }
            catch (JsonReaderException)
            {
                // Catch JSON reading exceptions if no response is recieved from URL
                // Display error message
                MessageBox.Show("Please check that you have specified a valid URL for client and try again");
                // Reset fields
                fnameField.Text = "";
                lnameField.Text = "";
                balanceField.Text = "";
                IndexVal.Text = "Enter Index";
                acntNoField.Text = "";
                pinField.Text = "";
                // Log message
                log.errorLogMessage("User attempted to search index using an invalid client URL");
            }
            catch(NullReferenceException)
            {
                // If any fields return null
                // Display error message
                MessageBox.Show("Received invalid JSON Object response, please check the URL entered and the data entered.");
                // Reset fields
                fnameField.Text = "";
                lnameField.Text = "";
                balanceField.Text = "";
                IndexVal.Text = "Enter Index";
                acntNoField.Text = "";
                pinField.Text = "";
                // log message
                log.errorLogMessage("User attempted to search index using an invalid client URL");
            }
        }

        /// <summary>
        /// Purpose: User clicks upload image button, then this method executes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Upload_Image(object sender, RoutedEventArgs e)
        {
            try
            { 
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp;)|*.jpg; *.jpeg; *.gif; *.bmp;";
                // Set filters to only look for image files
                // Open file
                bool? result = open.ShowDialog();
                // show file explorer
                if (result == true)
                {
                    int idx = Int32.Parse(IndexVal.Text);
                    // Convert entered index
                    if (idx >= 0 && idx < Int32.Parse(NoOfItems.Content.ToString()))
                    {
                        // If the index is within range
                        FilePathData file = new FilePathData();
                        // Create filepath object
                        string filePath = Path.GetFullPath(open.FileName);
                        file.filePath = filePath;
                        file.indexToUpdate = idx;
                        // Set file path object
                        RestRequest request = new RestRequest("api/webapi/");
                        request.AddJsonBody(file);
                        IRestResponse response = client.Post(request);
                        DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                        // Create Rest request to get upload image and set fields
                        fnameField.Text = dataInter.fname;
                        lnameField.Text = dataInter.lname;
                        balanceField.Text = dataInter.bal.ToString();
                        acntNoField.Text = dataInter.acct.ToString();
                        pinField.Text = dataInter.pin.ToString("D4"); 
                        BitmapImage image = new BitmapImage(new Uri(dataInter.filePath));
                        // Create and display image
                        img.Source = image;
                    }
                    else
                    {
                        // If the index is out range, throw format exception
                        throw new FormatException("Index out of range, please try again");
                    }
                }
            }
            catch(FormatException)
            {
                // Display error if invalid index
                MessageBox.Show("Invalid index entered, please try again");
                // Log error message
                log.errorLogMessage("User has entered an invalid index on image upload");
            }
            catch(JsonReaderException)
            {
                // If no response is recieved or URL entered was invalud
                MessageBox.Show("Please check that you have specified a valid URL for client and try again");
                // Display error message
                log.errorLogMessage("User attempted to upload image using an invalid client URL");
                // Log error message
            }

        }

        /// <summary>
        /// Purpose: When the user clicks the search button, search for a user 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Click_Search_btn(object sender, RoutedEventArgs e)
        {
            try
            { 
                SearchBtn.IsEnabled = false;
                // Disable search btn so user cannot search twice
                SearchLabel.Content = "Searching results for Last name";
                // Change label status
                if (searchTxt.Text.Length == 0)
                {
                    // Check search text is not empty - if empty, throw exception
                    throw new FormatException("Cannot allow empty string as last name, try again");
                }
                var regex = new Regex("^[a-zA-Z]*$");
                // Check for only letters
                if(!regex.IsMatch(searchTxt.Text))
                {
                    // If search text contains numbers or special characters
                    throw new FormatException("Cannot allow illegal characters");
                }
                found = false;
                // set no result found
                progressProcessingAsync();
                //Start progress bar
                SearchData mySearch = new SearchData();
                mySearch.searchStr = searchTxt.Text;
                //Create search object with string
                RestRequest request = new RestRequest("api/Search/");
                request.AddJsonBody(mySearch);
                IRestResponse response = await client.ExecutePostAsync(request);
                DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                // Create a search request with search object in body 
                if (dataInter != null)
                {
                    // Check response exists
                    SearchLabel.Content = "Search Complete";
                    found = true;
                    // set found bool condition to true to end progress bar
                    IndexVal.Text = dataInter.index.ToString();
                    fnameField.Text = dataInter.fname;
                    lnameField.Text = dataInter.lname;
                    balanceField.Text = dataInter.bal.ToString();
                    acntNoField.Text = dataInter.acct.ToString();
                    pinField.Text = dataInter.pin.ToString("D4");
                    // set fields
                    img.Source = new BitmapImage(new Uri(dataInter.filePath));
                    // Set image from account
                    SearchBtn.IsEnabled = true;
                    //re-enable the search bar
                }
                else
                {
                    // If no response, then throw a null exception
                    throw new NullReferenceException("Unable to find an entry with that lastname in the database");
                }
            }
            catch(FormatException)
            {
                // If the last name is invalid
                log.errorLogMessage("Invalid last name was entered for search");
                MessageBox.Show("Please enter a valid last name and try again");
                // display error message
                SearchLabel.Content = "Invalid last name entered, please try again";
                // Change status
                ProgBar.Value = 100;
                // End progress bar
                searchTxt.Text = "Enter Last Name";
                SearchBtn.IsEnabled = true;
                // re-enable search button
            }
            catch(NullReferenceException)
            {
                // End search
                found = true;
                // set progress bar
                ProgBar.Value = 100;
                // Display error message
                MessageBox.Show("Was unable to find last name for entry, please try again");
                SearchLabel.Content = "Invalid last name entered, please try again";
                SearchLabel.Content = "Search Complete";
                // set status label
                searchTxt.Text = "Enter Last Name";
                //reset search text
                // re-enable search bar
                SearchBtn.IsEnabled = true;
                // log error
                log.errorLogMessage("No last name was found for entry: " + searchTxt.Text);
            }
            catch(JsonReaderException)
            {
                // Error will be thrown if response is not found because user has changed URL
                log.errorLogMessage("User attempted to search for last name with invalid URL as client");
                // log URL error
                MessageBox.Show("Please check that you have specified a valid URL for client and try again");
                // Display error message
                found = true;
                // End search and enable search bar
                SearchBtn.IsEnabled = true;
            }
        }

        /// <summary>
        /// Displays the progress bar value when the user is searching
        /// </summary>
        private void progressProcessingAsync()
        {
            ProgBar.Value = 0;
            // Initially set bar to 0
            timerEnded = false;
            // Start timer
            // Below method runs while progress bar is running
            var progress = new Progress<int>(percent =>
            {
                ProgBar.Value = percent;
                // Set progress bar value
                ProgLabel.Content = percent.ToString() + "%";
                // Display percentage text
                // Timer is used to allow for expiry to state if result has been found withing 150 seconds
                if (timerEnded)
                {
                    //Change status label
                    SearchLabel.Content = "Account with last name searched does not exist";
                    SearchBtn.IsEnabled = true;
                    //re-enable search bar and change status
                    timerEnded = false;
                }
            });
            Task.Run(() => processProgress(progress));
            //Runs progress bar in another thread
        }

        /// <summary>
        /// Purpose: process the progress bar and add a timer to have a timeout if results aren't found
        /// Reference for timer functionality: https://www.tutorialspoint.com/Timer-in-Chash
        /// Author: Karthikeya Boyini
        /// Date Accessed: 03/05/2020
        /// </summary>
        /// <param name="progress"></param>
        public void processProgress(IProgress<int> progress)
        {
            timer.Interval = 150000;
            timer.Elapsed += OnTimerEnd;
            timer.AutoReset = true;
            timer.Enabled = true;
            //Set timer to end after 2.5 minutes and start timer
            progress.Report(0);
            //Report initial progress as 0 - before start
            for (int i = 0; i != 100; i++)
            {
                // Loop from 1 to 100%
                Thread.Sleep(150000/100);
                //Sleep current thread so count will last 2.5 minutes
                if (progress != null)
                {
                    progress.Report(i);
                    //Continue progress and report next value
                }
                if(found)
                {
                    //If result has been found using bool variable then
                    // stop the search and end the progress bar
                    progress.Report(100);
                    //reset search
                    found = false;
                    //exit loop
                    break;
                }
                
                if(timerEnded)
                {
                    //If 2.5 minutes has expired, then progress bar = 100
                    progress.Report(100);
                    found = false;
                    //exit loop
                    break;
                }
            }
        }

        /// <summary>
        /// Actions to perform if timer ends
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimerEnd(Object source, System.Timers.ElapsedEventArgs e)
        {
            timerEnded = true;
            //set timerEnded to true
            log.logTimerEnd();
            //log result
        }

        /// <summary>
        /// If the user clicks the update user button in the GUI, then update the user fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Update_User(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((validateTextFields()) && (validateRegex()))
                {
                    // if regex and text fields validation is passed
                    DataIntermed dataIm = new DataIntermed();
                    dataIm.index = Int32.Parse(IndexVal.Text);
                    dataIm.fname = fnameField.Text;
                    dataIm.lname = lnameField.Text;
                    dataIm.acct = Convert.ToUInt32(acntNoField.Text);
                    dataIm.bal = Convert.ToInt32(balanceField.Text);
                    dataIm.pin = Convert.ToUInt32(pinField.Text);
                    // set data intermediate fields to current fields displayed
                    // create web request to web api
                    RestRequest request = new RestRequest("api/webapi/");
                    // Add object to request body
                    request.AddJsonBody(dataIm);
                    IRestResponse response = client.Put(request);
                    DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                    //Get updated user fields from web api
                    //Reset the text with the user fields that were obtained
                    fnameField.Text = dataInter.fname;
                    lnameField.Text = dataInter.lname;
                    acntNoField.Text = dataInter.acct.ToString();
                    balanceField.Text = dataInter.bal.ToString();
                    pinField.Text = dataInter.pin.ToString();
                    //Change status label
                    AccountStatusLabel.Content = "Account Updated Successfully";
                }
                else
                {
                    // If validation fails, throw a format exception
                    throw new FormatException("invalid data entered into user fields");
                }
            }
            catch(FormatException)
            {
                // Display error message to window
                MessageBox.Show("Invalid data was entered into fields, please check user fields and index");
                // Log error
                log.errorLogMessage("Invalid data was entered in fields or index for update user");
            }
            catch(JsonReaderException)
            {
                // Invalid URL was entered by user so no response will be delivered
                MessageBox.Show("Please check that you have specified a valid URL for client and try again");
                log.errorLogMessage("User attempted to update user with invalid Base URL");
            }
            catch (NullReferenceException)
            {
                // No response recieved
                MessageBox.Show("Received invalid JSON Object response, please check the URL entered and the data entered.");
                // Clear fields
                fnameField.Text = "";
                lnameField.Text = "";
                balanceField.Text = "";
                IndexVal.Text = "Enter Index";
                acntNoField.Text = "";
                pinField.Text = "";
                // log error
                log.errorLogMessage("User attempted to search index using an invalid client URL");
            }
        }

        /// <summary>
        /// Used to validate the text fields
        /// </summary>
        /// <returns> if fields are valid</returns>
        private bool validateTextFields()
        {
            try
            {
                if ((Int32.Parse(IndexVal.Text) >= 0) && (Int32.Parse(IndexVal.Text)) < (Int32.Parse(NoOfItems.Content.ToString()))
                    && (!fnameField.Text.Equals("")) && (!lnameField.Text.Equals("")) && (!pinField.Text.Equals("")) &&
                    (!balanceField.Text.Equals("")) && (!acntNoField.Text.Equals("")) && (pinField.Text.Length == 4)
                    && (Convert.ToUInt32(balanceField.Text) > 0))
                {
                    // If all fields are not empty, null and index is valid and pin length is 4 digits
                    return true;
                }
                if (pinField.Text.Length != 4)
                {
                    // If pin length is not 4 digits, then display error
                    MessageBox.Show("Pin must be 4 digits to proceed");
                    // Log error message
                    log.errorLogMessage("invalid pin was entered by user");
                }
                // validation failed if return false
                return false;
            }
            catch(OverflowException)
            {
                log.errorLogMessage("User entered negative value and could not be processed");
                // If negative values are entered, overflow exception occurs
                // return false
                return false;
            }
        }

        /// <summary>
        /// Use regex to validate text fields
        /// </summary>
        /// <returns>if fields are valid or not</returns>
        private bool validateRegex()
        {
            bool fieldsTrue;
            var regex = new Regex("^[a-zA-Z]*$");
            var numRegex = new Regex("^[0-9]*$");
            // check last name and first name contain only letters
            if(regex.IsMatch(fnameField.Text) && regex.IsMatch(lnameField.Text))
            {
                if(numRegex.IsMatch(acntNoField.Text) && numRegex.IsMatch(pinField.Text) && numRegex.IsMatch(balanceField.Text))
                {
                    // check acnt number, pin and balance contain numbers only
                    // all fields are valid
                    fieldsTrue = true;
                }
                else
                {
                    log.errorLogMessage("Invalid data was entered in fields account number, pin or balance");
                    // log error message
                    // field is not a number
                    fieldsTrue = false;
                }
            }
            else
            {
                // first name or last name field failed
                fieldsTrue = false;
                // log error
                log.errorLogMessage("Invalid data was entered in first name or last name field");
            }
            return fieldsTrue;
        }

        /// <summary>
        /// Actions to be performed if the user clicks the URL button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Url_Btn(object sender, RoutedEventArgs e)
        {
            if(Uri.IsWellFormedUriString(UrlText.Text, UriKind.Absolute))
            {
                // check is valid URL
                URL = UrlText.Text;
                // change rest client
                client = new RestClient(URL);
                // change URL status
                URLStatus.Content = "URL Successfully changed";
                // log change
                log.logUrlChange(UrlText.Text);
            }
            else
            {
                URLStatus.Content = "Please enter a valid URL";
                // change status content
                MessageBox.Show("Invalid URL used, please try again");
                //display error
                // log message
                log.errorLogMessage("Invalid URL was entered by user");
            }
        }
    }
}

