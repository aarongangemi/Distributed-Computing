using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    public class ClientHost : IClient
    {

        public void RequestJob()
        {
            for (int i = 0; i < JobList.ListOfJobs.Count; i++)
            {
                if (!JobList.ListOfJobs.ElementAt(i).JobAssigned)
                {
                    JobList.ListOfJobs.ElementAt(i).JobAssigned = true;
                }
            }
        }

        public void UploadJobSolution()
        {
            //Return solution
        }
    }
}
