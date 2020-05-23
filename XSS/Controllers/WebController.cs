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
        private static List<MessageItems> messageList = new List<MessageItems>();
        // GET api/<controller>
        [Route("api/Web/StoreMessage/")]
        [HttpPost]
        public void StoreMessage([FromBody] MessageItems mItem)
        {
            if (!string.IsNullOrEmpty(mItem.subject) && !string.IsNullOrEmpty(mItem.message))
            {
                messageList.Add(mItem);
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
            return messageList;
        }
    }
}