using Domain;
using Domain.Enum;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly AppDbContext _context;

        public ExerciseRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Exercise> GetAll()
        {
            return _context.Exercises.ToList();
        }

        public List<Exercise> GetByMoodType(MoodType moodType)
        {
            return _context.Exercises
                .Where(e => e.ForMoodType == moodType)
                .ToList();
        }

        public Exercise? GetById(int id)
        {
            return _context.Exercises.Find(id);
        }

        public int Add(Exercise exercise)
        {
            _context.Exercises.Add(exercise);
            _context.SaveChanges();
            return exercise.Id;
        }

        public bool Update(Exercise exercise)
        {
            var existing = _context.Exercises.Find(exercise.Id);
            if (existing == null)
                return false;

            existing.CopyFrom(exercise);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var exercise = _context.Exercises.Find(id);
            if (exercise == null)
                return false;

            _context.Exercises.Remove(exercise);
            _context.SaveChanges();
            return true;
        }
    }
}
