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

        public void searchForValue(string str)
        {
            for(int i = 0; i < foob.GetNumEntries(); i++)
            {
                if (foob.GetLastName(i).Equals(str))
                {
                    return foob.GetValuesForEntry(i, out 1, out 2222, out 0, out "", out "");
                }
            }
        }
    }
}
