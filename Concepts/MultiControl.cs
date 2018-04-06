using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Concepts
{
    internal sealed class MultiControl : UserControl
    {
        private const int BUTTON_WIDTH = 20;
        
        private Button _addButton;
        private List<Control> _controls = new List<Control>();

        public MultiControl()
        {
            SuspendLayout();
            AddControls();    
            ResumeLayout();
            Layout += DoLayout;
        }

        public event Action AddPortal;

        public void AddControl(Control control)
        {
            var panel = new Panel();

            var minusButton = new Button
            {
                Size = new Size(BUTTON_WIDTH, BUTTON_WIDTH),
                Location = new Point(panel.Width - BUTTON_WIDTH, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Text = "-"
            };
            minusButton.Click += (o, e) => RemoveControl(panel);

            control.Height = panel.Height;
            control.Width = panel.Width - minusButton.Width;
            control.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;                        
            
            panel.Controls.Add(control);
            panel.Controls.Add(minusButton);
            
            _controls.Add(panel);
            Controls.Add(panel);
        }

        private void DoLayout(object sender, LayoutEventArgs layoutEventArgs)
        {
            var count = _controls.Count;
            var cols = Math.Min(2, count);
            var rows = (count + 1) / 2;

            var i = 0;
            foreach (var control in _controls)
            {
                var col = i % 2;
                var row = i / 2;
                var size = new Size((Width  - _addButton.Width) / cols, Height / rows);
                control.Size = size;
                control.Location = new Point(col * size.Width, row * size.Height);
                ++i;
            }
        }
        
        private void RemoveControl(Control control)
        {
            _controls.Remove(control);
            Controls.Remove(control);
        }
        
        private void AddControls()
        {
            _addButton = new Button();
            Controls.Add(_addButton);
            _addButton.Text = "+";
            _addButton.Size = new Size(BUTTON_WIDTH, BUTTON_WIDTH);
            _addButton.Location = new Point(Width - _addButton.Width, 0);
            _addButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _addButton.Click += (o, e) => AddPortal?.Invoke();
        }
    }
}