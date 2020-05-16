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
        public void RequestJob(out int idx, List<Job> jobList)
        {
            idx = -1;
            if (jobList.Count > 0)
            {
                for (int i = 0; i < jobList.Count; i++)
                {
                    if (!jobList.ElementAt(i).JobAssigned && !jobList.ElementAt(i).JobComplete)
                    {
                        jobList.ElementAt(i).JobAssigned = true;
                        idx = i;
                        break;
                    }
                }
            }
            
        }
        public void UploadJobSolution(string pyResult, int idx, List<Job> jobList)
        {
            jobList.ElementAt(idx).PythonResult = pyResult;
            jobList.ElementAt(idx).JobComplete = true;
        }
    }
}
