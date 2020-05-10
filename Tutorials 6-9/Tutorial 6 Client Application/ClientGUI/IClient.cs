using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_6_Client_Application.ClientGUIClass
{
    [ServiceContract]
    public interface IClient
    {
        [OperationContract]
        void RequestJob();
        [OperationContract]
        void DownloadJob();
        [OperationContract]
        void UploadJobSolution();
    }
}
