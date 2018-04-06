using System.Collections.Generic;

namespace Concepts
{
    internal sealed class Plugs
    {
        private readonly Dictionary<Hole, decimal> _values = new Dictionary<Hole, decimal>();

        public decimal this[Hole key]
        {
            get => _values.ContainsKey(key) ? _values[key] : 0m;
            set => _values[key] = value;
        }               
    }
}