using System;
using System.Text;

namespace AmbientService.src.models
{
    public class Sensor
    {
        public int Id;

        public string Name;

        public OutBoundEnum tempOut = OutBoundEnum.None;

        public OutBoundEnum humOut = OutBoundEnum.None;

        public OutBoundEnum presOut = OutBoundEnum.None;

        public MeasurementData Measurement;

        public MeasurementData_DAY Measurement_DAY;

        public bool IsNotContact = false;

        public DateTime LastDataTime = DateTime.MinValue;

        public DateTime LastSendEmailData = DateTime.MinValue;

        public DateTime LastSendEmailLowBattary = DateTime.MinValue;

        public string GetBoundString(string type)
        {
            StringBuilder builder = new StringBuilder();

            if (tempOut != OutBoundEnum.None)
            {
                builder.AppendLine(type);
                builder.AppendLine(tempOut == OutBoundEnum.Max ?
                    "T max = " + Measurement.MaxTemperature + " T = " + Measurement.Temperature :
                    "T min = " + Measurement.MinTemperature + " T = " + Measurement.Temperature);

                if ((tempOut == OutBoundEnum.Min && Measurement.Temperature > Measurement.MinTemperature) 
                    || tempOut == OutBoundEnum.Max && Measurement.Temperature < Measurement.MaxTemperature)
                {
                    tempOut = OutBoundEnum.None;
                }
            }

            if (humOut != OutBoundEnum.None)
            {
                builder.AppendLine(type);
                builder.AppendLine(humOut == OutBoundEnum.Max ?
                    "H max, % = " + Measurement.MaxHumidity + " H, % = " + Measurement.Humidity :
                    "H min, % = " + Measurement.MinHumidity + " H, % = " + Measurement.Humidity);

                if ((humOut == OutBoundEnum.Min && Measurement.Humidity > Measurement.MinHumidity)
                    || humOut == OutBoundEnum.Max && Measurement.Humidity < Measurement.MaxHumidity)
                {
                    humOut = OutBoundEnum.None;
                }
            }

            if (presOut != OutBoundEnum.None)
            {
                builder.AppendLine(type);
                builder.AppendLine(presOut == OutBoundEnum.Max ?
                    "P max, kpa = " + Measurement.MaxPressure + " P, kpa = " + Measurement.Pressure :
                    "P min, kpa = " + Measurement.MinPressure + " P, kpa = " + Measurement.Pressure);

                if ((presOut == OutBoundEnum.Min && Measurement.Pressure > Measurement.MinPressure)
                    || presOut == OutBoundEnum.Max && Measurement.Pressure < Measurement.MaxPressure)
                {
                    presOut = OutBoundEnum.None;
                }
            }

            return builder.ToString();
        }
    }
}
