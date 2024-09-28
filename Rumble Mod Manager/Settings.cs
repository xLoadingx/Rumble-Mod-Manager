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
    public partial class Settings : Form
    {
        RUMBLEModManager ModMangager;
        LaunchPage launchPage;
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        private FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

        public Settings(RUMBLEModManager manager, LaunchPage launch)
        {
            InitializeComponent();
            this.Text = "Settings";
            ModMangager = manager;
            launchPage = launch;
            LoadSettings();
            LoadCustomFont();
        }

        private void LoadCustomFont()
        {
            privateFonts.AddFontFile("GoodDogPlain.ttf");
            label1.Font = new Font(privateFonts.Families[0], 20.0F, FontStyle.Regular);
            textBox1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button1.Font = new Font(privateFonts.Families[0], 20.0F, FontStyle.Regular);
            SwitchProfile.Font = new Font(privateFonts.Families[0], 20.0F, FontStyle.Regular);
            button2.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RumblePath = textBox1.Text;
            Properties.Settings.Default.Save();
            UserMessage errorMessage = new UserMessage("Settings saved succesfully!", true);
            errorMessage.Show();

            if (ModMangager != null)
            {
                ModMangager.LoadMods();
            }
            else if (launchPage != null)
            {
                launchPage.CheckRumblePath();
            }
            this.Close();
        }

        private void LoadSettings()
        {
            textBox1.Text = Properties.Settings.Default.RumblePath;
        }

        private void SwitchProfile_Click(object sender, EventArgs e)
        {
            SwitchProfileScreen switchScreen = new SwitchProfileScreen(ModMangager);
            switchScreen.ShowDialog();
        }
    }
}
