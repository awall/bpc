using System.Collections.Generic;

namespace Concepts
{
    internal sealed class Concept
    {
        public string Name { get; set; }        
        public IEnumerable<Node> Nodes { get; set; } = new List<Node>();
        public IEnumerable<Effect> Effects { get; set; } = new List<Effect>();
    }
}