using System;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Tutorial_2;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    public delegate int SearchOperation(string str);
    
    public partial class MainWindow : Window
    {
        private BusinessServerInterface foob;
        private BitmapImage i;

        public MainWindow()
        {
            InitializeComponent();

            ChannelFactory<BusinessServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8200/BusinessService";
            tcp.OpenTimeout = new TimeSpan(0, 30, 0);
            tcp.CloseTimeout = new TimeSpan(0, 30, 0);
            tcp.SendTimeout = new TimeSpan(0, 30, 0);
            tcp.ReceiveTimeout = new TimeSpan(0, 30, 0);
            tcp.MaxBufferSize = 2147483647;
            tcp.MaxReceivedMessageSize = 2147483647;
            tcp.MaxBufferPoolSize = 2147483647;
      
            foobFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
            totalTxt.Text = foob.GetNumEntries().ToString();
    }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            string fname = "";
            string lname = "";
            int bal = 0;
            uint acct = 0;
            uint pin = 0;
            i = new BitmapImage();
            int idx;
            try
            {
                idx = Int32.Parse(IndexVal.Text);
                if(img.Source == null)
                {
                    img.Source = i;
                }
    
                foob.GetValuesForEntry(idx, out acct, out pin, out bal, out fname, out lname);
                Fname.Text = fname;
                LName.Text = lname;
                Balance.Text = bal.ToString("C");
                AcntNo.Text = acct.ToString();
                PIN.Text = pin.ToString("D4");
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
                String filePath = open.FileName;
                image = new BitmapImage(new Uri(filePath));
                img.Source = image;
            }
            
        }

        //The Search Button
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

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
            del = foob.SearchForValue;
            callbackDel = this.onAddCompletion;
            //SearchTxt.text is the field for the last name
            del.BeginInvoke(searchTxt.Text, callbackDel, null);
            System.Console.WriteLine("Waiting for Completion");
            System.Console.ReadLine();
        }

        private void onAddCompletion(IAsyncResult asyncRes)
        {
        
            uint acntNo, pin;
            int bal;
            string fname, lname;
            int iResult = 0;
            SearchOperation addDel;
            AsyncResult asyncObj = (AsyncResult)asyncRes;
            if (asyncObj.EndInvokeCalled == false)
            {
                progressProcessingAsync();
                addDel = (SearchOperation) asyncObj.AsyncDelegate;
                iResult = addDel.EndInvoke(asyncObj);
                Console.WriteLine("Result is: " + iResult);
            }
            asyncObj.AsyncWaitHandle.Close();
            if (iResult == -1)
                {
                    MessageBox.Show("Could not find existing record with entered last name");
                }
                else
                {
                    foob.GetValuesForEntry(iResult, out acntNo, out pin, out bal, out fname, out lname);
                    Fname.Text = fname;
                    LName.Text = lname;
                    Balance.Text = bal.ToString("C");
                    AcntNo.Text = acntNo.ToString();
                    PIN.Text = pin.ToString("D4");
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

        private async Task progressProcessingAsync()
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
