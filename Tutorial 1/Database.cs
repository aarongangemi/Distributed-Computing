using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_1
{
    internal class Database
    {
        private string GetFirstname()
        {
            int length = 7;
            StringBuilder str = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str.Append(letter);
            }
            return str.ToString();
        }

        private string GetLastname()
        {
            int length = 7;
            StringBuilder str = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str.Append(letter);
            }
            return str.ToString();
        }

        private uint GetPin()
        {
            Random random = new Random();
            return (uint)random.Next(1000, 9999);
        }

        private uint GetAcctNo()
        {

        }

        private int GetBalance()
        {

        }

        public void GetNextAccount(out uint pin, out uint acctNo,
                                   out string firstName, out string lastName,
                                   out int balance)
        {

        }
    }
}
