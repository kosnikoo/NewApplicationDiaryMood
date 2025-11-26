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
        // Backing fields для свойств
        private DateTime? _startDate;
        private DateTime? _endDate;
        private PlotModel? _statusPlotModel;
        private PlotModel? _masterPlotModel;
        private PlotModel? _monthPlotModel;
        private PlotModel? _moodPlotModel;
        // Свойства с уведомлением
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
        // Реализация интерфейса
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadMoodChart(MoodFilter filter)
        {
            // 1. Получаем данные из сервиса
            var data = _statisticsService.GetByMood(filter);

            // 2. Создаём модель диаграммы
            var plotModel = new PlotModel { Title = "" };

            // 3. Создаём ось категорий (для текстовых меток)
            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Left, // Слева для горизонтальных столбцов
                Title = "Типы настроения"
            };

            // 4. Заполняем метки оси (важно: в том же порядке, что и данные!)
            foreach (var item in data)
            {
                categoryAxis.Labels.Add(item.MoodName);
            }
            plotModel.Axes.Add(categoryAxis);

            // 5. Создаём числовую ось (для значений)
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom, // Снизу для значений
                Title = "Количество записей",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });

            // 6. Создаём серию столбцов
            var barSeries = new BarSeries
            {
                Title = "Количество записей",
                FillColor = OxyColor.FromRgb(79, 129, 189) // Цвет столбцов
            };

            // 7. Заполняем столбцы данными (порядок должен совпадать с Labels)
            foreach (var item in data)
            {
                barSeries.Items.Add(new BarItem { Value = item.Count });
            }
            plotModel.Series.Add(barSeries);

            _moodPlotModel = plotModel;
        }
        private void LoadMonthChart(MoodFilter filter)
        {
            var data = _statisticsService.GetMoodByMonth(filter);
            // 2. Создаём модель диаграммы
            var plotModel = new PlotModel { Title = "" };
            // 3. Создаём ось времени (категорий) снизу
            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Angle = -15, // Поворот меток для лучшей читаемости
                Title = "Месяцы"
            };
            // 4. Заполняем метки оси
            foreach (var item in data)
            {
                categoryAxis.Labels.Add(item.GetMonthName()); // "Янв 2025", "Фев 2025"...
            }
            plotModel.Axes.Add(categoryAxis);
            // 5. Создаём числовую ось (значений) слева
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Количество записей",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });
            // 6. Создаём серию линий
            var lineSeries = new LineSeries
            {
                Title = "Количество записей",
                Color = OxyColor.FromRgb(79, 129, 189),
                MarkerType = MarkerType.Circle, // Форма маркеров на точках
                MarkerSize = 4,
                MarkerFill = OxyColor.FromRgb(79, 129, 189)
            };
            // 7. Добавляем точки на график (x = индекс, y = значение)
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
