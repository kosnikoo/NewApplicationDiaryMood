using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Statistics
{
    public record MoodStatistic
    {
        public required string MoodName { get; set; }
        public required int Count { get; set; }
    }
}
