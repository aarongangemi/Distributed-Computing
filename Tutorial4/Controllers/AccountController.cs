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

        [Route("api/Account/Create/")]
        [HttpPost]
        public AccountDetailsStruct createAcnt([FromBody] uint userID)
        {
            uint acntID = acntAccess.CreateAccount(userID);
            acntAccess.SelectAccount(acntID);
            AccountDetailsStruct ads = new AccountDetailsStruct();
            ads.userId = userID;
            ads.acntBal = 0;
            ads.acntId = acntID;
            Bank.bankData.SaveToDisk();
            return ads;
        }

        [Route("api/Account/Deposit/}")]
        [HttpPost]
        public uint DepositValue([FromBody] AmountChanger amntObj)
        {
            acntAccess.SelectAccount(amntObj.accountID);
            acntAccess.Deposit(amntObj.amount);
            return acntAccess.GetBalance();
        }

        [Route("api/Account/Withdraw")]
        [HttpPost]
        public uint WithdrawValue([FromBody] AmountChanger amntObj)
        {
            acntAccess.SelectAccount(amntObj.accountID);
            acntAccess.Withdraw(amntObj.amount);
            return acntAccess.GetBalance();
        }



    }

}
