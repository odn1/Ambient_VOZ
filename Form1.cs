using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace ReportUT_
{
    delegate void AddProgressEventHandler(int val);

    public partial class Form1 :    Form  /// MaterialForm  
    {
        private event AddProgressEventHandler onProgress;
        private event AddProgressEventHandler onLabelText;
        private event AddProgressEventHandler onSet_End;
        bool Flag = false;


        string Path_ini = "AmbientRepService.dat";
        private Params pl = new Params();
        private OdbcConnector p_odbcConnector;

        private ReportDAYs RepDAYs  = new ReportDAYs();

        public List<Sensor> sensors = new List<Sensor>();
        //public SensorMes Sn = new SensorMes();

        public List<SensorMes> Listsensor_Mes = new List<SensorMes>();
   

        public SensorMes pSensorMes = new SensorMes();


        public Form1()
        {
            InitializeComponent();
            dateTimePicker1.MaxDate = DateTime.Now;
            DateTime dt = DateTime.Now;
            onProgress += new AddProgressEventHandler(Form1_onProgress);
            onLabelText += new AddProgressEventHandler(Form1_onLabelText);
            onSet_End  += new AddProgressEventHandler(Form1_onSet_End);

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
            dateTimePicker_Stop_Time.CustomFormat = "yyyy MMMM";
           
            ToolTip t = new ToolTip();
            t.SetToolTip(Button_Settings, "Настройки");

            

        }

    

        private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
        {

            dateTimePicker_2_Time.Visible = (!dateTimePicker_2_Time.Visible);
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
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



        private void materialSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
        }

        private void dateTimePicker_2_Time_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = dateTimePicker_2_Time.Value;
            if (dt < dateTimePicker1.Value) {
                     MessageBox.Show("начало периода  " + dateTimePicker1.Value.ToString() +
        "\nне может превышать его окончание  " + dateTimePicker_2_Time.Value.ToString()); 
                    dateTimePicker_2_Time.Value = dateTimePicker1.Value;
                  return; }

            RepDAYs.T12 = dateTimePicker_2_Time.Value.ToString();
        }

        private void dateTimePicker_Start_Time_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = dateTimePicker_Start_Time.Value;
            dateTimePicker1.Value = dt;//
            dateTimePicker_Stop_Time.Value = dt;
            RepDAYs.M11 = dateTimePicker_Start_Time.Value.ToString();
        }

        private void dateTimePicker_Stop_Time_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = dateTimePicker_Stop_Time.Value;
            RepDAYs.M21 = dateTimePicker_Stop_Time.Value.ToString();
        }

        private void Button_Reports_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Выбор местоположения для отчетов";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                text_Report.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Button_Exec_Report_Click(object sender, EventArgs e)
        {
            int Moun=0;

            if (checkBox1.Checked)
            {
            Moun =  dateTimePicker_Stop_Time.Value.Month - dateTimePicker1.Value.Month;
            if (dateTimePicker_Start_Time.Value>dateTimePicker_Stop_Time.Value)
                { MessageBox.Show("начальная дата  " + dateTimePicker1.Value.ToString()+
                "\nне может превышать конечную  " + dateTimePicker_Stop_Time.Value.ToString()); return; }
            }
            

            Flag = true;
            Seril_Param();

            p_odbcConnector = new OdbcConnector();

            sensors = p_odbcConnector.AllSensors();
            p_odbcConnector.Sens_Type_Limits();
            p_odbcConnector.AllSensorsRoom();

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

            // int iD = 3;
            string Bt = Button_Exec_Report.Text;
            
            Button_Exec_Report.Text = "%";
            label_Count.Focus();

            if (Moun>0)
    Listsensor_Mes = p_odbcConnector.AllSensors_Mes(dateTimePicker1.Value.ToString(), dateTimePicker_Stop_Time.Value.ToString());
else
            Listsensor_Mes = p_odbcConnector.AllSensors_Mes(dateTimePicker1.Value.ToString(), dateTimePicker1.Value.AddMonths(1).ToString());
  
            int CountSensors = sensors.Count;
            double procent = (double)(100 /( (double)CountSensors * (Moun+1)));
            double D_procent = 0;
            int prc = 0;
            RepDAYs.dateT1 = dateTimePicker1.Value;
            RepDAYs.dateT2 = dateTimePicker_2_Time.Value;
            // if (checkBox1.Checked) RepDAYs.dateT2 = dateTimePicker_2_Time.Value;
            ;
            #region [ Task.Run(()]
            Task.Run(() =>
            {
              //  int  Moun_n = 0;
               for (int Moun_n = 0; Moun_n <= Moun; Moun_n++)
                {
                    for (int sn_n = 0; sn_n < sensors.Count; sn_n++)
                    {
                        One_Sens_Day(Listsensor_Mes, ListStr, ListStr1, ListStr2, ListStr3, sensors[sn_n].Id, sn_n, Moun_n);
                        D_procent = D_procent + procent;
                        prc = (int)D_procent;

                        if (onProgress != null) onProgress(prc);
                        if (onSet_End != null) onSet_End(sn_n);

                        if (onLabelText != null) onLabelText(sn_n + 1);
                    }
                    if (Moun > 0)
                    {
                        //dateTm = 
                        RepDAYs.dateT1 = RepDAYs.dateT1.AddMonths(1);
                        // dateTm2 =
                        RepDAYs.dateT2 = RepDAYs.dateT2.AddMonths(1);
                    }
                    if (onProgress != null) onProgress(0);
                }

            });
            #endregion

            progressBar1.Value = 1;
        }


        private void One_Sens_Day(List<SensorMes> LSM,string[] ListStr, string[] ListStr1, string[] ListStr2, string[] ListStr3, int iDS, int numS, int Mountn)
        {
            DateTime dateTm= RepDAYs.dateT1;
            DateTime dateTm2= RepDAYs.dateT2;

           

            for (int i = 1; i < 32; i++)
            { ListStr[i] = ListStr1[i] = ListStr2[i] = "" ; }

            int countDays = System.DateTime.DaysInMonth(dateTm.Year, dateTm.Month);
            string HH_mm = dateTm.ToString(" HHч mmмин ");     // "_" + dateTm.Hour.ToString() + "_" + dateTm.Minute.ToString();


            for (int j = 1; j <= countDays; j++)
            {
                pSensorMes = p_odbcConnector.OneSensor(LSM,iDS, dateTm.ToString(), dateTm2.ToString(), 0);
                if (pSensorMes!=null)
                if (pSensorMes.Id != -1000)//{valid = false; progressBar1.Value += 5; break; }
                {
                    ListStr[j] = pSensorMes.TimeS.ToString("HH:mm"); // ();  //   //
                    ListStr1[j] = pSensorMes.Temperature.ToString();
                    ListStr2[j] = pSensorMes.Humidity.ToString();
                }
                else
                {
                    ListStr[j] = ListStr1[j] = ListStr2[j] = "";
                }

                dateTm = dateTm.AddDays(1);
               // if (checkBox2.Checked) dateTm2 = dateTm2.AddDays(1);
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
                    HH_mm = dateTm2.ToString(" HHч mmмин "); ;// "_" + dateTm.Hour.ToString() + "_" + dateTm.Minute.ToString();

                    for (int j = 1; j <= countDays; j++)
                    {
                        pSensorMes = p_odbcConnector.OneSensor(Listsensor_Mes,iDS, dateTm2.ToString(), dateTm2.ToString(), 0);
                    if (pSensorMes!=null)
                        if (pSensorMes.Id != -1000)//{valid = false; progressBar1.Value += 5; break; }
                        {
                            ListStr[j] = pSensorMes.TimeS.ToString("HH:mm"); //();  //   
                            ListStr1[j] = pSensorMes.Temperature.ToString();
                            ListStr2[j] = pSensorMes.Humidity.ToString();
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

        private void Save_Day_mes(string[] ListStr, string[] ListStr1, string[] ListStr2, string[] ListStr3, string HH_mm, int num)
        {
            SavePredator.SavePredator SP = new SavePredator.SavePredator();
            SP.Load(pl.Sample);
            var BM = SP.Bookmarks;
            if (checkBox3.Checked == true) SP.BM_Insert_Str("zona", pl.Room);
            else
                SP.BM_Insert_Str("zona", sensors[num].Zone);

            SP.BM_Insert_Str("t_min", sensors[num].Tmin);
            SP.BM_Insert_Str("t_max", sensors[num].Tmax);
            SP.BM_Insert_Str("h_min", sensors[num].Hmin);
            SP.BM_Insert_Str("h_max", sensors[num].Hmax);
            SP.BM_Insert_Str("sens_name", sensors[num].sType + " " + sensors[num].Name);
            SP.BM_Insert_Str("data_meas", RepDAYs.dateT1.ToString("MMMM, yyyy"));

            SP.BM_Insert_Line("HUM_TABLE", ListStr);
            SP.BM_Insert_Line("HUM_TABLE", ListStr1);
            SP.BM_Insert_Line("HUM_TABLE", ListStr2);

            SP.BM_Insert_Line("HUM_TABLE", ListStr3);
            SP.BM_Delete_Last_Row(new String[] { "HUM_TABLE" });


            string path = text_Report.Text; //+ @"{text_Report.Text}\\Отчеты\\{RepDAYs.dateT1.Year}\\{self.month_str}";
            path = path + "\\Отчеты\\" + RepDAYs.dateT1.Year.ToString() + "\\" + RepDAYs.dateT1.Month.ToString();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string doc_name = sensors[num].sType + " " + sensors[num].Name + HH_mm + ".docx";    ///UniTesS THB - 1С 170434 
            string S = path + "\\" + doc_name;
            SP.Save(S);
        }

        private void Button_Settings_Click(object sender, EventArgs e)
        {
            panel2.Visible = !panel2.Visible;
        }

        private void Button_Sample_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Выбор местоположения щаблона";
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

            }
            Stream stmSaveWrite = new FileStream(Path_ini, FileMode.Create, FileAccess.Write, FileShare.None);
            fmr.Serialize(stmSaveWrite, pl);
            stmSaveWrite.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Deseril_Param();
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

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
          //  checkBox3.CheckState = !checkBox3.CheckState;
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
            { progressBar1.Value = val;
                if (progressBar1.Value > 98) progressBar1.Value = 0;
               
               
             }

   
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
                label_Count.Text = "Всего:" + sensors.Count.ToString() + " / Обработано:" +  val.ToString();

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
                Button_Exec_Report.Text = ((100/ sensors.Count)* val).ToString() + "%";
                if (val == sensors.Count - 1)
                     {
                        { onProgress(100); Button_Exec_Report.Text = "Сформировать отчет"; }
                    } 
            }

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            

        }
    }

   
}
