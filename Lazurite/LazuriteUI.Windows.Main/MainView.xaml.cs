﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            this.Loaded += (o, e) => {
                this.menuItems.SelectionChanged += (o1, e1) => 
                    menuResolver.Resolver = menuItems.SelectedItem as IViewTypeResolverItem;
                this.menuItems.GetItems().First().Selected = true;
            };

            btHide.Click += (o, e) => {
                if (columnMenu.Width.Value == 170)
                {
                    columnMenu.Width = new GridLength(50);
                    rowTop.Height = new GridLength(0);
                    btHide.Icon = Icons.Icon.ArrowCollapsed;
                }
                else
                {
                    columnMenu.Width = new GridLength(170);
                    rowTop.Height = GridLength.Auto;
                    btHide.Icon = Icons.Icon.ArrowExpand;
                }
            };

            btShutdownParams.Click += (o, e) => OtherParamsView.Show();
        }
    }
}
