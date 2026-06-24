using System.Windows;

namespace Lab1_2_ListAdder
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputBox.Text))
            {
                ItemsList.Items.Add(InputBox.Text.Trim());
                InputBox.Clear();
                InputBox.Focus();
            }
        }
    }
}
