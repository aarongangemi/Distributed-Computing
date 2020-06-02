using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tutorial_8_Blockchain_Library;

namespace Tutorial_8_Web_Server.Models
{
    /// <summary>
    /// Purpose: To store a static list which keeps track of which clients are added to server
    /// Author: Aaron Gangemi
    /// Date Modified: 29/05/2020
    /// </summary>
    public static class ClientList
    {
        public static List<Client> clientList = new List<Client>();
    }
}