using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JobLibrary
{
    /// <summary>
    /// Purpose: The JobHost is the implementation of the server side functions. 
    /// These allow the client to either request a job or upload a job solution
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    public class JobHost : IClient
    {
        // Log field used to log data
        private Log log = new Log();

        /// <summary>
        /// Purpose: To allow the client to request the next available job in the job list
        /// </summary>
        /// <returns></returns>
        public Job RequestJob()
        {
            Job j = new Job();
            foreach(Job job in JobList.ListOfJobs)
            {
                // Find the first job in the list that has not been requested
                if(!job.JobRequested)
                {
                    job.JobRequested = true;
                    // set to requested
                    j = job;
                    log.logMessage("Job " + j.jobNumber + " was successfully requested");
                    // log retrieval
                    break;
                }
            }
            // return job
            return j;
        }

        /// <summary>
        /// Purpose: To upload the python result to the server at a given index in the job list
        /// </summary>
        /// <param name="pyResult"></param>
        /// <param name="idx"></param>
        public void UploadJobSolution(string pyResult, int idx)
        {
            // Loop through job list
            for(int i = 0; i < JobList.ListOfJobs.Count; i++)
            {
                if(JobList.ListOfJobs.ElementAt(i).jobNumber == idx)
                {
                    // find job and set result
                    JobList.ListOfJobs.ElementAt(i).PythonResult = pyResult;
                }
            }
        }
    }
}
