using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutorial4.Models
{
    internal class TransactionDetailsStruct
    {
        public uint transactionId;
        public uint senderId;
        public uint receiverId;
        public TransactionDetailsStruct()
        {
            transactionId = 0;
            senderId = 0;
            receiverId = 0;
        }
    }
}