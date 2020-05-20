using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlockchainIntermed
{
    public class Account
    {
        public static int accountIDTracker = 0;
        public int accountID;
        public float accountAmount;

        public Account()
        {
            accountIDTracker++;
            accountID = accountIDTracker;
            accountAmount = 0;
        }
    }
}