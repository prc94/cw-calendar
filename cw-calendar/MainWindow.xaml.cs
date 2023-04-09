using System.Windows;

namespace cw_calendar
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new CalendarOrganizer(this);
        }
    }
}
