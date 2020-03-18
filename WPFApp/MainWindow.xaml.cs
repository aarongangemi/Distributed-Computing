﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private int LogNumber = 0;
        private BitmapImage i;

        public MainWindow()
        {
            InitializeComponent();
            LogText.Text = "Access Log: " + "\n";
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
                Log("Search for entry at index: " + idx);
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
                Log("Uploaded image from path: " + filePath);
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
            DateTime now = DateTime.Now;
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
                    Log("Searched for: '" + lname + "' at: " + now.ToString("F"));
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Log(string logString)
        {
            LogNumber++;
            LogText.Text = LogText.Text + LogNumber.ToString() + ". " + logString + "\n";
        }


    }
    }
