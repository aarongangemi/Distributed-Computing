using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JobLibrary
{
    /// <summary>
    /// Purpose: The IClient class is an interface which utilizes the service contract field. 
    /// Stores the methods that are required on the server.
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    [ServiceContract]
    public interface IClient
    {
        /// <summary>
        /// Purpose: Allow the client to request the next available job
        /// </summary>
        /// <returns>The requested job</returns>
        [OperationContract]
        Job RequestJob();

        /// <summary>
        /// Purpose: To allow the client to upload the python solution to the given idx in the list
        /// </summary>
        /// <param name="pyResult"></param>
        /// <param name="idx"></param>
        [OperationContract]
        void UploadJobSolution(string pyResult, int idx);

    }
}
