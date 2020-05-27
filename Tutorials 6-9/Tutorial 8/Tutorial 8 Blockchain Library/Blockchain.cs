using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tutorial_8_Blockchain_Library
{
    public static class Blockchain
    {
        public static List<Block> BlockChain = new List<Block>();

        public static void generateGenesisBlock()
        {
            if(GetChainCount() == 0)
            {
                SHA256 sha256 = SHA256.Create();
                int val = 0;
                uint hashOffset = 0;
                string hashedString = "";
                while (!hashedString.StartsWith("12345"))
                {
                    hashOffset++;
                    string blockString = val.ToString() + val.ToString() + val.ToString() + hashOffset + "";
                    byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                    byte[] hashedData = sha256.ComputeHash(textBytes);
                    hashedString = BitConverter.ToUInt64(hashedData, 0).ToString();
                }
                BlockChain.Add(new Block(0, 0, 0, hashOffset, "", hashedString));
            }
        }

        public static void AddBlock(Block block)
        {
            BlockChain.Add(block);
        }
        public static int GetChainCount()
        {
            return BlockChain.Count;
        }

        public static float GetAccountBalance(uint acntID)
        {
            float balance = 0;
            if (acntID == 0)
            {
                balance = float.MaxValue;
            }
            else
            {
                foreach (Block block in Blockchain.BlockChain)
                {
                    if (block.walletIdFrom == acntID)
                    {
                        balance -= block.amount;
                    }
                    if (block.walletIdTo == acntID)
                    {
                        balance += block.amount;
                    }
                }
            }
            return balance;
        }

        public static bool ValidateBlock(Block block)
        {
            SHA256 sha256 = SHA256.Create();
            string blockString = block.walletIdFrom.ToString() + block.walletIdTo.ToString() + block.amount.ToString() + block.blockOffset + block.prevBlockHash;
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockString));
            string hashedString = BitConverter.ToUInt64(hash, 0).ToString();
            if ((GetAccountBalance(block.walletIdFrom) < block.amount) || (block.amount <= 0)
                || (block.prevBlockHash != BlockChain.Last().blockHash)
                || (!block.blockHash.StartsWith("12345")) || (block.amount < 0) || (block.walletIdFrom < 0) ||
                (block.walletIdTo < 0) || (GetAccountBalance(block.walletIdFrom) < 0) || (block.blockHash != hashedString))
            {
                return false;
            }
            return true;
        }
    }
}
