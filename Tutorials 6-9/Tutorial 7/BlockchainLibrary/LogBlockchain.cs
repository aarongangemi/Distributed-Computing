using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainLibrary
{
    public class LogBlockchain
    {
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "/WebStuff/BlockchainLog.txt");
        private StreamWriter writer;

        public void logData(string message)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
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
