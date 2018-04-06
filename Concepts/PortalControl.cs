using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Concepts
{
    internal sealed class PortalControl : UserControl
    {
        private const int LABEL_WIDTH = 70;
        private const int TEXT_WIDTH = 200;
        private const int TEXT_HEIGHT = 23;
        private const int LABEL_INDENT_Y = 4;
        private const int LABEL_INDENT_X = 4;
        private const int NUMBER_WIDTH = 100;

        private readonly TimeSeriesChart _timeSeries;

        private readonly SortedDictionary<Hole, LabeledControl<TextBox>> _holes =
            new SortedDictionary<Hole, LabeledControl<TextBox>>();

        private readonly LabeledControl<ComboBox> _series;
        private readonly LabeledControl<ComboBox> _nodes;

        public PortalControl()
        {
            Layout += DoLayout;
            SuspendLayout();

            _nodes = new LabeledControl<ComboBox>(new ComboBox(), "Nodes", TEXT_WIDTH);
            Controls.Add(_nodes);            
            _nodes.Control.SelectedValueChanged += (o, e) => { Changed?.Invoke(); };
            
            _series = new LabeledControl<ComboBox>(new ComboBox(), "Series", TEXT_WIDTH);
            Controls.Add(_series);
            _series.Control.SelectedValueChanged += (o, e) => { Changed?.Invoke(); };             

            _timeSeries = new TimeSeriesChart();
            Controls.Add(_timeSeries);

            ResumeLayout();
        }

        private void DoLayout(object sender, LayoutEventArgs layoutEventArgs)
        {
            _series.Location = new Point(0, 0);
            _nodes.Location = new Point(0, TEXT_HEIGHT);

            var y = 0;

            foreach (var control in _holes.Values)
            {
                control.Location = new Point(Size.Width - control.Width - LABEL_INDENT_X, y);
                y += TEXT_HEIGHT;
            }

            y = Math.Max(2 * TEXT_HEIGHT, y);

            _timeSeries.Location = new Point(0, y);
            _timeSeries.Size = new Size(Size.Width, Math.Max(0, Size.Height - y));
        }

        private sealed class LabeledControl<W> : UserControl where W : Control
        {
            internal LabeledControl(W control, string caption, int controlWidth)
            {
                Control = control;
                var label = new Label
                {
                    Text = caption,
                    Location = new Point(LABEL_INDENT_X, LABEL_INDENT_Y),
                    Width = LABEL_WIDTH
                };

                Controls.Add(label);
                Control.Location = new Point(label.Width + label.Location.X, 0);
                Control.Width = controlWidth;
                Control.Height = TEXT_HEIGHT;
                Controls.Add(Control);

                Height = Control.Height;
                Width = Control.Width + Control.Location.X + LABEL_INDENT_X;               
            }

            internal W Control { get; }

            internal new string Text => Control.Text;
        }

        public event Action Changed;

        public string Series => _series.Text;
        public string Node => _nodes.Text;

        public IEnumerable<String> SeriesList
        {
            set => SetItems(_series.Control, value);
        }

        public IEnumerable<String> NodeList
        {
            set => SetItems(_nodes.Control, value);
        }
        
        private void SetItems(ComboBox combo, IEnumerable<String> items) 
        {
            combo.Items.Clear();
            foreach (var label in items)
            {
                combo.Items.Add(label);
            }
        }

        public TimeSeries TimeSeries
        {
            get => _timeSeries.TimeSeries;
            set => _timeSeries.TimeSeries = value;
        }

        public IEnumerable<Hole> Holes
        {
            set
            {
                SuspendLayout();
                
                foreach (var control in _holes.Values)
                {
                    Controls.Remove(control);
                    control.Dispose();
                }                
                _holes.Clear();                
                foreach (var hole in value)
                {
                    var control = new LabeledControl<TextBox>(new TextBox(), hole.Name, NUMBER_WIDTH);
                    control.Control.TextChanged += (o, e) => { Changed?.Invoke(); };                    
                    Controls.Add(control);                    
                    _holes.Add(hole, control);
                }

                ResumeLayout();
            }
        }

        public decimal Maximum
        {
            set => _timeSeries.Maximum = value;
        }

        public decimal ValueForHole(Hole hole)
        {
            if (_holes.TryGetValue(hole, out var control))
            {
                if (decimal.TryParse(control.Text, out var result))
                {
                    return result;
                }
            }
            return 0m;
        }
    }
}