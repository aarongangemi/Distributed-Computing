using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace IDataServerInterface
{
    //Make this a service contract as it is a service interface

    [ServiceContract] //Defines that interface is for distributed objects
    public interface IDataServerInterface
    {
        //Each of these are service function contracts. They need to be tagged as OperationContracts.
        //Defines that method will be for distributed objects
        [OperationContract]
        int GetNumEntries();
        [OperationContract]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin,
                               out int bal, out string fname, out string lname,
                               out string filePath);
    }
}
