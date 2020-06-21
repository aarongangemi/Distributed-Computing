using Bis_GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Bis_GUI
{
    /// <summary>
    /// Purpose: Contains methods which logs data for various parts of the program
    /// </summary>
    /// <author>Aaron Gangemi</author>
    /// <Date Modified>23/05/2020</Date>
    public class LogData
    {
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "WPFApp/LogFiles/log.txt");
        // Set file path
        private StreamWriter writer;
        // Set stream writer

        /// <summary>Log the number of entries</summary>  
        /// <param name="noOfEntries"></param>
        public void logNumEntries(int noOfEntries)
        {
            try
            {
                writer = new StreamWriter(path, append: true); 
                // Create stream writer and append to file rather than overwrite
                writer.WriteLine("Log file function: Starting application");
                writer.WriteLine(noOfEntries + " results are in the database");
                // Write to file
                writer.Close();
                // Close writer
            }
            catch(DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log data");
                // If directory does not exist
            }
            catch(IOException)
            {
                Console.WriteLine("Unable to log data");
                // Any IO Exception occuring
            }
        }
        /// <summary>
        /// To log that a search has been completed for a data intermediate object
        /// </summary>
        /// <param name="dataInter"></param>
        public void logSearch(DataIntermed dataInter)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                // Create stream writer and allow to append instead of overwrite
                writer.WriteLine("Log file function: Search by lastname. ");
                writer.WriteLine("Account: " + dataInter.acct +
                    " has searched for last name: " + dataInter.lname
                    + " and found a result for: " + dataInter.fname + " " +
                    dataInter.lname);
                // Write data intermediate object to file
                writer.Close();
                // Close stream writer
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log data");
                // If directory cannot be found
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to log data");
                // If any IO Exception occurs
            }
        }

        /// <summary>
        /// To log that an image upload has occured
        /// </summary>
        /// <param name="filePath"></param>
        public void logImageUpload(string filePath)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                // Create stream writer and allow append
                writer.WriteLine("Log File Function: Upload profile image");
                writer.WriteLine("Image upload from File Path: " + filePath);
                // Write filepath to file
                writer.Close();
                // Close stream writer 
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log data");
                // Catch directory error
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to log data");
                // Catch any IO errors
            }
        }

        /// <summary>
        /// Log the index search and report the account details that were found
        /// </summary>
        /// <param name="dataInter"></param>
        public void logIndexSearch(DataIntermed dataInter)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                // Open stream writer for writing and append instead of overwrite
                writer.WriteLine("Log file function: Retrieve user");
                writer.WriteLine("Account: " + dataInter.acct +
                        " has searched for last name: " + dataInter.lname
                        + " and found a result for: " + dataInter.fname + " " +
                        dataInter.lname);
                // Write account data to file
                writer.Close();
                // Close writer
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log data");
                // Catch error if directory not located
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to log data");
                // Catch any IO error
            }
        }

        /// <summary>
        /// Log the successful account update
        /// </summary>
        /// <param name="dataInter"></param>
        public void updateAccount(DataIntermed dataInter)
        {
            try
            {
                writer = new StreamWriter(path, append: true);
                // Append using stream writer
                writer.WriteLine("Log file function: Retrieve user");
                writer.WriteLine("User " + dataInter.fname + " " + dataInter.lname + " at index: " + dataInter.index + " has been updated.");
                // Write account data to file
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log data");
                // Catch no directory exception
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to log data");
                // Catch IO Exception
            }
        }

        /// <summary>
        /// Logs error messages
        /// </summary>
        /// <param name="errorMessage"></param>
        public void errorLogMessage(string errorMessage)
        {
            try
            {
                // Create stream writer and append data
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Error Occurred: " + errorMessage);
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to log error");
                // Catch directory exception
            }
            catch (IOException)
            {
                // Catch IO Exception
                Console.WriteLine("Unable to log error");
            }
        }

        /// <summary>
        /// Log no result found
        /// </summary>
        /// <param name="lname"></param>
        public void noResultFound(string lname)
        {
            try
            {
                // Create stream writer and append data
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Search by lastname. ");
                writer.WriteLine("No results for last name: " + lname + "when searched");
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                // Catch directory exception
                Console.WriteLine("Unable to log data");
            }
            catch (IOException)
            {
                // Catch IO Exception
                Console.WriteLine("Unable to log data");
            }
        }

        /// <summary>
        /// Log data if the user changes the client URL
        /// </summary>
        /// <param name="urlText"></param>
        public void logUrlChange(string urlText)
        {
            try
            {
                // Create stream writer and append data
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Change of URL");
                writer.WriteLine("Base URL was successfully changed to: " + urlText);
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                // Catch directory exception
                Console.WriteLine("Unable to log data");
            }
            catch (IOException)
            {
                // Catch IO Exception
                Console.WriteLine("Unable to log data");
            }
        }

        /// <summary>
        /// Set path field to log data
        /// </summary>
        /// <param name="inPath"></param>
        public void setPath(string inPath)
        {
            path = inPath;
        }

        /// <summary>
        /// Log data on timer end 
        /// </summary>
        public void logTimerEnd()
        {
            try
            {
                // Create stream writer and append data
                writer = new StreamWriter(path, append: true);
                writer.WriteLine("Log file function: Bank transactions processed");
                writer.WriteLine("2.5 Minutes passed. Bank Transactions processed succesfully");
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                // Catch directory exception
                Console.WriteLine("Unable to log data");
            }
            catch (IOException)
            {
                // Catch IO Exception
                Console.WriteLine("Unable to log data");
            }
        }
    }
}