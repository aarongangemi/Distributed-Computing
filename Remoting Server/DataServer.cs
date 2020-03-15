using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Tutorial_1;

namespace Remoting_Server
{
    //Concurrency Mode indicates that server will be multi-threaded
    //UseSynchronisationContext indicates that we will handle synchronisation
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    //Must be internal because it must be accessed through the interface
    internal class DataServer : DataServerInterface
    {
        private DatabaseClass database = new DatabaseClass();
        public int GetNumEntries()
        {
            return database.GetNumRecords();
        }
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin,
                               out int bal, out string fname, out string lname)
        {
            acctNo = database.GetAcctNoByIndex(index);
            pin = database.GetPINByIndex(index);
            bal = database.GetBalanceByIndex(index);
            fname = database.GetFirstNameByIndex(index);
            lname = database.GetLastNameByIndex(index);
        }
    }
}
