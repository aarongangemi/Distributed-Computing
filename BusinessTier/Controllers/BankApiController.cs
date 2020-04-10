using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Tutorial4.Models;
using RouteAttribute = System.Web.Mvc.RouteAttribute;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;

namespace Tutorial4BusinessTier.Controllers
{

    public class BankApiController : ApiController
    {
        private string URL = "https://localhost:44305/";
        private RestClient client;


        [Route("api/BankApi/CreateUser/{fname}/{lname}")]
        [HttpPost]
        public UserDetailsStruct createUser(string fname, string lname)
        {
            client = new RestClient(URL);
            //Set user fields
            RestRequest userRequest = new RestRequest("api/User" + fname + "/" + lname);
            IRestResponse response = client.Post(userRequest);
            UserDetailsStruct user = JsonConvert.DeserializeObject<UserDetailsStruct>(response.Content);
            return user;
        }
    }
}