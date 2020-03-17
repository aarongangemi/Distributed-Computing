using Remoting_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial_2
{
    //Concurrency Mode indicates that server will be multi-threaded
    //UseSynchronisationContext indicates that we will handle synchronisation
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    //Must be internal because it must be accessed through the interface
    class BusinessServer : BusinessServerInterface
    {
        ChannelFactory<DataServerInterface> foobFactory;
        private DataServerInterface foob;
        NetTcpBinding tcp;
        string URL;
        public BusinessServer()
        {
            tcp = new NetTcpBinding();
            URL = "net.tcp://localhost:8100/DataService";
            tcp.MaxBufferSize = 2147483647;
            tcp.MaxReceivedMessageSize = 2147483647;
            tcp.MaxBufferPoolSize = 2147483647;
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();

        }
        public int GetNumEntries()
        {
            return foob.GetNumEntries();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fname, out string lname)
        {
            foob.GetValuesForEntry(index, out acctNo, out pin, out bal, out fname, out lname);
        }

        public int SearchForValue(string str)
        {
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
