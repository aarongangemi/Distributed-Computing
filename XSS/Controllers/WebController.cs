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
        [Route("api/Web/StoreMessage/")]
        [HttpPost]
        public void StoreMessage([FromBody] MessageItems mItem)
        {
            if (!string.IsNullOrEmpty(mItem.subject) && !string.IsNullOrEmpty(mItem.message))
            {
                writer = new StreamWriter(path, append: true);
                writer.WriteLine(mItem.subject + "," + mItem.message);
                writer.Close();
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Subject and message cannot be null or empty")),
                    ReasonPhrase = "subject or message is empty"
                };
                 throw new HttpResponseException(response);
                
            }

        }

        [Route("api/Web/GetMessages")]
        [HttpGet]
        public List<MessageItems> GetMessages()
        {

            StreamReader reader = new StreamReader(path);
            Messages messages = new Messages();
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] messageData = line.Split(',');
                MessageItems message = new MessageItems();
                message.subject = messageData[0];
                message.message = messageData[1];
                messages.messageList.Add(message);
            }
            reader.Close();
            return messages.messageList;
        }
    }
}