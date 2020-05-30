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
    /// <summary>
    /// Purpose: The web controller is used to store a messages list for a persistent XSS attack. This class allows
    /// messages and subjects to be added to the list of messages as well as retrieve the list.
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    public class WebController : ApiController
    {
        private static List<MessageItems> messageList = new List<MessageItems>();

        /// <summary>
        /// Purpose: To add the message item object containing a subject and message and add it to the list
        /// </summary>
        /// <param name="mItem"></param>
        [Route("api/Web/StoreMessage/")]
        [HttpPost]
        public void StoreMessage([FromBody] MessageItems mItem)
        {
            if (!string.IsNullOrEmpty(mItem.subject) && !string.IsNullOrEmpty(mItem.message))
            {
                // check if message item is not empty
                // add message item to list
                messageList.Add(mItem);
            }
            else
            {
                // if empty, throw exception
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Subject and message cannot be null or empty")),
                    ReasonPhrase = "subject or message is empty"
                };
                 throw new HttpResponseException(response);
            }

        }

        /// <summary>
        /// Purpose: To retrieve the message list
        /// </summary>
        /// <returns>Message list</returns>

        [Route("api/Web/GetMessages")]
        [HttpGet]
        public List<MessageItems> GetMessages()
        {
            return messageList;
        }
    }
}