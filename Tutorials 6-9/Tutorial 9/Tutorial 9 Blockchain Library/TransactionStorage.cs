using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_9_Blockchain_Library
{
    public class TransactionStorage
    {
        public static Queue<Transaction> TransactionQueue = new Queue<Transaction>();
    }
}
