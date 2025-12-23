using Domain;
using Domain.Enum;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlServer
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;
        private readonly IMoodRepository _moodRepository;
        private readonly ICourseRepository _courseRepository;

        public NotificationRepository(AppDbContext context, IMoodRepository moodRepository, ICourseRepository courseRepository)
        {
            _context = context;
            _moodRepository = moodRepository;
            _courseRepository = courseRepository;
        }

        public List<Notification> GetByUserId(int userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.NotDate)
                .ToList();
        }

        public Notification GetById(int id)
        {
            return _context.Notifications.Find(id);
        }

        public int Add(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return notification.Id;
        }

        public bool Update(Notification notification)
        {
            var existing = _context.Notifications.Find(notification.Id);
            if (existing == null)
                return false;

            existing.CopyFrom(notification);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification == null)
                return false;

            _context.Notifications.Remove(notification);
            _context.SaveChanges();
            return true;
        }

        public void GenerateRecommendationNotifications(int userId, IMoodRepository moodRepository, ICourseRepository courseRepository)
        {
            var filter = new MoodFilter
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now
            };

            var moodStats = moodRepository.GetMoodStatistics(userId, filter);

            if (moodStats.Any())
            {
                var mostFrequentMood = moodStats.OrderByDescending(kv => kv.Value).FirstOrDefault();

                if (mostFrequentMood.Value > 5)
                {
                    var recommendation = courseRepository.GetByCategory(mostFrequentMood.Key)
                        .FirstOrDefault();

                    var notification = new Notification
                    {
                        UserId = userId,
                        NotTitle = "Рекомендация курса",
                        NotDescription = $"За последние 30 дней у вас часто встречалось настроение: {mostFrequentMood.Key}",
                        NotMoodStatistic = $"{mostFrequentMood.Key}: {mostFrequentMood.Value} раз",
                        NotRecommendation = recommendation != null
                            ? $"Рекомендуем курс: {recommendation.Title}"
                            : "Посетите раздел 'Курсы' для подбора программы",
                        NotDate = DateTime.Now,
                        IsRead = false
                    };

                    Add(notification);
                }
            }

            // Дополнительно: генерация уведомлений при достижении 20 настроений
            GenerateCourseRecommendationNotifications(userId, moodRepository, courseRepository);
        }
        public void GenerateCourseRecommendationNotifications(int userId, IMoodRepository moodRepository, ICourseRepository courseRepository)
        {
            var filter = MoodFilter.Empty; // Без фильтра по дате - все настроения
            var moodStats = moodRepository.GetMoodStatistics(userId, filter);

            foreach (var moodStat in moodStats)
            {
                if (moodStat.Value >= 20 && moodStat.Key != Domain.Enum.MoodType.None)
                {
                    // Проверяем, не отправляли ли уже уведомление для этого количества
                    var existingNotification = _context.Notifications
                        .Where(n => n.UserId == userId &&
                                   n.NotTitle.Contains("Достижение") &&
                                   n.NotDescription.Contains($"{moodStat.Value} настроений типа {moodStat.Key}"))
                        .FirstOrDefault();

                    if (existingNotification == null)
                    {
                        var course = courseRepository.GetByCategory(moodStat.Key).FirstOrDefault();

                        var notification = new Notification
                        {
                            UserId = userId,
                            NotTitle = "🎯 Достижение!",
                            NotDescription = $"Вы добавили {moodStat.Value} настроений типа {moodStat.Key}",
                            NotMoodStatistic = $"{moodStat.Key}: {moodStat.Value} раз",
                            NotRecommendation = course != null
                                ? $"Рекомендуем курс: '{course.Title}' для работы с этим состоянием. Перейдите в раздел 'Курсы' для подробностей."
                                : "Посетите раздел 'Курсы' для подбора программы по этому типу настроения.",
                            NotDate = DateTime.Now,
                            IsRead = false
                        };

                        Add(notification);
                    }
                }
            }
        }
    }
}
