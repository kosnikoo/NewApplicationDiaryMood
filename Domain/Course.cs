using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public MoodType Category { get; set; }
        public string Link { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int DurationHours { get; set; }

        public void CopyFrom(Course other)
        {
            Title = other.Title;
            Description = other.Description;
            Category = other.Category;
            Link = other.Link;
            DurationHours = other.DurationHours;
        }
    }
}
