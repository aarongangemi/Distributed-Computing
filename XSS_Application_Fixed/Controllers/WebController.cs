﻿using System;
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
    public class WebController : ApiController
    {
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "/WebStuff/XSSDatabase.txt");
        private StreamWriter writer;
        // GET api/<controller>
        [Route("api/Web/StoreMessage/")]
        [HttpPost]
        public bool StoreMessage([FromBody] MessageItems mItem)
        {
            bool validString = true;
            if (string.IsNullOrEmpty(mItem.subject) || string.IsNullOrEmpty(mItem.message))
            {
                validString = false;
            }
            else
            { 
                for(int i = 0; i < mItem.subject.Length; i++)
                {
                    if(!Char.IsLetterOrDigit(mItem.subject, i))
                    {
                        validString = false;
                        break;
                    }
                }
                for(int j = 0; j < mItem.message.Length; j++)
                {
                    if(!Char.IsLetterOrDigit(mItem.message,j))
                    {
                        validString = false;
                        break;
                    }
                }
                if(validString)
                {
                    writer = new StreamWriter(path, append: true);
                    writer.WriteLine(mItem.subject + "," + mItem.message);
                    writer.Close();
                }
            }
            return validString;

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