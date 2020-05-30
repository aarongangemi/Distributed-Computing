using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial_8_Blockchain_Library
{
    /// <summary>
    /// Purpose: The interface/service contract implemented by the .NET remoting server
    /// Author: Aaron Gangemi
    /// Date Modified: 30/05/2020
    /// </summary>
    [ServiceContract]
    public interface IBlockchain
    {
        /// <summary>
        /// Purpose: return current blockchain
        /// </summary>
        /// <returns>Blockchain</returns>
        [OperationContract]
        List<Block> GetCurrentBlockchain();

        /// <summary>
        /// Purpose: return current block
        /// </summary>
        /// <returns>Last block</returns>
        [OperationContract]
        Block GetCurrentBlock();

        /// <summary>
        /// Purpose: add new transaction to chain
        /// </summary>
        /// <param name="transaction"></param>
        [OperationContract]
        void RecieveNewTransaction(Transaction transaction);
    }
}
