using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Concepts
{
    internal static class Concepts
    {
        public static readonly List<Concept> All =
            new List<Concept>
            {
                PriceDeck(),
                TemplateWell(),
                GEL(),
                ScaleFile()
            };
        
        private static Concept GEL()
        {
            return new Concept
            {
                Name = "gel",
                
                Nodes = new []
                {
                    new NodeBuilder("gel")
                    {
                        ["costs"] = new TimeSeries(0, new decimal[0]),
                        ["revenue"] = new TimeSeries(0, new decimal[0]),
                        ["operating_income"] = new TimeSeries(0, new decimal[0]),
                        ["truncated_operating_income"] = new TimeSeries(0, new decimal[0])
                    }.Build(),
                    new NodeBuilder("revenue")
                    {
                        ["revenue"] = new TimeSeries(0, new[] {54000m, 42000m, 32000m, 24000m, 18000m}),
                        ["truncated_revenue"] = new TimeSeries(0, new decimal[0]),
                    }.Build(),
                    new NodeBuilder("costs")
                    {
                        ["costs"] = new TimeSeries(0, new[] {34000m, 22000m, 16000m, 25000m, 32000m}),                        
                        ["truncated_costs"] = new TimeSeries(0, new decimal[0]),
                    }.Build()                                         
                },
                
                Effects = new Effect[]
                {
                    new OwnsEffect("gel", "revenue", 1.0m),
                    new OwnsEffect("gel", "costs", 1.0m),
                    new GelTruncationEffect("gel", new List<string>{"revenue", "costs"})                    
                }
            };
        }
        
        private static Concept TemplateWell()
        {
            return new Concept
            {
                Name = "template_well",
                
                Nodes = new []
                {                                       
                    new NodeBuilder("field").Build(), 
                    new NodeBuilder("template_well")
                    {
                        ["oil production"] = new TimeSeries(0, new [] {4000m, 3000m, 2000m, 1000m })
                    }.Build()                    
                },
                
                Effects = new Effect[]
                {
                    new RepeatEffect("field", "template_well", new Hole("well_count"), new Hole("cycle"))                    
                }
            };
        }
        
        private static Concept PriceDeck()
        {
            return new Concept
            {
                Name = "price_deck",
                
                Nodes = new []
                {
                    new NodeBuilder("canada")
                    {
                        ["oil production"] = new TimeSeries(0, new[] {54000m, 42000m, 32000m, 24000m, 18000m}),
                        ["oil revenue"] = new TimeSeries(0, new decimal[0])
                    }.Build(),
                   
                    new NodeBuilder("price").Build(), 
                    new NodeBuilder("low")
                    {
                        ["oil price"] = new TimeSeries(0, new [] {40.0m, 41, 42, 45, 43})
                    }.Build(),
                    new NodeBuilder("medium")
                    {
                        ["oil price"] = new TimeSeries(0, new [] {50.0m, 49, 47, 51, 53})
                    }.Build(),
                    new NodeBuilder("high")
                    {
                        ["oil price"] = new TimeSeries(0, new [] {60.0m, 59, 57, 64, 62})
                    }.Build()
                },
                
                Effects = new Effect[]
                {
                    new OwnsEffect("price", "low", new Hole("low")),
                    new OwnsEffect("price", "medium", new Hole("medium")),
                    new OwnsEffect("price", "high", new Hole("high")),
                    new RevenueEffect("canada", "price")
                }
            };
        }
        
        private static Concept ScaleFile()
        {
            return new Concept
            {
                Name = "scale_file",
                
                Nodes = new []
                {
                    new NodeBuilder("canada")
                    {
                        ["oil price"] = new TimeSeries(0, new [] {40.0m, 41, 42, 45, 43}),
                        ["oil revenue"] = new TimeSeries(0, new decimal[0])
                    }.Build(),
                                       
                    new NodeBuilder("scaled").Build(),
                    
                    new NodeBuilder("a1")
                    {                        
                        ["oil production"] = new TimeSeries(0, new[] {24000m, 16000m, 12000m, 8000m, 7000m}),
                        ["costs"] = new TimeSeries(0, new[] {440000m, 360000m, 320000m, 280000m, 270000m})
                    }.Build(),
                    
                    new NodeBuilder("a2")
                    {                        
                        ["oil production"] = new TimeSeries(0, new[] {21000m, 14000m, 10000m, 7000m, 6000m}),
                        ["costs"] = new TimeSeries(0, new[] {430000m, 350000m, 310000m, 270000m, 260000m})
                    }.Build(),
                    
                    new NodeBuilder("a3")
                    {                        
                        ["oil production"] = new TimeSeries(0, new[] {44000m, 46000m, 42000m, 38000m, 37000m}),
                        ["costs"] = new TimeSeries(0, new[] {640000m, 560000m, 520000m, 480000m, 470000m})
                    }.Build()
                },
                
                Effects = new Effect[]
                {
                    new OwnsEffect("scaled", "a1", 1m),
                    new OwnsEffect("scaled", "a2", 1m),
                    new ScaleEffect("scaled", new Hole("costs_scale"), new Hole("production_scale")),
                    new OwnsEffect("canada", "scaled", 1m),
                    new OwnsEffect("canada", "a3", 1m),                   
                    new RevenueEffect("canada", "canada")
                }
            };
        }
    }

    internal class GelTruncationEffect : Effect
    {
        private readonly string _group;
        private readonly IEnumerable<string> _members;

        public GelTruncationEffect(string group, IEnumerable<string> members)
        {
            _group = group;
            _members = members;
        }

        public IEnumerable<Hole> Holes => Enumerable.Empty<Hole>();

        public void Compute(WorldBuilder worldBuilder, Plugs plugs)
        {
            var costs = worldBuilder[_group]["costs"];
            var revenue = worldBuilder[_group]["revenue"];
            var operating_income = TimeSeries.Sum(new List<TimeSeries> {TimeSeries.Scale(-1m, costs), revenue});
            worldBuilder[_group]["operating_income"] = operating_income;

            var cum = 0m;
            var maxcum = 0m;
            var maxcumpos = -1;
            var i = 0;
            
            foreach (var value in operating_income.Values)
            {
                cum += value;
                if (cum > maxcum)
                {
                    maxcum = cum;
                    maxcumpos = i;
                }
                
                i++;
            }

            var truncationMonth = maxcumpos + operating_income.Range.Begin + 1;

            Truncate(truncationMonth, worldBuilder, _group, "operating_income");
            Truncate(truncationMonth, worldBuilder, _group, "revenue");
            Truncate(truncationMonth, worldBuilder, _group, "costs");
            foreach (var member in _members)
            {
                Truncate(truncationMonth, worldBuilder, member, "revenue");
                Truncate(truncationMonth, worldBuilder, member, "costs");
            }
        }

        private static void Truncate(int truncationMonth, WorldBuilder worldBuilder, string node, string seriesName)
        {
            if (!worldBuilder[node].TryGetValue(seriesName, out var series)) return;            
            var values = series.Values;
            var truncatedValues = new decimal[values.Length];

            for (var i = 0; i < values.Length; ++i)
            {
                if (i + series.Range.Begin >= truncationMonth) break;
                truncatedValues[i] = values[i];
            }
            
            worldBuilder[node][$"truncated_{seriesName}"] = new TimeSeries(series.Range.Begin, truncatedValues);
        }
    }

    internal class RepeatEffect : Effect
    {
        private readonly string _target;
        private readonly string _source;        
        private readonly Hole _count;
        private readonly Hole _cycle;

        public RepeatEffect(string target, string source, Hole count, Hole cycle)
        {
            _target = target;
            _source = source;
            _count = count;
            _cycle = cycle;
        }

        public IEnumerable<Hole> Holes => new List<Hole> {_count, _cycle}.AsEnumerable().Where(e => e != null);
        
        public void Compute(WorldBuilder worldBuilder, Plugs plugs)
        {
            foreach (var pair in worldBuilder[_source].SeriesByName)
            {
                var count = plugs[_count];
                var offset = 0;
                var cycle = (int) plugs[_cycle];
                while (count > 0m)
                {
                    var ratio = Math.Min(1m, count);
                    count--;
                    
                    var value = TimeSeries.Scale(ratio, pair.Value);
                    value = new TimeSeries(value.Range.Begin + offset, value.Values);
                    if (worldBuilder[_target].TryGetValue(pair.Key, out var existing))
                    {
                        value = TimeSeries.Sum(new List<TimeSeries>{ existing, value});
                    }
                    worldBuilder[_target][pair.Key] = value;

                    offset += cycle;
                }                                             
            }
        }
    }

    internal sealed class RevenueEffect : Effect
    {
        private readonly string _production;
        private readonly string _price;

        public RevenueEffect(string production, string price)
        {
            _production = production;
            _price = price;
        }

        public IEnumerable<Hole> Holes => Enumerable.Empty<Hole>();
        
        public void Compute(WorldBuilder worldBuilder, Plugs plugs)
        {
            var nodeBuilder = worldBuilder[_production];
            var seriesByName = new List<KeyValuePair<string, TimeSeries>>(nodeBuilder.SeriesByName);
            foreach (var pair in seriesByName)
            {
                if (!pair.Key.EndsWith("production")) continue;
                var matchingPriceName = pair.Key.Replace("production", "price");
                if (!worldBuilder[_price].TryGetValue(matchingPriceName, out var priceSeries)) continue;
                var revenue = TimeSeries.Product(new List<TimeSeries> {pair.Value, priceSeries});
                nodeBuilder[pair.Key.Replace("production", "revenue")] = revenue;
            }
        }
    }

    internal sealed class ScaleEffect : Effect
    {
        private readonly Hole _costs;
        private readonly Hole _production;
        private readonly string _node;

        public ScaleEffect(string node, Hole costs, Hole production)
        {
            _node = node;
            _costs = costs;
            _production = production;            
        }

        public IEnumerable<Hole> Holes => new List<Hole>{_costs, _production};

        public void Compute(WorldBuilder worldBuilder, Plugs plugs)
        {
            var keyValuePairs = new List<KeyValuePair<string, TimeSeries>>(worldBuilder[_node].SeriesByName);
            foreach (var pair in keyValuePairs)
            {
                var name = pair.Key;
                if (name.EndsWith("production"))
                {
                    worldBuilder[_node][name] = TimeSeries.Scale(plugs[_production], pair.Value);
                }
                if (name.EndsWith("costs"))
                {
                    worldBuilder[_node][name] = TimeSeries.Scale(plugs[_costs], pair.Value);
                }                
            }
        }
    }
    
    internal sealed class OwnsEffect : Effect
    {
        private readonly Hole _hole;
        private readonly decimal _ratio;
        private readonly string _child;
        private readonly string _parent;

        public OwnsEffect(string parent, string child, decimal ratio)
        {
            _parent = parent;
            _child = child;
            _ratio = ratio;
        }
        
        public OwnsEffect(string parent, string child, Hole hole)
        {
            _parent = parent;
            _child = child;
            _hole = hole;
        }

        public IEnumerable<Hole> Holes => _hole == null
            ? Enumerable.Empty<Hole>()
            : new List<Hole>{_hole};

        public void Compute(WorldBuilder worldBuilder, Plugs plugs)
        {
            foreach (var pair in worldBuilder[_child].SeriesByName)
            {
                var ratio = _hole == null ? _ratio : plugs[_hole];
                var value = TimeSeries.Scale(ratio, pair.Value);
                if (worldBuilder[_parent].TryGetValue(pair.Key, out var existing))
                {
                    value = TimeSeries.Sum(new List<TimeSeries>{ existing, value});
                }
                worldBuilder[_parent][pair.Key] = value;
            }
        }
    }
}