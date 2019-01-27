﻿using Lazurite.Security.Permissions;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    /// <summary>
    /// Логика взаимодействия для SelectPermissionView.xaml
    /// </summary>
    public partial class SelectPermissionView : UserControl
    {
        public SelectPermissionView()
        {
            InitializeComponent();

            foreach (var permissionType in Lazurite.Security.Utils.GetPermissionTypes().OrderBy(x=> Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(x)))
            {
                var itemView = new ItemView();
                itemView.Margin = new Thickness(0, 1, 0, 0);
                itemView.Tag = permissionType;
                itemView.Icon = Icons.Icon.ChevronRight;
                itemView.Content = Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(permissionType);
                itemView.Click += (o, e) => {
                    var permission = (IPermission)Activator.CreateInstance(permissionType);
                    PermissionCreated?.Invoke(permission);
                };
                itemsView.Children.Add(itemView);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<IPermission> PermissionCreated;

        public static void Show(Action<IPermission> callback)
        {
            var control = new SelectPermissionView();
            var dialog = new DialogView(control);
            control.PermissionCreated += (permission) => {
                callback?.Invoke(permission);
                dialog.Close();
            };
            dialog.Show();
        }
    }
}