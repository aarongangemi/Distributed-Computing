using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    public class TransactionController : ApiController
    {
        // GET api/<controller>
        BankDB.TransactionAccessInterface transactionAccess = Bank.bankData.GetTransactionInterface();

        // GET api/<controller>/5
        [Route("api/Transactions/Create/{amount}/{senderID}/{receiverID}")]
        [HttpPost]
        public void CreateTransaction(uint amount, uint senderID, uint receiverID)
        {
            TransactionDetailsStruct tran = new TransactionDetailsStruct();
            tran.transactionId = transactionAccess.CreateTransaction();
            tran.amount = amount;
            tran.senderId = senderID;
            tran.receiverId = receiverID;
            transactionAccess.SelectTransaction(tran.transactionId);
            transactionAccess.SetAmount(tran.amount);
            transactionAccess.SetSendr(tran.senderId);
            transactionAccess.SetRecvr(tran.receiverId);
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