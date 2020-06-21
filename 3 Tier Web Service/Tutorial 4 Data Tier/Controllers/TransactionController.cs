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
    /// <summary>
    /// Purpose: The transaction controller contains methods which allow transactions to be created and retrieved.
    /// These can be accessed using the specified API calls
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class TransactionController : ApiController
    {
        // GET api/<controller>
        BankDB.TransactionAccessInterface transactionAccess = Bank.bankData.GetTransactionInterface();
        /// <summary>
        /// Purpose: Create a transaction. Create transaction returns bool to indicate whether it was
        /// Successful or not
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="senderID"></param>
        /// <param name="receiverID"></param>
        /// <returns>if the transaction was successfully created or not</returns>
        // GET api/<controller>/5
        [Route("api/Transactions/Create/{amount}/{senderID}/{receiverID}")]
        [HttpPost]
        public bool CreateTransaction(uint amount, uint senderID, uint receiverID)
        {
            try
            {
                BankDB.AccountAccessInterface acct = Bank.bankData.GetAccountInterface();
                acct.SelectAccount(senderID);
                // Select account that transaction is being sent from
                uint balance = acct.GetBalance();
                // get the sender balance
                if (amount < balance)
                {
                    // only perform transaction if account has enough funds initially
                    TransactionDetailsStruct tran = new TransactionDetailsStruct();
                    tran.transactionId = transactionAccess.CreateTransaction();
                    // create transaction and set id to object
                    tran.amount = amount;
                    tran.senderId = senderID;
                    tran.receiverId = receiverID;
                    // set object fields
                    transactionAccess.SelectTransaction(tran.transactionId);
                    transactionAccess.SetAmount(tran.amount);
                    transactionAccess.SetSendr(tran.senderId);
                    transactionAccess.SetRecvr(tran.receiverId);
                    // set sender, reciever and amount using object
                    Debug.WriteLine(tran.transactionId);
                    // Print transaction ID
                    // indicate whether transaction was successfully created
                    return true;
                }
            }
            catch(Exception)
            {
                // If anything goes wrong with transaction, return false
                return false;
            }
            return false;
        }

        /// <summary>
        /// Purpose: To retrieve the associated transaction for the given transaction ID
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns>Transaction object</returns>
        // POST api/<controller>
        [Route("api/Transactions/{transactionId}")]
        [HttpGet]

        public TransactionDetailsStruct GetTransactions(uint transactionId)
        {
            try
            {
                TransactionDetailsStruct transaction = new TransactionDetailsStruct();
                // create object for transaction details to be stored
                transactionAccess.SelectTransaction(transactionId);
                // select the transaction
                transaction.transactionId = transactionId;
                transaction.senderId = transactionAccess.GetSendrAcct();
                transaction.receiverId = transactionAccess.GetRecvrAcct();
                transaction.amount = transactionAccess.GetAmount();
                // set object details
                // retrieve transaction
                return transaction;
            }
            catch(Exception)
            {
                // if transaction could not be retrieved, return null for error handling
                return null;
            }
        }
    }
    
}