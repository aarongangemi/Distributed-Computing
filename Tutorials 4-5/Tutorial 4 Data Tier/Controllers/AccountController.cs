using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    /// <summary>
    /// Purpose: The account controller allows the user to retrieve their account, 
    /// deposit money into their account, withdraw money from their account and create an account
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class AccountController : ApiController
    {
        // GET api/<controller>
        // GET api/<controller>
        BankDB.AccountAccessInterface acntAccess = Bank.bankData.GetAccountInterface();
        // GET api/<controller>/5
        /// <summary>
        /// Purpose: To retrieve the account details for the given account ID
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns>Account object</returns>
        [Route("api/Account/{accountID}")]
        [HttpGet]
        public AccountDetailsStruct GetAccountDetails(uint accountID)
        {
            try
            {
                AccountDetailsStruct ads = new AccountDetailsStruct();
                BankDB.AccountAccessInterface acct = Bank.bankData.GetAccountInterface();
                acct.SelectAccount(accountID);
                // select account
                ads.userId = acct.GetOwner();
                ads.acntBal = acct.GetBalance();
                ads.acntId = accountID;
                // assign account fields and return account
                return ads;
            }
            catch(Exception)
            {
                // if account doesn't exist, return null for error handling
                return null;
            }
        }

        /// <summary>
        /// Purpose: Create an account for an already existing user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>Account object</returns>
        [Route("api/Account/Create/{userID}")]
        [HttpPost]
        public AccountDetailsStruct createAcnt(uint userID)
        {
            uint acntID = acntAccess.CreateAccount(userID);
            // create account for user then assign ID
            acntAccess.SelectAccount(acntID);
            // select ID
            AccountDetailsStruct ads = new AccountDetailsStruct();
            ads.userId = userID;
            ads.acntBal = 0;
            ads.acntId = acntID;
            // Create object and set details 
            return ads;
        }

        /// <summary>
        /// Purpose: To deposit money into a given account.
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="amount"></param>
        /// <returns>amount deposited</returns>
        [Route("api/Account/Deposit/{accountID}/{amount}")]
        [HttpPost]
        public uint DepositValue(uint accountID, uint amount)
        {
            try
            {
                acntAccess.SelectAccount(accountID);
                acntAccess.Deposit(amount);
                // deposit money
                return amount;
            }
            catch(Exception)
            {
                // if something went wrong, return 0 so nothing is deposited
                return 0;
            }
        }

        /// <summary>
        /// Purpose: To withdraw an amount of money out of a given account
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="amount"></param>
        /// <returns>amount withdrawn</returns>
        [Route("api/Account/Withdraw/{accountID}/{amount}")]
        [HttpPost]
        public uint WithdrawValue(uint accountID, uint amount)
        {
            try
            {
                acntAccess.SelectAccount(accountID);
                // select associated account
                acntAccess.Withdraw(amount);
                //withdraw money
                return amount;
            }
            catch(Exception)
            {
                //return 0 so no money is withdrawn if something goes wrong
                return 0;
            }
        }
    }
}