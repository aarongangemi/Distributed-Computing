using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockServer
{

    //Means clock is a remotable class
    public class Clock : MarshalByRefObject
    {
        public string GetCurrentTime()
        {
            return DateTime.Now.ToLongTimeString();
        }
    }
}
