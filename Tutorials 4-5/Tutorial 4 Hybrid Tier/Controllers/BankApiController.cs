using Newtonsoft.Json;
using RestSharp;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;
using Tutorial_4_Hybrid_Tier.Models;

namespace Tutorial_4_Hybrid_Tier.Controllers
{
    public class BankApiController : ApiController
    {
        private string URL = "https://localhost:44312/";
        private RestClient client;
        private BusinessLogger log = new BusinessLogger();
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
                    log.errorLogMessage("Invalid data entered to retrieve account");
                    return null;
                }
            }
            catch(NullReferenceException)
            {
                log.errorLogMessage("Exception occured - invalid account found");
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
                log.errorLogMessage("Invalid data entered for deposit");
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
                log.errorLogMessage("Invalid data entered for withdrawal");
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
                    log.errorLogMessage("Invalid fname was entered - no first name found");
                    return null;
                }
            }
            foreach (char x in lname)
            {
                if (!char.IsLetter(x))
                {
                    log.errorLogMessage("Invalid lname was entered - no last name found");
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
            log.logMessage("Account and user saved succesfully");
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
                log.errorLogMessage("Invalid data entered, unable to retreive a user");
                return null;
            }
        }



        [Route("api/BankApi/CreateTransaction/{amount}/{senderID}/{receiverID}")]
        [HttpPost]
        public async Task<bool> CreateTransaction(uint amount, uint senderID, uint receiverID)
        {
            bool processed = false;
            var regex = new Regex("/^[0-9]*$/");
            if (!regex.IsMatch(amount.ToString()) && !regex.IsMatch(senderID.ToString()) && !regex.IsMatch(receiverID.ToString()))
            {
                client = new RestClient(URL);
                RestRequest restRequest = new RestRequest("api/Transactions/Create/" + amount + "/" + senderID + "/" + receiverID);
                IRestResponse response = client.Post(restRequest);
                bool transactionCreated = JsonConvert.DeserializeObject<bool>(response.Content);
                if (transactionCreated == true)
                {
                    processed = await OnDelayEnd();
                    log.logMessage("Transaction trying to be created - you will know in 30 seconds");
                    if (processed)
                    {
                        RestRequest saveRequest = new RestRequest("api/Save");
                        client.Post(saveRequest);
                        log.logMessage("Transaction successfully saved");
                    }
                    if (!processed)
                    {
                        log.errorLogMessage("Something went wrong with transaction");
                    }
                }
                else
                {
                    return false;
                }
            }
            return processed;
        }

        private async Task<bool> OnDelayEnd()
        {
            await Task.Delay(30000);
            bool processed = await Task.Run(() => processTransactions());
            return processed;
        }

        private bool processTransactions()
        {
            RestRequest request = new RestRequest("api/ProcessTransactions");
            IRestResponse response = client.Post(request);
            bool processed = JsonConvert.DeserializeObject<bool>(response.Content);
            log.logMessage("Transaction process called");
            return processed;
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
                log.errorLogMessage("Unable to retrieve transaction");
                return null;
            }
        }
    }
}