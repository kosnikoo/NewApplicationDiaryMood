using Domain;
using Domain.Enum;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer
{
    public class SqlServerMoodRepository : IMoodRepository
    {
        private readonly AppDbContext _context;
        private readonly Random _random = new Random();

        public SqlServerMoodRepository(AppDbContext context)
        {
            _context = context;
            NewSeedData();
        }

        public void NewSeedData()
        {
            var moodTypes = new[] {
                MoodType.Happy, MoodType.Sad, MoodType.Angry, MoodType.Tired
            };

            var moods = new List<Mood>();

            for (int i = 0; i < 50; i++)
            {
                var mood = new Mood
                {
                    MoodType = moodTypes[_random.Next(moodTypes.Length)],
                    MoodQuantity = _random.Next(1, 10),
                    Date = DateTime.Now.AddDays(-_random.Next(0, 180))
                };
                moods.Add(mood);
            }

            _context.Moods.AddRange(moods);
            _context.SaveChanges();
        }

        public List<Mood> GetAll(MoodFilter filter)
        {
            var query = _context.Moods.AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(r => r.Date >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(r => r.Date <= filter.EndDate.Value);

            if (filter.MoodType.HasValue)
                query = query.Where(r => r.MoodType == filter.MoodType.Value);

            return query.OrderBy(r => r.Date).ToList();
        }

        public Mood GetById(int id)
        {
            return _context.Moods.FirstOrDefault(m => m.ID == id);
        }

        public int Add(Mood mood)
        {
            _context.Moods.Add(mood);
            _context.SaveChanges();
            return mood.ID;
        }

        public bool Update(Mood mood)
        {
            var existing = _context.Moods.FirstOrDefault(m => m.ID == mood.ID);
            if (existing == null)
                return false;

            existing.MoodType = mood.MoodType;
            existing.MoodQuantity = mood.MoodQuantity;
            existing.Date = mood.Date;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var mood = _context.Moods.FirstOrDefault(m => m.ID == id);
            if (mood == null)
                return false;

            _context.Moods.Remove(mood);
            _context.SaveChanges();
            return true;
        }

        public List<Mood> GetByDate(DateTime date)
        {
            return _context.Moods
                .Where(m => m.Date.Date == date.Date)
                .OrderBy(m => m.Date)
                .ToList();
        }

        public List<Mood> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Moods
                .Where(m => m.Date >= startDate && m.Date <= endDate)
                .OrderBy(m => m.Date)
                .ToList();
        }

        public int Count()
        {
            return _context.Moods.Count();
        }

        public List<Mood> GetAll()
        {
            return _context.Moods.OrderBy(m => m.Date).ToList();
        }
    }
}
