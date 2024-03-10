using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ReportUT_
{
    static class Program
    {
        private static Mutex _syncObject;
        private const string _syncObjectName = "{E666FA11-AE0D-480e-9FCA-4BE9B8CDB4E9}";
        /// 
        /// Главная точка входа для приложения.
        /// 
        [STAThread]
        static void Main()
        {
            bool createdNew;
            _syncObject = new Mutex(true, _syncObjectName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Программа  \n"+ Application.ExecutablePath.ToString() + "   \nуже запущена.");// не обязательно
                return;
            }
            // Продолжаем выполнение

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

    }
}
