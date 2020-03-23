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
        public DataIntermed Post([FromBody]SearchData value)
        {
            DataIntermed dataInter = new DataIntermed();
            DataModel model = new DataModel();
            int i;
            uint acntNo, pin;
            int bal;
            string fname, lname;
            for (i = 0; i < model.getNumEntries(); i++)
            {
                model.GetValuesForEntry(i, out acntNo, out pin, out bal, out fname, out lname);
                if (lname.Equals(value.ToString()))
                {
                    dataInter.lname = lname;
                    dataInter.acct = acntNo;
                    dataInter.pin = pin;
                    dataInter.bal = bal;
                    dataInter.fname = fname;
                    break;
                }
            }
            return dataInter;
        }
    }
}
