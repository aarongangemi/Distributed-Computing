using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Tutorial_4_Business_Tier.Models
{
    public class LogClass
    {
        private StreamWriter writer;


        public void errorLogMessage(string errorMessage)
        {
            writer = new StreamWriter("C:/WebStuff/Tutorial_4_ErrorLog", append: true);
            writer.WriteLine("Error Occurred: " + errorMessage);
            writer.Close();
        }

        public void successLogMessage(string successMessage)
        {
            writer = new StreamWriter("C:/WebStuff/Tutorial_4_Log", append: true);
            writer.WriteLine("Log function performed: " + successMessage);
            writer.Close();
        }
    }
}