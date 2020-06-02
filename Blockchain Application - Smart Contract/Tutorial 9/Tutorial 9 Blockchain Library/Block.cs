using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_9_Blockchain_Library
{
    /// <summary>
    /// Purpose: The block class contains fields to create a block for the blockchain.
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public class Block
    {
        public static uint blockCounter = 0;
        public uint blockID;
        public string TransactionDetailsList;
        public uint blockOffset;
        public string prevBlockHash;
        public string blockHash;

        /// <summary>
        /// Purpose: The empty block constructor is required because in order for the service contract to work,
        /// if a block is returned, the interface requires an empty constructor to be in place. This is never called.
        /// </summary>
        public Block()
        {}

        /// <summary>
        /// Purpose: To create a block for each transaction
        /// </summary>
        /// <param name="blockOffset"></param>
        /// <param name="prevBlockHash"></param>
        /// <param name="blockHash"></param>
        public Block(uint blockOffset, string prevBlockHash, string blockHash)
        {
            blockCounter++;
            blockID = blockCounter;
            // serialize the list of transactions into a transaction string
            TransactionDetailsList = JsonConvert.SerializeObject(new List<string[]>());
            this.blockOffset = blockOffset;
            this.prevBlockHash = prevBlockHash;
            this.blockHash = blockHash;
        }

        /// <summary>
        /// Purpose: To add the python source code and result to the transaction list
        /// </summary>
        /// <param name="pythonSrc"></param>
        /// <param name="pythonResult"></param>
        public void AddPythonTransaction(string pythonSrc, string pythonResult)
        {
            byte[] decodedBytes = Convert.FromBase64String(pythonResult);
            pythonResult = Encoding.UTF8.GetString(decodedBytes);
            List<string[]> JsonTransactionList = JsonConvert.DeserializeObject<List<string[]>>(TransactionDetailsList);
            string[] PythonTransactionList = new string[2];
            PythonTransactionList[0] = pythonSrc;
            PythonTransactionList[1] = pythonResult;
            JsonTransactionList.Add(PythonTransactionList);
            TransactionDetailsList = JsonConvert.SerializeObject(JsonTransactionList);
        }
    }
}
