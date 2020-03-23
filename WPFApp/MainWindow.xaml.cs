using Bis_GUI;
using Newtonsoft.Json;
using RestSharp;
using System;
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
        private DataModel model;
        private string searchDataText;

        public MainWindow()
        {
            model = new DataModel();
            InitializeComponent();
            URL = "https://localhost:44383/";
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/webapi");
            IRestResponse numOfItems = client.Get(request);
            totalTxt.Text = numOfItems.Content;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            i = new BitmapImage();
            int idx;
            try
            {
                idx = Int32.Parse(IndexVal.Text);
                RestRequest request = new RestRequest("api/WebApi/" + idx.ToString());
                IRestResponse response = client.Get(request);
                DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(response.Content);
                if (img.Source == null)
                {
                    img.Source = i;
                }
                model.GetValuesForEntry(idx, out dataInter.acct, out dataInter.pin, out dataInter.bal, out dataInter.fname, out dataInter.lname);
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
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            searchDataText = searchTxt.Text;
            Fname.IsReadOnly = true;
            LName.IsReadOnly = true;
            Balance.IsReadOnly = true;
            AcntNo.IsReadOnly = true;
            PIN.IsReadOnly = true;
            IndexVal.IsReadOnly = true;
            imgBtn.IsEnabled = false;
            GoBtn.IsEnabled = false;
            SearchOperation del;
            AsyncCallback callbackDel;
            del = model.SearchForValue;
            callbackDel = this.onAddCompletion;
            //SearchTxt.text is the field for the last name
            del.BeginInvoke(searchTxt.Text, callbackDel, null);
            System.Console.WriteLine("Waiting for Completion");
            System.Console.ReadLine();
        }

        private void onAddCompletion(IAsyncResult asyncRes)
        {
            SearchData mySearch = new SearchData();
            mySearch.searchStr = searchDataText;
            RestRequest request = new RestRequest("api/Search/");
            request.AddJsonBody(mySearch);
            IRestResponse resp = client.Post(request);
            DataIntermed dataInter = JsonConvert.DeserializeObject<DataIntermed>(resp.Content);
            int iResult = 0;
            SearchOperation addDel;
            AsyncResult asyncObj = (AsyncResult)asyncRes;
            if (asyncObj.EndInvokeCalled == false)
            {
                progressProcessingAsync();
                addDel = (SearchOperation)asyncObj.AsyncDelegate;
                iResult = addDel.EndInvoke(asyncObj);
                Console.WriteLine("\n Result is: " + iResult);
            }
            asyncObj.AsyncWaitHandle.Close();
            if (iResult == -1)
            {
                MessageBox.Show("\n Could not find existing record with entered last name");
            }
            else
            {
                Fname.Text = dataInter.fname;
                LName.Text = dataInter.lname;
                Balance.Text = dataInter.bal.ToString("C");
                AcntNo.Text = dataInter.acct.ToString();
                PIN.Text = dataInter.pin.ToString("D4");
                Fname.IsReadOnly = false;
                LName.IsReadOnly = false;
                Balance.IsReadOnly = false;
                PIN.IsReadOnly = false;
                AcntNo.IsReadOnly = false;
                imgBtn.IsEnabled = true;
                GoBtn.IsEnabled = true;
                IndexVal.IsReadOnly = false;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            WPFApp.AccessLog objSecondWindow = new WPFApp.AccessLog();
            this.Visibility = Visibility.Hidden;
            objSecondWindow.Show();
        }

        private async void progressProcessingAsync()
        {
            var progress = new Progress<int>(percent =>
            {
                ProgBar.Value = percent;
            });
            await Task.Run(() => processProgress(progress));
        }

        public void processProgress(IProgress<int> progress)
        {
            for (int i = 0; i != 100; i++)
            {
                Thread.Sleep(100);
                if (progress != null)
                {
                    progress.Report(i);
                }
            }
        }
    }
}

