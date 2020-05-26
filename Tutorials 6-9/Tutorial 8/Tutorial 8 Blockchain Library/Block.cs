﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    public class Block
    {
        public static uint blockCounter = 0;
        public uint blockID;
        public uint walletIdFrom;
        public uint walletIdTo;
        public float amount;
        public uint blockOffset;
        public string prevBlockHash;
        public string blockHash;

        public Block()
        {

        }
        public Block(uint walletIdFrom, uint walletIdTo, float amount, uint blockOffset, string prevBlockHash, string blockHash)
        {
            blockCounter++;
            blockID = blockCounter;
            this.walletIdFrom = walletIdFrom;
            this.walletIdTo = walletIdTo;
            this.amount = amount;
            this.blockOffset = blockOffset;
            this.prevBlockHash = prevBlockHash;
            this.blockHash = blockHash;
        }
    }
}
