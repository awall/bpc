using System;

namespace Concepts
{
    internal sealed class Hole : IComparable<Hole>, IEquatable<Hole>
    {
        public Hole(string name)
        {
            Name = name;
        }
        
        public string Name { get; }

        public int CompareTo(Hole other)
        {
            return Name.CompareTo(other.Name);
        }

        public bool Equals(Hole other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Hole hole && Equals(hole);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}