using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{

    /// <summary>
    /// Purpose: The port counter class is used to track the current port number so
    /// no 2 clients will have the same port
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public static class PortCounter
    {
        public static int CurrentPort = 8100;
        // Field is static to keep track of port
    }
}
