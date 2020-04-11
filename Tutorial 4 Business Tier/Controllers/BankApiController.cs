using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Business_Tier.Controllers
{
    public class BankApiController : ApiController
    {
        private string URL = "https://localhost:44312/";
        private RestClient client;

        [Route("Main/api/BankApi/Account/{accountID}")]
        [HttpGet]
        public AccountDetailsStruct GetAccount(uint accountID)
        {
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/Account/"+accountID.ToString());
            IRestResponse response = client.Get(request);
            return JsonConvert.DeserializeObject<AccountDetailsStruct>(response.Content);
        }

        [Route("Main/api/BankApi/Deposit/{accountID}/{amount}")]
        public void DepositMoney(uint accountID, uint amount)
        {
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/Account/Deposit/"+accountID.ToString() + "/" + amount.ToString());
            client.Post(request);
        }

        [Route("Main/api/BankApi/Withdraw/{accountID}/{amount}")]
        [HttpPost]
        public void WithdrawMoney(uint accountID, uint amount)
        {
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/Account/Withdraw/" + accountID.ToString() + "/" + amount.ToString());
            client.Post(request);
        }

        [Route("api/BankApi/Create/{fname}/{lname}")]
        [HttpPost]
        public void createAccountAndUser(string fname, string lname)
        {
            //Set user fields
            client = new RestClient(URL);
            RestRequest userRequest = new RestRequest("api/User/Create/" + fname + "/" + lname);
            IRestResponse response = client.Post(userRequest);
            uint userID = JsonConvert.DeserializeObject<uint>(response.Content);
            RestRequest acntRequest = new RestRequest("api/Account/Create/" + userID.ToString());
            client.Post(acntRequest);
        }

        [Route("api/BankApi/GetUser/{userId}")]
        [HttpGet]
        public UserDetailsStruct GetUser(uint userId)
        {
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/User/" + userId.ToString());
            IRestResponse response = client.Get(request);
            return JsonConvert.DeserializeObject<UserDetailsStruct>(response.Content);
        }

        [Route("api/BankApi/CreateTransaction")]
        [HttpPost]
        public void createTransaction()
        {
            client = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/Transactions/Create");
            client.Post(restRequest);
        }

    }
}