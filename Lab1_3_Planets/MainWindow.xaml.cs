using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Lab1_3_Planets
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, string> info = new()
        {
            ["Меркурий"] = "Ближайшая к Солнцу планета. Имеет малые размеры и почти не имеет атмосферы.",
            ["Венера"] = "Вторая планета от Солнца. Известна плотной атмосферой и высоким парниковым эффектом.",
            ["Земля"] = "Третья планета от Солнца. Единственная известная планета с развитой биосферой.",
            ["Марс"] = "Четвертая планета от Солнца. Имеет красноватую поверхность и полярные шапки.",
            ["Юпитер"] = "Крупнейшая планета Солнечной системы, газовый гигант с Большим красным пятном.",
            ["Сатурн"] = "Газовый гигант, известный развитой системой колец.",
            ["Уран"] = "Ледяной гигант, ось вращения которого сильно наклонена к плоскости орбиты.",
            ["Нептун"] = "Самая дальняя из восьми планет Солнечной системы, отличается сильными ветрами."
        };
        public MainWindow()
        {
            InitializeComponent();
            foreach (var planet in info.Keys) PlanetsList.Items.Add(planet);
        }
        private void PlanetsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlanetsList.SelectedItem is string planet) InfoText.Text = info[planet];
        }
    }
}
