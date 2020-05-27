using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    [ServiceContract]
    public interface IBlockchain
    {
        [OperationContract]
        List<Block> GetCurrentBlockchain();

        [OperationContract]
        Block GetCurrentBlock();

        [OperationContract]
        void RecieveNewTransaction(Transaction transaction);
    }
}
