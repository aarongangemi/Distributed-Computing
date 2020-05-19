using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Tutorial_4_Data_Tier.Models
{
    public class LogClass
    {
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Tutorial4LogFiles/log.txt");
        private StreamWriter writer;
        public void logMessage(string message)
        {
            writer = new StreamWriter(path, append: true);
            writer.WriteLine("Log file function:");
            writer.WriteLine(message);
            writer.Close();
        }
        public void errorLogMessage(string errorMessage)
        {
            writer = new StreamWriter(path, append: true);
            writer.WriteLine("Error Occurred: " + errorMessage);
            writer.Close();
        }

    }
}