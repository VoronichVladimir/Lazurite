﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public class StringToSplittedStringLong : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value.ToString();
            if (str.Length < 20)
                return str;
            if (str.Length > 19)
            {
                var min = int.MaxValue;
                for (int i = 0; i<str.Length; i++)
                {
                    if (str[i].Equals(' '))
                        if (Math.Abs(str.Length / 2 - i) < Math.Abs(min))
                            min = str.Length / 2 - i;
                }
                if (min != int.MaxValue)
                    str = str.Insert(str.Length / 2 - min, "\r\n").Replace(" \r\n", "\r\n").Replace("\r\n ", "\r\n");
            }
            if (str.Length > 40)
                str = str.Substring(0, 38) + "...";
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
