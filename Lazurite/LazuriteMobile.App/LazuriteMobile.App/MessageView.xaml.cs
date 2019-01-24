﻿using LazuriteMobile.MainDomain;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageView : Grid
    {
        public MessageView (LazuriteNotification notification)
        {
            InitializeComponent ();
            lblText.Text = notification.Message.Text.Trim();
            lblDateTime.Text = notification.Message.DateTime.ToString();
            lblTitle.Text = notification.Message.Header.Trim();
            lblNew.IsVisible = !notification.IsRead;
        }
    }
}