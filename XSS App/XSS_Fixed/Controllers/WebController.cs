using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using XSS.Models;

namespace XSS.Controllers
{
    /// <summary>
    /// Purpose: The web controller is used to store a list of messages that are display in the admin web page.
    /// It contains validation that is used to prevent a persistent XSS attack.
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    public class WebController : ApiController
    {
        private static List<MessageItems> messageList = new List<MessageItems>();
        /// <summary>
        /// Purpose: To validate the passed in message, and if validation is successful, then store the message in the list
        /// </summary>
        /// <param name="mItem"></param>
        [Route("api/Web/StoreMessage/")]
        [HttpPost]
        public void StoreMessage([FromBody] MessageItems mItem)
        {
            bool validString = true;
            // indicates if validation was successful
            if(!string.IsNullOrEmpty(mItem.subject) && !string.IsNullOrEmpty(mItem.message))
            { 
                // message and subject cannot be empty fields
                for(int i = 0; i < mItem.subject.Length; i++)
                {
                    if(!Char.IsLetterOrDigit(mItem.subject, i))
                    {
                        validString = false;
                        // check each character of subject to check it is not a special character
                        break;
                    }
                }
                for(int j = 0; j < mItem.message.Length; j++)
                {
                    if(!Char.IsLetterOrDigit(mItem.message,j))
                    {
                        // check each character of message to check if it is not a special character
                        validString = false;
                        break;
                    }
                }
                if(validString)
                {
                    // if both checks have passed, then it is not an XSS attack so it can be added to list
                    messageList.Add(mItem);
                }
                else
                {
                    // if validation fails, throw exception back to web page
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        // response to print
                        Content = new StringContent(string.Format("Subject and message cannot contain invalid characters")),
                        ReasonPhrase = "subject or message contains invalid characters"
                    };
                    throw new HttpResponseException(response);
                }
            }
            else
            {
                // if string is null or empty, throw exception message back to user
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Subject and message cannot be null or empty")),
                    ReasonPhrase = "subject or message is empty"
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        /// Purpose: to retrieve the list of messages for the admin
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