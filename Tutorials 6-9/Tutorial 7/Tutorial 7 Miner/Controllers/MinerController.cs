
using BlockchainLibrary;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Http;

namespace Tutorial_7_Miner.Controllers
{
    /// <summary>
    /// Purpose: The miner controller is responsible for mining blocks and processing them into the blockchain.
    /// It communicates with the server to gather add the transaction to the block.
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public delegate void TransactionProcessing();
    public class MinerController : ApiController
    {
        // Queue to store transactions
        public static Queue<Transaction> TransactionQueue = new Queue<Transaction>();
        // bool to check if thread has started
        private static bool ThreadStarted = false;

        /// <summary>
        /// Purpose: To check if the thread has already been started and if not, then enqueue the given transaction
        /// Author: Aaron Gangemi
        /// Date Modified: 29/05/2020
        /// </summary>
        /// <param name="transaction"></param>
        [Route("api/Miner/AddTransaction/")]
        [HttpPost]
        public void AddTransaction([FromBody]Transaction transaction)
        {
            if (!ThreadStarted)
            {
                RestClient ServerClient = new RestClient("https://localhost:44353/");
                RestRequest TransactionRequest = new RestRequest("api/Server/GenerateGenesisBlock");
                // Generate the genesis block
                ServerClient.Post(TransactionRequest);
                Thread processingThread = new Thread(new ThreadStart(ProcessTransaction));
                // start the thread to process the transaction
                processingThread.Start();
                // thread has been started
                ThreadStarted = true;
            }
            // add transaction to queue
            TransactionQueue.Enqueue(transaction);
        }

        /// <summary>
        /// Purpose: To process any transactions in the queue and mine blocks from transactions
        /// </summary>
        private void ProcessTransaction()
        {
            while (true)
            {
                try
                {
                    // check if transaction are in the queue
                    if (TransactionQueue.Count > 0)
                    {
                        uint offset = 0;
                        Transaction transaction = TransactionQueue.Dequeue();
                        // get transaction from queue
                        string hashString = "";
                        bool isValidBlock = false;
                        RestClient client = new RestClient("https://localhost:44353/");
                        RestRequest BalanceRequest = new RestRequest("api/Server/GetBalance/" + transaction.walletIdFrom.ToString());
                        IRestResponse BalanceResponse = client.Get(BalanceRequest);
                        float balance = JsonConvert.DeserializeObject<float>(BalanceResponse.Content);
                        if (transaction.amount > 0 && transaction.walletIdFrom >= 0 && transaction.walletIdTo >= 0 && transaction.amount <= balance)
                        {
                            Debug.WriteLine("Adding new block to blockchain");
                            Debug.WriteLine("Wallet ID from = " + transaction.walletIdFrom);
                            Debug.WriteLine("Wallet ID to = " + transaction.walletIdTo);
                            Debug.WriteLine("Transaction amount = " + transaction.amount);
                            RestRequest BlockchainRequest = new RestRequest("api/Server/GetBlockchain");
                            IRestResponse BlockchainResponse = client.Get(BlockchainRequest);
                            List<Block> blockchain = JsonConvert.DeserializeObject<List<Block>>(BlockchainResponse.Content);
                            string prevBlockHash = blockchain.Last().blockHash;
                            Debug.WriteLine("Previous block hash = " + prevBlockHash);
                            SHA256 sha256 = SHA256.Create();
                            while (!hashString.StartsWith("12345"))
                            {
                                offset += 1;
                                string blockString = transaction.walletIdFrom.ToString() + transaction.walletIdTo.ToString() + transaction.amount.ToString() + offset + prevBlockHash;
                                byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                                byte[] hashedData = sha256.ComputeHash(textBytes);
                                hashString = BitConverter.ToUInt64(hashedData, 0).ToString();
                            }
                            Debug.WriteLine("Offset = " + offset);
                            Debug.WriteLine("Block hash = " + hashString);
                            Block block = new Block(transaction.walletIdFrom, transaction.walletIdTo, transaction.amount, offset, prevBlockHash, hashString);
                            RestRequest ValidationRequest = new RestRequest("api/Server/ValidateBlock/");
                            ValidationRequest.AddJsonBody(block);
                            IRestResponse ValidationResponse = client.Post(ValidationRequest);
                            isValidBlock = JsonConvert.DeserializeObject<bool>(ValidationResponse.Content);
                            if (isValidBlock)
                            {
                                RestRequest AddBlockRequest = new RestRequest("api/Server/AddBlock/");
                                AddBlockRequest.AddJsonBody(block);
                                client.Post(AddBlockRequest);
                                Debug.WriteLine("Block successfully added to Blockchain");
                                Debug.WriteLine("Transaction successful");
                                Debug.WriteLine(".................................................................");
                            }
                            else
                            {
                                Debug.WriteLine("Validation for block failed, Trying again");
                            }
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    Debug.WriteLine("No Transaction in queue");
                }
                catch (NullReferenceException)
                {
                    Debug.WriteLine("Something went wrong");
                }
            }
        }
    }
}
