using System.Collections.Generic;
using System.Linq;

namespace Concepts
{
    internal sealed class Node
    {
        private readonly IDictionary<string, TimeSeries> _series;
        
        public Node(string name, IDictionary<string, TimeSeries> series)
        {
            Name = name;
            _series = series;
        }
        
        public string Name { get; }
        public IEnumerable<KeyValuePair<string, TimeSeries>> SeriesByName => _series.AsEnumerable();

        public TimeSeries this[string name]
        {
            get => _series[name];
        }
    }
}