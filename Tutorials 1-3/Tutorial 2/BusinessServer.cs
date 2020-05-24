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
    
    // Concurrency Mode indicates that server will be multi-threaded
    // UseSynchronisationContext indicates that we will handle synchronisation
    // Must be internal because it must be accessed through the interface
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]

    ///<summary>Purpose: Business server which was used to represent the business tier in tutorial 2.
    ///Connects data tier to presentation tier. This class has been replaced with the web server.
    ///Author: Aaron Gangemi
    ///Date Modified: 24/05/2020</summary>
    class BusinessServer : BusinessServerInterface
    {
        private int LogNumber = 0;
        private ChannelFactory<IDataServerInterface.IDataServerInterface> foobFactory;
        private IDataServerInterface.IDataServerInterface foob;
        private NetTcpBinding tcp;
        string URL;

        /// <summary>
        /// Purpose: To create an instance of the business
        /// </summary>
        public BusinessServer()
        {
            // Open port
            tcp = new NetTcpBinding();
            URL = "net.tcp://localhost:8100/DataService";
            tcp.MaxBufferSize = 2147483647;
            tcp.MaxReceivedMessageSize = 2147483647;
            tcp.MaxBufferPoolSize = 2147483647;
            foobFactory = new ChannelFactory<IDataServerInterface.IDataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
            // Use foob to create channel and access data tier
        }

        /// <summary>
        /// Purpose: Return number of entries
        /// </summary>
        /// <returns>No of entries</returns>
        public int GetNumEntries()
        {
            return foob.GetNumEntries();
        }

        /// <summary>
        /// Called by presentation tier and used to get a value for specified entry at index at data tier
        /// </summary>
        /// <param name="index"></param>
        /// <param name="acctNo"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <param name="filePath"></param>
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, 
                                        out string fname, out string lname, out string filePath)
        {
            // Call data tier method to get values for entry
            foob.GetValuesForEntry(index, out acctNo, out pin, out bal, out fname, out lname, out filePath);
            Log("Retrieved values for Person: " + fname + " " + lname);
            // Log data
        }

        /// <summary>
        /// Search the database and check if the provided string matches a last name
        /// </summary>
        /// <param name="str"></param>
        /// <returns>index of last name</returns>
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
                // Loop through all entries
                GetValuesForEntry(i, out acntNo, out pin, out bal, out fname, out lname, out filePath);
                // Get Entry at index
                if (lname.Equals(str))
                {
                    // If last name is a match for strinf
                    val = i;
                    // store index and log data
                    Log("Found " + lname + " for entry at index: " + i + " at: "+ now.ToString("F"));
                    break;
                }
            }
            return val;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        ///<summary>Log data</summary>
        private void Log(string logString)
        {
            LogNumber++;
            Console.WriteLine(LogNumber.ToString() + ". " + logString + "\n");
        }



    }

    }
