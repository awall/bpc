using System;
using System.Collections.Generic;
using System.Linq;

namespace Concepts
{
    internal sealed class TimeSeries
    {       
        public TimeSeries(int begin, decimal[] values)
        {
            Values = values;
            Range = new Range(begin, begin + values.Length);
        }

        public decimal[] Values { get; }
        public Range Range { get; }

        public decimal Maximum => Values.Length == 0 ? 0m : Values.AsEnumerable().Max();

        public static TimeSeries Sum(IEnumerable<TimeSeries> summands)
        {
            var begin = 0;
            var end = 0;
            foreach (var ts in summands)
            {
                begin = Math.Min(begin, ts.Range.Begin);
                end = Math.Max(end, ts.Range.End);
            }

            var values = new decimal[end - begin];
            foreach (var ts in summands)
            {
                for (var i = ts.Range.Begin; i < ts.Range.End; ++i)
                {
                    values[i - begin] += ts.Values[i - ts.Range.Begin];
                }
            }
            
            return new TimeSeries(begin, values);
        }

        public static TimeSeries Scale(decimal ratio, TimeSeries series)
        {
            var values = new decimal[series.Values.Length];
            for (var i = 0; i < values.Length; ++i)
            {
                values[i] = ratio * series.Values[i];
            }
            return new TimeSeries(series.Range.Begin, values);
        }

        public static TimeSeries Product(IEnumerable<TimeSeries> factors)
        {
            var begin = 0;
            var end = 0;
            foreach (var ts in factors)
            {
                begin = Math.Min(begin, ts.Range.Begin);
                end = Math.Max(end, ts.Range.End);
            }

            var values = new decimal[end - begin];
            for (var i = 0; i < values.Length; ++i)
            {
                values[i] = 1m;
            }
            foreach (var ts in factors)
            {
                for (var i = begin; i < ts.Range.Begin; ++i)
                {
                    values[i - begin] = 0m;
                }
                for (var i = ts.Range.Begin; i < ts.Range.End; ++i)
                {
                    values[i - begin] *= ts.Values[i - ts.Range.Begin];
                }
                for (var i = ts.Range.End; i < end; ++i)
                {
                    values[i - begin] = 0m;
                }
            }
            
            return new TimeSeries(begin, values);
        }
    }
}
