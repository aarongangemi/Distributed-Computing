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

        [Route("api/Client/Remove/{clientNo}")]
        [HttpGet]
        public void RemoveClient(int clientNo)
        {
            try
            {
                for(int i = 0; i < ClientList.clientList.Count; i++)
                {
                    if (ClientList.clientList.ElementAt(i).port == clientNo.ToString())
                    {
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