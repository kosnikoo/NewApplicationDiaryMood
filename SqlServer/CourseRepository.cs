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
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Course> GetAll()
        {
            return _context.Courses.ToList();
        }

        public List<Course> GetByCategory(MoodType category)
        {
            return _context.Courses
                .Where(c => c.Category == category)
                .ToList();
        }

        public Course? GetById(int id)
        {
            return _context.Courses.Find(id);
        }

        public int Add(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
            return course.Id;
        }

        public bool Update(Course course)
        {
            var existing = _context.Courses.Find(course.Id);
            if (existing == null)
                return false;

            existing.CopyFrom(course);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
                return false;

            _context.Courses.Remove(course);
            _context.SaveChanges();
            return true;
        }
    }
}
