using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Concepts
{
    internal sealed class World
    {
        private readonly List<Node> _nodes;

        public World(IEnumerable<Node> nodes)
        {
            _nodes = new List<Node>(nodes); 
        }
        
        public TimeSeries GetSeries(string nodes, string series)
        {
            var matchingTimeSeries =
                _nodes //
                    .Where(n => Matches(nodes, n.Name)) //
                    .SelectMany(n => n.SeriesByName) //
                    .Where(t => Matches(series, t.Key)) //
                    .Select(t => t.Value);                       
            
            return TimeSeries.Sum(matchingTimeSeries);                        
        }

        private static bool Matches(string search, string candidate)
        {
            return search.Equals(candidate);
        }
    }
}