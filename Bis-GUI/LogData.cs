using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bis_GUI
{
    public class LogData
    {

        private string path;
        private StreamWriter writer;

        public LogData()
        {
            path = "C:/WebStuff/logFile.txt";
        }

        public void logSearch(DataIntermed dataInter)
        {
            writer = new StreamWriter(path, append: true);
            writer.WriteLine("Log file function: Search by lastname. ");
            writer.WriteLine("Account: " + dataInter.acct +
                " has searched for last name: " + dataInter.lname
                + " and found a result for: " + dataInter.fname + " " +
                dataInter.lname);
            writer.Close();
        }

        public void logImageUpload(string filePath)
        {
            writer = new StreamWriter(path, append: true);
            writer.WriteLine("Log File Function: Upload profile image");
            writer.WriteLine("Image upload from File Path: " + filePath);
            writer.Close();
        }

        public void logIndexSearch(DataIntermed dataInter)
        {
            writer = new StreamWriter(path, append: true);
            writer.WriteLine("Log file function: Retrieve user");
            writer.WriteLine("Account: " + dataInter.acct +
                    " has searched for last name: " + dataInter.lname
                    + " and found a result for: " + dataInter.fname + " " +
                    dataInter.lname);
            writer.Close();
        }

        public void updateAccount(UpdatedUser user)
        {
            writer = new StreamWriter(path, append: true);
            writer.WriteLine("Log file function: Retrieve user");
            writer.WriteLine("User " + user.fname + " " + user.lname + " at index: " + user.index + " has been updated.");
            writer.Close();
        }

        public void errorLogMessage(string errorMessage)
        {
            writer = new StreamWriter("C:/WebStuff/Tutorial_3_ErrorLog", append: true);
            writer.WriteLine("Error Occurred: " + errorMessage);
            writer.Close();
        }
    }
}
