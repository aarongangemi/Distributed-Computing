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
    //InstanceContextMode ensures data tier implemented will be singleton
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode =InstanceContextMode.Single)]
    //Must be internal because it must be accessed through the interface
    internal class DataServer : IDataServerInterface.IDataServerInterface
    { 
        private DatabaseClass database = new DatabaseClass();
        public int GetNumEntries()
        {
            return database.GetNumRecords();
        }
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin,
                               out int bal, out string fname, out string lname,
                               out string filePath)
        {
            acctNo = database.GetAcctNoByIndex(index);
            pin = database.GetPINByIndex(index);
            bal = database.GetBalanceByIndex(index);
            fname = database.GetFirstNameByIndex(index);
            lname = database.GetLastNameByIndex(index);
            filePath = database.getFilePath(index);
        }

        public void SetFilePath(string filePath, int index)
        {
            database.setFilePath(index, filePath);
        }
    }
}
