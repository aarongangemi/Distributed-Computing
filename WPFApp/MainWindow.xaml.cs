﻿using Microsoft.Win32;
using Remoting_Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tutorial_2;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BusinessServerInterface foob;
        private int index = 0;
        private BitmapImage i;

        public MainWindow()
        {
            InitializeComponent();
            ChannelFactory<BusinessServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8200/BusinessService";
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
            try
            {
                index = Int32.Parse(IndexVal.Text);
                if(img.Source == null)
                {
                    img.Source = i;
                }
    
                foob.GetValuesForEntry(index, out acct, out pin, out bal, out fname, out lname);
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
    }
}
