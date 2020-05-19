using ClientLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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

        [Route("api/Client/Remove/{idx}")]
        [HttpPost]
        public void RemoveCLient(int idx)
        {
            if(idx <= ClientList.clientList.Count)
            {
                ClientList.clientList.RemoveAt(idx);
            }
        }
    }
}