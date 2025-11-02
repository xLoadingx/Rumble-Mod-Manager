using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rumble_Mod_Manager
{
    public partial class Uninstall : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        public List<string> modPaths = new List<string>();

        public Uninstall(List<string> modNames, List<string> modPaths)
        {
            InitializeComponent();
            LoadCustomFont();
            this.Text = "Uninstall";
            if (modNames.Count == 1)
            {
                label2.Text = modNames[0];
            } else
            {
                label2.Text = $"{modNames.Count} Mod{(modNames.Count != 1 ? "s" : "")}";
            }
            this.modPaths = modPaths;

            this.KeyDown += new KeyEventHandler(Uninstall_KeyDown);
        }

        private void Uninstall_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                button1_Click(sender, e);
            } else if (e.KeyCode == Keys.Return)
            {
                button2_Click(sender, e);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void LoadCustomFont()
        {
            privateFonts.AddFontFile("GoodDogPlain.ttf");
            label1.Font = new Font(privateFonts.Families[0], 21.0F, FontStyle.Regular);
            label2.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            button2.Font = new Font(privateFonts.Families[0], 24.0F, FontStyle.Regular);
            button1.Font = new Font(privateFonts.Families[0], 24.0F, FontStyle.Regular);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
