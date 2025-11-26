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
        public readonly List<Mood> _mood = new();
        public void NewSeedData()
        {
            var random = new Random();
            var moodTypes = new[] {
            MoodType.Happy, MoodType.Sad, MoodType.Angry, MoodType.Tired
        };
            for (int i = 0; i < 50; i++)
            {
                var mood = new Mood
                {
                    ID = i + 1,
                    MoodType = moodTypes[random.Next(moodTypes.Length)],
                    MoodQuantity = random.Next(1, 10),
                    Date = DateTime.Now.AddDays(-random.Next(0, 180))
                };
                _mood.Add(mood);
            }
        }
        public List<Mood> GetAll(MoodFilter filter)
        {
            var result = _mood.AsEnumerable();
            if (filter.StartDate.HasValue)
                result = result.Where(r => r.Date >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                result = result.Where(r => r.Date <= filter.EndDate.Value);
            return result.ToList();
        }
       
    }
}
