﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Hybrid_Tier.Controllers
{
    public class BankApiController : ApiController
    {
        private string URL = "https://localhost:44312/";
        private RestClient client;
        StreamWriter writer;
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
                    writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                    writer.WriteLine("Retrieved Account: " + accountID.ToString());
                    writer.Close();
                    return JsonConvert.DeserializeObject<AccountDetailsStruct>(response.Content);
                }
                else
                {
                    Console.WriteLine("Invalid data found");
                    return null;
                }
            }
            catch (NullReferenceException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when retrieving account, please try again");
                writer.Close();
            }
            catch (HttpResponseException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when retrieving account, please try again");
                writer.Close();
            }
            return null;
        }

        [Route("api/BankApi/Deposit/{accountID}/{amount}")]
        public void DepositMoney(uint accountID, uint amount)
        {
            try
            {
                var regex = new Regex("/^[0-9]*$/");
                if (!regex.IsMatch(accountID.ToString()) || !regex.IsMatch(amount.ToString()))
                {
                    client = new RestClient(URL);
                    RestRequest request = new RestRequest("api/Account/Deposit/" + accountID.ToString() + "/" + amount.ToString());
                    client.Post(request);
                    RestRequest saveRequest = new RestRequest("api/Save");
                    client.Post(saveRequest);
                    writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                    writer.WriteLine("Deposited " + amount.ToString() + " successfully to account: " + accountID.ToString());
                    writer.Close();
                }
            }
            catch(NullReferenceException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when depositing money, please try again");
                writer.Close();
            }
            catch(HttpResponseException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when depositing money, please try again");
                writer.Close();
            }


        }

        [Route("api/BankApi/Withdraw/{accountID}/{amount}")]
        [HttpPost]
        public void WithdrawMoney(uint accountID, uint amount)
        {
            try
            {
                var regex = new Regex("/^[0-9]*$/");
                if (!regex.IsMatch(accountID.ToString()) || !regex.IsMatch(amount.ToString()))
                {
                    client = new RestClient(URL);
                    RestRequest request = new RestRequest("api/Account/Withdraw/" + accountID.ToString() + "/" + amount.ToString());
                    client.Post(request);
                    RestRequest saveRequest = new RestRequest("api/Save");
                    client.Post(saveRequest);
                    writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                    writer.WriteLine("Withdrew " + amount.ToString() + " successfully from account: " + accountID.ToString());
                    writer.Close();
                }
            }
            catch (NullReferenceException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when withdrawing money, please try again");
                writer.Close();
            }
            catch (HttpResponseException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when withdrawing money, please try again");
                writer.Close();
            }
        }

        [Route("api/BankApi/Create/{fname}/{lname}")]
        [HttpPost]
        public void createAccountAndUser(string fname, string lname)
        {
            try
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
                        writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                        writer.WriteLine("Created user: " + userID.ToString() + "(" + fname + " " + lname + ")" + " with associated account");
                        writer.Close();
                }
            }
            catch (NullReferenceException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when create a user and account, please try again");
                writer.Close();
            }
            catch (HttpResponseException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when creating a user and account, please try again");
                writer.Close();
            }

        }

        [Route("api/BankApi/GetUser/{userId}")]
        [HttpGet]
        public UserDetailsStruct GetUser(uint userId)
        {
            try
            {
                var regex = new Regex("/^[0-9]*$/");
                if (!regex.IsMatch(userId.ToString()))
                {
                    client = new RestClient(URL);
                    RestRequest request = new RestRequest("api/User/" + userId.ToString());
                    IRestResponse response = client.Get(request);
                    writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                    writer.WriteLine("Retrieved User " + userId.ToString() + " successfully");
                    writer.Close();
                    return JsonConvert.DeserializeObject<UserDetailsStruct>(response.Content);
                }
                else
                {
                    Console.WriteLine("invalid data found");
                    return null;
                }
            }
            catch (NullReferenceException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when retrieving a user, please try again");
                writer.Close();
            }
            catch (HttpResponseException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when retrieving a user, please try again");
                writer.Close();
            }
            return null;
        }

        [Route("api/BankApi/CreateTransaction/{amount}/{senderID}/{receiverID}")]
        [HttpPost]
        public void createTransaction(uint amount, uint senderID, uint receiverID)
        {
            try
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
                    client.Post(saveRequest);
                    writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                    writer.WriteLine("Transaction Created: Sender = " + senderID.ToString() + ", Receiver = " + receiverID.ToString() + ", amount = " + amount.ToString());
                    writer.Close();
                }
            }
            catch (NullReferenceException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when creating a transaction, please try again");
                writer.Close();
            }
            catch (HttpResponseException)
            {
                writer = new StreamWriter("C:/Users/61459/source/repos/aarongangemi/Distributed-Computing/Tutorial 4 Hybrid Tier/Web Service Log file.txt", append: true);
                writer.WriteLine("Exception caught when creating a transaction, please try again");
                writer.Close();
            }
        }

    }
}