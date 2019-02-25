﻿using LazuriteMobile.App.Switches.Bases.Converters;

namespace LazuriteMobile.App.Switches
{
    public static class ConvertersStatic
    {
        public static readonly BoolToDouble BoolToDouble = new BoolToDouble();
        public static readonly BoolInvert BoolInvert = new BoolInvert();
        public static readonly GeolocationDateTimeValueTypeToSplittedString GeolocationDateTimeValueTypeToSplittedString = new GeolocationDateTimeValueTypeToSplittedString();
        public static readonly StringToIcon StringToIcon = new StringToIcon();
        public static readonly StringToShortString StringToShortString = new StringToShortString(17);
        public static readonly StringToShortString StringToShortStringBig = new StringToShortString(40);
        public static readonly StringToShortString StringToShortStringSmall = new StringToShortString(8);
        public static readonly ValueTypeStringToBool ValueTypeStringToBool = new ValueTypeStringToBool();
        public static readonly ValueTypeStringToBoolInvert ValueTypeStringToBoolInvert = new ValueTypeStringToBoolInvert();
        public static readonly ValueTypeStringToDouble ValueTypeStringToDouble = new ValueTypeStringToDouble();
        public static readonly ValueTypeStringToDoubleRoundStr ValueTypeStringToDoubleRoundStr = new ValueTypeStringToDoubleRoundStr();
        public static readonly Selection_BoolToColor Selection_BoolToColor = new Selection_BoolToColor();
        public static readonly Background_BoolToColor Background_BoolToColor = new Background_BoolToColor();
        public static readonly Foreground_StateToColor Foreground_StateToColor = new Foreground_StateToColor();
        public static readonly ValueForeground_StateToColor ValueForeground_StateToColor = new ValueForeground_StateToColor();
        public static readonly IconColor_StateToColor IconColor_StateToColor = new IconColor_StateToColor();
        public static readonly StateToToggleIcon StateToToggleIcon = new StateToToggleIcon();
    }
}