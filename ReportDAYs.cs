using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using IniParser;
using IniParser.Model;
using System.IO;
using System.Windows;


namespace ReportUT_
{

    public class IniReader 
    {
       public static string Read(string Path1,string Key, string Section = "")
        {
            var parser = new FileIniDataParser();
            IniData ini = parser.ReadFile(Path1);
            return ini[Section][Key];
        }
    }
    public class Logger
    {
        #region Singlton
        private static Logger instanse;

        public static Logger GetInstanse()
        {
            return instanse is null ? instanse = new Logger() : instanse;
        }
        #endregion

        private string logPath = "Log.txt";
        private long maximumSize = 104857;

        private Logger()
        {

        }

        public void SetData(string operation, string message)
        {
            WriteInFile(DateTime.Now.ToString("G") + " || " + operation + "||" + message);
        }

        private void WriteInFile(string text)
        {
            using (StreamWriter writer = new StreamWriter(logPath, SizeIsCorrect()))
            {
                writer.WriteLine(text);
            }
        }

        private bool SizeIsCorrect()
        {
            try
            {
                FileInfo info = new FileInfo(logPath);

                return info.Length <= maximumSize;
            }
            catch (Exception)
            {
                using (File.Create(logPath))
                {

                }

                return true;
            }
        }
    }

    public class Sensor
    {
        public int Id;
        public string Name;
        public int iType;
        public string sType;
        public string Tmin;
        public string Tmax;
        public string Hmin;
        public string Hmax;
        public string Zone;

    }
    public class SensorMes
    {
        public int Id;  // проверка валидности -1000 - плохо
        public DateTime TimeS;
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
    public class OdbcConnector
    {

        private List<SensorMes> _sensorsMes = new List<SensorMes>();

        List<Sensor> sensors = new List<Sensor>();
        List<string> StrListRoom = new List<string>();

        public int[] type_to_min_temp = { 0, 0, 0, 0, 0, -25 };
        public int[] type_to_max_temp = { 50, 50, 50, 50, 50, 50 };
        public int[] type_to_min_hum = { 10, 10, 10, 10, -1, -1 };
        public int[] type_to_max_hum = { 90, 90, 90, 90, -1, -1 };


        private static string SELECT_ALL_SENSORS = "SELECT  ID_SENS, SENS_NAME, SENS_TYPE FROM S_CONDITIONAL_SENSORS     order by  ID_SENS ";
        private static string SELECT_ROOM_SENSORS = "SELECT ROOM_NAME FROM S_AMBIENT_MAP_ICO WHERE ROOM_ID = (SELECT MAP_ID FROM S_AMBIENT_MAP_THB WHERE THB_ID= ";
        static string SELECT_ALL_SENSORS_MES = "select  tcon.tcon_time,  tcon.tcon_temp,  tcon.tcon_hum,  tcon.tcon_pres,  tcon.id_sens from i_test_conditions tcon where tcon.tcon_time >=";

