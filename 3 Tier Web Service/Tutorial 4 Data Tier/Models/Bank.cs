using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutorial_4_Data_Tier.Models
{
    /// <summary>
    /// Purpose: To store the static bankDB object
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public static class Bank
    {
        public static BankDB.BankDB bankData = new BankDB.BankDB();
    }
}