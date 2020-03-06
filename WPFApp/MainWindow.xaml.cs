using Remoting_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;

        public MainWindow()
        {
            InitializeComponent();
            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
            totalTxt.Text = foob.GetNumEntries().ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            string fname = "";
            string lname = "";
            int bal = 0;
            uint acct = 0;
            uint pin = 0;
            try
            {
                index = Int32.Parse(IndexVal.Text);
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
    }
}