        public OdbcConnection connection;
        public OdbcConnector(string DSN_Str)
        {
            try
            {
                string connectionString = "DSN=" + DSN_Str;  //IniReader.Read("DBAllias", "DB");
                connection = new OdbcConnection(connectionString);
            }
            catch
            {
                if (connection.DataSource == "")
                {
                    MessageBox.Show("нет подключения к БД. \nВ настройках поверьте Источник данных(DSN)", "Err");
                    return;
                }
            }
        }
        public void OpenConnection()
        {
            try
            {
                // if (connection.DataSource!="")
                connection.Open();
            }
            catch
            {
                ;
            }

        }
        public void CloseConnection()
        {
            // if (connection.DataSource != "")
            connection.Close();
        }
        public void Sens_Type_Limits()
        {
            try
            {
                int Cnt = sensors.Count;
                if (Cnt <= 0) return;

                for (int i = 0; i < Cnt; i++)
                {
                    SensTo_TypeandLim(i);
                }

                return;
            }
            catch (InvalidCastException ex)
            {
                this.CloseConnection();
                Logger.GetInstanse().SetData("AllSensorsRoom", ex.Message); return;
            }

        }
        private void SensTo_TypeandLim(int i)
        {
            int TypeSens = sensors[i].iType;

            switch (TypeSens)  //sens_type = [2,             6,                 4,              8,                  12,                   14]
            {           //type_to_name = ['UniTesS THB-1', 'UniTesS THB-1С', 'UniTesS THB-1В', 'UniTesS THB-2', 'UniTesS THB-2В', 'UniTesS THB-2С']

                case (2):
                    sensors[i].sType = "UniTesS THB-1";
                    sensors[i].Tmax = type_to_max_temp[0].ToString();
                    sensors[i].Tmin = type_to_min_temp[0].ToString();
                    sensors[i].Hmax = type_to_max_hum[0].ToString();
                    sensors[i].Hmin = type_to_min_hum[0].ToString();
                    break;
                case (6):
                    sensors[i].sType = "UniTesS THB-1C";
                    sensors[i].Tmax = type_to_max_temp[1].ToString();
                    sensors[i].Tmin = type_to_min_temp[1].ToString();
                    sensors[i].Hmax = type_to_max_hum[1].ToString();
                    sensors[i].Hmin = type_to_min_hum[1].ToString();
                    break;
                case (4):
                    sensors[i].sType = "UniTesS THB-1B";
                    sensors[i].Tmax = type_to_max_temp[2].ToString();
                    sensors[i].Tmin = type_to_min_temp[2].ToString();
                    sensors[i].Hmax = type_to_max_hum[2].ToString();
                    sensors[i].Hmin = type_to_min_hum[2].ToString();
                    break;
                case (8):
                    sensors[i].sType = "UniTesS THB-2";
                    sensors[i].Tmax = type_to_max_temp[3].ToString();
                    sensors[i].Tmin = type_to_min_temp[3].ToString();
                    sensors[i].Hmax = type_to_max_hum[3].ToString();
                    sensors[i].Hmin = type_to_min_hum[3].ToString();
                    break;
                case (12):
                    sensors[i].sType = "UniTesS THB-2B";
                    sensors[i].Tmax = type_to_max_temp[4].ToString();
                    sensors[i].Tmin = type_to_min_temp[4].ToString();
                    sensors[i].Hmax = type_to_max_hum[4].ToString();
                    sensors[i].Hmin = type_to_min_hum[4].ToString();
                    break;
                case (14):
                    sensors[i].sType = "UniTesS THB-2C";
                    sensors[i].Tmax = type_to_max_temp[5].ToString();
                    sensors[i].Tmin = type_to_min_temp[5].ToString();
                    sensors[i].Hmax = type_to_max_hum[5].ToString();
                    sensors[i].Hmin = type_to_min_hum[5].ToString();
                    break;

            }
        }

        public SensorMes Get_DAY_MeasSensorId(List<SensorMes> Listsensor_Mes, int id, string Time1, string Time2, int NumOperation)
        {
            SensorMes element = new SensorMes();
            element.Id = -1000;
            DateTime D2 = DateTime.Parse(Time1).AddHours(4);
            try
            {
                element = new SensorMes();
                element = Listsensor_Mes.Where(w => w.Id == id).FirstOrDefault(p => p.TimeS > DateTime.Parse(Time1)
                && p.TimeS < D2);
                return element;
            }
            catch (Exception ex)
            {
                Logger.GetInstanse().SetData("Get_DAY_MeasSensorId", ex.Message);
                element.Id = -1000;
                return element;
            }
        }

