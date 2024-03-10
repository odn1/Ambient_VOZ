using AmbientService.src.utils;
using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace AmbientService.src.models
{
    public static class SensorConverter
    {
        private static string operationMessage = "Convert sensor in class";

        public static Sensor[] ToSensors(OdbcDataReader reader)
        {
            List<Sensor> sensors = new List<Sensor>();

            while (reader.Read())
            {
                try
                {
                    Sensor sensor = new Sensor();

                    sensor.Id = Convert.ToInt32(reader[0]);
                    sensor.Name = Convert.ToString(reader[1]);

                    sensors.Add(sensor);
                }
                catch (InvalidCastException ex)
                {
                    Logger.GetInstanse().SetData(operationMessage, ex.Message);
                }
            }

            return sensors.ToArray();
        }
    }
}
