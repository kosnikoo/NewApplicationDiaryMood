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
using System.Windows;

namespace NewApplicationDiaryMood.UI
{
    public partial class StatisticWindow : Window, INotifyPropertyChanged
    {
        private readonly StaticsService _statisticsService;
        private readonly int _userId;

        private DateTime? _startDate;
        private DateTime? _endDate;
        private PlotModel _monthPlotModel;
        private PlotModel _moodPlotModel;

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

        public PlotModel MonthPlotModel
        {
            get => _monthPlotModel;
            set
            {
                _monthPlotModel = value;
                OnPropertyChanged();
            }
        }

        public PlotModel MoodPlotModel
        {
            get => _moodPlotModel;
            set
            {
                _moodPlotModel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public StatisticWindow(IMoodRepository moodRepository, int userId)
        {
            InitializeComponent();
            _statisticsService = new StaticsService(moodRepository);
            _userId = userId;

            MonthPlotModel = new PlotModel();
            MoodPlotModel = new PlotModel();

            InitializeDatePickers();
            DataContext = this;
            LoadStatistics();
        }

        private void InitializeDatePickers()
        {
            StartDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
            EndDatePicker.SelectedDate = DateTime.Now;
        }

        private void LoadStatistics()
        {
            try
            {
                var filter = new MoodFilter
                {
                    StartDate = StartDatePicker.SelectedDate,
                    EndDate = EndDatePicker.SelectedDate
                };

                var monthlyStats = _statisticsService.GetMoodByMonth(_userId, filter);
                var moodStats = _statisticsService.GetByMood(_userId, filter);

                LoadMonthChart(monthlyStats);
                LoadMoodChart(moodStats);

                MonthlyStatsDataGrid.ItemsSource = monthlyStats.Select(m => new
                {
                    m.Year,
                    m.Month,
                    MonthName = m.GetMonthName(),
                    m.Count
                });

                MoodStatsDataGrid.ItemsSource = moodStats;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}");
            }
        }

        private void LoadMonthChart(List<Domain.Statistics.MonthStatistic> data)
        {
            var plotModel = new PlotModel { Title = "Динамика настроений по месяцам" };

            if (data.Any())
            {
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
                    MaximumPadding = 0.1,
                    Minimum = 0
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
            }
            else
            {
                plotModel.Title = "Нет данных за выбранный период";
            }

            MonthPlotModel = plotModel;
        }

        private void LoadMoodChart(List<Domain.Statistics.MoodStatistic> data)
        {
            var plotModel = new PlotModel { Title = "Распределение по типам настроений" };

            if (data.Any())
            {
                var categoryAxis = new CategoryAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Типы настроений"
                };

                var valueAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Количество записей",
                    Minimum = 0,
                    MinorStep = 1,
                    MajorStep = 1
                };

                plotModel.Axes.Add(categoryAxis);
                plotModel.Axes.Add(valueAxis);

                var barSeries = new BarSeries
                {
                    Title = "Количество",
                    FillColor = OxyColor.FromRgb(79, 129, 189),
                    StrokeColor = OxyColors.Black,
                    StrokeThickness = 1
                };

                int index = 0;
                foreach (var item in data.OrderByDescending(m => m.Count))
                {
                    categoryAxis.Labels.Add(item.MoodName);
                    barSeries.Items.Add(new BarItem(item.Count, index));
                    index++;
                }

                plotModel.Series.Add(barSeries);
            }
            else
            {
                plotModel.Title = "Нет данных за выбранный период";
            }

            MoodPlotModel = plotModel;
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate > EndDatePicker.SelectedDate)
            {
                MessageBox.Show("Дата начала не может быть больше даты окончания", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadStatistics();
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
            EndDatePicker.SelectedDate = DateTime.Now;
            LoadStatistics();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}