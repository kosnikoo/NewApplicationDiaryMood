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

        public List<MonthStatistic> GetMoodByMonth(int userId, MoodFilter filter)
        {
            var moods = _moodRepository.GetByUserId(userId, filter);
            return moods
                .GroupBy(x => new { x.EntryDate.Year, x.EntryDate.Month })
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

        public List<MoodStatistic> GetByMood(int userId, MoodFilter filter)
        {
            var moods = _moodRepository.GetByUserId(userId, filter);
            return moods
                .Where(x => x.MoodType != Domain.Enum.MoodType.None)
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
