using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tutorial_1
{
    public class DatabaseClass
    {
        List<DataStruct> dataStruct;
        public DatabaseClass()
        {
            dataStruct = new List<DataStruct>();
            Random rand = new Random();
            for (int i = 0; i < 100000; i++)
            {
                DataStruct ds = new DataStruct();
                DatabaseGenerator dataGen = new DatabaseGenerator(rand);
                dataGen.GetNextAccount(out ds.pin, out ds.acctNo,
                                       out ds.firstName, out ds.lastName,
                                       out ds.balance, out ds.filePath);
                dataStruct.Add(ds);
            }
        }

        public uint GetAcctNoByIndex(int index)
        {
            uint value;
            if(index >= 0 && index <= dataStruct.Count())
            {
                value = dataStruct.ElementAt(index).acctNo;
            }
            else
            {
                value = 0;
            }
            return value;    
        }

        public uint GetPINByIndex(int index)
        {
            uint value;
            if (index >= 0 && index <= dataStruct.Count())
            {
                value = dataStruct.ElementAt(index).pin;
            }
            else
            {
                value = 0;
            }
            return value;
        }

        public string GetFirstNameByIndex(int index)
        {
            string value;
            if (index >= 0 && index <= dataStruct.Count())
            {
                value = dataStruct.ElementAt(index).firstName;
            }
            else
            {
                value = "Invalid Index was found";
            }
            return value;
        }

        public string GetLastNameByIndex(int index)
        {
            string value;
            if (index >= 0 && index <= dataStruct.Count())
            {
                value = dataStruct.ElementAt(index).lastName;
            }
            else
            {
                value = "Invalid Index was found";
            }
            return value;
        }

        public int GetBalanceByIndex(int index)
        {
            int value;
            if (index >= 0 && index <= dataStruct.Count())
            {
                value = dataStruct.ElementAt(index).balance;
            }
            else
            {
                value = 0;
            }
            return value;
        }

        public int GetNumRecords()
        {
            return dataStruct.Count;
        }

        public string getFilePath(int index)
        {
            if(index >= 0 && index <= dataStruct.Count())
            {
                return dataStruct.ElementAt(index).filePath;
            }
            else
            {
                return null;
            }
        }

        public void setFilePath(int index, string filePath)
        {
             dataStruct.ElementAt(index).filePath = filePath;
        }
    }
}
