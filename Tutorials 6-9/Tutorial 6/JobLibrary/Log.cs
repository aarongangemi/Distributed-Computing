using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLibrary
{
    /// <summary>
    /// Purpose: To Log and data for the application in a file
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public class Log
    {
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "Tutorial6LogFiles/log.txt");
        // Set path for log file
        private StreamWriter writer;
        // define stream writer to write data

        /// <summary>
        /// Purpose: To log the provided success message in a file
        /// </summary>
        /// <param name="message"></param>
        public void logMessage(string message)
        {
            try
            { 
                writer = new StreamWriter(path, append: true);
                // to allow the stream writer to append
                writer.WriteLine("Log file function:");
                // to write the message to log function
                writer.WriteLine(message);
                // close writer
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                // if directory doesn't exist, write to console
                Console.WriteLine("Unable to log data");
            }
            catch (IOException)
            {
                // if anything else goes wrong, log to console
                Console.WriteLine("Unable to log data");
            }
        }

        /// <summary>
        /// Purpose: To log and append an error message to file
        /// </summary>
        /// <param name="message"></param>
        public void logError(string message)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                // to allow the stream writer to append
                writer.WriteLine("Error occured: ");
                // to write the message to log function
                writer.WriteLine(message);
                // close writer
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                // if anything with the directory goes wrong, then log to console
                Console.WriteLine("Unable to log data");
            }
            catch (IOException)
            {
                // if anything else goes wrong, log to console
                Console.WriteLine("Unable to log data");
            }
        }
    }
}
