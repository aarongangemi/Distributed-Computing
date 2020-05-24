using System;
using System.ServiceModel;

namespace Tutorial_3_Web_Service.Models
{
    /// <summary>
    /// Purpose: The data model 
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class DataModel
    {
        private NetTcpBinding tcp;
        private string URL;
        private ChannelFactory<IDataServerInterface.IDataServerInterface> foobFactory;
        private IDataServerInterface.IDataServerInterface foob;
        /// <summary>
        /// Purpose: Allows the GUI to communicate with business tier
        /// </summary>
        public DataModel()
        {
            tcp = new NetTcpBinding();
            URL = "net.tcp://localhost:8100/DataService";
            // Set URL and open port
            foobFactory = new ChannelFactory<IDataServerInterface.IDataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
            // access methods from other tier
        }

        /// <summary>
        /// Purpose: return the number of entries to web service
        /// </summary>
        /// <returns>num of entries</returns>
        public int getNumEntries()
        {
            return foob.GetNumEntries();
        }

        /// <summary>
        /// Purpose: To get the values fo the entry at an index
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="acntNo"></param>
        /// <param name="pin"></param>
        /// <param name="balance"></param>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <param name="filePath"></param>
        public void GetValuesForEntry(int idx, out uint acntNo, out uint pin, out int balance, out string fname, out string lname, out string filePath)
        {
            foob.GetValuesForEntry(idx, out acntNo, out pin, out balance, out fname, out lname, out filePath);
        }

        /// <summary>
        /// Update the file path when user selects update
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="index"></param>
        public void UpdateFilePath(string filePath, int index)
        {
            foob.SetFilePath(filePath, index);
        }

        /// <summary>
        /// Purpose: To update the user entry
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <param name="acntNo"></param>
        /// <param name="pin"></param>
        /// <param name="balance"></param>
        public void updateUserEntry(int idx, string fname, string lname, uint acntNo, uint pin, int balance)
        {
            // update user
            foob.updateUser(idx, fname, lname, acntNo, pin, balance);
        }

    }
}