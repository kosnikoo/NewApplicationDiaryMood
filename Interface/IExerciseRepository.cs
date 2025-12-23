using Domain.Enum;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IExerciseRepository
    {
        List<Exercise> GetAll();
        List<Exercise> GetByMoodType(MoodType moodType);
        Exercise? GetById(int id);
        int Add(Exercise exercise);
        bool Update(Exercise exercise);
        bool Delete(int id);
    }
}
