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
    public class WebApiController : ApiController
    {
        //GET api/<controller>
        DataModel model = new DataModel();
        public int Get()
        {
            return model.getNumEntries();
        }
        
        //GET api/<controller>/5
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

        //POST api/<controller>
        public DataIntermed Post(string value)
        {
            DataIntermed dataInter = new DataIntermed();
            DataModel model = new DataModel();
            int i;
            for (i = 0; i < model.getNumEntries(); i++)
            {
                model.GetValuesForEntry(i, out dataInter.acct, out dataInter.pin, out dataInter.bal, out dataInter.fname, out dataInter.lname, out dataInter.filePath);
                if (dataInter.lname.Equals(value))
                {
                    break;
                }
            }
            return dataInter;
        }


        public DataIntermed Post(int idx, [FromBody] string filePath)
        {
            DataIntermed dataInter = new DataIntermed();
            DataModel model = new DataModel();
            model.UpdateFilePath(filePath, idx);
            model.GetValuesForEntry(idx, out dataInter.acct, out dataInter.pin, out dataInter.bal, out dataInter.fname, out dataInter.lname, out dataInter.filePath);
            return dataInter;
        }
       
    }
    
}
