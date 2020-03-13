using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_1
{
    internal class DatabaseGenerator
    {
        private Random random;

        public DatabaseGenerator(Random rand)
        {
            random = rand;
        }
        private string GetFirstname()
        {
            int length = 7;
            StringBuilder str = new StringBuilder();
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
            return (uint)random.Next(1000, 9999);
        }

        private uint GetAcctNo()
        {
            return (uint)random.Next(10000000, 99999999);
        }

        private int GetBalance()
        {
            return random.Next(1, 999999999);
        }

        public string GetFilePath()
        {
            return "";
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
    }
}
