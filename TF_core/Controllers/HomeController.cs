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
            if (!User.Identities.First().IsAuthenticated)
                return View();
            else
            {
                MainModel.UserID = User.Identities.First().Name;
                Repo.db = new GraphClient(new Uri("http://localhost:11003/db/data"));
                //db = new GraphClient(new Uri("https://hobby-cjifkhhaanncgbkeedghmepl.dbs.graphenedb.com:24780/db/data/"), "a1oleg", "b.JgAr7iM4iqi3.PtrraWURkTLBhkB0");

                Repo.db.Connect();
                
                if (!User.Identities.First().Claims.Where(c => c.Type == "InDB").Any())
                {
                    Repo.MergeVertex(MainModel.UserID, "User");
                    User.Identities.First().AddClaim(new System.Security.Claims.Claim("InDB", "true"));
                }
                else if (User.Identities.First().Claims.Where(c => c.Type == "Complete").Single().Value == "true")
                {
                    Repo.GetTeams();
                    return View();
                }
                    
                GetTimeLine();
                
                return RedirectToAction("GetPool");
            }
        }       

        public ViewResult GetPool()
        {
            MainModel.Pool = new List<IUser>();
            while (MainModel.Pool.Count < 5)
            {
                IUser newUnit = MainModel.TimeLine.ElementAt(new Random().Next(0, MainModel.TimeLine.Count()));                

                if (!MainModel.Pool.Contains(newUnit) && !MainModel.Team.Where(u => u.Name == newUnit.Name).Any())
                {
                    MainModel.TimeLine.Remove(newUnit);
                    MainModel.Pool.Add(newUnit);
                }
                    
            }
            return View("Index");
        }

        public RedirectToActionResult DraftOrDrop(string ScreenName)
        {
            IUser PickedUnit = MainModel.Pool.Where(u => u.ScreenName == ScreenName).Single();
            MainModel.Pool.Remove(PickedUnit);

            MainModel.Team.Add(new Unit() { Name = PickedUnit.Name, ProfileImage = PickedUnit.ProfileImageUrl400x400, Friendly = MainModel.DoD });

            if (MainModel.Team.Count() < 10)
                return RedirectToAction("GetPool");
            else return RedirectToAction("FinalTeams");
        }

        public ViewResult FinalTeams()
        {
            foreach (Unit u in MainModel.Team)
            {
                Repo.MergeVertex(u.Name, "Unit");
                Repo.MergeRelationship(u.Name, u.Friendly.ToString());
            }
            MainModel.Pool = null;
            return View("Index");
        }        

        public void GetTimeLine()
        {
            Auth.SetUserCredentials("tsyORY4SD9j5b5JyXHbzQI0GA", "5J3LHiasGNSycdcaGkd8BYhNs6p1YxkM7OhUrG9dhShhZT585t",
               "218404828-6ytjbb1jPcY4GAvlLvdiGlqsN27WSjvvRWhPsp9n", "JzfiOUQ1bbxd8LIS6Ke04Nmy0VABaCpWgXRd5QRcABFFq");

            MainModel.TimeLine = new List<IUser>();

            foreach (ITweet tweet in Timeline.GetHomeTimeline().Distinct()) //Aggregate
                MainModel.TimeLine.Add(tweet.CreatedBy);
        }
        
        
        //[AllowAnonymous]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
