using Domain;
using Domain.Enum;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemory
{
    public class MoodRepository : IMoodRepository
    {
        private readonly List<Mood> _moods = new();

        public MoodRepository()
        {
            NewSeedData();
        }

        public void NewSeedData()
        {
            var random = new Random();
            var moodTypes = new[] {
                MoodType.Happy, MoodType.Sad, MoodType.Angry, MoodType.Tired,
                MoodType.Anxious, MoodType.Calm, MoodType.Energized,
                MoodType.Depressed, MoodType.Stressed, MoodType.Content
            };

            for (int i = 0; i < 50; i++)
            {
                var mood = new Mood
                {
                    Id = i + 1,
                    UserId = 1, // Тестовый пользователь
                    MoodType = moodTypes[random.Next(moodTypes.Length)],
                    MoodQuantity = random.Next(1, 10),
                    Date = DateTime.Now.AddDays(-random.Next(0, 180))
                };
                _moods.Add(mood);
            }
        }

        public List<Mood> GetAll(MoodFilter filter)
        {
            var result = _moods.AsEnumerable();
            if (filter.StartDate.HasValue)
                result = result.Where(r => r.Date >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                result = result.Where(r => r.Date <= filter.EndDate.Value);
            return result.ToList();
        }

        public List<Mood> GetByUserId(int userId, MoodFilter filter)
        {
            var result = _moods.Where(m => m.UserId == userId).AsEnumerable();
            if (filter.StartDate.HasValue)
                result = result.Where(r => r.Date >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                result = result.Where(r => r.Date <= filter.EndDate.Value);
            return result.ToList();
        }

        public Mood? GetById(int id)
        {
            return _moods.FirstOrDefault(m => m.Id == id);
        }

        public int Add(Mood mood)
        {
            mood.Id = _moods.Count > 0 ? _moods.Max(m => m.Id) + 1 : 1;
            _moods.Add(mood);
            return mood.Id;
        }

        public bool Update(Mood mood)
        {
            var existing = GetById(mood.Id);
            if (existing == null)
                return false;

            existing.CopyFrom(mood);
            return true;
        }

        public bool Delete(int id)
        {
            var mood = GetById(id);
            if (mood == null)
                return false;

            return _moods.Remove(mood);
        }

        public Dictionary<MoodType, int> GetMoodStatistics(int userId, MoodFilter filter)
        {
            var moods = GetByUserId(userId, filter);
            return moods
                .GroupBy(m => m.MoodType)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
