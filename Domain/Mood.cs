using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Mood
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public MoodType MoodType { get; set; } = MoodType.None;
        public int MoodQuantity { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;

        public void CopyFrom(Mood other)
        {
            UserId = other.UserId;
            MoodType = other.MoodType;
            MoodQuantity = other.MoodQuantity;
            EntryDate = other.EntryDate;
        }
    }
}
