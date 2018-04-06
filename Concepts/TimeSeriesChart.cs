using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Concepts
{
    internal class TimeSeriesChart : UserControl
    {
        private readonly Chart _chart;
        private TimeSeries _timeSeries;
        private decimal? _maximum;

        public TimeSeriesChart()
        {
            _chart = new Chart();
            Controls.Add(_chart);
            _chart.Dock = DockStyle.Fill;
        }

        public decimal Maximum {            
            set 
            {
                _maximum = value;
                if (_chart != null)
                {
                    _chart.ChartAreas[0].AxisY.Maximum = (double) value;
                }
            } 
        }

        public TimeSeries TimeSeries
        {
            get => _timeSeries;
            set
            {
                _timeSeries = value;

                _chart.ChartAreas.Clear();
                var chartArea = _chart.ChartAreas.Add("area");                

                _chart.Series.Clear();
                var series = _chart.Series.Add("series");
                series.Name = "series";
                series.ChartType = SeriesChartType.Area;

                series.XValueType = ChartValueType.Date;
                series.YValueType = ChartValueType.Double;                                

                chartArea.AxisX.MinorGrid.Enabled = false;
                chartArea.AxisX.MajorGrid.Enabled = false;
                chartArea.AxisX.LabelStyle.Format = "MMM yyyy";
                
                chartArea.AxisY.MinorGrid.Enabled = false;
                chartArea.AxisY.MajorGrid.Enabled = false;
                if (_maximum != null)
                {
                    chartArea.AxisY.Maximum = (double) _maximum;
                }

                var date = Dates.Y2K.AddMonths(_timeSeries.Range.Begin);
                foreach (var v in _timeSeries.Values)
                {
                    var nextdate = date.AddMonths(1);
                    series.Points.AddXY(date, v);
                    series.Points.AddXY(nextdate, v);
                    date = nextdate;
                }
            }
        }
    }
}
