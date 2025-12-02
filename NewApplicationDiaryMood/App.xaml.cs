using Data.SqlServer;
using Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service;
using SqlServer;
using System;
using System.IO;
using System.Windows;

namespace UI
{
    public partial class App : Application
    {
        private AppDbContext _dbContext;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // 1. ЧТАЕМ ФАЙЛ С НАСТРОЙКАМИ
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.database.json")
                    .Build();

                // 2. СОЗДАЕМ ПОДКЛЮЧЕНИЕ К БАЗЕ
                var factory = new AppDbContextFactory();
                _dbContext = factory.CreateDbContext(configuration);

                // 3. СОЗДАЕМ ТАБЛИЦЫ (ЕСЛИ ИХ НЕТ)
                _dbContext.Database.Migrate();

                // 4. СОЗДАЕМ РЕПОЗИТОРИЙ ДЛЯ БАЗЫ
                IMoodRepository moodRepository = new SqlServerMoodRepository(_dbContext);

                // 5. СОЗДАЕМ СЕРВИС СТАТИСТИКИ
                var staticsService = new StaticsService(moodRepository);

                // 6. ЗАПУСКАЕМ ОКНО
                var statisticWindow = new StatisticWindow(staticsService);
                statisticWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска: {ex.Message}\n\nПроверьте, что установлен LocalDB",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _dbContext?.Dispose();
            base.OnExit(e);
        }
    }
}
