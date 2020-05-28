using System.Collections.Generic;
using System.Linq;

namespace Tutorial_9_Blockchain_Library
{
    public class BlockchainHost : IBlockchain
    {
        public Block GetCurrentBlock()
        {
            return Blockchain.BlockChain.Last();
        }

        public List<Block> GetCurrentBlockchain()
        {
            return Blockchain.BlockChain;
        }


        public void RecieveNewTransaction(Transaction transaction)
        {
            TransactionStorage.TransactionQueue.Enqueue(transaction);
        }

        public static void AddBlock(Block block)
        {
            Blockchain.BlockChain.Add(block);
        }
    }
}
