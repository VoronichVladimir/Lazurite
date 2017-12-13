﻿using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class DateTimeView : UserControl
    {
        public DateTimeView()
        {
            InitializeComponent();
        }

        public DateTimeView(ScenarioBase scenario, UserVisualSettings visualSettings): this()
        {
            this.DataContext = new ScenarioModel(scenario, visualSettings);
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            var dateTime = DateTime.Now;
            DateTime.TryParse(((ScenarioModel)this.DataContext).ScenarioValue, out dateTime);
            var dateTimeSwitch = new DateTimeViewSwitch() {
                DateTime = dateTime
            };
            var dialog = new DialogView(dateTimeSwitch);
            dateTimeSwitch.Apply += (o, args) => {
                dialog.Close();
                ((ScenarioModel)this.DataContext).ScenarioValue = dateTimeSwitch.DateTime.ToString();
            };
            dialog.Show();
        }
    }
}
