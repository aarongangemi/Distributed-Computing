using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BankDB;

namespace Tutorial4.Models
{
    internal class AccountDetailsStruct
    {
        public uint acntId;
        public uint userId;
        public double acntBal;

        public AccountDetailsStruct()
        {
            acntId = 0;
            userId = 0;
            acntBal = 0.0;
        }
    }
}