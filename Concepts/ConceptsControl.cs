using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Concepts
{
    internal sealed class ConceptsControl : UserControl
    {
        private readonly ConceptControlFactory _factory;
        private readonly IEnumerable<Concept> _concepts;
        
        public ConceptsControl(ConceptControlFactory factory, IEnumerable<Concept> concepts)
        {
            _factory = factory;
            _concepts = concepts;
            SuspendLayout();
            AddControls();
            ResumeLayout();
        }

        private void AddControls()
        {
            var tabs = new TabControl {Dock = DockStyle.Fill};
            foreach (var concept in _concepts)
            {
                var page = new TabPage();
                page.Text = concept.Name;
                tabs.Controls.Add(page);
                var conceptControl = _factory.CreateConceptControl(concept);
                page.Controls.Add(conceptControl);
                conceptControl.Dock = DockStyle.Fill;
            }
            
            Controls.Add(tabs);
        }
    }
}
