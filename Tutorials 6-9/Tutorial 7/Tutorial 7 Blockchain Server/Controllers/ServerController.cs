using BlockchainLibrary;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Route("api/Server/GetBalance/{acntID}")]
        [HttpGet]
        public float GetAccountBalance(uint acntID)
        {
            float balance = 0;
            if(acntID == 0)
            {
                balance = float.MaxValue;
            }
            else
            {
                foreach (Block block in Blockchain.BlockChain)
                {
                    if (block.walletIdFrom == acntID)
                    {
                        balance -= block.amount;
                    }
                    if (block.walletIdTo == acntID)
                    {
                        balance += block.amount;
                    }
                }
            }
            return balance;
        }

        [Route("api/Server/ValidateBlock/")]
        [HttpPost]
        public bool ValidateBlock([FromBody] Block block)
        {
            SHA256 sha256 = SHA256.Create();
            string blockString = block.walletIdFrom.ToString() + block.walletIdTo.ToString() + block.amount.ToString() + block.blockOffset + block.prevBlockHash;
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockString));
            string hashedString = BitConverter.ToUInt64(hash, 0).ToString();
            if ((GetAccountBalance(block.walletIdFrom) < block.amount) || (block.amount <= 0)
                || (block.prevBlockHash != Blockchain.BlockChain.Last().blockHash)
                || (!block.blockHash.StartsWith("12345")) || (block.amount < 0) || (block.walletIdFrom < 0) || 
                (block.walletIdTo < 0) || (GetAccountBalance(block.walletIdFrom) < 0)|| (block.blockHash != hashedString))
            {
                return false;
            }
            return true;
        }

        [Route("api/Server/AddBlock")]
        [HttpPost]
        public void AddBlock([FromBody] Block block)
        {
            Blockchain.BlockChain.Add(block);
        }

        [Route("api/Server/GenerateGenesisBlock")]
        [HttpPost]
        public void GenerateGenesisBlock()
        {
            Blockchain.generateGenesisBlock();
        }
    }
}