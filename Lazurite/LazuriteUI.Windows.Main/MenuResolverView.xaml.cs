﻿using LazuriteUI.Icons;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для MenuResolverView.xaml
    /// </summary>
    public partial class MenuResolverView : UserControl
    {
        public static readonly DependencyProperty ResolverProperty;

        static MenuResolverView()
        {
            ResolverProperty = DependencyProperty.Register(nameof(Resolver), typeof(IViewTypeResolverItem), typeof(MenuResolverView), new FrameworkPropertyMetadata() {
                PropertyChangedCallback = (o,e) => {
                    var resolverView = o as MenuResolverView;
                    var @continue = new Action(() => {

                        if (resolverView.contentControl.Content is IDisposable d)
                            d.Dispose();

                        var type = ((IViewTypeResolverItem)e.NewValue).Type;
                        var displayName = (DisplayNameAttribute)type.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault();
                        var icon = LazuriteIconAttribute.GetIcon(type);
                        var control = (UIElement)Activator.CreateInstance(type);

                        resolverView.captionView.Content = displayName?.DisplayName;
                        resolverView.captionView.Icon = icon;
                        resolverView.contentControl.Content = control;

                        if (control is IInitializable c)
                            c.Initialize();

                    });
                    if (resolverView.contentControl.Content is IAllowSave s)
                        s.Save(@continue);
                    else @continue();
                }
            });
        }

        public IViewTypeResolverItem Resolver
        {
            get
            {
                return (IViewTypeResolverItem)GetValue(ResolverProperty);
            }
            set
            {
                SetValue(ResolverProperty, value);
            }
        }

        public MenuResolverView()
        {
            InitializeComponent();
        }
    }
}
