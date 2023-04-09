using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw_calendar
{
    public class CalendarEvent
    {
        public string Title { get; }

        public DateTime StartTime { get; }

        public CalendarEvent(string title, DateTime startTime)
        {
            Title = title;
            StartTime = startTime;
        }

    }
}
