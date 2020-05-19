using Blockchain_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace Tutorial_7_Miner.Controllers
{
    public class BlockchainController : ApiController
    {
        // GET api/<controller>
        [Route("api/Blockchain/AddTransaction")]
        [HttpPost]
        public void AddTransaction(uint walletIdFrom, uint walletIdTo, float amount)
        {
            /*SHA256 sha256 = SHA256.Create();
            string prevBlockHash, blockString, hashString;
            prevBlockHash = Blockchain.BlockChain.ElementAt(Blockchain.BlockChain.Count - 1).prevBlockHash;
            Blockchain.IncrementOffset();
            blockString = walletIdFrom.ToString() + walletIdTo.ToString() + amount.ToString() + Blockchain.hashOffset.ToString() + prevBlockHash;
            byte[] blockBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockString));
            hashString = "12345" + Encoding.Default.GetString(blockBytes) + "54321";
            Block block = new Block(walletIdFrom, walletIdTo, amount, Blockchain.hashOffset, prevBlockHash, hashString);
            Blockchain.BlockChain.Add(block);*/
        }
    }
}