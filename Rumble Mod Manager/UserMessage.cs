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
    public partial class UserMessage : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        public UserMessage(string message, bool showButton)
        {
            InitializeComponent();
            LoadCustomFont();

            ShowCloseButton(showButton);
            UpdateStatusMessage(message);
        }

        public void UpdateStatusMessage(string message)
        {
            label1.Text = message;
        }

        public void ShowCloseButton(bool showButton)
        {
            button1.Visible = showButton;
        }

        private void LoadCustomFont()
        {
            privateFonts.AddFontFile("GoodDogPlain.ttf");
            label1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }


    }
}
