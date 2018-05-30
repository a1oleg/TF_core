﻿using System.Collections.Generic;
using TF_core.Controllers;
using Tweetinvi.Models;

namespace TF_core.Models
{
    internal static class MainModel
    {
        public static List<Unit> Friendly { get; set; }
        public static List<IUser> Pool { get; set; }
        public static List<Unit> Enemy { get; set; }

        public static List<IUser> TimeLine { get; set; }

        public static bool DoD => (Friendly.Count + Enemy.Count) % 2 == 0;

        //public static string GetUserEmail() => ClaimsPrincipal User.Identities.First().Claims.Where(c => c.Type == "emails").Single().Value;

    }
    
}