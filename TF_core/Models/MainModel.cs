using System.Collections.Generic;
using TF_core.Controllers;
using Tweetinvi.Models;

namespace TF_core.Models
{
    public static class MainModel
    {
        public static List<Unit> Team { get; set; } = new List<Unit>();
        public static List<IUser> Pool { get; set; }        

        public static List<IUser> TimeLine { get; set; }

        public static string UserID { get; set; }

        public static bool DoD => Team.Count  % 2 == 0;

        //public static string GetUserEmail() => ClaimsPrincipal User.Identities.First().Claims.Where(c => c.Type == "emails").Single().Value;

    }
    
}