using BlockchainLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Tutorial_7_Blockchain_Server.Models
{
    public static class Blockchain
    {
        public static List<Block> BlockChain = new List<Block>();
        public static uint hashOffset = 0;

        public static void IncrementOffset()
        {
            hashOffset += 1;
        }

        public static void generateGenesisBlock()
        {
            SHA256 sha256 = SHA256.Create();
            int val = 0;
            string hashedString ="";
            while (!hashedString.StartsWith("12345"))
            {
                IncrementOffset();
                string blockString = val.ToString() + val.ToString() + val.ToString() + hashOffset + "";
                byte[] textBytes = Encoding.UTF8.GetBytes(blockString);
                byte[] hashedData = sha256.ComputeHash(textBytes);
                hashedString = BitConverter.ToUInt64(hashedData, 0).ToString();
            }
            BlockChain.Add(new Block(0, 0, 0, hashOffset, "", hashedString));
        }
    }
}
