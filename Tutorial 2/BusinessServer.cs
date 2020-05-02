using IDataServerInterface;
using Remoting_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tutorial_2
{
    //Concurrency Mode indicates that server will be multi-threaded
    //UseSynchronisationContext indicates that we will handle synchronisation
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    //Must be internal because it must be accessed through the interface
    class BusinessServer : BusinessServerInterface
    {
        private int LogNumber = 0;
        ChannelFactory<IDataServerInterface.IDataServerInterface> foobFactory;
        private IDataServerInterface.IDataServerInterface foob;
        
        NetTcpBinding tcp;
        string URL;
        public BusinessServer()
        {
            tcp = new NetTcpBinding();
            URL = "net.tcp://localhost:8100/DataService";
            tcp.MaxBufferSize = 2147483647;
            tcp.MaxReceivedMessageSize = 2147483647;
            tcp.MaxBufferPoolSize = 2147483647;
            foobFactory = new ChannelFactory<IDataServerInterface.IDataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();

        }
        public int GetNumEntries()
        {
            return foob.GetNumEntries();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fname, out string lname, out string filePath)
        {
            foob.GetValuesForEntry(index, out acctNo, out pin, out bal, out fname, out lname, out filePath);
            Log("Retrieved values for Person: " + fname + " " + lname);
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
                GetValuesForEntry(i, out acntNo, out pin, out bal, out fname, out lname, out filePath);
                if (lname.Equals(str))
                {
                    val = i;
                    Log("Found " + lname + " for entry at index: " + i + " at: "+ now.ToString("F"));
                    break;
                }
            }
            return val;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Log(string logString)
        {
            LogNumber++;
            Console.WriteLine(LogNumber.ToString() + ". " + logString + "\n");
        }



    }

    }
