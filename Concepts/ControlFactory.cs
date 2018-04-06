using System.Collections.Generic;
using System.Windows.Forms;

namespace Concepts
{
    sealed class ControlFactory : PortalControlFactory, ConceptControlFactory
    {
        public PortalControl CreatePortalControl(Concept concept)
        {
            var portal = new PortalControl();             
            new PortalController(portal, concept, WorldBuilder.CreateWorld).Control();
            return portal;
        }

        public Control CreateConceptControl(Concept concept)
        {
            return new ConceptControl(this, concept);
        }

        public Control CreateConceptsControl(IEnumerable<Concept> concepts)
        {
            return new ConceptsControl(this, concepts);
        }
    }
}