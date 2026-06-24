using System;
using System.Windows;
using System.Windows.Controls;

namespace Lab1_4_DateAge
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            for (int y = DateTime.Now.Year; y >= 1900; y--) YearBox.Items.Add(y);
            for (int m = 1; m <= 12; m++) MonthBox.Items.Add(m);
        }

        private void YearMonth_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (YearBox.SelectedItem == null || MonthBox.SelectedItem == null) return;
            int year = (int)YearBox.SelectedItem;
            int month = (int)MonthBox.SelectedItem;
            int days = DateTime.DaysInMonth(year, month);
            DayBox.Items.Clear();
            for (int d = 1; d <= days; d++) DayBox.Items.Add(d);
            DayBox.IsEnabled = true;
        }

        private void DayBox_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (YearBox.SelectedItem == null || MonthBox.SelectedItem == null || DayBox.SelectedItem == null) return;
            var start = new DateTime((int)YearBox.SelectedItem, (int)MonthBox.SelectedItem, (int)DayBox.SelectedItem);
            var now = DateTime.Today;
            if (start > now) { ResultText.Text = "Выбранная дата еще не наступила."; return; }
            int years = now.Year - start.Year;
            int months = now.Month - start.Month;
            int days = now.Day - start.Day;
            if (days < 0) { months--; days += DateTime.DaysInMonth(now.AddMonths(-1).Year, now.AddMonths(-1).Month); }
            if (months < 0) { years--; months += 12; }
            ResultText.Text = $"С выбранной даты прошло: {years} лет, {months} месяцев, {days} дней.";
        }
    }
}
