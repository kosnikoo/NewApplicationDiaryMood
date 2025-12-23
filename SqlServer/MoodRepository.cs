using Domain;
using Domain.Enum;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlServer
{
    public class MoodRepository : IMoodRepository
    {
        private readonly AppDbContext _context;

        public MoodRepository(AppDbContext context)
        {
            _context = context;
        }

        public int Add(Mood mood)
        {
            var existingMood = _context.Moods
                .Where(m => m.UserId == mood.UserId &&
                           m.EntryDate.Date == mood.EntryDate.Date &&
                           m.MoodType == mood.MoodType)
                .FirstOrDefault();

            int resultId;

            if (existingMood != null)
            {
                existingMood.MoodQuantity = mood.MoodQuantity;
                existingMood.EntryDate = mood.EntryDate;
                _context.SaveChanges();
                resultId = existingMood.Id;
            }
            else
            {
                _context.Moods.Add(mood);
                _context.SaveChanges();
                resultId = mood.Id;
            }
            CheckFor20MoodsAchievement(mood.UserId, mood.MoodType);

            return resultId;
        }

        private void CheckFor20MoodsAchievement(int userId, MoodType moodType)
        {
            try
            {
                var count = _context.Moods
                    .Count(m => m.UserId == userId && m.MoodType == moodType);
                if (count > 0 && count % 20 == 0)
                {
                    var course = _context.Courses
                        .Where(c => c.Category == moodType)
                        .FirstOrDefault();
                    var existingNotification = _context.Notifications
                        .Where(n => n.UserId == userId &&
                                   n.NotTitle == "🎯 Достижение!" &&
                                   n.NotDescription.Contains($"{count} настроений типа {moodType}") &&
                                   n.NotDate.Date == DateTime.Today)
                        .FirstOrDefault();

                    if (existingNotification == null && course != null)
                    {
                        var notification = new Notification
                        {
                            UserId = userId,
                            NotTitle = "🎯 Достижение!",
                            NotDescription = $"Поздравляем! Вы добавили {count} настроений типа {moodType}",
                            NotMoodStatistic = $"{moodType}: {count} раз",
                            NotRecommendation = $"Рекомендуем пройти курс: '{course.Title}' для углубленной работы с этим эмоциональным состоянием. Перейдите в раздел 'Курсы' для подробностей.",
                            NotDate = DateTime.Now,
                            IsRead = false
                        };

                        _context.Notifications.Add(notification);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке достижения: {ex.Message}");
            }
        }

        public Mood? GetById(int id)
        {
            return _context.Moods.Find(id);
        }

        public List<Mood> GetAll(MoodFilter filter)
        {
            var query = _context.Moods.AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(x => x.EntryDate >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                query = query.Where(x => x.EntryDate <= filter.EndDate.Value);

            return query.ToList();
        }

        public List<Mood> GetByUserId(int userId, MoodFilter filter)
        {
            var query = _context.Moods.Where(m => m.UserId == userId);

            if (filter.StartDate.HasValue)
                query = query.Where(x => x.EntryDate >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                query = query.Where(x => x.EntryDate <= filter.EndDate.Value);

            return query.ToList();
        }

        public bool Update(Mood mood)
        {
            var existing = _context.Moods.Find(mood.Id);
            if (existing == null)
                return false;

            existing.CopyFrom(mood);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var mood = _context.Moods.Find(id);
            if (mood == null)
                return false;

            _context.Moods.Remove(mood);
            _context.SaveChanges();
            return true;
        }

        public Dictionary<MoodType, int> GetMoodStatistics(int userId, MoodFilter filter)
        {
            var moods = GetByUserId(userId, filter);
            return moods
                .GroupBy(m => m.MoodType)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        public Mood? GetByUserIdAndDate(int userId, DateTime date)
        {
            return _context.Moods
                .Where(m => m.UserId == userId && m.EntryDate.Date == date.Date)
                .FirstOrDefault();
        }

        public Mood? GetLastMood(int userId)
        {
            return _context.Moods
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.EntryDate)
                .FirstOrDefault();
        }

        public int GetMoodCountLastDays(int userId, int days)
        {
            var startDate = DateTime.Now.AddDays(-days);
            return _context.Moods
                .Count(m => m.UserId == userId && m.EntryDate >= startDate);
        }

        public Dictionary<DayOfWeek, int> GetMoodStatisticsByDayOfWeek(int userId, MoodFilter filter)
        {
            var moods = GetByUserId(userId, filter);
            return moods
                .GroupBy(m => m.EntryDate.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public double GetAverageMoodIntensity(int userId, MoodFilter filter)
        {
            var moods = GetByUserId(userId, filter);
            if (!moods.Any())
                return 0;

            return Math.Round(moods.Average(m => m.MoodQuantity), 1);
        }

        public (MoodType Mood, int Count)? GetMostFrequentMood(int userId, MoodFilter filter)
        {
            var stats = GetMoodStatistics(userId, filter);
            if (!stats.Any())
                return null;

            var mostFrequent = stats.OrderByDescending(kv => kv.Value).First();
            return (mostFrequent.Key, mostFrequent.Value);
        }

        public List<(int Year, int Month, int Count)> GetMonthlyStatistics(int userId, MoodFilter filter)
        {
            var moods = GetByUserId(userId, filter);

            return moods
                .GroupBy(m => new { m.EntryDate.Year, m.EntryDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .Select(x => (x.Year, x.Month, x.Count))
                .ToList();
        }

        public bool HasMoodToday(int userId)
        {
            return _context.Moods
                .Any(m => m.UserId == userId && m.EntryDate.Date == DateTime.Today);
        }

        public int GetCurrentStreak(int userId)
        {
            var dates = _context.Moods
                .Where(m => m.UserId == userId)
                .Select(m => m.EntryDate.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            if (!dates.Any())
                return 0;

            var streak = 0;
            var currentDate = DateTime.Today;

            foreach (var date in dates)
            {
                if (date == currentDate)
                {
                    streak++;
                    currentDate = currentDate.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return streak;
        }

        public int GetBestStreak(int userId)
        {
            var dates = _context.Moods
                .Where(m => m.UserId == userId)
                .Select(m => m.EntryDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (!dates.Any())
                return 0;

            int bestStreak = 0;
            int currentStreak = 1;
            var previousDate = dates.First();

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - previousDate).Days == 1)
                {
                    currentStreak++;
                }
                else
                {
                    bestStreak = Math.Max(bestStreak, currentStreak);
                    currentStreak = 1;
                }
                previousDate = dates[i];
            }

            return Math.Max(bestStreak, currentStreak);
        }

        public List<DateTime> GetDaysWithGoodMood(int userId, int threshold = 7, MoodFilter filter = null)
        {
            var query = _context.Moods.Where(m => m.UserId == userId && m.MoodQuantity >= threshold);

            if (filter != null)
            {
                if (filter.StartDate.HasValue)
                    query = query.Where(x => x.EntryDate >= filter.StartDate.Value);
                if (filter.EndDate.HasValue)
                    query = query.Where(x => x.EntryDate <= filter.EndDate.Value);
            }

            return query
                .Select(m => m.EntryDate.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();
        }

        public bool DeleteAllUserMoods(int userId)
        {
            try
            {
                var moods = _context.Moods.Where(m => m.UserId == userId);
                _context.Moods.RemoveRange(moods);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Mood> ExportUserMoods(int userId)
        {
            return _context.Moods
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.EntryDate)
                .ToList();
        }
        public bool HasSameMoodTypeOnDate(int userId, MoodType moodType, DateTime date)
        {
            return _context.Moods
                .Any(m => m.UserId == userId &&
                         m.MoodType == moodType &&
                         m.EntryDate.Date == date.Date);
        }
    }
}