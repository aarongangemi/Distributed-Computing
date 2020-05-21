using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace XSSApplication.Controllers
{
    public class WebController : ApiController
    {
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "/WebStuff/XSSDatabase.txt");
        private StreamWriter writer;
        // GET api/<controller>
        [Route("api/Web/StoreMessage/{message}")]
        [HttpPost]
        public void StoreMessage(string message)
        {
            writer = new StreamWriter(path, append: true);
            writer.WriteLine(message);
            writer.Close();
        }
    }
}