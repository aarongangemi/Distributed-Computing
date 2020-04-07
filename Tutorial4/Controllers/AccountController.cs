using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial4.Models;

namespace Tutorial4.Controllers
{
    public class AccountController : ApiController
    {
        // GET api/<controller>
        BankDB.AccountAccessInterface acntAccess = Bank.bankData.GetAccountInterface();
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [Route("api/Account/{accountID}")]
        [HttpGet]
        public AccountDetailsStruct GetAccountDetails(uint accountID)
        {
            AccountDetailsStruct ads = new AccountDetailsStruct();
            BankDB.AccountAccessInterface acct = Bank.bankData.GetAccountInterface();
            acct.SelectAccount(accountID);
            ads.userId = acct.GetOwner();
            ads.acntBal = acct.GetBalance();
            ads.acntId = accountID;
            return ads;
        }

        [Route("api/Account/Create/{userID}")]
        [HttpPost]
        public uint createAcnt(uint userID)
        {
            uint acntID = acntAccess.CreateAccount(userID);
            acntAccess.SelectAccount(acntID);
            acntAccess.Deposit(52);
            return acntID;
        }

    }

}
