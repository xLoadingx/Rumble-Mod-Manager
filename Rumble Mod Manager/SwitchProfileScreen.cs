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
using System.Windows.Media;
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
        }

        private void LoadCustomFont()
        {
            privateFonts.AddFontFile("GoodDogPlain.ttf");
            Title.Font = new Font(privateFonts.Families[0], 30.0F, FontStyle.Bold);
            button1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button2.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            button3.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
        }

        private void LoadProfiles()
        {
            // Set the path to the profiles
            string profilesPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles");

            // Ensure the directory exists
            if (!Directory.Exists(profilesPath))
            {
                Directory.CreateDirectory(profilesPath);
            }

            // Get the JSON files (profiles) in the directory
            var modProfiles = Directory.GetFiles(profilesPath, "*.json")
                                       .OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant())
                                       .ToArray();

            // Clear the panel and start fresh
            panel2.Controls.Clear();

            // List to hold all the created mod panel controls
            List<ModPanelControl> modPanels = new List<ModPanelControl>();

            // Iterate over profiles and create corresponding panels
            foreach (var profile in modProfiles)
            {
                ModPanelControl modPanel = CreateProfilePanel(File.ReadAllText(profile));
                modPanels.Add(modPanel);
            }

            int panelWidth = 351;
            int panelHeight = 84;
            int verticalMargin = 10;

            int panel2Width = panel2.ClientSize.Width;
            int totalPanelsWidth = panelWidth;
            int startX = (panel2Width - totalPanelsWidth) / 2;

            for (int i = 0; i < modPanels.Count; i++)
            {
                modPanels[i].Size = new Size(panelWidth, panelHeight);
                modPanels[i].Location = new Point(startX, verticalMargin + i * (panelHeight + verticalMargin));
                panel2.Controls.Add(modPanels[i]);
            }

            // Configure scrolling behavior (similar to DisplayMods)
            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Maximum = 0;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;
        }

        private ModPanelControl CreateProfilePanel(string jsonString)
        {
            ModProfile profile = JsonSerializer.Deserialize<ModProfile>(jsonString);

            int totalEnabledMods = profile.enabledMods.Count;
            int totalDisabledMods = profile.disabledMods.Count;
            int totalMods = totalEnabledMods + totalDisabledMods;

            // Configure the mod panel for the profile
            ModPanelControl modPanel = new ModPanelControl
            {
                ModNameLabel = profile.ProfileName,
                ModLabelFont = new Font(privateFonts.Families[0], 15.0F, FontStyle.Bold), // Customize fonts as needed
                DetailsLabelFont = new Font(privateFonts.Families[0], 13.0F, FontStyle.Regular),
                DetailsLabel = $"Mods: {totalMods} ({totalEnabledMods} Enabled, {totalDisabledMods} Disabled)",
                BackColor = (profile.ProfileName == Properties.Settings.Default.LastLoadedProfile) ? System.Drawing.Color.Green : System.Drawing.Color.FromArgb(30, 30, 30),
                ModImage = Properties.Resources.UnknownMod,
                Tag = profile.ProfileName
            };

            modPanel.Click += ProfilePanel_Click;

            return modPanel;
        }

        private async void SwitchProfileScreen_Load(object sender, EventArgs e)
        {
            LoadProfiles();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedPanel != null)
            {
                RUMBLEModManager.LoadProfile(selectedPanel.ModNameLabel, manager);
                LoadProfiles();
            }
        }

        private void ProfilePanel_Click(object sender, EventArgs e)
        {
            ModPanelControl panel = sender as ModPanelControl;
            if (panel != null)
            {
                // Deselect the previous panel
                if (selectedPanel != null)
                {
                    selectedPanel.BackColor = (selectedPanel.ModNameLabel == Properties.Settings.Default.LastLoadedProfile) ? System.Drawing.Color.Green : System.Drawing.Color.FromArgb(30, 30, 30);
                }

                if (selectedPanel != panel)
                {
                    selectedPanel = panel;
                    selectedPanel.BackColor = System.Drawing.Color.LightBlue;
                    button2.Visible = true;
                    button3.Visible = true;
                }
                else
                {
                    selectedPanel = null;
                    button2.Visible = false;
                    button3.Visible = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedPanel != null)
            {
                string profilePath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", $"{selectedPanel.ModNameLabel}_profile.json");

                using (Uninstall delete = new Uninstall(selectedPanel.ModNameLabel, profilePath))
                {
                    if (delete.ShowDialog() == DialogResult.OK)
                    {
                        File.Delete(profilePath);

                        if (selectedPanel.ModNameLabel == Properties.Settings.Default.LastLoadedProfile)
                        {
                            RUMBLEModManager.LoadProfile(Properties.Settings.Default.PreviousLoadedProfile, manager);
                        }

                        LoadProfiles();
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedPanel != null)
            {
                using (UserInput inputForm = new UserInput("Enter the new profile name:", selectedPanel.ModNameLabel))
                {
                    if (inputForm.ShowDialog() == DialogResult.OK)
                    {
                        string newProfileName = inputForm.InputString;

                        if (!string.IsNullOrEmpty(newProfileName))
                        {
                            string profilesPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles");
                            string oldProfilePath = Path.Combine(profilesPath, $"{selectedPanel.ModNameLabel}_profile.json");
                            string newProfilePath = Path.Combine(profilesPath, $"{newProfileName}_profile.json");

                            if (File.Exists(newProfilePath))
                            {
                                UserMessage errorMessage = new UserMessage("A profile with this name already exists. Please choose a different name.", true);
                                errorMessage.ShowDialog();
                                return;
                            }

                            try
                            {
                                File.Move(oldProfilePath, newProfilePath);

                                if (Properties.Settings.Default.LastLoadedProfile == selectedPanel.ModNameLabel)
                                {
                                    Properties.Settings.Default.LastLoadedProfile = newProfileName;
                                    Properties.Settings.Default.Save();
                                }

                                selectedPanel.ModNameLabel = newProfileName;
                            }
                            catch (Exception ex)
                            {
                                UserMessage errorMessage = new UserMessage($"Failed to rename the profile. Error: {ex.Message}", true);
                                errorMessage.ShowDialog();
                            }
                        } else
                        {
                            UserMessage errorMessage = new UserMessage($"Profile name cannot be empty.", true);
                            errorMessage.ShowDialog();
                        }
                    }
                }
            }
        }
    }
}
