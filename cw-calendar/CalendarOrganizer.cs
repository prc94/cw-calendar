using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace cw_calendar
{
    public class CalendarOrganizer : INotifyPropertyChanged
    {
        private readonly MainWindow Window;

        public CalendarOrganizer(MainWindow window)
        {
            Window = window;

            window.Loaded += (_, _) => {
                Load();
                Window.Calendar.SelectedDate = DateTime.Today;
            };
        }

        public readonly Dictionary<DateTime, ObservableCollection<CalendarEvent>> Events = new();

        private DateTime? selectedDate;
        public DateTime? SelectedDate
        {
            get => selectedDate;
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;

                    if (value is DateTime date && !Events.ContainsKey(date))
                    {

                        Events[date] = NewEventsCollection();
                    }

                    OnPropertyChanged(nameof(SelectedDate));
                    OnPropertyChanged(nameof(HasDateSelected));
                    OnPropertyChanged(nameof(SelectedDateEvents));
                    OnPropertyChanged(nameof(NoEventsTextVisible));
                    OnPropertyChanged(nameof(CanDeleteEvent));
                }
            }
        }

        private ObservableCollection<CalendarEvent> NewEventsCollection(IEnumerable<CalendarEvent>? events = null)
        {
            var collection = new ObservableCollection<CalendarEvent>();

            if (events != null)
                foreach (CalendarEvent e in events)
                    collection.Add(e);

            collection.CollectionChanged += (_, args) =>
            {
                if (args.NewItems?.Count != 0)
                {
                    OnPropertyChanged(nameof(NoEventsTextVisible));
                }
            };

            return collection;
        }

        public bool HasDateSelected => SelectedDate.HasValue;

        public ObservableCollection<CalendarEvent>? SelectedDateEvents
        {
            get
            {
                if (SelectedDate is not DateTime date)
                    return null;

                return Events[date];
            }
        }

        public Visibility NoEventsTextVisible
        {
            get
            {
                if (SelectedDate is not DateTime date || Events[date].Count != 0)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
        }

        private CalendarEvent? selectedEvent;
        public CalendarEvent? SelectedEvent
        {
            get
            {
                return selectedEvent;
            }
            set
            {
                selectedEvent = value;
                OnPropertyChanged(nameof(SelectedEvent));
                OnPropertyChanged(nameof(CanDeleteEvent));
                OnPropertyChanged(nameof(EventDescriptionBoxVisible));
            }
        }

        public Visibility EventDescriptionBoxVisible
        {
            get
            {
                if (SelectedEvent is CalendarEvent e && !string.IsNullOrEmpty(e.Description))
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        public bool CanDeleteEvent
        {
            get
            {
                return HasDateSelected && SelectedEvent != null;
            }
        }

        private const string SAVE_FILE_NAME = "calendar.json";

        public void Save()
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new(SAVE_FILE_NAME))
            using (JsonTextWriter writer = new(sw))
                serializer.Serialize(writer, Events);
        }

        private void Load()
        {
            if (File.Exists(SAVE_FILE_NAME)) {
                JsonSerializer serializer = new();

                using (StreamReader sr = new(SAVE_FILE_NAME))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var EventsByDate = serializer.Deserialize<IDictionary<DateTime, IEnumerable<CalendarEvent>>>(reader);
                    if (EventsByDate != null)
                        foreach (var entry in EventsByDate)
                            Events[entry.Key] = NewEventsCollection(entry.Value);
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
