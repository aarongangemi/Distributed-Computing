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
        public void RequestJob(out string text, out int idx)
        {
            int val;
            text = "";
            idx = -1;
            if (JobList.ListOfJobs.Count() != 0)
            {
                for (int i = 0; i < JobList.ListOfJobs.Count; i++)
                {
                    if (!JobList.ListOfJobs.ElementAt(i).JobAssigned)
                    {
                        val = i;
                        JobList.ListOfJobs.ElementAt(i).JobAssigned = true;
                        byte[] data = Convert.FromBase64String(JobList.ListOfJobs.ElementAt(val).PythonSrc);
                        text = Encoding.UTF8.GetString(data);
                        idx = val;
                        break;
                    }
                }
            }
            
        }

        public void UploadJobSolution()
        {
            //Return solution
        }
    }
}
