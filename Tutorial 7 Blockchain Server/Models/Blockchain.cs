using System.Collections.Generic;
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
    }
}
