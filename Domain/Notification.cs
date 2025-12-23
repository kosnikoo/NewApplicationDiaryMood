using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string NotTitle { get; set; }
        public string NotDescription { get; set; }
        public string NotMoodStatistic { get; set; }
        public DateTime NotDate { get; set; } = DateTime.Now;
        public string NotRecommendation { get; set; }

        public bool IsRead { get; set; } = false;

        public void CopyFrom(Notification other)
        {
            UserId = other.UserId;
            NotTitle = other.NotTitle;
            NotDescription = other.NotDescription;
            NotMoodStatistic = other.NotMoodStatistic;
            NotDate = other.NotDate;
            NotRecommendation = other.NotRecommendation;
            IsRead = other.IsRead;
        }
    }
}
