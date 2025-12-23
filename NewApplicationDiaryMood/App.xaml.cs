using Domain;
using Domain.Enum;
using Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SqlServer;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace NewApplicationDiaryMood
{
    public partial class App : Application
    {
        private AppDbContext _dbContext;
        private IMoodRepository _moodRepository;
        private IUserRepository _userRepository;
        private ICourseRepository _courseRepository;
        private INotificationRepository _notificationRepository;
        private IExerciseRepository _exerciseRepository;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                InitializeApp();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка при запуске: {ex.Message}\n{ex.StackTrace}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Необработанная ошибка в UI: {e.Exception.Message}", "Ошибка");
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Необработанная ошибка: {(e.ExceptionObject as Exception)?.Message}", "Ошибка");
        }

        private void InitializeApp()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var factory = new MoodDbContextFactory();
            _dbContext = factory.CreateDbContext(configuration);
            _dbContext.Database.Migrate();

            _userRepository = new UserRepository(_dbContext);
            _moodRepository = new MoodRepository(_dbContext);
            _courseRepository = new CourseRepository(_dbContext);
            _exerciseRepository = new ExerciseRepository(_dbContext);

            _notificationRepository = new NotificationRepository(_dbContext, _moodRepository, _courseRepository);

            SeedInitData();

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ShowLoginWindow();
        }

        private void ShowLoginWindow()
        {

            var loginWindow = new UI.LoginWindow(_userRepository);

            loginWindow.LoginSuccessful += (user) =>
            {

                StartMainWindow(user);
                loginWindow.Close();
                
            };

            loginWindow.Closed += (s, args) =>
            {
                if (this.MainWindow == null)
                {
                    Current.Shutdown();
                }
            };
            loginWindow.Show();
        }

        private void StartMainWindow(User user)
        {
            try
            {
                _notificationRepository.GenerateRecommendationNotifications(
                    user.Id,
                    _moodRepository,
                    _courseRepository
                );
                var mainWindow = new UI.MainWindow(
                    _moodRepository,
                    _userRepository,
                    _courseRepository,
                    _notificationRepository,
                    _exerciseRepository,
                    user.Id
                );
                this.MainWindow = mainWindow;
                mainWindow.Closed += (s, args) =>
                {
                    Current.Shutdown();
                };

                mainWindow.Show();
            }
            catch (Exception ex)
            {
                Current.Shutdown();
            }
        }

        private void SeedInitData()
        {
            try
            {
                var existingUsers = _userRepository.GetAll();

                if (existingUsers.Any())
                {
                    return;
                }

                var testUser = new User
                {
                    FirstName = "Тестовый",
                    LastName = "Пользователь",
                    Email = "test@example.com",
                    Phone = "+79999999999",
                    HashPassword = "password",
                    SubActivation = false,
                    RegistrationDate = DateTime.Now
                };

                int userId = _userRepository.Add(testUser);
                var courses = new[]
                {
                    new Course
                    {
                        Title = "Курс медитации для снятия стресса",
                        Description = "Научитесь техникам медитации для снижения уровня стресса и тревожности",
                        Category = MoodType.Stressed,
                        Link = "https://example.com/meditation"
                    },
                    new Course
                    {
                        Title = "Курс позитивного мышления",
                        Description = "Развивайте позитивное мышление для улучшения настроения и мотивации",
                        Category = MoodType.Depressed,
                        Link = "https://example.com/positive"
                    },
                    new Course
                    {
                        Title = "Техники управления гневом",
                        Description = "Научитесь контролировать эмоции в стрессовых ситуациях",
                        Category = MoodType.Angry,
                        Link = "https://example.com/anger"
                    },
                    new Course
                    {
                        Title = "Управление тревогой",
                        Description = "Практические техники для снижения уровня тревожности",
                        Category = MoodType.Anxious,
                        Link = "https://example.com/anxiety"
                    },
                    new Course
                    {
                        Title = "Восстановление энергии",
                        Description = "Методики для борьбы с усталостью",
                        Category = MoodType.Tired,
                        Link = "https://example.com/energy"
                    }
                };

                foreach (var course in courses)
                {
                    int courseId = _courseRepository.Add(course);
                }

                var exercises = new[]
                {
                    new Exercise
                    {
                        Title = "Дыхательная техника 4-7-8",
                        Description = "Авторская техника для снятия напряжения и тревоги",
                        ForMoodType = MoodType.Anxious,
                        Instructions = "1. Сядьте удобно\n2. Вдох через нос 4 секунды\n3. Задержите дыхание 7 секунд\n4. Медленно выдохните через рот 8 секунд\n5. Повторите 5 раз",
                        DurationMinutes = 5,
                        IsAuthor = true
                    },
                    new Exercise
                    {
                        Title = "Прогрессивная мышечная релаксация",
                        Description = "Авторская техника для расслабления тела и снятия усталости",
                        ForMoodType = MoodType.Tired,
                        Instructions = "Поочередно напрягайте и расслабляйте группы мышц:\n1. Ступни\n2. Икры\n3. Бедра\n4. Живот\n5. Руки\n6. Плечи\n7. Лицо",
                        DurationMinutes = 10,
                        IsAuthor = true
                    },
                    new Exercise
                    {
                        Title = "Дневник благодарности",
                        Description = "Авторское упражнение для улучшения настроения",
                        ForMoodType = MoodType.Sad,
                        Instructions = "1. Запишите 5 вещей, за которые вы благодарны сегодня\n2. Опишите подробно каждую\n3. Подумайте, как эти вещи влияют на вашу жизнь",
                        DurationMinutes = 10,
                        IsAuthor = true
                    },
                    new Exercise
                    {
                        Title = "Медитация осознанности",
                        Description = "Авторская методика для снятия стресса",
                        ForMoodType = MoodType.Stressed,
                        Instructions = "1. Найдите тихое место\n2. Закройте глаза и сосредоточьтесь на дыхании\n3. Наблюдайте за мыслями без оценки",
                        DurationMinutes = 15,
                        IsAuthor = true
                    },
                    new Exercise
                    {
                        Title = "Техника переключения эмоций",
                        Description = "Авторская методика быстрого изменения эмоционального состояния",
                        ForMoodType = MoodType.Depressed,
                        Instructions = "1. Определите текущую эмоцию\n2. Вспомните ситуацию с противоположной эмоцией\n3. 'Перекройте' текущее состояние воспоминанием",
                        DurationMinutes = 10,
                        IsAuthor = true
                    }
                };

                foreach (var exercise in exercises)
                {
                    int exerciseId = _exerciseRepository.Add(exercise);
                }
                var random = new Random();
                var moodTypes = Enum.GetValues(typeof(MoodType))
                    .Cast<MoodType>()
                    .Where(m => m != MoodType.None)
                    .ToArray();

                int moodCount = 0;
                for (int i = 0; i < 25; i++)
                {
                    var mood = new Mood
                    {
                        UserId = userId,
                        MoodType = moodTypes[random.Next(moodTypes.Length)],
                        MoodQuantity = random.Next(1, 11),
                        EntryDate = DateTime.Now.AddDays(-random.Next(0, 30))
                    };
                    _moodRepository.Add(mood);
                    moodCount++;
                }
                var notification = new Notification
                {
                    UserId = userId,
                    NotTitle = "Добро пожаловать!",
                    NotDescription = "Спасибо за регистрацию в Дневнике настроения. Начните отслеживать свое эмоциональное состояние!",
                    NotMoodStatistic = "",
                    NotRecommendation = "Добавьте первое настроение, чтобы начать работу с приложением.",
                    NotDate = DateTime.Now,
                    IsRead = false
                };
                _notificationRepository.Add(notification);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации данных: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _dbContext?.Dispose();
            base.OnExit(e);
        }
    }
}