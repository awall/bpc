using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Concepts
{
    internal sealed class NodeBuilder
    {
        private readonly Dictionary<string, TimeSeries> _series = new Dictionary<string, TimeSeries>();

        public NodeBuilder(string name)
        {
            Name = name;
        }

        public NodeBuilder(Node node)
        {
            Name = node.Name;
            foreach (var pair in node.SeriesByName)
            {
                this[pair.Key] = pair.Value;
            }
        }

        public bool TryGetValue(string name, out TimeSeries series)
        {
            return _series.TryGetValue(name, out series);
        }
        
        public TimeSeries this[string name]
        {
            get => _series[name];
            set => _series[name] = value;
        }
        
        public string Name { get; }

        public IEnumerable<KeyValuePair<string, TimeSeries>> SeriesByName
        {
            get => _series.AsEnumerable();
        }

        public Node Build()
        {
            return new Node(Name, new Dictionary<string, TimeSeries>(_series));
        }
    }
}