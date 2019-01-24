﻿using LazuriteMobile.App.Controls;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases
{
    public class SwitchItemView: ItemView
    {
        public SwitchItemView()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            VerticalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
            Selectable = false;
            IconVisibility = false;
            BackgroundColor = Color.Transparent;
        }
    }
}
