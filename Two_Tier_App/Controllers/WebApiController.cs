using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Two_Tier_App.Models;

namespace Two_Tier_App.Controllers
{
    /// <summary>
    /// Purpose: This is the code for the DC exam. The web api controller is used to store all functions
    /// Author: Aaron Gangemi
    /// Date Modified: 19/06/2020
    /// </summary>
    public class WebApiController : ApiController
    {
        /// <summary>
        /// Purpose: store whether the user is logged in or not
        /// </summary>
        private static bool isLoggedIn;

        /// <summary>
        /// Purpose: To add a comment to the comment database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        [Route("api/WebApi/CreateProfile/{username}/{password}")]
        [HttpPost]
        public void CreateProfile(string username, string password)
        {
            CommentDatabase.commentDb.AddUser(username, password);
        }

        /// <summary>
        /// Purpose: To log in the user. If the user cannot be logged in then an exception is thrown
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("api/WebApi/LoginUser/{username}/{password}")]
        [HttpPost]
        public bool LoginUser(string username, string password)
        {
            isLoggedIn = false;
            try
            {
                CommentDatabase.commentDb.LoginUser(username, password);
                // log in user and set to true
                isLoggedIn = true;
            }
            catch(ExamLib.NoUserException)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    // response to print
                    Content = new StringContent(string.Format("Unable to login user")),
                    ReasonPhrase = "User is not logged in"
                };
                throw new HttpResponseException(response);
            }
            return isLoggedIn;
        }

        /// <summary>
        /// Purpose: Allow the user to submit a new comment
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="username"></param>
        [Route("api/WebApi/SubmitComment/{comment}/{username}")]
        [HttpPost]
        public void SubmitComment(string comment, string username)
        {
            bool commentAdded = false;
            // comment not added yet - use bool variable
            if(isLoggedIn)
            {
                // if the user has logged in, add their comment
                CommentDatabase.commentDb.CreateComment(username, comment);
                commentAdded = true;
            }
            if(!commentAdded)
            {
                // if comment was not added then throw HTTP exception
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    // response to print
                    Content = new StringContent(string.Format("comment was not added - invalid username entered")),
                    ReasonPhrase = "User is not logged in"
                };
                throw new HttpResponseException(response);
            }
            else if(!isLoggedIn)
            {
                // if the user is not logged in, tell them
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    // response to print
                    Content = new StringContent(string.Format("User not logged in")),
                    ReasonPhrase = "User is not logged in"
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        /// Purpose: To request a comment
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/WebApi/RequestComment/{username}")]
        [HttpGet]
        public List<string> RequestComment(string username)
        {
            // create a temporary list to store the comments that will be returned to the view
            // only stores comments without the username
            List<string> commentListTemp = new List<string>();
            foreach (var comments in CommentDatabase.commentDb.GetComments())
            {
                if(comments.ElementAt(0).Equals(username))
                {
                    // if the user is the one in the list then add their comments to the temp list
                    commentListTemp.Add(comments.ElementAt(1));
                }
            }
            // send temp list back to user
            return commentListTemp;
        }

        /// <summary>
        /// Sends the comments list back to the view
        /// </summary>
        /// <returns></returns>
        [Route("api/WebApi/RetrieveComments")]
        [HttpGet]
        public List<List<string>> RetrieveComments()
        {
            return CommentDatabase.commentDb.GetComments();
        }
    }
}