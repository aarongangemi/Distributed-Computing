using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    [ServiceContract]
    public interface IClient
    {
        [OperationContract]
        void RequestJob();

        [OperationContract]
        void UploadJobSolution();
    }
}
