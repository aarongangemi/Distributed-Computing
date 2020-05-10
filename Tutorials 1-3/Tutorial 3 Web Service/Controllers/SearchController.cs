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
    public class SearchController : ApiController
    {
        public DataIntermed Post([FromBody] SearchData value)
        {
            LogData log = new LogData();
            DataIntermed dataInter = new DataIntermed();
            DataModel model = new DataModel();
            for (int i = 0; i < model.getNumEntries(); i++)
            {
                model.GetValuesForEntry(i, out dataInter.acct, out dataInter.pin, out dataInter.bal, out dataInter.fname, out dataInter.lname, out dataInter.filePath);
                if (dataInter.lname.Equals(value.searchStr))
                {
                    log.logSearch(dataInter);
                    dataInter.index = i;
                    return dataInter;
                }
            }
            log.noResultFound(value.searchStr);
            return null;
        }

    }
}
