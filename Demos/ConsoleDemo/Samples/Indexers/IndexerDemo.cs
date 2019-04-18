using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Indexers
{
    public class League
    {
        private Dictionary<string, int> _scores = new Dictionary<string, int>();

        public IReadOnlyIndexer<string, int, int> Scores { get; }

        public League()
        {
            Scores = MvvmKit.Indexers.ReadOnly(_scores)
                                     .And((int i) => _scores.ElementAt(i).Value);
        }
    }

    public static class IndexerDemo
    {
        public static void Run()
        {
            var league = new League();
            var liverpoolScore = league.Scores["Liverpool"];
            var firstTeamScore = league.Scores[0];
        }
    }
}
