using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public MoodType ForMoodType { get; set; }
        public string Instructions { get; set; }
        public int DurationMinutes { get; set; }

        public bool IsAuthor { get; set; } = false;

        public void CopyFrom(Exercise other)
        {
            Title = other.Title;
            Description = other.Description;
            ForMoodType = other.ForMoodType;
            Instructions = other.Instructions;
            DurationMinutes = other.DurationMinutes;
            IsAuthor = other.IsAuthor;
        }
    }
}
