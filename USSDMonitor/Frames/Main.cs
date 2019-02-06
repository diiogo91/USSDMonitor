using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using MaterialSkin.Animations;
using USSDMonitor.Frames;

namespace USSDMonitor
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
        }

        private void materialTabSelector1_Click(object sender, EventArgs e)
        {

        }

        private void btnMcel_Click(object sender, EventArgs e)
        {
            Main.ActiveForm.Hide();
            FrmMcel frm = new FrmMcel();
            frm.Show(this);
        }

        private void btnVodacom_Click(object sender, EventArgs e)
        {
            Main.ActiveForm.Hide();
            FrmVodacom frm = new FrmVodacom();
            frm.Show(this);
        }

        private void btnMovitel_Click(object sender, EventArgs e)
        {
            Main.ActiveForm.Hide();
            FrmMovitel frm = new FrmMovitel();
            frm.Show(this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Deseja fechar a aplicação?", "Fechar Aplicação", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Deseja fechar a aplicação?", "Fechar Aplicação", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }
    }
}
