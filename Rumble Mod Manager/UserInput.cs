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

        public UserInput(string TitleText, string TextBoxText = null)
        {
            InitializeComponent();
            label1.Text = TitleText;
            textBox1.Text = TextBoxText;

            LoadCustomFont();

            textBox1.KeyDown += new KeyEventHandler(textBox1_KeyDown);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2_Click(sender, e);
                e.SuppressKeyPress = true;
            } else if (e.KeyCode == Keys.Escape)
            {
                button1_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void LoadCustomFont()
        {
            privateFonts.AddFontFile("GoodDogPlain.ttf");
            label1.Font = new Font(privateFonts.Families[0], 20.0F, FontStyle.Regular);
            textBox1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button2.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InputString = textBox1.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
