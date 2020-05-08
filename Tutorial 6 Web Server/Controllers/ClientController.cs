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
            return ClientBase.clientList;
        }

        [Route("api/Client/Register/")]
        [HttpPost]
        public void RegisterClient([FromBody] Client client)
        {
            //Register client here
            ClientBase.clientList.Add(client);
        }

    }
}