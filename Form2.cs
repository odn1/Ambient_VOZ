using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportUT_
{
    public partial class Ошибка : Form
    {
        public Ошибка(string S)
        {
            InitializeComponent();
            label1.Text = S;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
