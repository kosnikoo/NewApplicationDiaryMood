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
        public int ID { get; set; }
        public MoodType MoodType { get; set; } = MoodType.None;
        public int MoodQuantity { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
