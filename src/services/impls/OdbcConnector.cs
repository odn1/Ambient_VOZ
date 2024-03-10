using AmbientService.src.models;
using AmbientService.src.utils;
using System;
using System.Data.Odbc;
using System.Globalization;
using System.Windows;

namespace AmbientService.src.services.impls
{
    class OdbcConnector : IConnector
    {
        #region Singlton
        private static IConnector instanse;

        public static IConnector GetInstanse()
        {
            return instanse is null ? instanse = new OdbcConnector() : instanse;
        }
        #endregion

        private static string SELECT_ALL_SENSORS = "SELECT ID_SENS, SENS_NAME FROM S_CONDITIONAL_SENSORS";
        private static string SELECT_LAST_ID = "SELECT max(ID_TCON) FROM I_TEST_CONDITIONS WHERE ID_SENS = ?";
        private static string SELECT_DATA = "SELECT ID_TCON," +
            "TCON_TEMP," +
            "TCON_TEMP_LOW," +
            "TCON_TEMP_HIGH," +
            "TCON_HUM," +
            "TCON_HUM_LOW," +
            "TCON_HUM_HIGH," +
            "TCON_PRES," +
            "TCON_PRES_LOW," +
            "TCON_PRES_HIGH," +
            "TCON_BATT_LEVEL," +
            "TCON_TIME " +
            "FROM I_TEST_CONDITIONS WHERE ID_TCON = ?";

        //        private static string SELECT_DAY_TIME =
        //"select FIRST 1 tcon.tcon_time,  tcon.tcon_temp,  tcon.tcon_hum,  tcon.tcon_pres,  tcon.id_sens from i_test_conditions tcon" +
        //"where tcon.tcon_time >= ?         and       tcon.id_sens in ? order by  tcon.tcon_time";
        //and tcon.tcon_time< ? 


        private static string SELECT_DAY_TIME =
"select FIRST 1  tcon.id_sens , tcon.tcon_time,  tcon.tcon_temp,  tcon.tcon_hum,  tcon.tcon_pres from i_test_conditions tcon where  tcon.id_sens = "; //= ? and tcon.tcon_time >= ";
        //" ?          and tcon.tcon_time< ?     and  tcon.id_sens in ? order by  tcon.tcon_time";


        private OdbcConnection connection;

        private OdbcConnector()
        {
            string connectionString = "DSN=" + IniReader.Read("DBAllias", "DB");

            connection = new OdbcConnection(connectionString);
        }

        public Sensor[] GetAllSensors()
        {
            OdbcCommand command = connection.CreateCommand();
            command.CommandText = SELECT_ALL_SENSORS;

            OdbcDataReader dataReader = command.ExecuteReader();

            return SensorConverter.ToSensors(dataReader);
        }


        public MeasurementData GetMeasurementDataBySensorId(int id)
        {
            OdbcCommand command = connection.CreateCommand();

            command.CommandText = SELECT_DATA;
            int t = GetLastMeasurementDataIdBySensorId(id);
            command.Parameters.Add("@ID_TCON", OdbcType.Int).Value = GetLastMeasurementDataIdBySensorId(id);
            OdbcDataReader dataReader = command.ExecuteReader();
            return MeasurementConverter.ConvertTo(dataReader);
        }

        public void OpenConnection()
        {
            connection.Open();
        }
        public void CloseConnection()
        {
            connection.Close();
        }

        public int GetLastMeasurementDataIdBySensorId(int id)
        {
            try
            {
                OdbcCommand command = connection.CreateCommand();
                command.CommandText = SELECT_LAST_ID;
                command.Parameters.Add("@ID_SENS", OdbcType.Int).Value = id;

                OdbcDataReader dataReader = command.ExecuteReader();
                dataReader.Read();
                return Convert.ToInt32(dataReader[0]);
            }

            catch { return -133; }

        }
        public MeasurementData_DAY Get_DAY_MeasurementDataIdBySensorId(int id, string Time1)
        {
            OdbcCommand command = connection.CreateCommand();
            command.CommandText = SELECT_DAY_TIME;
            string S = "";
            S = command.CommandText + " "+ id.ToString();
            S = S + " and tcon.tcon_time >= " + "'"+ Time1 + "'";
            command.CommandText = S;

            OdbcDataReader dataReader = command.ExecuteReader();
          //  dataReader.Read();
          // MessageBox.Show ( dataReader[0].ToString());
            return MeasurementConverter.ConvertTo_DAY(dataReader);

        }


    }
}
