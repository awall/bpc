using System;
using System.Collections.Generic;
using System.Linq;

namespace Concepts
{
    internal sealed class PortalController
    {
        private readonly PortalControl _control;
        private readonly Concept _concept;
        private readonly Func<Concept, Plugs, World> _worldBuilder;

        public PortalController(PortalControl control, Concept concept, Func<Concept, Plugs, World> worldBuilder)
        {
            _control = control;
            _concept = concept;
            _worldBuilder = worldBuilder;
        }

        public void Control()
        {
            _control.SuspendLayout();
            _control.Holes = Holes();
            _control.NodeList = _concept.Nodes.Select(n => n.Name);
            _control.SeriesList = _concept.Nodes.SelectMany(n => n.SeriesByName.Select(k => k.Key));
            Recompute();
            _control.ResumeLayout();
            _control.Changed += Recompute;
        }

        private void Recompute()
        {
            var plugs = new Plugs();
            foreach (var hole in Holes())
            {
                plugs[hole] = _control.ValueForHole(hole);
            }

            var world = _worldBuilder.Invoke(_concept, plugs);
            var series = _control.Series;
            var nodes = _control.Node;
            var timeSeries = world.GetSeries(nodes, series);
            _control.TimeSeries = timeSeries;
        }

        private IEnumerable<Hole> Holes()
        {
            return _concept.Effects.SelectMany(e => e.Holes);
        }
    }
}