using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_7_Blockchain_Server.Models
{
    public class Block
    {
        public static uint blockID;
        public uint walletIdFrom;
        public uint walletIdTo;
        public float amount;
        public uint blockOffset;
        public string prevBlockHash;
        public string blockHash;

        public Block(uint walletIdFrom, uint walletIdTo, float amount, uint blockOffset, string prevBlockHash, string blockHash)
        {
            blockID = blockID + 1;
            this.walletIdFrom = walletIdFrom;
            this.walletIdTo = walletIdTo;
            this.amount = amount;
            this.blockOffset = blockOffset;
            this.prevBlockHash = prevBlockHash;
            this.blockHash = blockHash;
        }
    }
}
