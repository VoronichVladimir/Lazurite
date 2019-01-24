﻿using LazuriteUI.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches.SwitchSettings
{
    /// <summary>
    /// Логика взаимодействия для SwitchSettings.xaml
    /// </summary>
    public partial class SwitchSetting : UserControl
    {
        public SwitchSetting()
        {
            InitializeComponent();
        }
        
        private void ItemView1_Click(object sender, RoutedEventArgs e)
        {
            StuckUILoadingWindow.Show("Загрузка иконок...", 
                () =>
                {
                    var switchIconSelect = new SwitchIconSelect(((ScenarioModel)DataContext), false);
                    var dialog = new DialogView(switchIconSelect);
                    dialog.Caption = "Выберите иконку, которая будет отображаться над переключателем. Для поиска нужной иконки начните вводить текст в поле ввода.";
                    switchIconSelect.OkClick += (o, args) => dialog.Close();
                    dialog.Show();
                }
            );
        }

        private void ItemView2_Click(object sender, RoutedEventArgs e)
        {
            StuckUILoadingWindow.Show("Загрузка иконок...",
                () =>
                {
                    var switchIconSelect = new SwitchIconSelect(((ScenarioModel)DataContext), true);
                    var dialog = new DialogView(switchIconSelect);
                    dialog.Caption = "Выберите иконку, которая будет отображаться над переключателем. Для поиска нужной иконки начните вводить текст в поле ввода.";
                    switchIconSelect.OkClick += (o, args) => dialog.Close();
                    dialog.Show();
                }
            );
        }
    }
}
