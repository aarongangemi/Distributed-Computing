using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    [ServiceContract]
    public interface IClient
    {
        [OperationContract]
        void RequestJob();

        [OperationContract]
        void UploadSolution();

    }
}
