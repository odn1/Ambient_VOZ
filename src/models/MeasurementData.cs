using System;
using System.Text;

namespace AmbientService.src.models
{
    public class MeasurementData
    {
        public int Id;

        public float BatteryLevel;

        public DateTime Time;

        private float temperature;
        public float Temperature
        {
            set
            {
                temperature = value == -127 || value == 127 ? float.NaN : value;
            }
            get
            {
                return temperature;
            }
        }

        public float MinTemperature;

        public float MaxTemperature;

        private float humidity;
        public float Humidity
        {
            set
            {
                humidity = value == -127 || value == 127 ? float.NaN : value;
            }
            get
            {
                return humidity;
            }
        }

        public float MinHumidity;

        public float MaxHumidity;

        private float pressure;
        public float Pressure
        {
            set
            {
                pressure = value == -127 || value == 127 ? float.NaN : value;
            }
            get
            {
                return pressure;
            }
        }

        public float MinPressure;

        public float MaxPressure;
    }
    public class MeasurementData_DAY
    {
        public int Id;

         public DateTime Time;

        private float temperature;
        public float Temperature
        {
            set
            {
                temperature = value == -127 || value == 127 ? float.NaN : value;
            }
            get
            {
                return temperature;
            }
        }

        private float humidity;
        public float Humidity
        {
            set
            {
                humidity = value == -127 || value == 127 ? float.NaN : value;
            }
            get
            {
                return humidity;
            }
        }

        private float pressure;
        public float Pressure
        {
            set
            {
                pressure = value == -127 || value == 127 ? float.NaN : value;
            }
            get
            {
                return pressure;
            }
        }

    }

}
