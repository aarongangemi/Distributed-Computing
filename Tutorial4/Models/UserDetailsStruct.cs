using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BankDB;

namespace Tutorial4.Models
{
    public class UserDetailsStruct
    {
        public uint userId;
        public string firstName;
        public string lastName;
        
        public UserDetailsStruct()
        {
            userId = 0;
            firstName = "";
            lastName = "";
        }
    }
}