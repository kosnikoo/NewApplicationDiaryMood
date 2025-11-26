using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Statistics
{
    public record MonthStatistic
    {
        public required int Year { get; set; }
        public required int Month { get; set; }
        public required int Count { get; set; }
        public string GetMonthName()
        {
            var date = new DateTime(Year, Month, 1);
            return date.ToString("MMM yyyy");
        }
    }
}
