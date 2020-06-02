using BlockchainLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Tutorial_7_Blockchain_Server.Models
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
            int val = 0;
            string hashedString ="";
            uint hashOffset = 0;
            // brute force a hash that begins with "12345".
            // continuously checks if the hash generated has "12345" and if it doesn't then increment the offset and try again
            while (!hashedString.StartsWith("12345"))
            {
                hashOffset += 1;
                string blockString = val.ToString() + val.ToString() + val.ToString() + hashOffset + "";
                byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                byte[] hashedData = sha256.ComputeHash(textBytes);
                hashedString = BitConverter.ToUInt64(hashedData, 0).ToString();
            }
            // add block to blochcain with hardcoded values
            BlockChain.Add(new Block(0, 0, 0, hashOffset, "", hashedString));
        }
    }
}
