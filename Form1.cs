using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace ReportUT_
{

   


    delegate void AddProgressEventHandler(int val);

    public partial class Form1 : Form  /// MaterialForm  
    {

        public void ShowMyDialogBox(string S)
        {
            Ошибка testDialog = new Ошибка(S);
             testDialog.Text = "Ошибка";
            if (testDialog.ShowDialog(this) == DialogResult.OK)
            {   ;
            }

            testDialog.Dispose();
        }


        private event AddProgressEventHandler onProgress;
        private event AddProgressEventHandler onLabelText;
        private event AddProgressEventHandler onSet_End;

        //string SS = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        string Path_ini = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) +
            "\\UniTesS\\AmbientRepService.dat";


        private Params pl = new Params();
        private OdbcConnector p_odbcConnector;

        private ReportDAYs RepDAYs = new ReportDAYs();

        public List<Sensor> sensors = new List<Sensor>();

        public List<SensorMes> Listsensor_Mes = new List<SensorMes>();

        public SensorMes pSensorMes = new SensorMes();


        public Form1()
        {
            InitializeComponent();
            dateTimePicker1.MaxDate = DateTime.Now;
            DateTime dt = DateTime.Now;
            onProgress += new AddProgressEventHandler(Form1_onProgress);
            onLabelText += new AddProgressEventHandler(Form1_onLabelText);
            onSet_End += new AddProgressEventHandler(Form1_onSet_End);

            dateTimePicker1.Value = new DateTime(dt.Year, dt.Month, 1, dt.Hour, dt.Minute, dt.Second);
            dateTimePicker_2_Time.Value = new DateTime(dt.Year, dt.Month, 1, dt.Hour, dt.Minute, dt.Second);
            dateTimePicker_Start_Time.Value = new DateTime(dt.Year, dt.Month, 1, dt.Hour, dt.Minute, dt.Second);
            dateTimePicker_Stop_Time.Value = new DateTime(dt.Year, dt.Month, 1, dt.Hour, dt.Minute, dt.Second);
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy MMMM HH:mm";
            dateTimePicker1.ShowUpDown = true;


            dateTimePicker_2_Time.MaxDate = DateTime.Now;
            dateTimePicker_2_Time.Format = DateTimePickerFormat.Custom;
            // dateTimePicker_2_Time.Value  = DateTime.Now;
            dateTimePicker_2_Time.ShowUpDown = true;
            dateTimePicker_2_Time.CustomFormat = " HH:mm";

            dateTimePicker_Start_Time.MaxDate = DateTime.Now;
            dateTimePicker_Start_Time.ShowUpDown = true;
            dateTimePicker_Start_Time.CustomFormat = "yyyy MMMM";

            dateTimePicker_Stop_Time.MaxDate = DateTime.Now;
            dateTimePicker_Stop_Time.ShowUpDown = true;
            dateTimePicker_Stop_Time.CustomFormat = "MMMM";

            ToolTip t = new ToolTip();
            t.SetToolTip(Button_Settings, "Настройки");

        }

        private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker_2_Time.Visible = (!dateTimePicker_2_Time.Visible);
            if (Control.ModifierKeys == Keys.Control) checkBox1.Visible = true;
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (RepDAYs.BM2)
            {
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = "HH:mm";
            }
            else
            {
                dateTimePicker1.CustomFormat = "yyyy MMMM HH:mm";
            }


            DateTime dt = dateTimePicker1.Value;
            //  dateTimePicker1.Value = new DateTime(dt.Year, dt.Month, 1, dt.Hour, dt.Minute, dt.Second);

            dateTimePicker1.MaxDate = DateTime.Now;
            dateTimePicker_2_Time.MaxDate = DateTime.Now;
            dateTimePicker_2_Time.Value = dateTimePicker1.Value;

            RepDAYs.T11 = dateTimePicker1.Value.ToShortDateString();
            RepDAYs.T12 = dateTimePicker_2_Time.Value.ToString();

            dateTimePicker1.Value = DateTime.Parse(RepDAYs.T11);
        }



        private void materialSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
        }

        private void dateTimePicker_2_Time_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = dateTimePicker_2_Time.Value;
            if (dt < dateTimePicker1.Value)
            {
                MessageBox.Show("начало периода  " + dateTimePicker1.Value.ToString() +
   "\nне может превышать его окончание  " + dateTimePicker_2_Time.Value.ToString(), "Ошибка");
                dateTimePicker_2_Time.Value = dateTimePicker1.Value;
                return;
            }

            RepDAYs.T12 = dateTimePicker_2_Time.Value.ToString();
        }

        private void dateTimePicker_Start_Time_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = dateTimePicker_Start_Time.Value;
            dateTimePicker1.Value = dt;//
            dateTimePicker_Stop_Time.Value = dt;
            // RepDAYs.M11 = dateTimePicker_Start_Time.Value.ToString();
        }

        private void dateTimePicker_Stop_Time_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = dateTimePicker_Stop_Time.Value;
            // RepDAYs.M21 = dateTimePicker_Stop_Time.Value.ToString();
        }

        private void Button_Reports_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Выбор местоположения для жуналов учета";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                text_Report.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Button_Exec_Report_Click(object sender, EventArgs e)
        {
            int Moun = 0;

       
            if (checkBox1.Checked)
            {
                Moun = dateTimePicker_Stop_Time.Value.Month - dateTimePicker1.Value.Month;
                if (dateTimePicker_Start_Time.Value > dateTimePicker_Stop_Time.Value)
                {
                    MessageBox.Show("начальная дата  " + dateTimePicker1.Value.ToString() +
                  "\nне может превышать конечную  " + dateTimePicker_Stop_Time.Value.ToString()); return;
                }
            }



            if (!Directory.Exists(text_Report.Text) )
                {
                MessageBox.Show("Нет каталога для вывода отчетов:" + text_Report.Text, "Ошибка");
                panel2.Visible = true;
                return;
            }
            if (!File.Exists(text_Sample.Text))
            {
                MessageBox.Show("Нет файла   для шаблона:" + text_Sample.Text, "Ошибка");
                panel2.Visible = true;
                return;
            }


            Seril_Param();

            Button_Exec_Report.Text = "соедениение с БД  >>>>>>>";


            string sTime = "Время";
            string sTemp = "Температура, °C";
            string sHum = "Относительная влажность, %";
            string sRespPerson = " Подпись лица, ответственного за внесение данных";

            String[] ListStr = new String[32];
            String[] ListStr1 = new String[32];
            String[] ListStr2 = new String[32];
            String[] ListStr3 = new String[1];
            ListStr[0] = sTime;
            ListStr1[0] = sTemp;
            ListStr2[0] = sHum;
            ListStr3[0] = sRespPerson;

            string Bt = Button_Exec_Report.Text;
            label_Count.Focus();

          
            RepDAYs.dateT1 = dateTimePicker1.Value;
            RepDAYs.dateT2 = dateTimePicker_2_Time.Value;
            #region [ Task.Run(()]
     
            Task.Run(() =>
           {
               int CountSensors ;
               double procent ;
               double D_procent = 0;
               int prc = 0;
               try
               {

                   try
                   {
                       p_odbcConnector = new OdbcConnector(pl.DSN);
                       p_odbcConnector.F_DB = true;

                       if (onLabelText != null) onLabelText(-1);  //  label_Count.Text = "Чтение датчиков";
                       sensors = p_odbcConnector.AllSensors();
                       p_odbcConnector.Sens_Type_Limits();
                       p_odbcConnector.AllSensorsRoom();

                        CountSensors = sensors.Count;
                        procent = (double)(100 / ((double)CountSensors * (Moun + 1)));
                        D_procent = 0;
                        prc = 0;

                   }
                   catch (Exception ex)
                   {
                       Logger.GetInstanse().SetData("Get_DAY_MeasSensorId", ex.Message);
                       MessageBox.Show(ex.Message);
                       return;
                   }



                   for (int Moun_n = 0; Moun_n <= Moun; Moun_n++)
                   {
                       Listsensor_Mes = p_odbcConnector.AllSensors_Mes(RepDAYs.dateT1.ToString(), RepDAYs.dateT1.AddMonths(1).ToString());
                
                       for (int sn_n = 0; sn_n < sensors.Count; sn_n++)
                       {

                           One_Sens_Day(Listsensor_Mes, ListStr, ListStr1, ListStr2, ListStr3, sensors[sn_n].Id, sn_n, Moun_n);
                           D_procent = D_procent + procent;
                           prc = (int)D_procent;

                           if (onProgress != null) onProgress(prc);
                           if (onSet_End != null) onSet_End(prc);

                           if (onLabelText != null) onLabelText(sn_n + 1);
                          
                       }
                       if (Moun > 0)
                       {
                           RepDAYs.dateT1 = RepDAYs.dateT1.AddMonths(1);
                           RepDAYs.dateT2 = RepDAYs.dateT2.AddMonths(1);
                       }
                       if (onProgress != null) onProgress(0);
                       if (onLabelText != null) onLabelText(sensors.Count);// Button_Exec_Report.Text = "Сформировать отчет";
                       Application.DoEvents();
                   }
                   if (p_odbcConnector.F_DB)
                   {
                       onProgress(100);
                       Application.DoEvents();
                       MessageBox.Show("Фомирование отчетов выполнено", "Сообщение");
                   }    
                       
                   else
                   {
                       Action action = () => ShowMyDialogBox("нет подключения к БД. \nВ настройках проверьте Источник данных(DSN)");
                          if (InvokeRequired)
                           Invoke(action);
                       else
                           action();
                   }
                   if (onSet_End != null) onSet_End(-1);// Button_Exec_Report.Text = "Сформировать отчет";
               }
               catch (SqlException ee)
               {
                   MessageBox.Show("Превышено время ожидания ответа от сервера БД " + ee.ToString());
               }
           });
            #endregion
            progressBar1.Value = 1;
        }


        private void One_Sens_Day(List<SensorMes> LSM, string[] ListStr, string[] ListStr1, string[] ListStr2, string[] ListStr3, int iDS, int numS, int Mountn)
        {
            DateTime dateTm = RepDAYs.dateT1;
            DateTime dateTm2;

            try
            {
                for (int i = 1; i < 32; i++)
                { ListStr[i] = ListStr1[i] = ListStr2[i] = ""; }

                int countDays = System.DateTime.DaysInMonth(dateTm.Year, dateTm.Month);
                string HH_mm = dateTm.ToString(" HHч mmмин ");


                for (int j = 1; j <= countDays; j++)
                {

                    pSensorMes = p_odbcConnector.OneSensor(LSM, iDS, dateTm.ToString(), dateTm.ToString(), 0, pl.DSN);


                    if (pSensorMes != null)
                        if (pSensorMes.Id != -1000)
                        {
                            ListStr[j] = pSensorMes.TimeS.ToString("HH:mm"); // ();  //   //
                            if (float.IsNaN(pSensorMes.Temperature))
                            ListStr1[j] = "N/A";
                           else 
                                ListStr1[j] = pSensorMes.Temperature.ToString("0.0");

                            if (float.IsNaN(pSensorMes.Humidity))
                                ListStr2[j] = "N/A";
                            else
                                ListStr2[j] = pSensorMes.Humidity.ToString("0.0");
                        }
                        else
                        {
                            ListStr[j] = ListStr1[j] = ListStr2[j] = "";
                        }

                    dateTm = dateTm.AddDays(1);

                }

                Save_Day_mes(ListStr, ListStr1, ListStr2, ListStr3, HH_mm, numS);



                dateTm = RepDAYs.dateT1;
                dateTm2 = RepDAYs.dateT2;
                if (checkBox2.Checked)
                    if (dateTm != dateTm2)
                    {
                        for (int i = 1; i < 32; i++)
                        { ListStr[i] = ListStr1[i] = ListStr2[i] = ""; }

                        countDays = System.DateTime.DaysInMonth(dateTm2.Year, dateTm2.Month);
                        HH_mm = dateTm2.ToString(" HHч mmмин ");

                        for (int j = 1; j <= countDays; j++)
                        {
                            pSensorMes = p_odbcConnector.OneSensor(Listsensor_Mes, iDS, dateTm2.ToString(), dateTm2.ToString(), 0, pl.DSN);
                            if (pSensorMes != null)
                                if (pSensorMes.Id != -1000)
                                {
                                    ListStr[j] = pSensorMes.TimeS.ToString("HH:mm"); //();  //
                                    if (float.IsNaN(pSensorMes.Temperature))
                                        ListStr1[j] = "N/A";
                                           else
                                            ListStr1[j] = pSensorMes.Temperature.ToString("0.0");

                                    if (float.IsNaN(pSensorMes.Humidity))
                                        ListStr2[j] = "N/A";
                                    else
                                        ListStr2[j] = pSensorMes.Humidity.ToString("0.0");
                                  
                                }
                                else
                                {
                                    ListStr[j] = ListStr1[j] = ListStr2[j] = "";
                                }
                            dateTm2 = dateTm2.AddDays(1);
                        }
                        Save_Day_mes(ListStr, ListStr1, ListStr2, ListStr3, HH_mm, numS);
                    }


            }
            catch (Exception ex)
            {
                Logger.GetInstanse().SetData("One_Sens_Day", ex.Message);
                MessageBox.Show(ex.Message);
                return;
            }

        }

        private void Save_Day_mes(string[] ListStr, string[] ListStr1, string[] ListStr2, string[] ListStr3, string HH_mm, int num)
        {
            if (sensors.Count == 0) return;
            int k = 0;
            for (int i =1; i< ListStr.Length;i++)
            {
                if (ListStr[i] != "")
                    k++;
            }
if (k==0)                   return;

            SavePredator.SavePredator SP = new SavePredator.SavePredator();
            SP.Load(pl.Sample);
            var BM = SP.Bookmarks;
            if (checkBox4.Checked == true) SP.BM_Insert_Str("Time_Pov", pl.Date_POV);
            if (checkBox3.Checked == true) SP.BM_Insert_Str("zona", pl.Room);
            else
                SP.BM_Insert_Str("zona", sensors[num].Zone);

            SP.BM_Insert_Str("t_min", sensors[num].Tmin);
            SP.BM_Insert_Str("t_max", sensors[num].Tmax);
 
            SP.BM_Insert_Str("sens_name", sensors[num].sType + " " + sensors[num].Name);
            SP.BM_Insert_Str("data_meas", RepDAYs.dateT1.ToString("MMMM, yyyy"));

            SP.BM_Insert_Line("HUM_TABLE", ListStr);

            SP.BM_Insert_Line("HUM_TABLE", ListStr1);

            // с влажностью
            if (sensors[num].iType == 8 || sensors[num].iType == 6 || sensors[num].iType == 4 || sensors[num].iType == 2)
            {
                SP.BM_Insert_Str("h_min", sensors[num].Hmin);
                SP.BM_Insert_Str("h_max", sensors[num].Hmax);
                SP.BM_Insert_Line("HUM_TABLE", ListStr2);
            }
            SP.BM_Insert_Line("HUM_TABLE", ListStr3);

            SP.BM_Delete_Last_Row(new String[] { "HUM_TABLE" });


            string path = text_Report.Text; //+ @"{text_Report.Text}\\Отчеты\\{RepDAYs.dateT1.Year}\\{self.month_str}";
            path = path + "\\Отчеты\\" + RepDAYs.dateT1.Year.ToString() + "\\"
                + RepDAYs.dateT1.ToString ("MMMM") ;
            // + RepDAYs.dateT1.Month.ToString();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string Sens_name = sensors[num].Name;
          //  Sens_name = "test.//\\";
            Sens_name = Sens_name.Replace(".", "_");
            Sens_name = Sens_name.Replace(",", "_");
            Sens_name = Sens_name.Replace("/", "");
            Sens_name = Sens_name.Replace("\\", "");

            string doc_name = sensors[num].sType + " " + Sens_name + HH_mm + ".docx";    ///UniTesS THB - 1С 170434 
            string S = path + "\\" + doc_name;
            SP.Save(S);
        }

        private void Button_Settings_Click(object sender, EventArgs e)
        {
            panel2.Visible = !panel2.Visible;
        }

        private void Button_Sample_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Выбор местоположения шаблона";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                text_Sample.Text = openFileDialog1.FileName;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Seril_Param();
        }

        private void Seril_Param()
        {
            //save data
            BinaryFormatter fmr = new BinaryFormatter();

            {
                pl.Room = text_Room.Text;
                pl.DSN = text_DSN.Text;
                pl.Report = text_Report.Text;
                pl.Sample = text_Sample.Text;
                pl.Date_POV = text_Date_POV.Text;

            }
            Stream stmSaveWrite = new FileStream(Path_ini, FileMode.Create, FileAccess.Write, FileShare.None);
            fmr.Serialize(stmSaveWrite, pl);
            stmSaveWrite.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Deseril_Param();
            string path = "C:\\Users\\Public\\Documents\\UniTesS\\UniTessAS.ini";
            string Alias = "";
            if (File.Exists(path))
            {
                Alias = IniReader.Read(path, "DBAlias", "UniTesS%20Ambient%20Software");
                if (Alias != null && Alias != "")
                    text_DSN.Text = Alias;
            }
            string logPath = "C:\\Users\\Public\\Documents\\UniTesS\\UT_Report_Log.txt";
            File.Create(logPath).Close();

        }

        private void Deseril_Param()
        {
            BinaryFormatter fmr = new BinaryFormatter();

            if (File.Exists(Path_ini))
            {

                //("POS file = " + sPOSFile);
                Stream stmSaveRead = new FileStream(Path_ini, FileMode.Open, FileAccess.Read, FileShare.Read);
                bool bError = false;
                try
                {
                    pl = (Params)fmr.Deserialize(stmSaveRead);
                }
                catch
                {
                    bError = true;
                }
                text_Room.Text = pl.Room;
                text_DSN.Text = pl.DSN;
                text_Report.Text = pl.Report;
                text_Sample.Text = pl.Sample;
                text_Date_POV.Text = pl.Date_POV;
                stmSaveRead.Close();
                 
                if (bError)
                {
                    MessageBox.Show("Error reading AmbientRepService.dat");
                    stmSaveRead.Close();
                    return;
                }

            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            RepDAYs.BT21 = !RepDAYs.BT21;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            RepDAYs.BM2 = !RepDAYs.BM2;

            if (RepDAYs.BM2)
            {
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                // Display the date as "Mon 27 Feb 2012".  
                dateTimePicker1.CustomFormat = "HH:mm";
            }
            else
            {
                dateTimePicker1.CustomFormat = "yyyy MMMM HH:mm";
            }


            DateTime dt = dateTimePicker1.Value;
            //  dateTimePicker1.Value = new DateTime(dt.Year, dt.Month, 1, dt.Hour, dt.Minute, dt.Second);

            dateTimePicker1.MaxDate = DateTime.Now;
            dateTimePicker_2_Time.MaxDate = DateTime.Now;
            dateTimePicker_2_Time.Value = dateTimePicker1.Value;

            RepDAYs.T11 = dateTimePicker1.Value.ToShortDateString();
            RepDAYs.T12 = dateTimePicker_2_Time.Value.ToString();

            dateTimePicker1.Value = DateTime.Parse(RepDAYs.T11);

        }

        void Form1_onLabelText(int val)
        {
            if (label_Count.InvokeRequired)
            {
                this.BeginInvoke(
                    new AddProgressEventHandler(Form1_onLabelText),
                    new object[] { val });
            }
            else
            {
                label_Count.Text = "Всего:" + sensors.Count.ToString() + " / Обработано:" + val.ToString();

                if (val == -1)
                {
                    label_Count.Text = "Чтение датчиков";
                }
                if (val == sensors.Count)
                {
                    onProgress(100);  //Button_Exec_Report.Text = "Сформировать отчет";
                }

            }
        }

        private void Form1_onSet_End(int val)
        {
            if (Button_Exec_Report.InvokeRequired)
            {
                this.BeginInvoke(
                    new AddProgressEventHandler(Form1_onSet_End),
                    new object[] { val });
            }
            else
            {
                if (val ==-1)
                { Button_Exec_Report.Text = "сформировать отчеты"; return; }


                if (sensors.Count == 0) { Button_Exec_Report.Text = "соедениение с БД"; return; }
                double D = 100.0 / (sensors.Count * val + 1);

                Button_Exec_Report.Text = val.ToString() + "%";

                //if (val == sensors.Count-1)
                //{
                //    onProgress(100); Button_Exec_Report.Text = "Сформировать отчет";
                //}
            }

        }

        void Form1_onProgress(int val)
        {
            if (progressBar1.InvokeRequired)
            {
                this.BeginInvoke(
                    new AddProgressEventHandler(Form1_onProgress),
                    new object[] { val });
            }
            else
            //progressBar1.Value += ;
            {
                if (val < progressBar1.Minimum) return;
                if (val > progressBar1.Maximum) return;
                progressBar1.Value = val;
                if (progressBar1.Value > 98) progressBar1.Value = 0;
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }


}
