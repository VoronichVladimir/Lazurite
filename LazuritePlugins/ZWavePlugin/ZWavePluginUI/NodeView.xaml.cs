﻿using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System.Windows;
using System.Windows.Controls;

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для NodeView.xaml
    /// </summary>
    public partial class NodeView : UserControl, ISelectable
    {
        public NodeView(Node node)
        {
            InitializeComponent();
            this.itemView.SelectionChanged += (o, e) => {
                SelectionChanged?.Invoke(this, e);
            };
            Node = node;

            // =)
            this.itemView.Icon = LazuriteUI.Icons.Icon.Connect;
            if (node.ProductName.ToLower().Contains("usb"))
                this.itemView.Icon = LazuriteUI.Icons.Icon.Usb;
            if (node.ProductName.ToLower().Contains("light"))
                this.itemView.Icon = LazuriteUI.Icons.Icon.LightbulbHueOn;
            if (node.ProductName.ToLower().Contains("sensor"))
                this.itemView.Icon = LazuriteUI.Icons.Icon.ManSensor;
        }
        
        private Node _node;

        public event RoutedEventHandler SelectionChanged;
        
        public event RoutedEventHandler Click
        {
            add
            {
                itemView.Click += value;
            }
            remove
            {
                itemView.Click -= value;
            }
        }
        
        public Node Node
        {
            get
            {
                return _node;
            }
            private set
            {
                _node = value;
                this.itemView.Content = _node.ProductName;
                if (_node.Failed)
                {
                    this.itemView.Content += string.Format("(id {0}; node failed)", _node.Id);
                    this.Opacity = 0.5;
                }
            }
        }

        public bool Selected
        {
            get
            {
                return itemView.Selected;
            }
            set
            {
                itemView.Selected = value;
            }
        }

        public bool Selectable
        {
            get
            {
                return itemView.Selectable;
            }
            set
            {
                itemView.Selectable = value;
            }
        }
    }
}
