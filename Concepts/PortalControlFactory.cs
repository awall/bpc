using System.Windows.Forms;

namespace Concepts
{
    internal interface PortalControlFactory
    {
        PortalControl CreatePortalControl(Concept concept);
    }
}