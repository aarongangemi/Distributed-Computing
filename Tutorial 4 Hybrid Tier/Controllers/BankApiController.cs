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
        private string URL = "https://localhost:44312/";
        private RestClient client;
        private LogClass log = new LogClass();
        [Route("api/BankApi/Account/{accountID}")]
        [HttpGet]
        public AccountDetailsStruct GetAccount(uint accountID)
        {
            var regex = new Regex("/^[0-9]*$/");
            if (!regex.IsMatch(accountID.ToString()))
            {
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Account/" + accountID.ToString());
                IRestResponse response = client.Get(request);
                log.successLogMessage("Account ID was used to retrieve account: " + accountID.ToString());
                return JsonConvert.DeserializeObject<AccountDetailsStruct>(response.Content);
            }
            else
            {
                log.errorLogMessage("Error retrieving account: " + accountID.ToString() + ". Account ID in invalid format");
                return null;
            }
        }

        [Route("api/BankApi/Deposit/{accountID}/{amount}")]
        public void DepositMoney(uint accountID, uint amount)
        {
            var regex = new Regex("/^[0-9]*$/");
            if (!regex.IsMatch(accountID.ToString()) || !regex.IsMatch(amount.ToString()))
            {
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Account/Deposit/" + accountID.ToString() + "/" + amount.ToString());
                client.Post(request);
                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);
                log.successLogMessage(amount.ToString() + "was successfully deposited into account: " + accountID.ToString());
                
            }
            else
            {
                log.errorLogMessage("Unable to deposit " + amount + "with account ID: " + accountID);
            }
        }

        [Route("api/BankApi/Withdraw/{accountID}/{amount}")]
        [HttpPost]
        public void WithdrawMoney(uint accountID, uint amount)
        {
            var regex = new Regex("/^[0-9]*$/");
            if (!regex.IsMatch(accountID.ToString()) || !regex.IsMatch(amount.ToString()))
            {
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Account/Withdraw/" + accountID.ToString() + "/" + amount.ToString());
                client.Post(request);
                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);
                log.successLogMessage(amount.ToString() + "was successfully withdrawn from account: " + accountID.ToString());
            }
            else
            {
                log.errorLogMessage("Unable to withdraw " + amount + "with account ID: " + accountID 
                    + ". Incorrect format of either account ID or amount");
            }
        }

        [Route("api/BankApi/Create/{fname}/{lname}")]
        [HttpPost]
        public void createAccountAndUser(string fname, string lname)
        {
            //Set user fields
            var regex = new Regex("/[!@#$%^\"&*(),.?:{}|<>]/g");
            if (!regex.IsMatch(fname) || !regex.IsMatch(lname))
            {
                client = new RestClient(URL);
                RestRequest userRequest = new RestRequest("api/User/Create/" + fname + "/" + lname);
                IRestResponse response = client.Post(userRequest);
                uint userID = JsonConvert.DeserializeObject<uint>(response.Content);
                RestRequest acntRequest = new RestRequest("api/Account/Create/" + userID.ToString());
                client.Post(acntRequest);
                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);
                log.successLogMessage("Account and user successfully created: first name = " + fname + ", last name = " + lname);
            }
            else
            {
                log.errorLogMessage("unable to create account and user because first name and last name are in incorrect format");
            }
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
                log.successLogMessage("user: " + userId.ToString() + " was successfully retrieved");
                return JsonConvert.DeserializeObject<UserDetailsStruct>(response.Content);
            }
            else
            {
                log.errorLogMessage("Unable to retrieve user: " + userId + " . User ID has invalid format.");
                return null;
            }
        }



        [Route("api/BankApi/CreateTransaction/{amount}/{senderID}/{receiverID}")]
        [HttpPost]
        public void createTransaction(uint amount, uint senderID, uint receiverID)
        {
            var regex = new Regex("/^[0-9]*$/");
            if (!regex.IsMatch(amount.ToString()) || !regex.IsMatch(senderID.ToString()) || !regex.IsMatch(receiverID.ToString()))
            {
                client = new RestClient(URL);
                RestRequest restRequest = new RestRequest("api/Transactions/Create/" + amount + "/" + senderID + "/" + receiverID);
                client.Post(restRequest);
                RestRequest transactionRequest = new RestRequest("api/ProcessTransactions");
                client.Post(transactionRequest);
                RestRequest saveRequest = new RestRequest("api/Save");
            }
            else
            {
                log.errorLogMessage("Unable to create transaction of amount: " + 
                    amount.ToString() + " to " + receiverID.ToString() 
                    + ". Either amount, sender ID or reciever ID had invalid format.");
            }
        }

    }
}