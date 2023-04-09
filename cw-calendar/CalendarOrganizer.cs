using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cw_calendar
{
    public class CalendarOrganizer : INotifyPropertyChanged
    {
        private readonly MainWindow Window;

        public CalendarOrganizer(MainWindow window)
        {
            Window = window;

            window.NewEventButton.Click += NewEventButton_Click;
            window.DeleteEventButton.Click += DeleteEventButton_Click;

            window.EventsListView.SelectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(CanDeleteEvent));
            };
        }

        // Define a dictionary to store the events for each date
        private readonly Dictionary<DateTime, ObservableCollection<CalendarEvent>> eventsByDate = new();

        private DateTime? selectedDate;
        public DateTime? SelectedDate
        {
            get => selectedDate;
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;

                    if (value is DateTime date && !eventsByDate.ContainsKey(date))
                    {
                        var collection = new ObservableCollection<CalendarEvent>();

                        collection.CollectionChanged += (_, args) =>
                        {
                            if (args.NewItems?.Count != 0)
                            {
                                OnPropertyChanged(nameof(NoEventsTextVisible));
                            }
                        };

                        eventsByDate[date] = collection;
                    }

                    // Display the list of events for the selected date in the ListBox

                    OnPropertyChanged(nameof(SelectedDate));
                    OnPropertyChanged(nameof(HasDateSelected));
                    OnPropertyChanged(nameof(SelectedDateEvents));
                    OnPropertyChanged(nameof(NoEventsTextVisible));
                    OnPropertyChanged(nameof(CanDeleteEvent));
                }
            }
        }

        public bool HasDateSelected => SelectedDate.HasValue;

        public ObservableCollection<CalendarEvent>? SelectedDateEvents
        {
            get
            {
                if (SelectedDate is not DateTime date)
                    return null;

                return eventsByDate[date];
            }
        }

        public Visibility NoEventsTextVisible
        {
            get
            {
                if (SelectedDate is not DateTime date || eventsByDate[date].Count != 0)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
        }

        public bool CanDeleteEvent
        {
            get
            {
                return HasDateSelected && Window.EventsListView.SelectedItem != null;
            }
        }

        private void NewEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDate is not DateTime date)
                return;

            // Prompt the user to enter an event for the selected date
            string eventName = Microsoft.VisualBasic.Interaction.InputBox("Enter an event for " + date.ToShortDateString(), "New Event");

            // Add the event to the list for the selected date if the user entered a name
            if (!string.IsNullOrEmpty(eventName))
            {
                eventsByDate[date].Add(new CalendarEvent(eventName, DateTime.Now));
            }
        }

        private void DeleteEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDate is not DateTime date)
                return;

            if (Window.EventsListView.SelectedItem is CalendarEvent evt)
            {
                eventsByDate[date]?.Remove(evt);
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
