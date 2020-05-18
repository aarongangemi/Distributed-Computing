using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    public class JobHost : IClient
    {
        public Job RequestJob()
        {
            Job j = new Job();
            foreach(Job job in JobList.ListOfJobs)
            {
                if(!job.JobRequested)
                {
                    job.JobRequested = true;
                    j = job;
                    break;
                }
            }
          
            return j;
        }
        public void UploadJobSolution(string pyResult, int idx)
        {
            for(int i = 0; i < JobList.ListOfJobs.Count; i++)
            {
                if(JobList.ListOfJobs.ElementAt(i).jobNumber == idx)
                {
                    JobList.ListOfJobs.ElementAt(i).PythonResult = pyResult;
                }
            }
        }
    }
}
