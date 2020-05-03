using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IDataServerInterface
{
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

            [OperationContract]
            void SetFilePath(string filePath, int index);

            [OperationContract]
            void updateUser(int idx, string fname, string lname, uint acntNo, uint pin, int balance);
        }
    
}
