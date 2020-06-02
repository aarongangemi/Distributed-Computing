using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tutorial_1
{
    /// <summary>
    /// Generates element randomly for database
    /// </summary>
    internal class DatabaseGenerator
    {
        private Random random;

        /// <summary>
        /// Database generator constructor - set random field
        /// </summary>
        /// <param name="rand"></param>
        public DatabaseGenerator(Random rand)
        {
            random = rand;
        }

        /// <summary>
        /// Purpose: To return a random first name from the list in the method 
        /// Reference: List of names obtained from: https://www.rong-chang.com/namesdict/popular_names.htm
        /// Reference: https://www.c-sharpcorner.com/article/how-to-select-a-random-string-from-an-array-of-strings/
        /// </summary>
        /// <returns>First name</returns>
        private string GetFirstname()
        {
            // list of first names to generate from
            string[] fnameArray = { "James", "John", "Robert", "Michael", "William",
                "David", "Richard", "Charles", "Joseph", "Thomas", "Christopher", 
                "Daniel", "Paul", "Mark", "Donald", "George", "Kenneth", "Steven", 
                "Edward", "Brian", "Ronalds", "Anthony", "Kevin", "Jason", "Aaron" };
            int arrayIdx = random.Next(fnameArray.Length);
            // return random element from array
            return fnameArray[arrayIdx];
        }

         /// <summary>
         /// Purpose: To return a random last name from the list in the method
         /// Reference: List of last names obtained from: https://www.worldatlas.com/articles/the-25-most-popular-last-names-in-the-united-states.html
         /// </summary>
         /// <returns>Last name</returns>
        private string GetLastname()
        {
            // List of last names to take from
            string[] lnameArray = {"Smith", "Johnson","Williams", "Jones", "Brown", "Davis",
                "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson",
                "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez",
                "Robinson","Clark", "Rodriguez", "Lewis", "Lee", "Walker"};
            int arrayIdx = random.Next(lnameArray.Length);
            // Pick and return random last name
            return lnameArray[arrayIdx];
        }

        /// <summary>
        /// Purpose: Generate a random pin number between 1000 and 9999
        /// </summary>
        /// <returns>PIN</returns>
        private uint GetPin()
        {
            return (uint)random.Next(1000, 9999);
        }

        /// <summary>
        /// Purpose: Generate a random account number
        /// </summary>
        /// <returns>Acnt number</returns>
        private uint GetAcctNo()
        {
            return (uint)random.Next(10000000, 99999999);
        }

        /// <summary>
        /// Purpose: Generate a balance between 1 and 9999999
        /// </summary>
        /// <returns>balance</returns>
        private int GetBalance()
        {
            return random.Next(1, 99999999);
        }

        /// <summary>
        /// Purpose: Used to created an account for passed in fields
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="acctNo"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="balance"></param>
        /// <param name="filePath"></param>
        public void GetNextAccount(out uint pin, out uint acctNo,
                                   out string firstName, out string lastName,
                                   out int balance, out string filePath)
        {
            pin = GetPin();
            acctNo = GetAcctNo();
            firstName = GetFirstname();
            lastName = GetLastname();
            balance = GetBalance();
            filePath = GetFilePath();
        }

        /// <summary>
        /// Purpose: To set the file path as the default image
        /// Image obtained from: https://www.pinterest.com.au/pin/494481234082475699/
        /// Author: John Connaly Jr
        /// Date Accessed: 02/05/2020
        /// </summary>
        /// <returns>Filepath</returns>
        private string GetFilePath()
        {
            return Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Images/ProfileImage.jpg");
        }
    }
}
