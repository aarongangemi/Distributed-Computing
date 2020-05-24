using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Tutorial_1;

namespace Remoting_Server
{
    // Concurrency Mode indicates that server will be multi-threaded
    // UseSynchronisationContext indicates that we will handle synchronisation
    // InstanceContextMode ensures data tier implemented will be singleton
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode =InstanceContextMode.Single)]
    // Must be internal because it must be accessed through the interface
    ///<summary>Purpose: The data server class which inherits off of the data server interface. 
    /// Used to get and update values for the GUI</summary>
    internal class DataServer : IDataServerInterface.IDataServerInterface
    { 
        private DatabaseClass database = new DatabaseClass(); 
        // Create instance of database


        /// <summary>
        /// Returns number of entries in database
        /// </summary>
        /// <returns>number of entries</returns>
        public int GetNumEntries()
        {
            return database.GetNumRecords();
        }
        /// <summary>
        /// Purpose: Get values for the entry at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="acctNo"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <param name="filePath"></param>
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin,
                               out int bal, out string fname, out string lname,
                               out string filePath)
        {
            acctNo = database.GetAcctNoByIndex(index);
            pin = database.GetPINByIndex(index);
            bal = database.GetBalanceByIndex(index);
            fname = database.GetFirstNameByIndex(index);
            lname = database.GetLastNameByIndex(index);
            filePath = database.getFilePath(index);
        }

        /// <summary>
        /// Purpose: Used to update the filepath in the database
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="index"></param>
        public void SetFilePath(string filePath, int index)
        {
            // First perform validation on filepath
            if(filePath != null && filePath != "" && index > 0 && index < GetNumEntries())
            {
                // send valid idx and filepath
                database.setFilePath(index, filePath);
            }
        }

        /// <summary>
        /// Update user details in the database
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <param name="acntNo"></param>
        /// <param name="pin"></param>
        /// <param name="balance"></param>
        public void updateUser(int idx, string fname, string lname, uint acntNo, uint pin, int balance)
        {
            // Perform validation on entries in database
            if((idx > 0) && (idx < GetNumEntries()) && (fname.Length > 0) && (lname.Length > 0) && (acntNo > 0) && (pin.ToString().Length == 4) && (balance > 0))
            {
                database.updateUser(idx, fname, lname, acntNo, pin, balance);
                // If information is valid then update database
            }
        }
    }
}
