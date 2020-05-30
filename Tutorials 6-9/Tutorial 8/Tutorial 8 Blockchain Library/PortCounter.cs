using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    /// <summary>
    /// Purpose: To store fields which keep track of the client number and current port.
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    public static class PortCounter
    {
        public static int CurrentPort = 8100;
        public static int ClientCounter = -1;
    }
}
