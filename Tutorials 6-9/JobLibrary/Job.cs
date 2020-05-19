using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JobLibrary
{
    public class Job
    {
        public string PythonSrc;
        public string PythonResult;
        public bool JobRequested;
        public byte[] hash;
        public int jobNumber;

        public Job() 
        {
            JobRequested = false;
        }

        public void setHash(byte[] hash)
        {
            this.hash = hash;
        }

        public void setPythonSrc(string src)
        {
            PythonSrc = src;
        }

        public void setJobNumber(int num)
        {
            jobNumber = num;
        }
    }
}
