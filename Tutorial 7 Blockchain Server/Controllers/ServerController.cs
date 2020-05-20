using BlockchainIntermed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Tutorial_7_Blockchain_Server.Models;

namespace Tutorial_7_Blockchain_Server.Controllers
{
    public class ServerController : ApiController
    {
        [Route("api/Server/GetBlockchain")]
        [HttpGet]
        public List<Block> GetBlockchain()
        {
            return Blockchain.BlockChain;
        }

        [Route("api/Server/GetNoOfBlocks")]
        [HttpGet]
        public int GetChainCount()
        {
            return Blockchain.BlockChain.Count;
        }

        [Route("api/Server/GetAccounts")]
        [HttpGet]
        public List<Account> GetAccounts()
        {
            return AccountList.listOfAccounts;
        }

        [Route("api/Server/GetBalance/{acntID}")]
        [HttpGet]
        public float GetAccountBalance(uint acntID)
        {
            foreach(Account acnt in AccountList.listOfAccounts)
            {
                if(acnt.accountID == acntID)
                {
                    return acnt.accountAmount;
                }
            }
            return -1;
        }

        [Route("api/Server/AddAcnt/")]
        [HttpPost]
        public void CreateAcnt()
        {
            Account acnt = new Account();
            if (AccountList.listOfAccounts.Count == 0)
            {
                acnt.accountAmount = int.MaxValue;
            }
            AccountList.listOfAccounts.Add(acnt);
        }

        [Route("api/Server/SubmitBlock/{walletIDFrom}/{walletIDTo}/{amount}")]
        [HttpPost]
        public void SubmitBlock(uint walletIDFrom, uint walletIDTo, uint amount)
        {
            bool validBlock = true;
            SHA256 sha256 = SHA256.Create();
            string prevBlockHash, blockString, hashString;
            prevBlockHash = Blockchain.BlockChain.ElementAt(Blockchain.BlockChain.Count - 1).prevBlockHash;
            Blockchain.IncrementOffset();
            blockString = walletIDFrom.ToString() + walletIDTo.ToString() + amount.ToString() + Blockchain.hashOffset.ToString();
            byte[] blockBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockString));
            hashString = "12345" + Encoding.Default.GetString(blockBytes) + "54321";
            Block block = new Block(walletIDFrom, walletIDTo, amount, Blockchain.hashOffset, prevBlockHash, hashString);
            foreach(Block b in Blockchain.BlockChain)
            {
                if(b.blockID > block.blockID)
                {
                    validBlock = false;
                    break;
                }
            }
            foreach(Account acnt in AccountList.listOfAccounts)
            {
                if(acnt.accountID == walletIDFrom)
                {
                    if(acnt.accountAmount < block.amount)
                    {
                        validBlock = false;
                        break;
                    }
                }
            }
            if(amount <= 0)
            {
                validBlock = false;
            }
            if(block.blockOffset % 5 != 0)
            {
                validBlock = false;
            }
            if(block.prevBlockHash != Blockchain.BlockChain.ElementAt(Blockchain.BlockChain.Count).blockHash)
            {
                validBlock = false;
            }
            if(!(block.blockHash.StartsWith("12345") && block.blockHash.EndsWith("54321")))
            {
                validBlock = false;
            }
            if(amount < 0 || walletIDFrom < 0 || walletIDTo < 0)
            {
                validBlock = false;
            }
            if(validBlock)
            {
                Blockchain.BlockChain.Add(block);
            }
        }
    }
}