using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tutorial_1
{
    /// <summary>
    /// Purpose: The database class is used to add pre existing records to the database
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class DatabaseClass
    {
        List<DataStruct> dataStruct; 
        /// <summary>
        /// Constructor for database, create 100000 records
        /// </summary>
        public DatabaseClass()
        {
            dataStruct = new List<DataStruct>();
            Random rand = new Random();
            // Used to select random data from generator
            for (int i = 0; i < 100000; i++)
            {
                // Create 100,000 accounts in database
                DataStruct ds = new DataStruct();
                DatabaseGenerator dataGen = new DatabaseGenerator(rand);
                // Generate random data for database
                dataGen.GetNextAccount(out ds.pin, out ds.acctNo,
                                       out ds.firstName, out ds.lastName,
                                       out ds.balance, out ds.filePath);
                // Get next account in database
                dataStruct.Add(ds);
                // Add data struct object
            }
        }

        /// <summary>
        /// Purpose: To get the account number by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Acnt Number</returns>
        public uint GetAcctNoByIndex(int index)
        {
            uint value;
            if(index >= 0 && index <= dataStruct.Count())
            {
                // if index is in range, then get element
                value = dataStruct.ElementAt(index).acctNo;
            }
            else
            {
                value = 0;
            }
            return value;    
        }
        /// <summary>
        /// Purpose: get associated pin number for account by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>PIN Number</returns>
        public uint GetPINByIndex(int index)
        {
            uint value;
            if (index >= 0 && index <= dataStruct.Count())
            {
                value = dataStruct.ElementAt(index).pin;
                // if index is in range, then get element
            }
            else
            {
                // set value to first element
                value = 0;
            }
            return value;
        }

        /// <summary>
        /// Purpose: To get the first name by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>First name</returns>
        public string GetFirstNameByIndex(int index)
        {
            string value;
            if (index >= 0 && index <= dataStruct.Count())
            {
                // if index is in range, then get element
                value = dataStruct.ElementAt(index).firstName;
            }
            else
            {
                // value is null
                value = "";
            }
            return value;
        }

        /// <summary>
        /// Purpose: To get the last name by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Last name</returns>
        public string GetLastNameByIndex(int index)
        {
            string value;
            if (index >= 0 && index <= dataStruct.Count())
            {
                // if index is in range, then get element
                value = dataStruct.ElementAt(index).lastName;
            }
            else
            {

                value = "";
            }
            return value;
        }

        /// <summary>
        /// Purpose: To get the associated balance of an account by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Balance</returns>

        public int GetBalanceByIndex(int index)
        {
            int value;
            if (index >= 0 && index <= dataStruct.Count())
            {
                // if index is in range, then get element
                value = dataStruct.ElementAt(index).balance;
            }
            else
            {
                value = 0;
            }
            return value;
        }

        /// <summary>
        /// Purpose: Get the number of records in the data struct = 100K
        /// </summary>
        /// <returns>Number of records</returns>
        public int GetNumRecords()
        {
            return dataStruct.Count;
        }

        /// <summary>
        /// Purpose: Get the file path of an image in the data struct
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Filepath in data struct</returns>
        public string getFilePath(int index)
        {
            // if index is in range, then get element
            if (index >= 0 && index <= dataStruct.Count())
            {
                //get filepath
                return dataStruct.ElementAt(index).filePath;
            }
            else
            {
                //no filepath exists
                return null;
            }
        }

        /// <summary>
        /// Set the file path in the database on update
        /// </summary>
        /// <param name="index"></param>
        /// <param name="filePath"></param>
        public void setFilePath(int index, string filePath)
        {
            //used to update the filepath in the database
             dataStruct.ElementAt(index).filePath = filePath;
        }

        /// <summary>
        /// Update user details
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <param name="acntNo"></param>
        /// <param name="pin"></param>
        /// <param name="balance"></param>
        public void updateUser(int idx, string fname, string lname, uint acntNo, uint pin, int balance)
        {
            //Update user fields below
            dataStruct.ElementAt(idx).firstName = fname;
            dataStruct.ElementAt(idx).lastName = lname;
            dataStruct.ElementAt(idx).acctNo = acntNo;
            dataStruct.ElementAt(idx).pin = pin;
            dataStruct.ElementAt(idx).balance = balance;
        }
    }
}
