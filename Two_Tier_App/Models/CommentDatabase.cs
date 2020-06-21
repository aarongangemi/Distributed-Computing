using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Two_Tier_App.Models
{
    /// <summary>
    /// Purpose: The comment database class used to store the comment database as a static object
    /// Author: Aaron Gangemi
    /// Date Modified: 19/06/2020
    /// </summary>
    public static class CommentDatabase
    {
        public static ExamLib.CommentDatabase commentDb = new ExamLib.CommentDatabase();
    }
}