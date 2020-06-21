using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Tutorial_4_Hybrid_Tier.Models
{
    /// <summary>
    /// Purpose: The logging class is used to log messages or errors in the business/hybrid tier (Bank API)
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class BusinessLogger
    {
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Tutorial4LogFiles/log.txt");
        // Create path to log files to
        private StreamWriter writer;

        /// <summary>
        /// Purpose: To log a given message to the desginated file path/file
        /// </summary>
        /// <param name="message"></param>
        public void logMessage(string message)
        {
            writer = new StreamWriter(path, append: true);
            //create a stream writer to append to
            writer.WriteLine("Log file function:");
            writer.WriteLine(message);
            // log message and close writer
            writer.Close();
        }

        /// <summary>
        /// Purpose: To log an error message to the file
        /// </summary>
        /// <param name="errorMessage"></param>
        public void errorLogMessage(string errorMessage)
        {
            writer = new StreamWriter(path, append: true);
            // create a stream writer to file for appending
            writer.WriteLine("Error Occurred: " + errorMessage);
            // write message and close writer
            writer.Close();
        }

    }
}