using System;
using System.Windows;

namespace cw_calendar
{
    /// <summary>
    /// Логика взаимодействия для AddEventDialog.xaml
    /// </summary>
    public partial class AddEventDialog : Window
    {
        private readonly Action<CalendarEvent> Callback;

        public AddEventDialog(Action<CalendarEvent> callback)
        {
            Callback = callback;
            InitializeComponent();
            timePicker.Value = DateTime.Now;

            var updateAddButton = () =>
            {
                if (string.IsNullOrEmpty(titleTextBox.Text) || timePicker.Value == null)
                    addButton.IsEnabled = false;
                else
                    addButton.IsEnabled = true;
            };

            titleTextBox.TextChanged += (_, _) =>
            {
                updateAddButton();
            };

            timePicker.ValueChanged += (_, _) =>
            {
                updateAddButton();
            };
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(titleTextBox.Text) && timePicker.Value is DateTime time)
            {
                var newEvent = new CalendarEvent(titleTextBox.Text, time, descriptionTextBox.Text);

                Callback(newEvent);

                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
