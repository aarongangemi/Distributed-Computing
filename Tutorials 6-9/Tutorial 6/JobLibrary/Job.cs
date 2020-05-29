using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JobLibrary
{
    /// <summary>
    /// Purpose: The Job class initializes a Job object for the client to complete.
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public class Job
    {
        public string PythonSrc;
        public string PythonResult;
        public bool JobRequested;
        public byte[] hash;
        public int jobNumber;

        /// <summary>
        /// Purpose: To construct a job.
        /// </summary>
        public Job() 
        {
            JobRequested = false;
        }

        /// <summary>
        /// Purpose: To set the job hash
        /// </summary>
        /// <param name="hash"></param>
        public void setHash(byte[] hash)
        {
            this.hash = hash;
        }

        /// <summary>
        /// Purpose: To assign the python source code to the job
        /// </summary>
        /// <param name="src"></param>
        public void setPythonSrc(string src)
        {
            PythonSrc = src;
        }

        /// <summary>
        /// Purpose: To assign a job number to a given job
        /// </summary>
        /// <param name="num"></param>
        public void setJobNumber(int num)
        {
            jobNumber = num;
        }
    }
}
