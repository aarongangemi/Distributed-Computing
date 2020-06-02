using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_9_Blockchain_Library
{
    /// <summary>
    /// Purpose: The transaction storage class stores any collections which are required to store transactions.
    /// The transaction queue is used to store transactions that are required to be processed. The completed
    /// transactions list is required to store any submitted transactions.
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    public class TransactionStorage
    {
        public static Queue<Transaction> TransactionQueue = new Queue<Transaction>();
        public static List<Transaction> CompletedTransactions = new List<Transaction>();
    }
}
