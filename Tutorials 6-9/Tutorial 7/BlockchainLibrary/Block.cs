using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainLibrary
{
    /// <summary>
    /// Purpose: The block class contains fields to create a block for the blockchain.
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
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

        /// <summary>
        /// Purpose: The empty block constructor is required because in order for the service contract to work,
        /// if a block is returned, the interface requires an empty constructor to be in place. This is never called.
        /// </summary>
        public Block()
        {}

        /// <summary>
        /// Purpose: The block constructor is used to create a block for each transaction. The block sets all
        /// fields required for a block.
        /// </summary>
        /// <param name="walletIdFrom"></param>
        /// <param name="walletIdTo"></param>
        /// <param name="amount"></param>
        /// <param name="blockOffset"></param>
        /// <param name="prevBlockHash"></param>
        /// <param name="blockHash"></param>
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
