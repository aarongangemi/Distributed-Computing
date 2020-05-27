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
        [Route("api/Client/GetClientList")]
        [HttpGet]
        public List<Client> GetClientList()
        {
            return ClientList.clientList;
        }

        [Route("api/Client/Register/")]
        [HttpPost]
        public void RegisterClient([FromBody] Client client)
        {
            ClientList.clientList.Add(client);
        }

        [Route("api/Client/UpdateCount/{idx}")]
        [HttpPut]
        public void UpdateCount(int idx)
        {
            ClientList.clientList.ElementAt(idx).jobsCompleted++;
        }

        [Route("api/Client/Remove/{portNo}")]
        [HttpGet]
        public void RemoveClient(int portNo)
        {
            try
            {
                for (int i = 0; i < ClientList.clientList.Count; i++)
                {
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