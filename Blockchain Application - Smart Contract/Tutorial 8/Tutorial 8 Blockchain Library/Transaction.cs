using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    /// <summary>
    /// Purpose: Used to create a transaction object for tutorial 7 and contains fields required to complete transaction
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public class Transaction
    {
        public uint walletIdFrom;
        public uint walletIdTo;
        public float amount;
    }
}
