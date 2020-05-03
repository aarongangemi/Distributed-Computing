using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tutorial_1
{
    internal class DatabaseGenerator
    {
        private Random random;

        public DatabaseGenerator(Random rand)
        {
            random = rand;
        }

        /****************************************************************
         * Purpose: To return a random first name from the list in the method 
         * Reference: List of names obtained from: https://www.rong-chang.com/namesdict/popular_names.htm
         * Reference: https://www.c-sharpcorner.com/article/how-to-select-a-random-string-from-an-array-of-strings/
         * ***************************************************************/
        private string GetFirstname()
        {
            string[] fnameArray = { "James", "John", "Robert", "Michael", "William",
                "David", "Richard", "Charles", "Joseph", "Thomas", "Christopher", 
                "Daniel", "Paul", "Mark", "Donald", "George", "Kenneth", "Steven", 
                "Edward", "Brian", "Ronalds", "Anthony", "Kevin", "Jason", "Aaron" };
            int arrayIdx = random.Next(fnameArray.Length);
            return fnameArray[arrayIdx];
        }

        /*****************************************************************
         * Purpose: To return a random last name from the list in the method
         * Reference: List of last names obtained from: https://www.worldatlas.com/articles/the-25-most-popular-last-names-in-the-united-states.html
         *****************************************************************/
        private string GetLastname()
        {
            string[] lnameArray = {"Smith", "Johnson","Williams", "Jones", "Brown", "Davis",
                "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson",
                "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez",
                "Robinson","Clark", "Rodriguez", "Lewis", "Lee", "Walker"};
            int arrayIdx = random.Next(lnameArray.Length);
            return lnameArray[arrayIdx];
        }

        private uint GetPin()
        {
            return (uint)random.Next(1000, 9999);
        }

        private uint GetAcctNo()
        {
            return (uint)random.Next(10000000, 99999999);
        }

        private int GetBalance()
        {
            return random.Next(1, 99999999);
        }
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

        /******************************************************
         * Purpose: To set the file path as the default image
         * Image obtained from: https://www.pinterest.com.au/pin/494481234082475699/
         * Author: John Connaly Jr
         * Date Accessed: 02/05/2020
         * *****************************************************/
        private string GetFilePath()
        {
            return "C:/WebStuff/ProfileImage.jpg";
        }
    }
}
