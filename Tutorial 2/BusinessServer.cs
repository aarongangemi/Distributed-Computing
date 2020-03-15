using Remoting_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_2
{
    //Concurrency Mode indicates that server will be multi-threaded
    //UseSynchronisationContext indicates that we will handle synchronisation
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    //Must be internal because it must be accessed through the interface
    class BusinessServer : BusinessServerInterface
    {
        private DataServerInterface foob;
        
        public int GetNumEntries()
        {
            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
            return foob.GetNumEntries();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fname, out string lname)
        {
            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
            foob.GetValuesForEntry(index, out acctNo, out pin, out bal, out fname, out lname);
        }

        public int SearchForValue(string str)
        {
            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
            int i;
            uint acntNo, pin;
            int bal;
            int val = -1;
            string fname, lname;
            for (i = 0; i < foob.GetNumEntries(); i++)
            {
                GetValuesForEntry(i, out acntNo, out pin, out bal, out fname, out lname);
                if (lname.Equals(str))
                {
                    val = i;
                    break;
                }
            }
            return val;
        }


        }

    }
