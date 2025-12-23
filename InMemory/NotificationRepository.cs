using Domain;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InMemory
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly List<Notification> _notifications = new();

        public List<Notification> GetByUserId(int userId)
        {
            return _notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.NotDate)
                .ToList();
        }

        public Notification? GetById(int id)
        {
            return _notifications.FirstOrDefault(n => n.Id == id);
        }

        public int Add(Notification notification)
        {
            notification.Id = _notifications.Count > 0 ? _notifications.Max(n => n.Id) + 1 : 1;
            _notifications.Add(notification);
            return notification.Id;
        }

        public bool Update(Notification notification)
        {
            var existing = GetById(notification.Id);
            if (existing == null)
                return false;

            existing.CopyFrom(notification);
            return true;
        }

        public bool Delete(int id)
        {
            var notification = GetById(id);
            if (notification == null)
                return false;

            return _notifications.Remove(notification);
        }

        public void GenerateRecommendationNotifications(int userId, IMoodRepository moodRepository, ICourseRepository courseRepository)
        {
            var filter = new MoodFilter
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now
            };

            var moodStats = moodRepository.GetMoodStatistics(userId, filter);

            var mostFrequentMood = moodStats.OrderByDescending(kv => kv.Value).FirstOrDefault();

            if (mostFrequentMood.Value > 10)
            {
                var recommendation = courseRepository.GetByCategory(mostFrequentMood.Key)
                    .FirstOrDefault();

                if (recommendation != null)
                {
                    var notification = new Notification
                    {
                        UserId = userId,
                        NotTitle = "Рекомендация курса",
                        NotDescription = $"За последние 30 дней у вас часто встречалось настроение: {mostFrequentMood.Key}",
                        NotMoodStatistic = $"{mostFrequentMood.Key}: {mostFrequentMood.Value} раз",
                        NotRecommendation = $"Рекомендуем курс: {recommendation.Title}",
                        NotDate = DateTime.Now
                    };

                    Add(notification);
                }
            }
        }
    }
}
