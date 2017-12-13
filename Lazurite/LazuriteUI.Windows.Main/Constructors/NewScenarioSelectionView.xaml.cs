﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для NewScenarioSelectionView.xaml
    /// </summary>
    public partial class NewScenarioSelectionView : UserControl
    {
        public NewScenarioSelectionView()
        {
            InitializeComponent();
        }

        private void btSingleActionScenario_Click(object sender, RoutedEventArgs e)
        {
            SingleActionScenario?.Invoke();
        }

        private void btComplexScenario_Click(object sender, RoutedEventArgs e)
        {
            CompositeScenario?.Invoke();
        }

        private void btRemoteScenario_Click(object sender, RoutedEventArgs e)
        {
            RemoteScenario?.Invoke();
        }

        public event Action SingleActionScenario;
        public event Action CompositeScenario;
        public event Action RemoteScenario;
    }
}
