using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial4.Models;

namespace Tutorial4BusinessTier.Controllers
{

    public class BankApiController : ApiController
    {
        private string URL = "https://localhost:44304/";
        private RestClient client;
        uint acntBalance;
        // GET api/<controller>/5
        public async void GetAccount()
        {
            RestRequest request = new RestRequest("api/Account/{accountID}");
            IRestResponse response = await client.ExecuteGetAsync(request);
            AccountDetailsStruct acntStruct = JsonConvert.DeserializeObject<AccountDetailsStruct>(response.Content);
            //Use Account struct to show fields
        }
        // POST api/<controller>
        public async void DepositMoney()
        {
            //Create HTML amount input form here
            AmountChanger amntObj = new AmountChanger();
            //Need to set amntObj fields
            RestRequest request = new RestRequest("api/Account/Deposit/{accountID}");
            request.AddJsonBody(amntObj);
            IRestResponse response = await client.ExecutePostAsync(request);
            acntBalance = JsonConvert.DeserializeObject<uint>(response.Content);
        }

        public async void WithdrawMoney()
        {
            //Create HTML amount input form here
            AmountChanger amntObj = new AmountChanger();
            //Need to set amntObj fields
            RestRequest request = new RestRequest("api/Account/Withdraw");
            request.AddJsonBody(amntObj);
            IRestResponse response = await client.ExecutePostAsync(request);
            acntBalance = JsonConvert.DeserializeObject<uint>(response.Content);
        }

        public async void createAccountAndUser()
        {
            UserDetailsStruct user = new UserDetailsStruct();
            AccountDetailsStruct acnt = new AccountDetailsStruct();
            //Set user fields
            RestRequest userRequest = new RestRequest("api/User");
            userRequest.AddJsonBody(userRequest);
            IRestResponse response = await client.ExecutePostAsync(userRequest);
            uint userID = JsonConvert.DeserializeObject<uint>(response.Content);
            RestRequest acntRequest = new RestRequest("api/Account/Create/");
            acntRequest.AddJsonBody(acntRequest);
            IRestResponse acntResponse = await client.ExecutePostAsync(acntRequest);
            uint acntID = JsonConvert.DeserializeObject<uint>(response.Content);
        }

        public async void GetUser()
        {
            RestRequest request = new RestRequest("api/User/{userId}");
            IRestResponse response = await client.ExecuteGetAsync(request);
            UserDetailsStruct user = JsonConvert.DeserializeObject<UserDetailsStruct>(response.Content);
        }

      public async void createTransaction()
      {
            TransactionDetailsStruct transaction = new TransactionDetailsStruct();
            RestRequest restRequest = new RestRequest("api/Transactions");
            restRequest.AddJsonBody(transaction);
            IRestResponse response = await client.ExecutePostAsync(restRequest);
            transaction = JsonConvert.DeserializeObject<TransactionDetailsStruct>(response.Content);
      }
    }
}