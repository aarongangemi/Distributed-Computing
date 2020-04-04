using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutorial_4.Models
{
    public class User
    {
        private int id;
        private string firstName;
        private string lastName;

        public User(int id, string firstName, string lastName)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
        }
    }
}