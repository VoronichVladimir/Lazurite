﻿using Lazurite.CoreActions;
using Lazurite.IOC;
using Lazurite.Windows.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ComplexActionView.xaml
    /// </summary>
    public partial class ComplexActionView : UserControl, IConstructorElement
    {
        private PluginsManager _manager = Singleton.Resolve<PluginsManager>();

        private ComplexAction _action;

        public ComplexActionView(ComplexAction action)
        {
            InitializeComponent();
            buttons.AddNewClick += () =>
            {
                SelectCoreActionView.Show((type) => {
                    var newActionHolder = new ActionHolder()
                    {
                        Action = _manager.CreateInstanceOf(type)
                    };
                    Insert(newActionHolder, 0);
                });
            };
            buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            Refresh(action);
        }

        public ComplexActionView() : this(new ComplexAction())
        {
            //do nothing
        }

        public ActionHolder ActionHolder
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool EditMode
        {
            get;
            set;
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;

        public void Refresh(ComplexAction action)
        {
            _action = action;
            stackPanel.Children.Clear();
            foreach (var actionHolder in _action.ActionHolders)
                Insert(actionHolder);
        }

        private void Insert(ActionHolder actionHolder, int position=-1)
        {
            if (position == -1)
                position = stackPanel.Children.Count;
            var control = ActionControlResolver.Create(actionHolder.Action);
            var constructorElement = control as IConstructorElement;
            constructorElement.Modified += (element) => Modified?.Invoke(element);
            constructorElement.NeedRemove += (element) => {
                _action.ActionHolders.Remove(actionHolder);
                stackPanel.Children.Remove(control);
                Modified?.Invoke(this);
            };
            constructorElement.NeedAddNext += (element) => {
                SelectCoreActionView.Show((type) => {
                    var index = stackPanel.Children.IndexOf(control);
                    var newActionHolder = new ActionHolder() {
                        Action = _manager.CreateInstanceOf(type)
                    };
                    _action.ActionHolders.Insert(index, newActionHolder);
                    Insert(newActionHolder, index);
                    Modified?.Invoke(this);
                });
            };
            stackPanel.Children.Insert(position, control);
        }

        public void MakeRemoveButtonInvisible()
        {
            this.buttons.RemoveVisible = false;
        }
    }
}