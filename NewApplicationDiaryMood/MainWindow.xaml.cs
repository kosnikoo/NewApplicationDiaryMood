using Domain;
using Interface;
using System;
using System.Linq;
using System.Windows;
using UI;

namespace NewApplicationDiaryMood.UI
{
    public partial class MainWindow : Window
    {
        private readonly IMoodRepository _moodRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IExerciseRepository _exerciseRepository;
        private readonly int _userId;
        private User _currentUser;

        public MainWindow(
            IMoodRepository moodRepository,
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            INotificationRepository notificationRepository,
            IExerciseRepository exerciseRepository,
            int userId)
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _moodRepository = moodRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _notificationRepository = notificationRepository;
            _exerciseRepository = exerciseRepository;
            _userId = userId;

            AddMoodButton.Click += AddMoodButton_Click;
            StatisticsButton.Click += StatisticsButton_Click;
            CoursesButton.Click += CoursesButton_Click;
            ExercisesButton.Click += ExercisesButton_Click;
            SubscriptionButton.Click += SubscriptionButton_Click;
            ProfileButton.Click += ProfileButton_Click;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadCurrentUser();
                LoadLastMood();
                CheckNotifications();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCurrentUser()
        {
            _currentUser = _userRepository.GetById(_userId);
            if (_currentUser != null)
            {
                WelcomeTextBlock.Text = $"Добро пожаловать, {_currentUser.FirstName}!";
                UserInfoText.Text = $"{_currentUser.FirstName} {_currentUser.LastName}";
                SubscriptionStatusTextBlock.Text = _currentUser.SubActivation
                    ? "⭐ Подписка активна"
                    : "Подписка не активна";
            }
        }

        private void LoadLastMood()
        {
            var lastMood = _moodRepository.GetLastMood(_userId);

            if (lastMood != null)
            {
                LastMoodText.Text = $"Последнее настроение: {lastMood.MoodType} ({lastMood.MoodQuantity}/10)";
                LastMoodDate.Text = $"Дата: {lastMood.EntryDate:dd.MM.yyyy HH:mm}";
            }
            else
            {
                LastMoodText.Text = "Не добавлено";
                LastMoodDate.Text = "";
            }
        }

        private void CheckNotifications()
        {
            try
            {
                var notifications = _notificationRepository.GetByUserId(_userId);
                if (notifications.Any(n => !n.IsRead))
                {
                    var unreadNotifications = notifications.Where(n => !n.IsRead).ToList();

                    foreach (var notification in unreadNotifications)
                    {
                        var notificationWindow = new NotificationWindow(notification);
                        notificationWindow.Owner = this;
                        notificationWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        notificationWindow.ShowDialog();

                        notification.IsRead = true;
                        _notificationRepository.Update(notification);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке уведомлений: {ex.Message}");
            }
        }

        private void AddMoodButton_Click(object sender, RoutedEventArgs e)
        {
            var moodWindow = new MoodWindow(_moodRepository, _userId);
            moodWindow.Owner = this;
            moodWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (moodWindow.ShowDialog() == true)
            {
                LoadLastMood();
                try
                {
                    _notificationRepository.GenerateRecommendationNotifications(
                        _userId,
                        _moodRepository,
                        _courseRepository
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка генерации уведомлений: {ex.Message}");
                }
            }
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var statisticsWindow = new StatisticWindow(_moodRepository, _userId);
            statisticsWindow.Owner = this;
            statisticsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            statisticsWindow.ShowDialog();
        }

        private void CoursesButton_Click(object sender, RoutedEventArgs e)
        {
            var coursesWindow = new CoursesWindow(_courseRepository);
            coursesWindow.Owner = this;
            coursesWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            coursesWindow.ShowDialog();
        }

        private void SubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCurrentUser();

            if (_currentUser.SubActivation)
            {
                MessageBox.Show("✅ Ваша подписка уже активна!\n\n" +
                               "Вам доступны все авторские упражнения и премиум-функции.",
                               "Подписка активна",
                               MessageBoxButton.OK, MessageBoxImage.Information);

                var subscriptionWindow = new SubscriptionWindow(_userRepository, _userId);
                subscriptionWindow.Owner = this;
                subscriptionWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                subscriptionWindow.ShowDialog();
                return;
            }
            var subscriptionWindowPay = new SubscriptionWindow(_userRepository, _userId);
            subscriptionWindowPay.Owner = this;
            subscriptionWindowPay.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (subscriptionWindowPay.ShowDialog() == true)
            {
                LoadCurrentUser();
                MessageBox.Show("✅ Подписка успешно активирована!\n\n" +
                               "Теперь доступны все авторские упражнения и премиум-функции.",
                               "Успех",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCurrentUser();

            if (!_currentUser.SubActivation)
            {
                var result = MessageBox.Show("Для доступа к упражнениям требуется подписка. Хотите оформить подписку?",
                    "Подписка", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var subscriptionWindow = new SubscriptionWindow(_userRepository, _userId);
                    subscriptionWindow.Owner = this;
                    subscriptionWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                    if (subscriptionWindow.ShowDialog() == true)
                    {
                        LoadCurrentUser();
                        OpenExercisesWindow();
                    }
                }
            }
            else
            {
                OpenExercisesWindow();
            }
        }

        private void OpenExercisesWindow()
        {
            var exercisesWindow = new MoodExercisesWindow(_exerciseRepository, _userId, _currentUser.SubActivation);
            exercisesWindow.Owner = this;
            exercisesWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            exercisesWindow.ShowDialog();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(_userRepository, _userId);
            profileWindow.Owner = this;
            profileWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (profileWindow.ShowDialog() == true)
            {
                LoadCurrentUser();
            }
        }
    }
}