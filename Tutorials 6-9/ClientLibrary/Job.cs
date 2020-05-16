using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    [DataContract]
    public class Job
    {
        public string PythonSrc;
        public bool JobAssigned;
        public string PythonResult;
        public bool JobComplete;
        public byte[] hash;
        public Job(string src, byte[] hash)
        {
            PythonSrc = src;            
            this.hash = hash;
            JobAssigned = false;
            JobComplete = false;
        }
    }
}
