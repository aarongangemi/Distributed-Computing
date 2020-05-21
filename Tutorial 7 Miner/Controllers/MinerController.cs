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
using System.Threading.Tasks;
using System.Web.Http;

namespace Tutorial_7_Miner.Controllers
{
    public class MinerController : ApiController
    {
        [Route("api/Blockchain/AddTransaction/{walletIdFrom}/{walletIdTo}/{amount}")]
        [HttpPost]
        public async Task<bool> AddTransaction(uint walletIdFrom, uint walletIdTo, float amount)
        {
            Debug.WriteLine(walletIdFrom.ToString() + "............." + walletIdTo.ToString() + "..........." + amount.ToString());
            RestClient client = new RestClient("https://localhost:44353/");
            string prevBlockHash;
            RestRequest BlockchainRequest = new RestRequest("api/Server/GetBlockchain");
            IRestResponse BlockchainResponse = await client.ExecuteGetAsync(BlockchainRequest);
            List<Block> blockchain = JsonConvert.DeserializeObject<List<Block>>(BlockchainResponse.Content);
            Debug.WriteLine("Got blockchain list, count = " + blockchain.Count);
            RestRequest IncrementRequest = new RestRequest("api/Server/Increment");
            client.Put(IncrementRequest);
            RestRequest OffsetRequest = new RestRequest("api/Server/GetOffset");
            IRestResponse OffsetResponse = await client.ExecuteGetAsync(OffsetRequest);
            uint offset = JsonConvert.DeserializeObject<uint>(OffsetResponse.Content);
            Debug.WriteLine(offset);
            if(blockchain.Count == 0)
            {
                prevBlockHash = "";
                Debug.WriteLine("HASH IS EMPTY");
            }
            else
            {
                prevBlockHash = blockchain.Last().blockHash;
                Debug.WriteLine("HASH IS FULL");
            }
            SHA256 sha256 = SHA256.Create();
            string blockString = walletIdFrom.ToString() + walletIdTo.ToString() + amount.ToString() + offset + prevBlockHash;
            byte[] blockBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockString));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < blockBytes.Length; i++)
            {
                builder.Append(blockBytes[i].ToString("x2"));
            }
            string hashString = "12345" + builder.ToString() + "54321";
            Debug.WriteLine("Miner hash is: " + hashString);
            Block block = new Block(walletIdFrom, walletIdTo, amount, offset, prevBlockHash, hashString);
            RestRequest ValidationRequest = new RestRequest("api/Server/ValidateBlock/");
            ValidationRequest.AddJsonBody(block);
            IRestResponse ValidationResponse = await client.ExecutePostAsync(ValidationRequest);
            bool isValidBlock = JsonConvert.DeserializeObject<bool>(ValidationResponse.Content);
            Debug.WriteLine("IS VALID = " + isValidBlock);
            if(isValidBlock)
            {
                RestRequest AddBlockRequest = new RestRequest("api/Server/AddBlock/");
                AddBlockRequest.AddJsonBody(block);
                await client.ExecutePostAsync(AddBlockRequest);
            }
            return isValidBlock;
        }
    }
}