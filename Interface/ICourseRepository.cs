using Domain;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface ICourseRepository
    {
        List<Course> GetAll();
        List<Course> GetByCategory(MoodType category);
        Course GetById(int id);
        int Add(Course course);
        bool Update(Course course);
        bool Delete(int id);
    }
}
