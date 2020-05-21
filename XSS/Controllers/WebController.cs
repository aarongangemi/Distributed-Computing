using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XSS.Models;

namespace XSS.Controllers
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

        [Route("api/Web/GetMessages")]
        [HttpGet]
        public List<MessageItems> GetMessages()
        {
            StreamReader reader = new StreamReader(path);
            
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] messageData = line.Split(',');
                MessageItems message = new MessageItems();
                message.subject = messageData[0];
                message.message = messageData[1];
                Messages.messageList.Add(message);
            }
            return Messages.messageList;
        }
    }
}