using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Subscription
    {
        public int Id { get; set; }
        public string SubStatus { get; set; }
        public string SubPrice { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public int UserId { get; set; }

        public void CopyFrom(Subscription other)
        {
            SubStatus = other.SubStatus;
            SubPrice = other.SubPrice;
            StartDate = other.StartDate;
            EndDate = other.EndDate;
            UserId = other.UserId;
        }
    }
}
