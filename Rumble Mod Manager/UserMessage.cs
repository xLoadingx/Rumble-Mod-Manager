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
    public partial class UserMessage : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        public UserMessage(string message, bool showButton = true, bool showYesNo = false, bool showCopyDialog = false)
        {
            InitializeComponent();
            LoadCustomFont();

            ShowButtons(showButton, showYesNo, showCopyDialog);
            UpdateStatusMessage(message);
        }

        public static void Show(string message, string title, bool showButton, bool showYesNo = false)
        {
            UserMessage userMessage = new UserMessage(message, showButton, showYesNo);
            userMessage.Text = title;
            userMessage.Show();
        }

        public static void ShowDialog(string message, string title, bool showButton, bool showYesNo = false)
        {
            UserMessage userMessage = new UserMessage(message, showButton, showYesNo);
            userMessage.Text = title;
            userMessage.ShowDialog();
        }

        public void UpdateStatusMessage(string message)
        {
            label1.Text = message;
        }

        public void ShowButtons(bool showButton, bool showYesNo = false, bool showCopyLog = false)
        {
            button1.Visible = showButton && !showYesNo;
            button2.Visible = showYesNo;
            button3.Visible = showYesNo;
            button4.Visible = showCopyLog && !showYesNo;
        }

        private void LoadCustomFont()
        {
            privateFonts.AddFontFile("GoodDogPlain.ttf");
            label1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button2.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button3.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button4.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(label1.Text);

                button4.Text = "Copied!";
                await Task.Delay(1000);
                button4.Text = "Copy Log";
            } catch (ExternalException ex)
            {
                UserMessage.ShowDialog($"Failed to copy to clipboard: {ex.StackTrace}", "Error", true);
            }
            
        }
    }
}
