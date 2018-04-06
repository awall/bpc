using System.Collections.Generic;

namespace Concepts
{
    internal interface Effect
    {
        IEnumerable<Hole> Holes { get; }
        void Compute(WorldBuilder worldBuilder, Plugs plugs);
    }
}