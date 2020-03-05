using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_1
{
    internal class DatabaseGenerator
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
            Random random = new Random();
            return (uint)random.Next(10000000, 99999999);
        }

        private int GetBalance()
        {
            Random random = new Random();
            return random.Next(1, 999999999);
        }

        public void GetNextAccount(out uint pin, out uint acctNo,
                                   out string firstName, out string lastName,
                                   out int balance)
        {
            pin = GetPin();
            acctNo = GetAcctNo();
            firstName = GetFirstname();
            lastName = GetLastname();
            balance = GetBalance();
        }
    }
}
