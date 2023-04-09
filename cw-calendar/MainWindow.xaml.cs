using System;
using System.Windows;

namespace cw_calendar
{
    public partial class MainWindow : Window
    {
        private readonly CalendarOrganizer Backend;

        public MainWindow()
        {
            InitializeComponent();

            Backend = new CalendarOrganizer(this);

            DataContext = Backend;
        }

        private void NewEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (Backend.SelectedDate is not DateTime date)
                return;

            new AddEventDialog((e) => { Backend.Events[date].Add(e); }).ShowDialog();
        }

        private void DeleteEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (Backend.SelectedDate is not DateTime date)
                return;

            if (EventsListView.SelectedItem is CalendarEvent evt)
            {
                Backend.Events[date]?.Remove(evt);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Backend.Save();
        }
    }
}
