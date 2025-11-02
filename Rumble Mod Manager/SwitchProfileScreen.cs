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
            string profilesPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles");

            if (!Directory.Exists(profilesPath))
            {
                Directory.CreateDirectory(profilesPath);
            }

            var modProfiles = Directory.GetFiles(profilesPath, "*.json")
                                       .OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant())
                                       .ToArray();

            panel2.Controls.Clear();

            List<ModPanelControl> modPanels = new List<ModPanelControl>();

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

            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Maximum = 0;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;
        }

        private ModPanelControl CreateProfilePanel(string jsonString)
        {
            ModProfile profile = JsonSerializer.Deserialize<ModProfile>(jsonString);

            if (profile == null)
                return null;

            int totalEnabledMods = profile.enabledMods.Count;
            int totalDisabledMods = profile.disabledMods.Count;
            int totalMods = totalEnabledMods + totalDisabledMods;

            ModPanelControl modPanel = new ModPanelControl
            {
                ModNameLabel = profile.ProfileName,
                ModLabelFont = new Font(privateFonts.Families[0], 15.0F, FontStyle.Bold),
                DetailsLabelFont = new Font(privateFonts.Families[0], 13.0F, FontStyle.Regular),
                DetailsLabel = $"Mods: {totalMods} ({totalEnabledMods} Enabled, {totalDisabledMods} Disabled)",
                BackColor = (profile.ProfileName == Properties.Settings.Default.LastLoadedProfile) ? System.Drawing.Color.Green : System.Drawing.Color.FromArgb(30, 30, 30),
                ModImage = Properties.Resources.UnknownMod,
                Tag = profile.ProfileName
            };
            modPanel.label1.Visible = false;

            modPanel.Click += ProfilePanel_Click;

            return modPanel;
        }

        private void SwitchProfileScreen_Load(object sender, EventArgs e)
        {
            LoadProfiles();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (selectedPanel != null)
            {
                Properties.Settings.Default.PreviousLoadedProfile = Properties.Settings.Default.LastLoadedProfile;

                await RUMBLEModManager.LoadProfile(selectedPanel.ModNameLabel, manager);
                Properties.Settings.Default.LastLoadedProfile = selectedPanel.ModNameLabel;
                Properties.Settings.Default.Save();

                LoadProfiles();
            }
        }

        private void ProfilePanel_Click(object sender, EventArgs e)
        {
            ModPanelControl panel = sender as ModPanelControl;
            if (panel != null)
            {
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
                    Export.Visible = true;
                }
                else
                {
                    selectedPanel = null;
                    button2.Visible = false;
                    button3.Visible = false;
                    Export.Visible = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedPanel != null)
            {
                List<string> profilePaths = new List<string> {
                    Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", $"{selectedPanel.ModNameLabel}_profile.json")
                };

                List<string> modNames = new List<string>
                {
                    selectedPanel.ModNameLabel
                };

                using (Uninstall delete = new Uninstall(modNames, profilePaths))
                {
                    if (delete.ShowDialog() == DialogResult.OK)
                    {
                        File.Delete(profilePaths[0]);

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

                        if (!string.IsNullOrEmpty(newProfileName) && IsValidFileName(newProfileName))
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
                                string jsonContent = File.ReadAllText(oldProfilePath);
                                ModProfile profile = JsonSerializer.Deserialize<ModProfile>(jsonContent);
                                profile.ProfileName = newProfileName;
                                string updatedJson = JsonSerializer.Serialize(profile);
                                File.WriteAllText(newProfilePath, updatedJson);
                                File.Delete(oldProfilePath);

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
                        }
                        else
                        {
                            UserMessage errorMessage = new UserMessage("Profile name is invalid or contains special characters that are not allowed.", true);
                            errorMessage.ShowDialog();
                        }
                    }
                }
            }
        }

        private bool IsValidFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return !fileName.Any(ch => invalidChars.Contains(ch));
        }

        private void Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Export Mod Profile",
                FileName = $"{selectedPanel.ModNameLabel}_profile.json"
            };


            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourcePath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", $"{selectedPanel.ModNameLabel}_profile.json");

                File.Copy(sourcePath, saveFileDialog.FileName, true);

                File.SetLastWriteTime(saveFileDialog.FileName, DateTime.Now);

                UserMessage success = new UserMessage("Mod profile exported successfully!");
                success.Show();
            }
        }

        private bool IsModInstalled(string modName)
        {
            foreach (var modPanel in manager.allMods)
            {
                if (modPanel.ModNameLabel == modName)
                {
                    return true;
                }
            }

            return false;
        }

        private async void Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Import Mod Profile"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            string sourcePath = openFileDialog.FileName;
            string targetPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", Path.GetFileName(sourcePath));
            string profileName = Path.GetFileNameWithoutExtension(targetPath).Split('_')[0];

            try
            {
                string targetDir = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                if (File.Exists(targetPath))
                {
                    UserMessage message = new UserMessage("A mod profile with the same name already exists. Do you want to overwrite it?", false, true);
                    if (message.ShowDialog() != DialogResult.Yes) return;
                }

                File.Copy(sourcePath, targetPath, true);

                string jsonContent = await File.ReadAllTextAsync(sourcePath);
                var profile = JsonSerializer.Deserialize<ModProfile>(jsonContent);

                if (profile == null || profile.enabledMods == null || profile.disabledMods == null)
                {
                    UserMessage.Show("Invalid or incomplete profile file.", "Error", true);
                    return;
                }

                LoadProfiles();
                RUMBLEModManager.LoadProfile(profileName, manager);

                foreach (var mod in profile.enabledMods.Concat(profile.disabledMods))
                {
                    if (!IsModInstalled(mod.ModName))
                    {
                        if (!string.IsNullOrWhiteSpace(mod.DownloadLink))
                        {
                            await RUMBLEModManager.DownloadModFromURL(mod.DownloadLink, manager);
                        }
                    }
                }

                UserMessage.Show("Profile imported successfully!", "Import Complete", true);
            }
            catch (IOException ioEx)
            {
                UserMessage.Show($"File error during import: {ioEx.Message}", "Profile Import Error", true);
            }
            catch (JsonException jsonEx)
            {
                UserMessage.Show($"Error parsing profile: {jsonEx.Message}", "Profile Import Error", true);
            }
            catch (Exception ex)
            {
                UserMessage.Show($"Unexpected error: {ex.Message}", "Profile Import Error", true);
            }
        }
    }
}
