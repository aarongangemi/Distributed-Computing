using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_9_Blockchain_Library
{
    public class Block
    {
        public static uint blockCounter = 0;
        public uint blockID;
        public string TransactionDetailsList;
        public uint blockOffset;
        public string prevBlockHash;
        public string blockHash;

        public Block()
        {}
        public Block(uint blockOffset, string prevBlockHash, string blockHash)
        {
            blockCounter++;
            blockID = blockCounter;
            TransactionDetailsList = JsonConvert.SerializeObject(new List<string[]>());
            this.blockOffset = blockOffset;
            this.prevBlockHash = prevBlockHash;
            this.blockHash = blockHash;
        }

        public void AddPythonTransaction(string pythonSrc, string pythonResult)
        {
            List<string[]> JsonTransactionList = JsonConvert.DeserializeObject<List<string[]>>(TransactionDetailsList);
            string[] PythonTransactionList = new string[2];
            PythonTransactionList[0] = pythonSrc;
            PythonTransactionList[1] = pythonResult;
            JsonTransactionList.Add(PythonTransactionList);
            TransactionDetailsList = JsonConvert.SerializeObject(JsonTransactionList);
        }
    }
}
