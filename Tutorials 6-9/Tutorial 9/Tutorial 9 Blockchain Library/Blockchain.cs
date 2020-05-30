using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tutorial_9_Blockchain_Library
{
    /// <summary>
    /// Purpose: The blockchain class stores the blockchain and the method used to generate the genesis block.
    /// The genesis block is the first block in the blockchain
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public static class Blockchain
    {
        public static List<Block> BlockChain = new List<Block>();

        /// <summary>
        /// Purpose: To generate the first initial block of the blockchain
        /// </summary>
        public static void generateGenesisBlock()
        {
            SHA256 sha256 = SHA256.Create();
            string hashedString = "";
            Block b = new Block(0, "", hashedString);
            while (!hashedString.StartsWith("12345"))
            {
                // brute force a hash to start with 12345
                b.blockOffset++;
                // concatenate a string with block details
                string blockString = b.blockID + b.TransactionDetailsList + b.blockOffset + b.prevBlockHash;
                byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                byte[] hashedData = sha256.ComputeHash(textBytes);
                // compute hash and get integer representation of hash
                hashedString = BitConverter.ToUInt64(hashedData, 0).ToString();
            }
            b.blockHash = hashedString;
            // set hash and add first block to chain
            BlockChain.Add(b);
            Debug.WriteLine("Geneis Block successfully added");
        }

        /// <summary>
        /// Purpose: Add a block to the blockchain
        /// </summary>
        /// <param name="block"></param>
        public static void AddBlock(Block block)
        {
            BlockChain.Add(block);
        }
        /// <summary>
        /// Purpose: To retreive the number of blocks in the chain
        /// </summary>
        /// <returns>Number of blocks in chain</returns>
        public static int GetChainCount()
        {
            return BlockChain.Count;
        }

        /// <summary>
        /// Purpose: To validate the passed in block to confirm that it meets the criteria to join the blockchain
        /// </summary>
        /// <param name="block"></param>
        /// <returns>Whether or not the block can join the chain</returns>
        public static bool ValidateBlock(Block block)
        {
            SHA256 sha256 = SHA256.Create();
            string blockString = block.blockID + block.TransactionDetailsList + block.blockOffset + block.prevBlockHash;
            // concatenate the block elements into a string for hashing
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockString));
            string hashedString = BitConverter.ToUInt64(hash, 0).ToString();
            // get int value for hash
            // check amount, balance, previous block hash and current block hash, 
            // what the block starts with, sender and reciever fields
            if ((block.prevBlockHash != BlockChain.Last().blockHash)
                || (!block.blockHash.StartsWith("12345")) || (block.blockHash != hashedString))
            {
                // if one or more fields is true, then return false indicating do not add to chain
                return false;
            }
            // if all fields are validated, then allow chain to be valid
            return true;
        }
    }
}
