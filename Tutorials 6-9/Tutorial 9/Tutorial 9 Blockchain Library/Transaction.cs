using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_9_Blockchain_Library
{
    public class Transaction
    {
        public string PythonSrc;
        public string PythonResult;
        public int TransactionId;

        public Transaction() { }
        public Transaction(string PythonSrc, string PythonResult, int TransactionId)
        {
            this.PythonSrc = PythonSrc;
            this.PythonResult = PythonResult;
            this.TransactionId = TransactionId;
        }
    }
}
