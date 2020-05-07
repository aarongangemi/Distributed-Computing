using Bis_GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Bis_GUI
{
    public class LogData
    {
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "WPFApp/LogFiles/log.txt");
        private StreamWriter writer;

        public void logNumEntries(int noOfEntries)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Starting application");
                writer.WriteLine(noOfEntries + " results are in the database");
                writer.Close();
            }
            catch(DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log data");
            }
            catch(IOException)
            {
                Console.WriteLine("Unable to log data");
            }
        }

        public void logSearch(DataIntermed dataInter)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Search by lastname. ");
                writer.WriteLine("Account: " + dataInter.acct +
                    " has searched for last name: " + dataInter.lname
                    + " and found a result for: " + dataInter.fname + " " +
                    dataInter.lname);
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

        public void logImageUpload(string filePath)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log File Function: Upload profile image");
                writer.WriteLine("Image upload from File Path: " + filePath);
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

        public void logIndexSearch(DataIntermed dataInter)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Retrieve user");
                writer.WriteLine("Account: " + dataInter.acct +
                        " has searched for last name: " + dataInter.lname
                        + " and found a result for: " + dataInter.fname + " " +
                        dataInter.lname);
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

        public void updateAccount(DataIntermed dataInter)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Retrieve user");
                writer.WriteLine("User " + dataInter.fname + " " + dataInter.lname + " at index: " + dataInter.index + " has been updated.");
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

        public void errorLogMessage(string errorMessage)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Error Occurred: " + errorMessage);
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log error");
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to log error");
            }
        }

        public void noResultFound(string lname)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Search by lastname. ");
                writer.WriteLine("No results for last name: " + lname + "when searched");
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

        public void logUrlChange(string urlText)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Change of URL");
                writer.WriteLine("Base URL was successfully changed to: " + urlText);
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

        public void setPath(string inPath)
        {
            path = inPath;
        }
        public void logTimerEnd()
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Bank transactions processed");
                writer.WriteLine("2 Minutes passed. Bank Transactions processed succesfully");
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