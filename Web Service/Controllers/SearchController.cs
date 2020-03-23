using Bis_GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_Service.Models;

namespace Web_Service.Controllers
{
    public class SearchController : ApiController
    {
        public DataIntermed Post([FromBody] SearchData value)
        {
            DataIntermed dataInter = new DataIntermed();
            DataModel model = new DataModel();
            for (int i = 0; i < model.getNumEntries(); i++)
            {
                model.GetValuesForEntry(i, out dataInter.acct, out dataInter.pin, out dataInter.bal, out dataInter.fname, out dataInter.lname);
                if (dataInter.lname.Equals(value.searchStr))
                {
                    break;
                }
            }
            return dataInter;
        }
    }
}
