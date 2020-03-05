using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_1
{
    public class DatabaseClass
    {
        List<DataStruct> dataStruct;
        public DatabaseClass()
        {
            dataStruct = new List<DataStruct>();
            for(int i = 0; i < 500; i++)
            {
                DataStruct ds = new DataStruct();
                DatabaseGenerator dataGen = new DatabaseGenerator();
                dataGen.GetNextAccount(out ds.pin, out ds.acctNo,
                                       out ds.firstName, out ds.lastName,
                                       out ds.balance);
                dataStruct.Add(ds);
            }
        }
    }
}
