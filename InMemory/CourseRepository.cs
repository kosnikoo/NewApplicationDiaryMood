using Domain;
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
    public class CourseRepository : ICourseRepository
    {
        private readonly List<Course> _courses = new();

        public CourseRepository()
        {
            // Тестовые курсы
            _courses.AddRange(new[]
            {
                new Course
                {
                    Id = 1,
                    Title = "Курс медитации для снятия стресса",
                    Description = "Научитесь техникам медитации для снижения уровня стресса",
                    Category = MoodType.Stressed,
                    Link = "https://example.com/meditation"
                },
                new Course
                {
                    Id = 2,
                    Title = "Курс позитивного мышления",
                    Description = "Развивайте позитивное мышление для улучшения настроения",
                    Category = MoodType.Depressed,
                    Link = "https://example.com/positive"
                },
                new Course
                {
                    Id = 3,
                    Title = "Курс управления гневом",
                    Description = "Техники контроля эмоций и управления гневом",
                    Category = MoodType.Angry,
                    Link = "https://example.com/anger"
                }
            });
        }

        public List<Course> GetAll()
        {
            return _courses;
        }

        public List<Course> GetByCategory(MoodType category)
        {
            return _courses.Where(c => c.Category == category).ToList();
        }

        public Course? GetById(int id)
        {
            return _courses.FirstOrDefault(c => c.Id == id);
        }

        public int Add(Course course)
        {
            course.Id = _courses.Count > 0 ? _courses.Max(c => c.Id) + 1 : 1;
            _courses.Add(course);
            return course.Id;
        }

        public bool Update(Course course)
        {
            var existing = GetById(course.Id);
            if (existing == null)
                return false;

            existing.CopyFrom(course);
            return true;
        }

        public bool Delete(int id)
        {
            var course = GetById(id);
            if (course == null)
                return false;

            return _courses.Remove(course);
        }
    }
}
