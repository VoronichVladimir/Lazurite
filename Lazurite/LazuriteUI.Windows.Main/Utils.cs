﻿using System.Diagnostics;
using System.Windows;

namespace LazuriteUI.Windows.Main
{
    public static class Utils
    {
        public static void RestartApp()
        {
            Process.Start(Lazurite.Windows.Utils.Utils.GetAssemblyPath(typeof(App).Assembly));
            Application.Current.Shutdown();
        }
    }
}