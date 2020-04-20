using Remoting_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Web_Service.Models
{
    public class DataModel
    {
        private NetTcpBinding tcp;
        private string URL;
        ChannelFactory<DataServerInterface> foobFactory;
        DataServerInterface foob;
        public DataModel()
        {
            tcp = new NetTcpBinding();
            URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
        }

        public int getNumEntries()
        {
            return foob.GetNumEntries();
        }

        public void GetValuesForEntry(int idx, out uint acntNo, out uint pin, out int balance, out string fname, out string lname)
        {
            foob.GetValuesForEntry(idx, out acntNo, out pin, out balance, out fname, out lname);
        }

        public int SearchForValue(string str)
        {
            DateTime now = DateTime.Now;
            int i;
            uint acntNo, pin;
            int bal;
            int val = -1;
            string fname, lname, filePath;
            for (i = 0; i < foob.GetNumEntries(); i++)
            {
                GetValuesForEntry(i, out acntNo, out pin, out bal, out fname, out lname);
                if (lname.Equals(str))
                {
                    val = i;
                    Console.WriteLine("Found " + lname + " for entry at index: " + i + " at: " + now.ToString("F"));
                    break;
                }
            }
            return val;
        }
    }
}