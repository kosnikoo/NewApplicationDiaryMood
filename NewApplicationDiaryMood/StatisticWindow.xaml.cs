using Interface;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI
{
    public partial class StatisticWindow : Window, INotifyPropertyChanged
    {

        private readonly StaticsService _statisticsService;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private PlotModel? _statusPlotModel;
        private PlotModel? _masterPlotModel;
        private PlotModel? _monthPlotModel;
        private PlotModel? _moodPlotModel;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }
        public PlotModel? StatusPlotModel
        {
            get => _statusPlotModel;
            set
            {
                _statusPlotModel = value;
                OnPropertyChanged();
            }
        }
        public PlotModel? MasterPlotModel
        {
            get => _masterPlotModel;
            set
            {
                _masterPlotModel = value;
                OnPropertyChanged();
            }
        }
        public PlotModel? MonthPlotModel
        {
            get => _monthPlotModel;
            set
            {
                _monthPlotModel = value;
                OnPropertyChanged();
            }
        }
        public PlotModel? MoodPlotModel
        {
            get => _moodPlotModel;
            set
            {
                _moodPlotModel = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadMoodChart(MoodFilter filter)
        {
            var data = _statisticsService.GetByMood(filter);

            var plotModel = new PlotModel { Title = "" };

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Left,
                Title = "Типы настроения"
            };

            foreach (var item in data)
            {
                categoryAxis.Labels.Add(item.MoodName);
            }
            plotModel.Axes.Add(categoryAxis);

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Количество записей",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });


            var barSeries = new BarSeries
            {
                Title = "Количество записей",
                FillColor = OxyColor.FromRgb(79, 129, 189) 
            };


            foreach (var item in data)
            {
                barSeries.Items.Add(new BarItem { Value = item.Count });
            }
            plotModel.Series.Add(barSeries);

            MoodPlotModel = plotModel;
        }
        private void LoadMonthChart(MoodFilter filter)
        {
            var data = _statisticsService.GetMoodByMonth(filter);

            var plotModel = new PlotModel { Title = "" };

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Angle = -15, 
                Title = "Месяцы"
            };
            foreach (var item in data)
            {
                categoryAxis.Labels.Add(item.GetMonthName());
            }
            plotModel.Axes.Add(categoryAxis);
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Количество записей",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });

            var lineSeries = new LineSeries
            {
                Title = "Количество записей",
                Color = OxyColor.FromRgb(79, 129, 189),
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColor.FromRgb(79, 129, 189)
            };
            for (int i = 0; i < data.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, data[i].Count));
            }
            plotModel.Series.Add(lineSeries);
            MonthPlotModel = plotModel;
        }
        private void LoadStatistics()
        {
            var filter = new MoodFilter { StartDate = StartDate, EndDate = EndDate };
            LoadMonthChart(filter);
            LoadMoodChart(filter);
        }
        public StatisticWindow(StaticsService statisticsService)
        {
            InitializeComponent();
            _statisticsService = statisticsService;
            DataContext = this;
            LoadStatistics();
        }
        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }
        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            StartDate = null;
            EndDate = null;
            LoadStatistics();
        }
    }
}
