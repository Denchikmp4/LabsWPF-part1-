using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace Lab1_5_FileMenu
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*" };
            if (dlg.ShowDialog() == true)
            {
                TextList.Items.Clear();
                foreach (var line in File.ReadAllLines(dlg.FileName)) TextList.Items.Add(line);
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*" };
            if (dlg.ShowDialog() == true)
            {
                using var writer = new StreamWriter(dlg.FileName);
                foreach (var item in TextList.Items) writer.WriteLine(item.ToString());
            }
        }
    }
}
