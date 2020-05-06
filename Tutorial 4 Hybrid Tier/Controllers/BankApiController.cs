using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Tutorial_4_Business_Tier.Models;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Hybrid_Tier.Controllers
{
    public class BankApiController : ApiController
    {
        private static System.Timers.Timer timer;
        private string URL = "https://localhost:44312/";
        private RestClient client;
        [Route("api/BankApi/Account/{accountID}")]
        [HttpGet]
        public AccountDetailsStruct GetAccount(uint accountID)
        {
            try
            {
                var regex = new Regex("/^[0-9]*$/");
                if (!regex.IsMatch(accountID.ToString()))
                {
                    client = new RestClient(URL);
                    RestRequest request = new RestRequest("api/Account/" + accountID.ToString());
                    IRestResponse response = client.Get(request);
                    return JsonConvert.DeserializeObject<AccountDetailsStruct>(response.Content);
                }
                else
                {
                    return null;
                }
            }
            catch(NullReferenceException)
            {
                return null;
            }
        }

        [Route("api/BankApi/Deposit/{accountID}/{amount}")]
        [HttpPost]
        public uint DepositMoney(uint accountID, uint amount)
        {
            var regex = new Regex("/^[0-9]*$/");
            if (!regex.IsMatch(accountID.ToString()) && !regex.IsMatch(amount.ToString()))
            {
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Account/Deposit/" + accountID.ToString() + "/" + amount.ToString());
                IRestResponse response = client.Post(request);
                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);
                return Convert.ToUInt32(response.Content);
            }
            else
            {
                return 0;
            }

        }

        [Route("api/BankApi/Withdraw/{accountID}/{amount}")]
        [HttpPost]
        public uint WithdrawMoney(uint accountID, uint amount)
        {
            var regex = new Regex("/^[0-9]*$/");
            if (!regex.IsMatch(accountID.ToString()) && !regex.IsMatch(amount.ToString()))
            {
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Account/Withdraw/" + accountID.ToString() + "/" + amount.ToString());
                IRestResponse response = client.Post(request);
                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);
                return Convert.ToUInt32(response.Content);
            }
            else
            {
                return 0;
            }
        }

        [Route("api/BankApi/Create/{fname}/{lname}")]
        [HttpPost]
        public UserDetailsStruct createAccountAndUser(string fname, string lname)
        {
            foreach (char c in fname)
            {
                if (!char.IsLetter(c))
                {
                    return null;
                }
            }
            foreach (char x in lname)
            {
                if (!char.IsLetter(x))
                {
                    return null;

                }
            }
            client = new RestClient(URL);
            RestRequest userRequest = new RestRequest("api/User/Create/" + fname + "/" + lname);
            IRestResponse response = client.Post(userRequest);
            UserDetailsStruct user = JsonConvert.DeserializeObject<UserDetailsStruct>(response.Content);
            RestRequest acntRequest = new RestRequest("api/Account/Create/" + user.userId.ToString());
            client.Post(acntRequest);
            RestRequest saveRequest = new RestRequest("api/Save");
            client.Post(saveRequest);
            return user;
        }

        [Route("api/BankApi/GetUser/{userId}")]
        [HttpGet]
        public UserDetailsStruct GetUser(uint userId)
        {
            var regex = new Regex("/^[0-9]*$/");
            if (!regex.IsMatch(userId.ToString()))
            {
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/User/" + userId.ToString());
                IRestResponse response = client.Get(request);
                return JsonConvert.DeserializeObject<UserDetailsStruct>(response.Content);
            }
            else
            {
                return null;
            }
        }



        [Route("api/BankApi/CreateTransaction/{amount}/{senderID}/{receiverID}")]
        [HttpPost]
        public void createTransaction(uint amount, uint senderID, uint receiverID)
        {

            var regex = new Regex("/^[0-9]*$/");
            if (!regex.IsMatch(amount.ToString()) && !regex.IsMatch(senderID.ToString()) && !regex.IsMatch(receiverID.ToString()))
            {
                client = new RestClient(URL);
                RestRequest restRequest = new RestRequest("api/Transactions/Create/" + amount + "/" + senderID + "/" + receiverID);
                client.Post(restRequest);
                timer = new System.Timers.Timer();
                timer.Interval = 120000;
                timer.Elapsed += OnTimerEnd;
                timer.AutoReset = true;
                timer.Enabled = true;
                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);
            }
            
        }

        private void OnTimerEnd(Object source, System.Timers.ElapsedEventArgs e)
        {
            RestRequest request = new RestRequest("api/ProcessTransactions");
            client.Post(request);
        }

        [Route("api/BankApi/GetTransaction/{transactionId}")]
        [HttpGet]
        public TransactionDetailsStruct GetTransaction(uint transactionId)
        {
            try
            {
                client = new RestClient(URL);
                RestRequest restRequest = new RestRequest("api/Transactions/" + transactionId);
                IRestResponse transResponse = client.Get(restRequest);
                TransactionDetailsStruct tran = JsonConvert.DeserializeObject<TransactionDetailsStruct>(transResponse.Content);
                return tran;
            }
            catch(NullReferenceException)
            {
                return null;
            }
        }


    }
}