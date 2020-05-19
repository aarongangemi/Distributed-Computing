using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLibrary
{
    public class Log
    {
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "Tutorial6LogFiles/log.txt");
        private StreamWriter writer;

        public void logMessage(string message)
        {
            try
            { 
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function:");
                writer.WriteLine(message);
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log data");
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to log data");
            }
        }

        public void logError(string message)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Error occured: ");
                writer.WriteLine(message);
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log data");
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to log data");
            }
        }
    }
}
