using ClientLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_6_Web_Server.Models;

namespace Tutorial_6_Web_Server.Controllers
{
    public class ClientController : ApiController
    {
        /// <summary>
        /// Purpose: To retrieve the client list
        /// </summary>
        /// <returns>Client list</returns>
        [Route("api/Client/GetClientList")]
        [HttpGet]
        public List<Client> GetClientList()
        {
            return ClientList.clientList;
        }

        /// <summary>
        /// Purpose: To register a new client by adding to list
        /// </summary>
        /// <param name="client"></param>
        [Route("api/Client/Register/")]
        [HttpPost]
        public void RegisterClient([FromBody] Client client)
        {
             ClientList.clientList.Add(client);
        }

        /// <summary>
        /// Purpose: TO update the number of jobs a client has completed
        /// </summary>
        /// <param name="idx"></param>
        [Route("api/Client/UpdateCount/{idx}")]
        [HttpPut]
        public void UpdateCount(int idx)
        {
            ClientList.clientList.ElementAt(idx).jobsCompleted++;
        }

        /// <summary>
        /// Purpose: To remove a client based on the given port number
        /// </summary>
        /// <param name="clientNo"></param>
        [Route("api/Client/Remove/{clientNo}")]
        [HttpGet]
        public void RemoveClient(int clientNo)
        {
            try
            {
                for(int i = 0; i < ClientList.clientList.Count; i++)
                {
                    // loop through clients
                    if (ClientList.clientList.ElementAt(i).port == clientNo.ToString())
                    {
                        // remove
                        ClientList.clientList.RemoveAt(i);
                        break;
                    }
                }
            }
            catch(ArgumentOutOfRangeException)
            {
                //No clients left
            }
        }
    }
}