using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TF_core.Models;
using Tweetinvi;
using Tweetinvi.Models;

namespace TF_core.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Repo.db = new GraphClient(new Uri("http://localhost:11003/db/data"));
            //db = new GraphClient(new Uri("https://hobby-cjifkhhaanncgbkeedghmepl.dbs.graphenedb.com:24780/db/data/"), "a1oleg", "b.JgAr7iM4iqi3.PtrraWURkTLBhkB0");

            Repo.db.Connect();

            string email = User.Identities.First().Claims.Where(c => c.Type == "emails").Single().Value;
            if (User.Identities.First().Claims.Where(c => c.Type == "InDB") == null)
            {
                Repo.MergeVertex(email, "User");
                User.Identities.First().AddClaim(new System.Security.Claims.Claim("InDB", "true"));
            }
            else if (User.Identities.First().Claims.Where(c => c.Type == "Complete").Single().Value == "true")
                return RedirectToAction("GetTeamsFromDB");

            GetTimeLine();
            return RedirectToAction("CreateTeams");  
        }

        public ViewResult GetTeamsFromDB()
        {
            MainModel.Friendly = new List<Unit>(Repo.GetWithRelName(User.Identities.First().Claims.Where(c => c.Type == "emails").Single().Value, "Friendly"));
            MainModel.Enemy = new List<Unit>(Repo.GetWithRelName(User.Identities.First().Claims.Where(c => c.Type == "emails").Single().Value, "Enemy"));

            return View("Index");
        }

        public IActionResult CheckTeams()
        {
            if (MainModel.Friendly.Count() + MainModel.Enemy.Count() >= 10)
            {
                MainModel.Pool = null;
                return View("Index");
            }                
            else
                return RedirectToAction("GetPool");
        }

        public void GetTimeLine()
        {
            Auth.SetUserCredentials("tsyORY4SD9j5b5JyXHbzQI0GA", "5J3LHiasGNSycdcaGkd8BYhNs6p1YxkM7OhUrG9dhShhZT585t",
               "218404828-6ytjbb1jPcY4GAvlLvdiGlqsN27WSjvvRWhPsp9n", "JzfiOUQ1bbxd8LIS6Ke04Nmy0VABaCpWgXRd5QRcABFFq");

            List<IUser> timeline = new List<IUser>();

            foreach (ITweet tweet in Timeline.GetHomeTimeline().Distinct()) //Aggregate
                MainModel.TimeLine.Add(tweet.CreatedBy);            
        }

        public ViewResult GetPool()
        {
            MainModel.Pool = new List<IUser>();

            while (MainModel.Pool.Count < 5)
            {
                IUser newUnit = MainModel.TimeLine.ElementAt(new Random().Next(0, MainModel.TimeLine.Count()));
                MainModel.TimeLine.Remove(newUnit);

                if (!MainModel.Friendly.Concat(MainModel.Enemy).Where(u => u.ScreenName == newUnit.ScreenName).Any())
                    MainModel.Pool.Add(newUnit);
            }
            return View("Index");
        }

        public ViewResult DraftOrDrop(string ScreenName/*, bool FriendlyOrNot*/)
        {
            IUser PickedUnit = MainModel.Pool.Where(u => u.ScreenName == ScreenName).Single();
            MainModel.Pool.Remove(PickedUnit);

            var targetTeam = MainModel.DoD ? MainModel.Friendly : MainModel.Enemy;

            targetTeam.Add(new Unit() { ScreenName = PickedUnit.ScreenName, ProfileImage = PickedUnit.ProfileImageUrl400x400 });
            
            return View ("Index");
        }


        //[AllowAnonymous]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
