using Domain;
using Domain.Statistics;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class StaticsService
    {
        private readonly IMoodRepository _moodRepository;
        public StaticsService(IMoodRepository moodRepository)
        {
            _moodRepository = moodRepository;
        }

        public List<MonthStatistic> GetMoodByMonth(MoodFilter filter)
        {
            var moods = _moodRepository.GetAll(filter);
            return moods
            .GroupBy(x => new { x.Date.Year, x.Date.Month })
            .Select(g => new MonthStatistic
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();
        }
        public List<MoodStatistic> GetByMood(MoodFilter filter)
        {
            var moods = _moodRepository.GetAll(filter);
            return moods
            .Where(x => x.MoodType != null)
            .GroupBy(x => x.MoodType)
            .Select(g => new MoodStatistic
            {
                MoodName = g.Key.ToString(),
                Count = g.Count()
            })
            .OrderByDescending(s => s.Count)
            .ToList();
        }
    }
}
