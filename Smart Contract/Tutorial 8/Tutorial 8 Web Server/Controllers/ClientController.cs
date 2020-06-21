using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_8_Blockchain_Library;
using Tutorial_8_Web_Server.Models;

namespace Tutorial_8_Web_Server.Controllers
{
    public class ClientController : ApiController
    {
        
        /// <summary>
        /// Purpose: To retrieve the client list using a get request
        /// </summary>
        /// <returns></returns>
        [Route("api/Client/GetClientList")]
        [HttpGet]
        public List<Client> GetClientList()
        {
            return ClientList.clientList;
        }

        /// <summary>
        /// Purpose: To register a new client with the web server
        /// </summary>
        /// <param name="client"></param>
        [Route("api/Client/Register/")]
        [HttpPost]
        public void RegisterClient([FromBody] Client client)
        {
            ClientList.clientList.Add(client);
        }

        /// <summary>
        /// To update the count for the number of jobs a client has completed
        /// </summary>
        /// <param name="idx"></param>
        [Route("api/Client/UpdateCount/{idx}")]
        [HttpPut]
        public void UpdateCount(int idx)
        {
            ClientList.clientList.ElementAt(idx).jobsCompleted++;
        }

        /// <summary>
        /// To remove a client based on the given port number
        /// </summary>
        /// <param name="portNo"></param>
        [Route("api/Client/Remove/{portNo}")]
        [HttpGet]
        public void RemoveClient(int portNo)
        {
            try
            {
                for (int i = 0; i < ClientList.clientList.Count; i++)
                {
                    // loop through clients, find port and remove
                    if (ClientList.clientList.ElementAt(i).port == portNo.ToString())
                    {
                        ClientList.clientList.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // No clients left
            }
        }
    }
}