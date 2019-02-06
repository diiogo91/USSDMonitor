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

namespace USSDMonitor
{
    public partial class FrmMovitel : MaterialForm
    {
        public FrmMovitel()
        {
            InitializeComponent();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames().OrderBy(o => o))
            {
                comboBox1.Items.Add(s);
            }
            if (comboBox1.Items.Count != 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void FrmMovitel_Load(object sender, EventArgs e)
        {
            MaterialSkinManager SkinManager = MaterialSkinManager.Instance;
            SkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            SkinManager.ColorScheme = new ColorScheme(MaterialSkin.Primary.Pink900, MaterialSkin.Primary.Pink900, MaterialSkin.Primary.Pink900, MaterialSkin.Accent.Orange700, MaterialSkin.TextShade.WHITE);
            this.ControlBox = false;
        }

        private void btnMcel_Click(object sender, EventArgs e)
        {
            FrmMovitel.ActiveForm.Close();
            Main frm = new Main();
            frm.Show();
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
