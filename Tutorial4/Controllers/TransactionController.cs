using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial4.Models;

namespace Tutorial4.Controllers
{
    public class TransactionController : ApiController
    {
        // GET api/<controller>
        BankDB.TransactionAccessInterface transactionAccess = Bank.bankData.GetTransactionInterface();

        // GET api/<controller>/5
        [Route("api/Transactions/Create/{acnt1}/{acnt2}/{amount}")]
        [HttpPost]
        public TransactionDetailsStruct CreateTransaction(uint acnt1, uint acnt2, uint amount)
        {
            TransactionDetailsStruct transactionStruct = new TransactionDetailsStruct();
            transactionStruct.transactionId = transactionAccess.CreateTransaction();
            transactionAccess.SelectTransaction(transactionStruct.transactionId);
            transactionAccess.SetAmount(amount);
            transactionAccess.SetSendr(acnt1);
            transactionAccess.SetRecvr(acnt2);
            transactionStruct.senderId = transactionAccess.GetSendrAcct();
            transactionStruct.receiverId = transactionAccess.GetRecvrAcct();
            transactionStruct.amount = transactionAccess.GetAmount();
    
            return transactionStruct;
        }

        // POST api/<controller>
        [Route("api/Transactions")]
        [HttpGet]
        public TransactionDetailsStruct GetTransaction()
        {
            List<uint> transactionList = transactionAccess.GetTransactions();
            TransactionDetailsStruct transaction = new TransactionDetailsStruct();
            transactionAccess.SelectTransaction(transactionList.ElementAt(transactionList.Count() - 1));
            transaction.transactionId = transactionList.ElementAt(transactionList.Count() - 1);
            transaction.senderId = transactionAccess.GetSendrAcct();
            transaction.receiverId = transactionAccess.GetRecvrAcct();
            transaction.amount = transactionAccess.GetAmount();
            return transaction;
        }

    }
}