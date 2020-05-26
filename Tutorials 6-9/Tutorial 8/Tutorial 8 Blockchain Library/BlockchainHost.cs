using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    public class BlockchainHost : IBlockchain
    {
        private List<Block> Blockchain = new List<Block>();
        public Block GetCurrentBlock()
        {
            return Blockchain.Last();
        }

        public List<Block> GetCurrentBlockchain()
        {
            return Blockchain;
        }

        public void RecieveNewTransaction()
        {
            throw new NotImplementedException();
        }
    }
}
