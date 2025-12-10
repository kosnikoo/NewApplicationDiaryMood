using Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SqlServer;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;


namespace NewApplicationDiaryMood
{
    public partial class App : Application
    {
        private IReservationRepository _reservationRepository = null!;
        private IMasterRepository _masterRepository = null!;
        private AppDbContext _dbContext = null!;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // 1. Чтение конфигурации из файла
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.database.json")
            .Build();
            // 2. Создание DbContext через фабрику
            var factory = new MoodDbContextFactory();
            _dbContext = factory.CreateDbContext(configuration);
            // 3. ВАЖНО: Применение миграций автоматически при запуске
            _dbContext.Database.Migrate();
            // 4. Создание репозиториев на основе DbContext
            _reservationRepository = new ReservationRepository(_dbContext);
            _masterRepository = new MasterRepository(_dbContext);
            // 5. Заполнение тестовыми данными (только если БД пустая)
            SeedInitData();
            // 6. Запуск главного окна
            var mainWindow = new ReservationsListWindow(_reservationRepository, _masterRepository);
            mainWindow.Show();
        }
        private void SeedInitData()
        {
            // Проверяем, есть ли уже данные в БД
            if (_masterRepository.GetAll().Any())
            {
                // Данные уже есть, пропускаем заполнение
                return;
            }
            // Остальной код SeedInitData остаётся без изменений
            // ...
        }
        protected override void OnExit(ExitEventArgs e)
        {
            // ВАЖНО: Освобождаем ресурсы DbContext при закрытии приложения
            _dbContext?.Dispose();
            base.OnExit(e);
        }
    }

}
