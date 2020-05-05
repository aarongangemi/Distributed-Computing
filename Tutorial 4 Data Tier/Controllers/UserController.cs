using System;
using System.Text.RegularExpressions;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    public class UserController : ApiController
    {

        BankDB.UserAccessInterface userAccess = Bank.bankData.GetUserAccess();


        // GET api/<controller>/5
        [Route("api/User/Create/{fname}/{lname}")]
        [HttpPost]
        public UserDetailsStruct createUser(string fname, string lname)
        {
             UserDetailsStruct uds = new UserDetailsStruct();
             uds.userId = userAccess.CreateUser();
             userAccess.SelectUser(uds.userId);
             uds.firstName = fname;
             uds.lastName = lname;
             userAccess.SetUserName(uds.firstName, uds.lastName);
             userAccess.GetUserName(out uds.firstName, out uds.lastName);
             return uds;  
        }

        [Route("api/User/{userId}")]
        [HttpGet]
        public UserDetailsStruct GetUser(uint userId)
        {
            try
            {
                string FirstName, LastName;
                userAccess.SelectUser(userId);
                userAccess.GetUserName(out FirstName, out LastName);
                UserDetailsStruct userData = new UserDetailsStruct();
                userData.firstName = FirstName;
                userData.lastName = LastName;
                userData.userId = userId;
                return userData;
            }
            catch(Exception)
            {
                Console.WriteLine("Invalid user ID entered");
                return null;
            }
        }
    }
}