﻿using Lazurite.IOC;
using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Security
{
    /// <summary>
    /// Логика взаимодействия для GroupsSelectView.xaml
    /// </summary>
    public partial class GroupsSelectView : UserControl
    {
        private static UsersRepositoryBase Repository = Singleton.Resolve<UsersRepositoryBase>();

        public GroupsSelectView(string[] selectedGroupsIds)
        {
            InitializeComponent();
            groupsView.SelectedGroupsIds = selectedGroupsIds;
        }

        public UserGroupBase[] SelectedGroups
        {
            get
            {
                return groupsView.SelectedGroupsIds.Select(x=>Repository.Groups.First(z=>z.Name.Equals(x))).ToArray();
            }
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            ApplyClicked?.Invoke();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action ApplyClicked;

        public static void Show(Action<UserGroupBase[]> callback, string[] selectedGroupsIds)
        {
            var control = new GroupsSelectView(selectedGroupsIds);
            var dialogView = new DialogView(control);
            dialogView.Caption = "Выберите группы";
            control.ApplyClicked += () =>
            {
                callback?.Invoke(control.SelectedGroups);
                dialogView.Close();
            };
            dialogView.Show();
        }
    }
}
