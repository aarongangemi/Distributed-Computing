using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class Job
    {
        public string PythonSrc;
        public bool JobAssigned;
        public string PythonResult;
        public int JobId;
        public Job(string PythonSrc)
        {
            this.PythonSrc = PythonSrc;
        }
    }
}
