using Bis_GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Windows.Media.Imaging;
using Tutorial_3_Web_Service.Models;

namespace Tutorial_3_Web_Service.Controllers
{
    /// <summary>
    /// Purpose: Used to get the values from the database
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class GetValuesController : ApiController
    {
        /// <summary>
        /// Purpose: Get the data intermed object and set account fields fo object using getValuesForEntry()
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Data intermed object</returns>
        public DataIntermed Get(int id)
        {
            // Require get request
            LogData log = new LogData();
            DataModel model = new DataModel();
            DataIntermed dataIm = new DataIntermed();
            model.GetValuesForEntry(id, out dataIm.acct, out dataIm.pin, out dataIm.bal, out dataIm.fname, out dataIm.lname, out dataIm.filePath);
            // Get values
            log.logIndexSearch(dataIm);
            // Log data
            return dataIm;
        }

    }
}
