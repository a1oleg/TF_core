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

        internal static void GetTeams()
        {
            MainModel.Team = new List<Unit>(db.Cypher
               .Match($"(a)-[]->(b)")
               .Where((Unit a) => a.Name == MainModel.UserID)
               .ReturnDistinct
               (b => b.CollectAs<Unit>())
               .Results.First());
        }

        internal static void MergeVertex(string name, string label)
        {           
            db.Cypher
           .Merge("(a{Name:$n})")
           .WithParam("n", name)
           .Set($"a:{label}")
           .ExecuteWithoutResults();            
        }

        internal static void MergeRelationship(string second, string relname)
        {
            db.Cypher
            .Match("(a)", "(b)")
            .Where((Unit a) => a.Name == MainModel.UserID)
            .AndWhere((Unit b) => b.Name == second)
            .Merge($"(a)-[:{relname}]->(b)")
            .ExecuteWithoutResults();
        }
    }
}