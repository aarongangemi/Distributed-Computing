using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tutorial_2
{
    /// <summary>
    /// Purpose: The business server interface, used to communicate with data tier
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    [ServiceContract] 
    // Defines that interface is for distributed objects
    public interface BusinessServerInterface
    {
        // Each of these are service function contracts. They need to be tagged as OperationContracts.
        // Defines that method will be for distributed objects
        [OperationContract]
        int GetNumEntries(); 
        // Get num entries
        [OperationContract]
        // Get values for entry at idx
        void GetValuesForEntry(int index, out uint acctNo, out uint pin,
                               out int bal, out string fname, out string lname, out string filePath);
        [OperationContract]
        // Search for value
        int SearchForValue(string str);
    }
}
