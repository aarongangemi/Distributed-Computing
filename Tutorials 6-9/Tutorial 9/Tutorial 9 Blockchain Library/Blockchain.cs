using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tutorial_9_Blockchain_Library
{
    public static class Blockchain
    {
        public static List<Block> BlockChain = new List<Block>();

        public static void generateGenesisBlock()
        {
            SHA256 sha256 = SHA256.Create();
            string hashedString = "";
            Block b = new Block(0, "", hashedString);
            while (!hashedString.StartsWith("12345"))
            {
                b.blockOffset++;
                string blockString = b.blockID + b.TransactionDetailsList + b.blockOffset + b.prevBlockHash;
                byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                byte[] hashedData = sha256.ComputeHash(textBytes);
                hashedString = BitConverter.ToUInt64(hashedData, 0).ToString();
            }
            b.blockHash = hashedString;
            BlockChain.Add(b);
            Debug.WriteLine("Geneis Block successfully added");
        }

        public static void AddBlock(Block block)
        {
            BlockChain.Add(block);
        }
        public static int GetChainCount()
        {
            return BlockChain.Count;
        }

        public static bool ValidateBlock(Block block)
        {
            SHA256 sha256 = SHA256.Create();
            string blockString = block.blockID + block.TransactionDetailsList + block.blockOffset + block.prevBlockHash;
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockString));
            string hashedString = BitConverter.ToUInt64(hash, 0).ToString();
            if ((block.prevBlockHash != BlockChain.Last().blockHash)
                || (!block.blockHash.StartsWith("12345")) || (block.blockHash != hashedString))
            {
                return false;
            }
            return true;
        }
    }
}
