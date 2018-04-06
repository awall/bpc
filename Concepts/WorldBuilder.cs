using System.Collections.Generic;
using System.Linq;

namespace Concepts
{
    internal class WorldBuilder
    {
        private readonly Dictionary<string, NodeBuilder> _nodes = new Dictionary<string, NodeBuilder>();

        public NodeBuilder this[string name]
        {
            get => _nodes[name];
            set => _nodes[name] = value;
        }

        public World Build()
        {
            var nodes = new List<Node>(
                _nodes.Values.Select(n => n.Build()).AsEnumerable());
            return new World(nodes);
        }
        
        public static World CreateWorld(Concept concept, Plugs plugs)
        {
            var worldBuilder = new WorldBuilder();
            foreach (var node in concept.Nodes)
            {
                worldBuilder[node.Name] = new NodeBuilder(node);
            }

            foreach (var effect in concept.Effects)
            {
                effect.Compute(worldBuilder, plugs);
            }
            return worldBuilder.Build();
        }
    }
}