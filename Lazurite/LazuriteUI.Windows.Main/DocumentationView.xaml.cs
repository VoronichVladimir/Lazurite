﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для DocumentationView.xaml
    /// </summary>
    public partial class DocumentationView : Grid
    {
        public DocumentationView()
        {
            InitializeComponent();
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            var url = "https://github.com/noant/Lazurite/wiki";

            try
            {
                Process.Start(url);
            }
            catch
            {
                Process.Start("IEXPLORE.EXE", url); //crutch
            }
        }
    }
}
