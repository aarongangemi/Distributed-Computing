using Bis_GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_3_Web_Service.Models;

namespace Tutorial_3_Web_Service.Controllers
{
    /// <summary>
    /// Purpose: Used to search for a last name that matches the search string
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class SearchController : ApiController
    {
        /// <summary>
        /// Purpose: A POST Method which check the last name
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Data intermed object</returns>
        public DataIntermed Post([FromBody] SearchData value)
        {
            LogData log = new LogData();
            DataIntermed dataInter = new DataIntermed();
            DataModel model = new DataModel();
            for (int i = 0; i < model.getNumEntries(); i++)
            {
                // Loop through entries
                model.GetValuesForEntry(i, out dataInter.acct, out dataInter.pin, out dataInter.bal, out dataInter.fname, out dataInter.lname, out dataInter.filePath);
                if (dataInter.lname.Equals(value.searchStr))
                {
                    // Check if last name matches
                    log.logSearch(dataInter);
                    dataInter.index = i;
                    return dataInter; // return account that matches
                }
            }
            log.noResultFound(value.searchStr);
            // log if no result found
            return null;
        }

    }
}
