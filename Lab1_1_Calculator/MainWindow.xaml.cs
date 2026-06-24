using System;
using System.Windows;
using System.Windows.Controls;

namespace Lab1_1_Calculator
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(ABox.Text, out double a) || !double.TryParse(BBox.Text, out double b))
            {
                ResultText.Text = "Ошибка: введите два числа.";
                return;
            }

            string operation = ((Button)sender).Content.ToString() ?? "+";
            double result;
            if (operation == "+") result = a + b;
            else if (operation == "-") result = a - b;
            else if (operation == "*") result = a * b;
            else
            {
                if (Math.Abs(b) < 0.0000001)
                {
                    ResultText.Text = "Ошибка: деление на ноль невозможно.";
                    return;
                }
                result = a / b;
            }
            ResultText.Text = $"Результат: {a} {operation} {b} = {result}";
        }
    }
}
