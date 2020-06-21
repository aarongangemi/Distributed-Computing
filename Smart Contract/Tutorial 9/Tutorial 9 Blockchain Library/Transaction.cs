using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_9_Blockchain_Library
{
    /// <summary>
    /// Purpose: The purpose of the transaction class is to store fields which create a transaction.
    /// This includes python code, python result and a transaction ID which represents the client 
    /// that submitted the transaction.
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    public class Transaction
    {
        public string PythonSrc;
        public string PythonResult;
        public int TransactionId;

        /// <summary>
        /// Purpose: The Transaction class requires an empty constructor so it can be
        /// used for the .NET remoting server. This constructor is never called.
        /// </summary>
        public Transaction() { }

        /// <summary>
        /// Purpose: Alternate constructor used to create a transaction by setting all fields
        /// </summary>
        /// <param name="PythonSrc"></param>
        /// <param name="PythonResult"></param>
        /// <param name="TransactionId"></param>
        public Transaction(string PythonSrc, string PythonResult, int TransactionId)
        {
            this.PythonSrc = PythonSrc;
            this.PythonResult = PythonResult;
            this.TransactionId = TransactionId;
        }
    }
}
