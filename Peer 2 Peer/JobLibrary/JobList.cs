using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLibrary
{
    /// <summary>
    /// Purpose: To set a static list of jobs so that new jobs can be added and retrieved throughout the program
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public static class JobList
    {
        public static List<Job> ListOfJobs = new List<Job>();

    }
}
