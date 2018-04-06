namespace Concepts
{
    public struct Range
    {
        public Range(int begin, int end)
        {
            Begin = begin;
            End = end;
        }

        public int Begin { get; }
        public int End { get; }
    }
}