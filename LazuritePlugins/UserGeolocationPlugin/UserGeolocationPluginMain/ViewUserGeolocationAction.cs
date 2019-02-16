﻿using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using System;
using System.Linq;
using UserGeolocationPluginUI;

namespace UserGeolocationPluginMain
{
    [OnlyGetValue]
    [HumanFriendlyName("Открыть геокоординаты пользователя")]
    [SuitableValueTypes(typeof(GeolocationValueType))]
    [LazuriteIcon(Icon.MapLocation)]
    [Category(Category.Geolocation)]
    public class ViewUserGeolocationAction : IAction, IUsersGeolocationAccess
    {
        public string Caption
        {
            get => $"'{GetUserName()}'";
            set { }
        }

        public string UserId { get; set; }

        public string DeviceId { get; set; }

        private string GetUserName()
        {
            var user = _needUsers?.Invoke().FirstOrDefault(x => x.Id.Equals(UserId));
            return user?.Name ?? "[неизвестный]";
        }

        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new GeolocationValueType();

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        private Func<IGeolocationTarget[]> _needUsers;

        public void SetNeedTargets(Func<IGeolocationTarget[]> needUsers) => _needUsers = needUsers;

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            var info = UserGeolocationPlugin.Utils
                .GetUserCurrentGeolocation(_needUsers?.Invoke(), UserId, DeviceId);
            var geolocationData = new GeolocationData(info.Geolocation.Latitude, info.Geolocation.Longtitude, info.DateTime);
            return geolocationData.ToString();
        }

        public void Initialize()
        {
            //
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            var window = new GetUserGeolocationActionWindow();
            var users = _needUsers?.Invoke();
            window.Users = users;
            window.Refresh();
            window.SelectedUser = users.FirstOrDefault(x => x.Id.Equals(UserId));
            window.SelectedDevice = DeviceId;
            if (window.ShowDialog() ?? false)
            {
                DeviceId = window.SelectedDevice;
                UserId = window.SelectedUser.Id;
                return true;
            }
            else return false;
        }
    }
}
