using AmbientService.src.utils;
using System;
using System.Data.Odbc;

namespace AmbientService.src.models
{
    public static class MeasurementConverter
    {
        private static string operationMessage = "Convert measurement in class";

        public static MeasurementData ConvertTo(OdbcDataReader reader)
        {
            try
            {
                MeasurementData element = new MeasurementData();
               // if (reader.RecordsAffected == -1) return element;
                reader.Read();

                
                
                element.Id = Convert.ToInt32(reader[0]);

                element.Temperature = (float)Convert.ToDouble(reader[1]);


                element.MinTemperature = (float)Convert.ToDouble(reader[2]);
                element.MaxTemperature = (float)Convert.ToDouble(reader[3]);


                element.Humidity = (float)Convert.ToDouble(reader[4]);
                element.MinHumidity = (float)Convert.ToDouble(reader[5]);
                element.MaxHumidity = (float)Convert.ToDouble(reader[6]);

                element.Pressure = (float)Convert.ToDouble(reader[7]);
                element.MinPressure = (float)Convert.ToDouble(reader[8]);
                element.MaxPressure = (float)Convert.ToDouble(reader[9]);

                element.BatteryLevel = (float)Convert.ToDouble(reader[10]);

                element.Time = Convert.ToDateTime(reader[11]);

                return element;
            } 
            catch (InvalidCastException ex)
            {
                Logger.GetInstanse().SetData(operationMessage, ex.Message);

                return null;
            }
        }
        public static MeasurementData_DAY ConvertTo_DAY(OdbcDataReader reader)
        {
            try
            {
                MeasurementData_DAY element = new MeasurementData_DAY();
                //  if (reader.RecordsAffected == -1) return element;
                if (!reader.HasRows) {
                    element.Id = -1000;
                    return element; }
                reader.Read();

                //      public int Id;        public DateTime Time;           element.Id = Convert.ToInt32(reader[0]);

                element.Id = Convert.ToInt32(reader[0]);
                element.Time = Convert.ToDateTime(reader[1]);
               element.Temperature = (float)Convert.ToDouble(reader[2]);
                element.Humidity = (float)Convert.ToDouble(reader[3]);
                //element.Pressure = (float)Convert.ToDouble(reader[3]);
                 return element;
            }
            catch (InvalidCastException ex)
            {
                Logger.GetInstanse().SetData(operationMessage, ex.Message);

                return null;
            }
        }

    }

}
