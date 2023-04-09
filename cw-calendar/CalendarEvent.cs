using System;

namespace cw_calendar
{
    [Serializable]
    public class CalendarEvent
    {
        public string Title { get; }

        public DateTime StartTime { get; }

        public string? Description { get; }

        public CalendarEvent(string title, DateTime startTime, string? description)
        {
            Title = title;
            StartTime = startTime;
            Description = description;
        }
    }
}
