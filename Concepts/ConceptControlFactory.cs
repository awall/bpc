using System.Collections.Generic;
using System.Windows.Forms;

namespace Concepts
{
    internal interface ConceptControlFactory
    {
        Control CreateConceptControl(Concept concept);

        Control CreateConceptsControl(IEnumerable<Concept> concepts);
    }
}