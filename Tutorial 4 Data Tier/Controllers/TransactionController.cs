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
            try
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
            catch(Exception)
            {
                Console.WriteLine("Unable to create transaction");
            }
        }

        // POST api/<controller>
        [Route("api/Transactions/{transactionId}")]
        [HttpGet]
        public TransactionDetailsStruct GetTransactions(uint transactionId)
        {
            try
            {
                TransactionDetailsStruct transaction = new TransactionDetailsStruct();
                transactionAccess.SelectTransaction(transactionId);
                transaction.transactionId = transactionId;
                transaction.senderId = transactionAccess.GetSendrAcct();
                transaction.receiverId = transactionAccess.GetRecvrAcct();
                transaction.amount = transactionAccess.GetAmount();
                return transaction;
            }
            catch(Exception)
            {
                Console.WriteLine("Transaction doesn't exist");
                return null;
            }
        }

        [Route("api/TransactionList")]
        [HttpGet]
        public List<uint> getList()
        {
            return transactionAccess.GetTransactions();
        }
    }
    
}