        public string Get_RoomSensorId(int id)
        {
            string element = "";
            try
            {
                OdbcCommand command = connection.CreateCommand();
                command.CommandTimeout = 0;
                string S = "";
                S = SELECT_ROOM_SENSORS + " " + id.ToString() + ")";
                command.CommandText = S;
                OdbcDataReader dataReader = command.ExecuteReader();
                dataReader.Read();
                if (!dataReader.HasRows)
                {
                    element = "";
                    return element;
                }
                element = dataReader[0].ToString(); // NAme
                return element;
            }
            catch (Exception ex)
            {
                Logger.GetInstanse().SetData("Get_RoomSensorId", ex.Message);
                return "";
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Time1"> time for one month</param>
        /// <param name="Time2"> too times for one month</param>
        /// <param name="NumOperation">  0 - only Time1; 1 - Time1,2; 3 - Month 1,2 ;</param>
        /// <returns></returns>
        public SensorMes OneSensor(List<SensorMes> LSM, int id, string Time1, string Time2, int NumOperation, string DSN_Str)
        {

            SensorMes Sn = new SensorMes();
            Sn.Id = -1000;
            try
            {
                OdbcConnector ODC = new OdbcConnector(DSN_Str);
                // _sensors = new List<Sensor>();

                ODC.OpenConnection();
                Sn = ODC.Get_DAY_MeasSensorId(LSM, id, Time1, Time2, NumOperation);
                ODC.CloseConnection();
                return Sn;
            }
            catch
            {
                this.CloseConnection();
                return Sn;
            }

        }
        public List<Sensor> AllSensors()
        {
            string operationMessage = "AllSensors";
            OdbcDataReader dataReader;

            try
            {
                this.OpenConnection();
                if (connection.DataSource == "")
                {
                    MessageBox.Show("нет подключения к БД. \nВ настройках проверьте Источник данных(DSN)");
                    return sensors;
                }
                OdbcCommand command = connection.CreateCommand();
                command.CommandTimeout = 0;
                command.CommandText = SELECT_ALL_SENSORS;
                System.Threading.Thread.Sleep(3000);

                try
                {
                     dataReader = command.ExecuteReader();
                }

                catch (Exception exe)
                {
                    this.CloseConnection();
                    Logger.GetInstanse().SetData(operationMessage, exe.Message);
                    return sensors;
                }


                sensors.Clear();
                while (dataReader.Read())
                {
                    Sensor sensor = new Sensor();
                    sensor.Id = Convert.ToInt32(dataReader[0]);
                    sensor.Name = Convert.ToString(dataReader[1]);
                    sensor.iType = Convert.ToInt32(dataReader[2]);
                    sensors.Add(sensor);
                }
                this.CloseConnection();
                return sensors;
            }

            catch (InvalidCastException ex)
            {
                this.CloseConnection();
                Logger.GetInstanse().SetData(operationMessage, ex.Message);
                return sensors;
            }
        }

        public List<SensorMes> AllSensors_Mes(string Time1, string Time2)
        {
            string operationMessage = "AllSensors_Mes";

            try
            {
                this.OpenConnection();
                OdbcCommand command = connection.CreateCommand();
                command.CommandTimeout = 0;
                string S = SELECT_ALL_SENSORS_MES + "'" + Time1 + "'" + " and tcon.tcon_time <=" + "'" + Time2 + "'" + "   order by  tcon.tcon_time"; ;

                command.CommandText = S;
                OdbcDataReader dataReader = command.ExecuteReader();
                _sensorsMes.Clear();
                while (dataReader.Read())
                {
                    SensorMes sensorMes = new SensorMes();
                    sensorMes.TimeS = Convert.ToDateTime(dataReader[0]);
                    sensorMes.Temperature = Convert.ToSingle(dataReader[1]);
                    sensorMes.Humidity = Convert.ToSingle(dataReader[2]);
                    sensorMes.Pressure = Convert.ToSingle(dataReader[3]);
                    sensorMes.Id = Convert.ToInt32(dataReader[4]);
                    _sensorsMes.Add(sensorMes);
                }
                this.CloseConnection();
                return _sensorsMes;
            }

            catch (Exception ex)
            {
                this.CloseConnection();
                Logger.GetInstanse().SetData(operationMessage, ex.Message);
                SensorMes sensorMes = new SensorMes();
                sensorMes.Id = -1000;
                _sensorsMes.Add(sensorMes);
                return _sensorsMes;
            }
        }

        public void AllSensorsRoom()
        {
            try
            {
                int Cnt = sensors.Count;
                if (Cnt <= 0) return;
                StrListRoom.Clear();
                this.OpenConnection();
                for (int i = 0; i < Cnt; i++)
                {
                    sensors[i].Zone = Get_RoomSensorId(sensors[i].Id);
                }
                this.CloseConnection();
                return;
            }
            catch (InvalidCastException ex)
            {
                this.CloseConnection();
                Logger.GetInstanse().SetData("AllSensorsRoom", ex.Message);
                return;
            }

        }

    }


    class ReportDAYs
    {
        public ReportDAYs()
        {
        }

        string T1 = DateTime.Now.ToString();
        string T2 = DateTime.Now.ToString();

        public DateTime dateT1 = DateTime.Now;
        public DateTime dateT2 = DateTime.Now;

        bool bT2 = false;
        bool bM2 = false;

        public string T11 { get => T12; set => T12 = value; }
        public string T12 { get => T1; set => T1 = value; }
        public string T21 { get => T2; set => T2 = value; }


        public bool BM2 { get => bM2; set => bM2 = value; }
        public bool BT21 { get => bT2; set => bT2 = value; }
    }


}



