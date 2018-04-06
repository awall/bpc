using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Concepts
{
    internal sealed class ConceptControl : UserControl
    {
        private readonly PortalControlFactory _factory;
        private readonly Concept _concept;
        private readonly List<PortalControl> _portals = new List<PortalControl>();

        public ConceptControl(PortalControlFactory factory, Concept concept)
        {
            _factory = factory;
            _concept = concept;
            SuspendLayout();
            AddControls();
            ResumeLayout();
        }

        private void AddControls()
        {
            var tabs = new TabControl {Dock = DockStyle.Fill};
            Controls.Add(tabs);

            var page1 = new TabPage("setup");            
            tabs.Controls.Add(page1);
            var picture = new PictureBox();
            picture.ImageLocation = $"{_concept.Name}.png";
            picture.Dock = DockStyle.Fill;
            page1.Controls.Add(picture);
            
            var page2 = new TabPage("evaluation");
            tabs.Controls.Add(page2);
            
            var portals = new MultiControl {Dock = DockStyle.Fill};
            Action addPortal = () =>
            {
                var portalControl = _factory.CreatePortalControl(_concept);
                portals.AddControl(portalControl);
                portalControl.Disposed += (o, e) => _portals.Remove(portalControl);
                portalControl.Changed += RecalibrateMaximums;
                _portals.Add(portalControl);
            }; 
            portals.AddPortal += addPortal;
            addPortal.Invoke();
            page2.Controls.Add(portals);
        }

        private void RecalibrateMaximums()
        {
            var max = 10m;
            foreach (var portal in _portals)
            {                
                max = Math.Max(max, portal.TimeSeries.Maximum * 1.15m);
            }

            foreach (var portal in _portals)
            {
                portal.Maximum = max;
            }
        }
    }
}
