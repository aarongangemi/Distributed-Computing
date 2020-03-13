using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_2
{
    [ServiceContract] //Defines that interface is for distributed objects
    public interface BusinessServerInterface
    {
        //Each of these are service function contracts. They need to be tagged as OperationContracts.
        //Defines that method will be for distributed objects
        [OperationContract]
        int GetNumEntries();
        [OperationContract]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin,
                               out int bal, out string fname, out string lname);
    }
}
