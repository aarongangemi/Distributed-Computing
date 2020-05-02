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

        public int Get()
        {
            DataModel model = new DataModel();
            return model.getNumEntries();
        }
        public DataIntermed Get(int id)
        {
            DataModel model = new DataModel();
            DataIntermed dataIm = new DataIntermed();
            uint acntNo, pin;
            int balance;
            string fname, lname, filePath;
            model.GetValuesForEntry(id, out acntNo, out pin, out balance, out fname, out lname, out filePath);
            dataIm.acct = acntNo;
            dataIm.pin = pin;
            dataIm.bal = balance;
            dataIm.fname = fname;
            dataIm.lname = lname;
            dataIm.filePath = filePath;
            return dataIm;
        }

    }
}
