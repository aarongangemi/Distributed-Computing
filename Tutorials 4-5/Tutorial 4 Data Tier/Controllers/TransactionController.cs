using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private LogClass log = new LogClass();

        // GET api/<controller>/5
        [Route("api/Transactions/Create/{amount}/{senderID}/{receiverID}")]
        [HttpPost]
        public bool CreateTransaction(uint amount, uint senderID, uint receiverID)
        {
            try
            {
                BankDB.AccountAccessInterface acct = Bank.bankData.GetAccountInterface();
                acct.SelectAccount(senderID);
                uint balance = acct.GetBalance();
                if (amount < balance)
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
                    Debug.WriteLine(tran.transactionId);
                    return true;
                }
            }
            catch(Exception)
            {
                log.errorLogMessage("Unable to create transaction - invalid account may be entered");
            }
            return false;
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
                log.logMessage("Transaction " + transactionId + " count not be retrieved");
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