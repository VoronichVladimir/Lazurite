﻿using LazuriteUI.Icons;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для ActionCategoryHeaderView.xaml
    /// </summary>
    public partial class ActionCategoryHeaderView : Grid
    {
        public static readonly DependencyProperty IconProperty;
        public static readonly DependencyProperty ContentProperty;

        static ActionCategoryHeaderView()
        {
            IconProperty = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(ActionCategoryHeaderView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    ((ActionCategoryHeaderView)o).iconView.Icon = (Icon)e.NewValue;
                }
            });
            ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(ActionCategoryHeaderView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    ((ActionCategoryHeaderView)o).label.Content = e.NewValue;
                }
            });
        }

        public Icon Icon
        {
            get
            {
                return (Icon)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public object Content
        {
            get
            {
                return (string)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public ActionCategoryHeaderView()
        {
            InitializeComponent();
        }
    }
}
