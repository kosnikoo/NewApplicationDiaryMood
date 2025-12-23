using Domain.Enum;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InMemory
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly List<Exercise> _exercises = new();

        public ExerciseRepository()
        {
            // Тестовые упражнения
            _exercises.AddRange(new[]
            {
                new Exercise
                {
                    Id = 1,
                    Title = "Дыхательная техника 4-7-8",
                    Description = "Техника для снятия напряжения",
                    ForMoodType = MoodType.Anxious,
                    Instructions = "1. Вдох через нос 4 секунды\n2. Задержите дыхание 7 секунд\n3. Выдох через рот 8 секунд\nПовторите 5 раз",
                    DurationMinutes = 5
                },
                new Exercise
                {
                    Id = 2,
                    Title = "Прогрессивная мышечная релаксация",
                    Description = "Техника для расслабления тела",
                    ForMoodType = MoodType.Tired,
                    Instructions = "Поочередно напрягайте и расслабляйте группы мышц от пальцев ног до лица",
                    DurationMinutes = 10
                },
                new Exercise
                {
                    Id = 3,
                    Title = "Техника благодарности",
                    Description = "Упражнение для повышения позитивных эмоций",
                    ForMoodType = MoodType.Sad,
                    Instructions = "Запишите 3 вещи, за которые вы благодарны сегодня",
                    DurationMinutes = 5
                }
            });
        }

        public List<Exercise> GetAll()
        {
            return _exercises;
        }

        public List<Exercise> GetByMoodType(MoodType moodType)
        {
            return _exercises.Where(e => e.ForMoodType == moodType).ToList();
        }

        public Exercise? GetById(int id)
        {
            return _exercises.FirstOrDefault(e => e.Id == id);
        }

        public int Add(Exercise exercise)
        {
            exercise.Id = _exercises.Count > 0 ? _exercises.Max(e => e.Id) + 1 : 1;
            _exercises.Add(exercise);
            return exercise.Id;
        }

        public bool Update(Exercise exercise)
        {
            var existing = GetById(exercise.Id);
            if (existing == null)
                return false;

            existing.CopyFrom(exercise);
            return true;
        }

        public bool Delete(int id)
        {
            var exercise = GetById(id);
            if (exercise == null)
                return false;

            return _exercises.Remove(exercise);
        }
    }
}
