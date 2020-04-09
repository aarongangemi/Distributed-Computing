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
        [Route("api/Transactions/Create/")]
        [HttpPost]
        public TransactionDetailsStruct CreateTransaction([FromBody]TransactionDetailsStruct tran)
        {
            Bank.bankData.ProcessAllTransactions();
            tran.transactionId = transactionAccess.CreateTransaction();
            transactionAccess.SelectTransaction(tran.transactionId);
            transactionAccess.SetAmount(tran.amount);
            transactionAccess.SetSendr(tran.senderId);
            transactionAccess.SetRecvr(tran.receiverId);
            tran.senderId = transactionAccess.GetSendrAcct();
            tran.receiverId = transactionAccess.GetRecvrAcct();
            tran.amount = transactionAccess.GetAmount();
            return tran;
        }

        // POST api/<controller>
        [Route("api/Transactions")]
        [HttpGet]
        public TransactionDetailsStruct GetTransactions()
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