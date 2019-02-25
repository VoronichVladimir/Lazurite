﻿namespace LazuriteMobile.MainDomain
{
    public class GeolocationListenerSettings
    {
        public static readonly GeolocationListenerSettings WithoutEnergySave = new GeolocationListenerSettings(60000, 5);
        public static readonly GeolocationListenerSettings Normal = new GeolocationListenerSettings(180000, 15);
        public static readonly GeolocationListenerSettings EnergySave1 = new GeolocationListenerSettings(5 * 60000, 30);
        public static readonly GeolocationListenerSettings EnergySave2 = new GeolocationListenerSettings(10 * 60000, 60);
        public static readonly GeolocationListenerSettings Ultra = new GeolocationListenerSettings(40 * 60000, 1000);
        public static readonly GeolocationListenerSettings Off = new GeolocationListenerSettings(-1, -1);

        public GeolocationListenerSettings(int millisecondsInterval, int metersInterval)
        {
            MillisecondsInterval = millisecondsInterval;
            MetersInterval = metersInterval;
        }

        public GeolocationListenerSettings()
        {
            // Empty
        }

        public int MillisecondsInterval { get; set; }
        public int MetersInterval { get; set; }
    }
}