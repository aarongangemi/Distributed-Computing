using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_6_Client_Application.ClientGUIClass
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    public class ClientHost : IClient
    {
        public void DownloadJob()
        {   
            
        }

        public void RequestJob()
        {
            
        }

        public void UploadJobSolution()
        {
            
        }
    }
}
