using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public static class MutexClass
    {
        public static bool MutexLock = false;

        public static void Acquire()
        {
            while(MutexLock)
            { }
            MutexLock = true;
        }

        public static void Release()
        {
            MutexLock = false;
        }

    }


}
