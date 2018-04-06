using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;

namespace Concepts
{
    public sealed class MainForm : Form
    {
        public MainForm()
        {
            SuspendLayout();
            Text = "BP Concepts";
            Size = new Size(1500, 1000);
            AddControls();
            ResumeLayout();
        }

        private void AddControls()
        {
            var controlFactory = new ControlFactory();
            var page = controlFactory.CreateConceptsControl(Concepts.All);
            page.Dock = DockStyle.Fill;
            Controls.Add(page);
        }
    }
}
