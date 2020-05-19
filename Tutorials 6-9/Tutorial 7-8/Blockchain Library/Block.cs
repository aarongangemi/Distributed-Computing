using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain_Library
{
    public class Block
    {
        private static uint blockID;
        private uint walletIdFrom;
        private uint walletIdTo;
        private float amount;
        private uint blockOffset;
        private string prevBlockHash;
        private string blockHash;

        public Block(uint blockOffset)
        {
            this.blockOffset = blockOffset;
            blockID = blockID + 1;
            prevBlockHash = "";
            blockHash = GenerateHash();
        }

        public string GenerateHash()
        {
            SHA256 sha256 = SHA256.Create();
            string hashData = "12345" + prevBlockHash + blockOffset + "54321";
            byte[] byteData = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashData));
            return Encoding.Default.GetString(byteData);
        }

        public void setWalletIDFrom(uint walletIDFrom)
        {
            walletIdFrom = walletIDFrom;
        }

        public void setWalletIDTo(uint walletIDTo)
        {
            walletIdTo = walletIDTo;
        }

        public void setAmount(float amount)
        {
            this.amount = amount;
        }

        


    }
}
