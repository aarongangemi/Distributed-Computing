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
    public class GetValuesController : ApiController
    {
        public DataIntermed Get(int id)
        {
            LogData log = new LogData();
            DataModel model = new DataModel();
            DataIntermed dataIm = new DataIntermed();
            model.GetValuesForEntry(id, out dataIm.acct, out dataIm.pin, out dataIm.bal, out dataIm.fname, out dataIm.lname, out dataIm.filePath);
            log.logIndexSearch(dataIm);
            return dataIm;
        }

    }
}
