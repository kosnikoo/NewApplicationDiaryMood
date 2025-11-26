using Interface;
using Service;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InMemory;

namespace UI
{
    public partial class MainWindow : Window
    {
        
        private readonly IMoodRepository _moodRepository;
        public MainWindow()
        {
            InitializeComponent();
            _moodRepository = new MoodRepository();
            _moodRepository.NewSeedData();
        }
        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            
            var statisticsService = new StaticsService(_moodRepository);
            var window = new StatisticWindow(statisticsService);
            window.ShowDialog();
        }
    }
}