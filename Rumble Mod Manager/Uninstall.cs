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
        private RUMBLEModManager form1;
        string modPath = string.Empty;

        public Uninstall(string modName, string modPath, RUMBLEModManager form1)
        {
            InitializeComponent();
            LoadCustomFont();
            this.Text = "Uninstall";
            label2.Text = modName;
            this.modPath = modPath;
            this.form1 = form1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
            if (File.Exists(modPath))
            {
                File.Delete(modPath);
                MessageBox.Show("Mod Uninstalled Succesfully!");
                form1.LoadMods();
                this.Close();
            } else
            {
                MessageBox.Show("Something went wrong. File path does not exist.");
            }
        }
    }
}
