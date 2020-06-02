using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    /// <summary>
    /// Purpose: To store a queue which is used to store transactions
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    public class TransactionStorage
    {
        public static Queue<Transaction> TransactionQueue = new Queue<Transaction>();
    }
}
