
using Bis_GUI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Http;
using Tutorial_3_Web_Service.Models;

namespace Tutorial_3_Web_Service.Controllers
{
    /// <summary>
    /// Purpose: Stores methods used to retrieve or update data in the GUI. Uses API calls
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class WebApiController : ApiController
    {
        //GET api/<controller>
        private DataModel model = new DataModel();

        /// <summary>
        /// Return number of entries in database
        /// </summary>
        /// <returns>No of entries</returns>
        public int Get()
        {
            LogData log = new LogData();
            log.logNumEntries(model.getNumEntries());
            // Log no of entries
            return model.getNumEntries();
        }

        /// <summary>
        /// Purpose: Update the file path and return the updated account with new filepath
        /// </summary>
        /// <param name="fileValue"></param>
        /// <returns>Updated data intermed object</returns>
        public DataIntermed Post([FromBody] FilePathData fileValue)
        {
            LogData log = new LogData();
            DataIntermed dataInter = new DataIntermed();
            DataModel model = new DataModel();
            model.UpdateFilePath(fileValue.filePath, fileValue.indexToUpdate);
            // Update the file path
            model.GetValuesForEntry(fileValue.indexToUpdate, out dataInter.acct, out dataInter.pin, out dataInter.bal, out dataInter.fname, out dataInter.lname, out dataInter.filePath);
            // Gets the update account
            log.logImageUpload(fileValue.filePath);
            // Log the image upload
            return dataInter;
        }

        /// <summary>
        /// Purpose: Update the user entry
        /// </summary>
        /// <param name="dataIm"></param>
        /// <returns>Data intermed object</returns>
        public DataIntermed Put([FromBody] DataIntermed dataIm)
        {
            LogData log = new LogData();
            DataModel model = new DataModel();
            model.updateUserEntry(dataIm.index, dataIm.fname, dataIm.lname, dataIm.acct, dataIm.pin, dataIm.bal);
            // Update user entry
            model.GetValuesForEntry(dataIm.index, out dataIm.acct, out dataIm.pin, out dataIm.bal, out dataIm.fname, out dataIm.lname, out dataIm.filePath);
            // get new account
            log.updateAccount(dataIm);
            // log update
            return dataIm; 
        }
    }
    
}
