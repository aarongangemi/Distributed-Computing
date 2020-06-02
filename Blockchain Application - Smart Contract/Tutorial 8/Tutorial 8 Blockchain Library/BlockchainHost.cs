using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    /// <summary>
    /// Purpose: Blockchain host is the class that is implemented by the .NET remoting server
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    public class BlockchainHost : IBlockchain
    {
        /// <summary>
        /// Purpose: Retrieve the current block in the blockchain
        /// </summary>
        /// <returns>Last block in chain</returns>
        public Block GetCurrentBlock()
        {
            return Blockchain.BlockChain.Last();
        }

        /// <summary>
        /// Purpose: Retrieve the blockchain
        /// </summary>
        /// <returns>Blockchain</returns>
        public List<Block> GetCurrentBlockchain()
        {
            return Blockchain.BlockChain;
        }

        /// <summary>
        /// Purpose:To enqueue the given transaction in the transaction queue
        /// </summary>
        /// <param name="transaction"></param>
        public void RecieveNewTransaction(Transaction transaction)
        {
            TransactionStorage.TransactionQueue.Enqueue(transaction);
        }

        /// <summary>
        /// Purpose: to add a block to the chain
        /// </summary>
        /// <param name="block"></param>
        public static void AddBlock(Block block)
        {
            Blockchain.BlockChain.Add(block);
        }
    }
}
