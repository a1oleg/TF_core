using System;
using System.Collections.Generic;
using System.Linq;
using Neo4jClient;
using TF_core.Models;

namespace TF_core.Controllers
{
    internal static class Repo
    {
        public static GraphClient db;

        internal static IEnumerable<Unit> GetWithRelName(string email, string relname)
        {            
            return  db.Cypher
               .Match($"(a)-[:{relname}]->(b)")
               .Where((Unit a) => a.ScreenName == email)
               .ReturnDistinct
               (b => b.CollectAs<Unit>())
               .Results.First();
        }

        internal static void MergeVertex(string name, string label)
        {           
            db.Cypher
           .Merge("(a{Name:$n})")
           .WithParam("n", name)
           .Set($"a:{label}")
           .ExecuteWithoutResults();            
        }

    }
}