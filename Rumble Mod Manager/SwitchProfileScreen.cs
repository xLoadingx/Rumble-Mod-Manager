using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rumble_Mod_Manager
{
    public partial class SwitchProfileScreen : Form
    {
        RUMBLEModManager manager;
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        private ModPanelControl selectedPanel;

        public SwitchProfileScreen(RUMBLEModManager manager)
        {
            InitializeComponent();
            LoadCustomFont();
            this.manager = manager;

            button2.Visible = false;
            button3.Visible = false;
            Export.Visible = false;
            Import.Visible = (manager != null);
        }

        private void LoadCustomFont()
        {
            privateFonts.AddFontFile("GoodDogPlain.ttf");
            Title.Font = new Font(privateFonts.Families[0], 30.0F, FontStyle.Bold);
            button1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button2.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button3.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            Export.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            Import.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
        }

        private void LoadProfiles()
        {
            panel2.Controls.Clear();

            List<ModProfile> profiles = ProfileSystem.AllProfiles;
            int panelWidth = 351;
            int panelHeight = 84;
            int verticalMargin = 10;

            int panel2Width = panel2.ClientSize.Width;
            int totalPanelsWidth = panelWidth;
            int startX = (panel2Width - totalPanelsWidth) / 2;

            for (int i = 0; i < profiles.Count; i++)
            {
                ModProfile profile = profiles[i];
                ModPanelControl modPanel = CreateProfilePanel(profile);
                modPanel.Size = new Size(panelWidth, panelHeight);
                modPanel.Location = new Point(startX, verticalMargin + i * (panelHeight + verticalMargin));
                panel2.Controls.Add(modPanel);
            }

            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Maximum = 0;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;
        }

        private ModPanelControl CreateProfilePanel(ModProfile profile)
        {
            var modPanel = new ModPanelControl
            {
                ModNameLabel = profile.Name,
                ModLabelFont = new Font(privateFonts.Families[0], 15.0F, FontStyle.Bold),
                DetailsLabelFont = new Font(privateFonts.Families[0], 13.0F, FontStyle.Regular),
                DetailsLabel = $"Mods: {profile.EnabledModIds.Count + profile.DisabledModIds.Count} | {profile.EnabledModIds.Count} Enabled, {profile.DisabledModIds.Count} Disabled",
                BackColor = (ProfileSystem.CurrentProfile?.Name == profile.Name) ? Color.Green : Color.FromArgb(30, 30, 30),
                ModImage = Properties.Resources.UnknownMod,
                Tag = profile
            };

            modPanel.Click += ProfilePanel_Click;
            return modPanel;
        }

        private void SwitchProfileScreen_Load(object sender, EventArgs e)
        {
            LoadProfiles();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (selectedPanel?.Tag is ModProfile selectedProfile)
            {
                ProfileSystem.ApplyProfile(selectedProfile);
            }
        }

        private void ProfilePanel_Click(object sender, EventArgs e)
        {
            if (sender is ModPanelControl panel && panel.Tag is ModProfile profile)
            {
                if (selectedPanel != panel)
                {
                    if (selectedPanel != null && selectedPanel.Tag is ModProfile oldProfile)
                    {
                        selectedPanel.BackColor = (ProfileSystem.CurrentProfile?.Name == oldProfile.Name)
                            ? Color.Green
                            : Color.FromArgb(30, 30, 30);
                    }

                    selectedPanel = panel;
                    panel.BackColor = Color.DodgerBlue;

                    button2.Visible = true;
                    button3.Visible = true;
                    Import.Visible = true;
                    Export.Visible = true;
                    AddProfile.Visible = false;
                }
                else
                {
                    selectedPanel = null;
                    panel.BackColor = (ProfileSystem.CurrentProfile?.Name == profile.Name)
                            ? Color.Green
                            : Color.FromArgb(30, 30, 30);

                    button2.Visible = false;
                    button3.Visible = false;
                    Import.Visible = false;
                    Export.Visible = false;
                    AddProfile.Visible = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void Export_Click(object sender, EventArgs e)
        {

        }

        private bool IsModInstalled(string modName)
        {
            return false;
        }

        private async void Import_Click(object sender, EventArgs e)
        {

        }

        private void AddProfile_Click(object sender, EventArgs e)
        {
            using (UserInput inputForm = new UserInput("Profile Name:"))
            {
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string userInput = inputForm.InputString;

                    ProfileSystem.CreateNewProfile(userInput);
                    manager.DisplayMods(ProfileSystem.CurrentProfile);
                }
            }
        }
    }
}
