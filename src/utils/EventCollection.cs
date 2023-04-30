using AmbientService.src.models;
using System;

namespace AmbientService.src.utils
{
    public delegate void AnnounceEvent(Sensor data);

    public static class EventCollection
    {
        public static event AnnounceEvent OutBeyond;
        public static event AnnounceEvent InBeyond;
        public static event AnnounceEvent LowBattery;
        public static event AnnounceEvent NotContact;
        public static event AnnounceEvent OnContact;

        public static void CallOutBeyond(Sensor data)
        {
            OutBeyond?.Invoke(data);
        }

        public static void CallInBeyond(Sensor data)
        {
            InBeyond?.Invoke(data);
        }

        public static void CallLowBattery(Sensor data)
        {
            LowBattery?.Invoke(data);
        }

        public static void CallNotContact(Sensor data)
        {
            NotContact?.Invoke(data);
        }

        public static void CallonContact(Sensor data)
        {
            OnContact?.Invoke(data);
        }
    }
}
