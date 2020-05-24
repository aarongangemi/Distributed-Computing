using System;
using System.Text.RegularExpressions;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    /// <summary>
    /// Purpose: The user controller is used to create a user and retrieve a given user
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class UserController : ApiController
    {
        BankDB.UserAccessInterface userAccess = Bank.bankData.GetUserAccess();

        /// <summary>
        /// Purpose: To create a user based on the given first name and last name
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <returns>user object with all user details</returns>
        // GET api/<controller>/5
        [Route("api/User/Create/{fname}/{lname}")]
        [HttpPost]
        public UserDetailsStruct createUser(string fname, string lname)
        {
            UserDetailsStruct uds = new UserDetailsStruct();
            // create user object
            uds.userId = userAccess.CreateUser();
            // create user and assign new ID to user object
            userAccess.SelectUser(uds.userId);
            uds.firstName = fname;
            uds.lastName = lname;
            // set first name and last name
            userAccess.SetUserName(uds.firstName, uds.lastName);
            userAccess.GetUserName(out uds.firstName, out uds.lastName);
            // set username
            return uds;  
        }

        /// <summary>
        /// Purpose: To retrieve the given user based on ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>User object</returns>
        [Route("api/User/{userId}")]
        [HttpGet]
        public UserDetailsStruct GetUser(uint userId)
        {
            try
            {
                string FirstName, LastName;
                userAccess.SelectUser(userId);
                // select user with user ID
                userAccess.GetUserName(out FirstName, out LastName);
                // get the username of the given user
                UserDetailsStruct userData = new UserDetailsStruct();
                userData.firstName = FirstName;
                userData.lastName = LastName;
                userData.userId = userId;
                // get user details and return user object
                return userData;
            }
            catch(Exception)
            {
                // if user cannot be found, return null
                return null;
            }
        }
    }
}