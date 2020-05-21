using BlockchainLibrary;
using System.Collections.Generic;
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
            hashOffset += 5;
        }

        public static void generateGenesisBlock()
        {
            SHA256 sha256 = SHA256.Create();
            int val = 0;
            string blockString = val.ToString() + val.ToString() + int.MaxValue.ToString() + 0 + "";
            byte[] blockBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockString));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < blockBytes.Length; i++)
            {
                builder.Append(blockBytes[i].ToString("x2"));
            }
            IncrementOffset();
            BlockChain.Add(new Block(0, 0, float.MaxValue, hashOffset, "", "12345" + builder.ToString() + "54321"));
        }
    }
}
