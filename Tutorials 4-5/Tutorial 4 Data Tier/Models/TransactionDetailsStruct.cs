using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutorial_4_Data_Tier.Models
{
    public class TransactionDetailsStruct
    {
        public uint transactionId;
        public uint senderId;
        public uint receiverId;
        public uint amount;
    }
}