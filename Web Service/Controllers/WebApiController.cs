﻿using Bis_GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_Service.Models;

namespace Web_Service.Controllers
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
            string fname, lname;
            model.GetValuesForEntry(id, out acntNo, out pin, out balance, out fname, out lname);
            dataIm.acct = acntNo;
            dataIm.pin = pin;
            dataIm.bal = balance;
            dataIm.fname = fname;
            dataIm.lname = lname;
            return dataIm;
        }

        //POST api/<controller>
        public void Post([FromBody]string value)
        {

        }
        
        //PUT api/<controller>/5
        public void Put(int id,[FromBody]string value)
        {

        }

        //DELETE api/<controller>/5
        public void Delete(int id)
        {

        }

        
    }
    
}
