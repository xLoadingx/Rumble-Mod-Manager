using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rumble_Mod_Manager
{
    public partial class UserInput : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        public string InputString { get; private set; }

        public UserInput(string TitleText)
        {
            InitializeComponent();
            label1.Text = TitleText;

            LoadCustomFont();
        }

        private void LoadCustomFont()
        {
            privateFonts.AddFontFile("GoodDogPlain.ttf");
            label1.Font = new Font(privateFonts.Families[0], 20.0F, FontStyle.Regular);
            textBox1.Font = new Font(privateFonts.Families[0], 9.0F, FontStyle.Regular);
            button2.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InputString = textBox1.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
