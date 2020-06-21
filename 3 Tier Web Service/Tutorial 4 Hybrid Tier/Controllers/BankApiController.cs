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
    /// <summary>
    /// Purpose: The Bank API Controller acts contains the methods that are required by the presentation part of the hybrid tier. 
    /// It successfully retrieves the data from the data tier and communicates the results with the presentation tier.
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class BankApiController : ApiController
    {
        private string URL = "https://localhost:44312/";
        private RestClient client;
        private BusinessLogger log = new BusinessLogger();
        /// <summary>
        /// Purpose: used to retrieve account using given account ID
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns>Account retrieved</returns>
        [Route("api/BankApi/Account/{accountID}")]
        [HttpGet]
        public AccountDetailsStruct GetAccount(uint accountID)
        {
            try
            {
                var regex = new Regex("/^[0-9]*$/");
                // use regex to check account ID is a number
                if (!regex.IsMatch(accountID.ToString()))
                {
                    // create a new rest client and retrieve data from data tier
                    client = new RestClient(URL);
                    RestRequest request = new RestRequest("api/Account/" + accountID.ToString());
                    IRestResponse response = client.Get(request);
                    // call get request
                    log.logMessage("Attempting to retrieve: " + accountID.ToString());
                    // log message and return successful account object
                    return JsonConvert.DeserializeObject<AccountDetailsStruct>(response.Content);
                }
                else
                {
                    // log the error message
                    log.errorLogMessage("Invalid data entered to retrieve account");
                    // if the account ID contains characters other than numbers,
                    // do not look into data tier, return null to presentation tier and presentation tier
                    // handles the error
                    return null;
                }
            }
            catch(NullReferenceException)
            {
                log.errorLogMessage("Exception occured - invalid account found");
                // if the data tier returns null, null reference exception is thrown.
                // they the presentation tier will handle the null return and display an error message
                return null;
            }
        }

        /// <summary>
        /// Purpose: To deposit money into a given account
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="amount"></param>
        /// <returns>Amount deposited</returns>
        [Route("api/BankApi/Deposit/{accountID}/{amount}")]
        [HttpPost]
        public uint DepositMoney(uint accountID, uint amount)
        {
            var regex = new Regex("/^[0-9]*$/");
            // use regex to check both account and amount are numbers
            if ((!regex.IsMatch(accountID.ToString())) && (!regex.IsMatch(amount.ToString())))
            {
                // if both data types are valid, create new rest client and request
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Account/Deposit/" + accountID.ToString() + "/" + amount.ToString());
                // deposit money using data tier API call
                IRestResponse response = client.Post(request);
                log.logMessage("Depositing " + amount.ToString() + " into " + accountID.ToString());
                // log deposit message
                RestRequest saveRequest = new RestRequest("api/Save");
                log.logMessage("Saved to disk");
                // save data to disk and log result
                client.Post(saveRequest);
                //return the amount deposited, if amount is 0, error handled at presentation tier
                return Convert.ToUInt32(response.Content);
            }
            else
            {
                // log the error message
                log.errorLogMessage("Invalid data entered for deposit");
                // return 0 to represent no money was deposited
                return 0;
            }

        }

        /// <summary>
        /// Purpose: to withdraw money from a given account
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="amount"></param>
        /// <returns>Amount withdrawn</returns>
        [Route("api/BankApi/Withdraw/{accountID}/{amount}")]
        [HttpPost]
        public uint WithdrawMoney(uint accountID, uint amount)
        {
            var regex = new Regex("/^[0-9]*$/");
            // use regex to check that account id and amount are correct data type
            if (!regex.IsMatch(accountID.ToString()) && !regex.IsMatch(amount.ToString()))
            {
                // if valid, create new rest client and request
                client = new RestClient(URL);
                // create api call to withdraw from data tier
                RestRequest request = new RestRequest("api/Account/Withdraw/" + accountID.ToString() + "/" + amount.ToString());
                IRestResponse response = client.Post(request);
                // log message
                log.logMessage("Withdrawing " + amount.ToString() + "from " + accountID.ToString());
                RestRequest saveRequest = new RestRequest("api/Save");
                // save data to disk
                log.logMessage("Saved to disk");
                client.Post(saveRequest);
                // return amount to represent withdrawal was successful
                return Convert.ToUInt32(response.Content);
            }
            else
            {
                // log the error
                log.errorLogMessage("Invalid data entered for withdrawal");
                // return 0 to demonstrate no money was taken out
                return 0;
            }
        }

        /// <summary>
        /// Purpose: to create an account and user 
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <returns>user object</returns>
        [Route("api/BankApi/Create/{fname}/{lname}")]
        [HttpPost]
        public UserDetailsStruct createAccountAndUser(string fname, string lname)
        {
            // check if each character in first name and last name is a letter
            foreach (char c in fname)
            {
                if (!char.IsLetter(c))
                {
                    // if not letter, then log message and report back to presentation tier
                    log.errorLogMessage("Invalid fname was entered - no first name found");
                    return null;
                }
            }
            foreach (char x in lname)
            {
                if (!char.IsLetter(x))
                {
                    // if not letter, then log message and report back to presentation tier
                    log.errorLogMessage("Invalid lname was entered - no last name found");
                    return null;

                }
            }
            // create new rest client and rest request
            client = new RestClient(URL);
            // rest request to create user ID
            RestRequest userRequest = new RestRequest("api/User/Create/" + fname + "/" + lname);
            IRestResponse response = client.Post(userRequest);
            UserDetailsStruct user = JsonConvert.DeserializeObject<UserDetailsStruct>(response.Content);
            // newly created user stored in object
            // use user ID to create account
            RestRequest acntRequest = new RestRequest("api/Account/Create/" + user.userId.ToString());
            client.Post(acntRequest);
            // create account from data tier using rest request
            RestRequest saveRequest = new RestRequest("api/Save");
            client.Post(saveRequest);
            // save any new user and account data to disk
            log.logMessage("Account and user saved succesfully");
            // log the error message
            // return user
            return user;
        }

        /// <summary>
        /// Purpose: to retrieve a user based on given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>user object</returns>
        [Route("api/BankApi/GetUser/{userId}")]
        [HttpGet]
        public UserDetailsStruct GetUser(uint userId)
        {
            var regex = new Regex("/^[0-9]*$/");
            // check user ID is a number
            if (!regex.IsMatch(userId.ToString()))
            {
                // if valid, then create new client and request to data tier for given user
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/User/" + userId.ToString());
                IRestResponse response = client.Get(request);
                // get the given user data
                log.logMessage("User " + userId.ToString() + " was retrieved");
                // log the success message
                // return the user
                return JsonConvert.DeserializeObject<UserDetailsStruct>(response.Content);
            }
            else
            {
                // if user could not be found, log the message and return null which
                //is handled by presentation tier
                log.errorLogMessage("Invalid data entered, unable to retreive a user");
                return null;
            }
        }


        /// <summary>
        /// Purpose: To create a new transaction
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="senderID"></param>
        /// <param name="receiverID"></param>
        /// <returns>if the transaction was processed</returns>
        [Route("api/BankApi/CreateTransaction/{amount}/{senderID}/{receiverID}")]
        [HttpPost]
        public async Task<bool> CreateTransaction(uint amount, uint senderID, uint receiverID)
        {
            // check if amount, sender and reciever are corrected data type
            bool processed = false;
            var regex = new Regex("/^[0-9]*$/");
            if ((!regex.IsMatch(amount.ToString())) && (!regex.IsMatch(senderID.ToString())) && (!regex.IsMatch(receiverID.ToString())))
            {
                // if fields are valid
                client = new RestClient(URL);
                // call data tier to make new transaction
                RestRequest restRequest = new RestRequest("api/Transactions/Create/" + amount + "/" + senderID + "/" + receiverID);
                IRestResponse response = client.Post(restRequest);
                // execute
                bool transactionCreated = JsonConvert.DeserializeObject<bool>(response.Content);
                // transactionCreated is true if the transaction created was successful, false if not - determined by data tier
                if (transactionCreated == true)
                {
                    // if transaction worked
                    processed = await OnDelayEnd();
                    // processes transaction 60 seconds after created - starts timer
                    log.logMessage("Transaction trying to be created - you will know in 30 seconds");
                    if (processed)
                    {
                        // if timer has ended
                        RestRequest saveRequest = new RestRequest("api/Save");
                        client.Post(saveRequest);
                        log.logMessage("Transaction successfully saved and completed");
                        // log transaction was processed
                    }
                    if (!processed)
                    {
                        // if transaction could not be processed, then log error
                        log.errorLogMessage("Something went wrong with transaction");
                    }
                }
                else
                {
                    // log message if transaction was unsuccessful and return false
                    log.logMessage("Unable to complete transaction");
                    // error handled at presentation tier
                    return false;
                }
            }
            return processed;
        }

        /// <summary>
        /// Purpose: wait 30 seconds before processing transaction
        /// </summary>
        /// <returns>if the transaction was processed or not</returns>
        private async Task<bool> OnDelayEnd()
        {
            await Task.Delay(30000);
            bool processed = await Task.Run(() => processTransactions());
            return processed;
        }

        /// <summary>
        /// Purpose: To call the data tier to process the transactions currently in queue
        /// </summary>
        /// <returns>If transaction was processed or not</returns>
        private bool processTransactions()
        {
            RestRequest request = new RestRequest("api/ProcessTransactions");
            IRestResponse response = client.Post(request);
            // Create and execute rest request to process transactions
            bool processed = JsonConvert.DeserializeObject<bool>(response.Content);
            // log transaction
            log.logMessage("Transaction process called");
            return processed;
        }

        /// <summary>
        /// Purpose: To retrieve the transaction for the given transaction ID
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns>Transaction retrieved</returns>
        [Route("api/BankApi/GetTransaction/{transactionId}")]
        [HttpGet]
        public TransactionDetailsStruct GetTransaction(uint transactionId)
        {
            try
            {
                // create a new rest client and request
                client = new RestClient(URL);
                RestRequest restRequest = new RestRequest("api/Transactions/" + transactionId);
                IRestResponse transResponse = client.Get(restRequest);
                // call get request in data tier
                TransactionDetailsStruct tran = JsonConvert.DeserializeObject<TransactionDetailsStruct>(transResponse.Content);
                // create transaction
                // log message
                log.logMessage("Transaction retrieved");
                return tran;
            }
            catch(NullReferenceException)
            {
                // log transaction error
                log.errorLogMessage("Unable to retrieve transaction");
                // return null to presentation tier which checks for null object
                return null;
            }
        }
    }
}