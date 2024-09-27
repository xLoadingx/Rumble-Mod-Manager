namespace Rumble_Mod_Manager
{
    using Microsoft.Windows.Themes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Diagnostics;
    using System.Drawing.Text;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using static Rumble_Mod_Manager.ThunderstoreMods;

    public partial class RUMBLEModManager : Form
    {
        private ModPanelControl selectedPanel;
        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        private ThunderstoreMods.Mod CurrentlySelectedMod;
        private string CurrentlySelectedVersion;
        private string CurrentlySelectedName;
        private bool isLoadingDisplay = false;

        private static CustomMaps _customMapsInstance;
        private static ThunderstoreMods _thunderstoreModsInstance;
        private static Settings _settingsInstance;

        Dictionary<string, string> modMappings = new Dictionary<string, string>();

        public RUMBLEModManager()
        {
            InitializeComponent();
            this.Text = "RUMBLE Mod Manager";
            SettingsButton.Controls.Add(pictureBox1);
            pictureBox1.Location = new Point(SettingsButton.Width - pictureBox1.PreferredSize.Width - 23, SettingsButton.Height - pictureBox1.PreferredSize.Height - 20);
            pictureBox1.BackColor = Color.Transparent;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ModManager_KeyDown);

            StartManager();
        }

        private async void StartManager()
        {
            string url = "https://raw.githubusercontent.com/xLoadingx/mod-maps/main/modMapping.json";
            modMappings = await GetModMappingsAsync(url);

            LoadCustomFont();
            LoadMods();

            panel2.AllowDrop = true;
            panel2.DragEnter += new DragEventHandler(panel2_DragEnter);
            panel2.DragDrop += new DragEventHandler(panel2_DragDrop);
        }

        private void panel2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0 && files[0].EndsWith(".zip"))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }

            if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Html))
            {
                string url = (string)e.Data.GetData(DataFormats.Text)
                            ?? (string)e.Data.GetData(DataFormats.UnicodeText)
                            ?? (string)e.Data.GetData(DataFormats.Html);

                // Check if it's a valid URL
                if (!string.IsNullOrEmpty(url) && url.Contains("thunderstore.io/package/download/"))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private async void panel2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (file.EndsWith(".zip"))
                    {
                        InstallMod(file, true);
                        MessageBox.Show($"Mod '{Path.GetFileName(file)}' installed successfully!");
                    }
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string url = (string)e.Data.GetData(DataFormats.Text);
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute) && url.Contains("thunderstore.io/package/download/"))
                {
                    await DownloadModFromURL(url);
                }
                else
                {
                    MessageBox.Show("Invalid link or unsupported file type. Please drop a valid Thunderstore link or a .zip file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            LoadMods();
        }

        private async Task DownloadModFromURL(string url)
        {
            string tempZipPath = Path.Combine(Properties.Settings.Default.RumblePath, "temp_mod.zip");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }

                    InstallMod(tempZipPath, true);

                    MessageBox.Show("Mod downloaded and installed successfully from URL!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while downloading or installing the mod: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                }
            }
        }

        public static async Task<Dictionary<string, string>> GetModMappingsAsync(string url)
        {
            HttpClient client = new HttpClient();

            try
            {
                // Download the JSON file from the provided URL
                string json = await client.GetStringAsync(url);

                // Parse the JSON into a dynamic object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);

                // Extract the ModMappings section and convert it to a dictionary
                var modDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject["ModMappings"].ToString());

                return modDictionary;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return null;
            }
        }

        private void LoadCustomFont()
        {
            using (FileStream fs = new FileStream("GoodDogPlain.ttf", FileMode.Open, FileAccess.Read))
            {
                privateFonts.AddFontFile(fs.Name);
            }
            UninstallButton.Font = new Font(privateFonts.Families[0], 26.0F, FontStyle.Regular);
            ToggleModLabel.Font = new Font(privateFonts.Families[0], 26.0F, FontStyle.Regular);
            ThunderstoreButton.Font = new Font(privateFonts.Families[0], 26.0F, FontStyle.Regular);
            CustomMapsDownloadButton.Font = new Font(privateFonts.Families[0], 22.0F, FontStyle.Regular);
            SettingsButton.Font = new Font(privateFonts.Families[0], 24.0F, FontStyle.Regular);
            ModNameLabel.Font = new Font(privateFonts.Families[0], 24.0F, FontStyle.Regular);
            ModVersionLabel.Font = new Font(privateFonts.Families[0], 26.0F, FontStyle.Regular);
            DateUpdated.Font = new Font(privateFonts.Families[0], 16.0F, FontStyle.Regular);
            ModAuthorLabel.Font = new Font(privateFonts.Families[0], 15.0F, FontStyle.Regular);
            DependenciesLabel.Font = new Font(privateFonts.Families[0], 11.0F, FontStyle.Regular);
            ModDescriptionLabel.Font = new Font(privateFonts.Families[0], 15.0F, FontStyle.Regular);
            WelcomeLabel.Font = new Font(privateFonts.Families[0], 20.0F, FontStyle.Regular);
        }

        private void Settings_Button_Click(object sender, EventArgs e)
        {
            // Check if the instance already exists and is still open
            if (_settingsInstance == null || _settingsInstance.IsDisposed)
            {
                _settingsInstance = new Settings(this, null);
                _settingsInstance.FormClosed += (s, args) => _settingsInstance = null; // Clear the instance when the form is closed
                _settingsInstance.Show();
            }
            else
            {
                _settingsInstance.BringToFront();
            }
        }


        public void LoadMods()
        {
            UninstallButton.Visible = false;
            ModNameLabel.Visible = false;
            ModVersionLabel.Visible = false;
            ModAuthorLabel.Visible = false;
            ModDescriptionLabel.Visible = false;
            DependenciesLabel.Visible = false;
            DateUpdated.Visible = false;
            ModPictureDisplay.Visible = false;
            ToggleModButton.Visible = false;
            ToggleModLabel.Visible = false;
            if (selectedPanel != null)
            {
                selectedPanel.BackColor = Color.FromArgb(30, 30, 30);
                selectedPanel = null;
            }
            string rumblePath = Properties.Settings.Default.RumblePath;
            if (string.IsNullOrEmpty(rumblePath))
            {
                MessageBox.Show("Rumble path is not set. Please set it in the settings.");
                return;
            }

            string modsPath = Path.Combine(rumblePath, "Mods");
            if (!Directory.Exists(modsPath))
            {
                MessageBox.Show("Mods folder not found in the specified Rumble path. Please ensure that MelonLoader is installed correctly and you have run the game once (to the T-Pose screen) without any mods installed.");
                return;
            }

            DisplayMods(Properties.Settings.Default.RumblePath);
        }

        private void ModManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R && !isLoadingDisplay)
            {
                DisplayMods(Properties.Settings.Default.RumblePath);
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                using (UserInput inputForm = new UserInput("Profile Name:"))
                {
                    if (inputForm.ShowDialog() == DialogResult.OK)
                    {
                        string userInput = inputForm.InputString;

                        SaveProfile(userInput);
                    }
                }
                e.SuppressKeyPress = true;
            }
        }

        private void DisplayMods(string rumblePath)
        {
            isLoadingDisplay = true;

            var customMapsPath = Path.Combine(rumblePath, "UserData", "CustomMultiplayerMaps", "Maps");
            if (Directory.Exists(customMapsPath) && CustomMapsCache.MapsByPage.Count > 0)
            {
                ThunderstoreButton.Font = new Font(privateFonts.Families[0], 22.0F, FontStyle.Regular);
                ThunderstoreButton.Size = new Size(161, 48);
                CustomMapsDownloadButton.Visible = true;
            }
            else
            {
                ThunderstoreButton.Font = new Font(privateFonts.Families[0], 26.0F, FontStyle.Regular);
                ThunderstoreButton.Size = new Size(326, 48);
                CustomMapsDownloadButton.Visible = false;
            }

            string modsPath = Path.Combine(rumblePath, "Mods");
            string disabledModsPath = Path.Combine(rumblePath, "DisabledMods");

            if (!Directory.Exists(disabledModsPath))
            {
                Directory.CreateDirectory(disabledModsPath);
            }

            var enabledModFiles = Directory.GetFiles(modsPath, "*.dll").OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant()).ToArray();
            var disabledModFiles = Directory.GetFiles(disabledModsPath, "*.dll").OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant()).ToArray();

            WelcomeLabel.Visible = enabledModFiles.Length == 0 && disabledModFiles.Length == 0;

            panel2.Controls.Clear();
            panel2.Controls.Add(WelcomeLabel);

            List<ModPanelControl> modPanels = new List<ModPanelControl>();

            foreach (var modFile in enabledModFiles)
            {
                ModPanelControl modPanel = CreateModPanel(modFile, true, rumblePath);
                modPanels.Add(modPanel);
            }

            foreach (var modFile in disabledModFiles)
            {
                ModPanelControl modPanel = CreateModPanel(modFile, false, rumblePath);
                modPanels.Add(modPanel);
            }

            int panelWidth = 580;
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

            isLoadingDisplay = false;
        }

        private ModPanelControl CreateModPanel(string modFile, bool isEnabled, string rumblePath)
        {
            // Extract the mod name from the file name
            string modFileName = Path.GetFileName(modFile);

            // Try to get the mod name from the mapping dictionary
            modMappings.TryGetValue(modFileName, out string modNameFromMapping);

            string modVersionStr = (string)GetMelonLoaderModInfo(Path.Combine(rumblePath, isEnabled ? "Mods" : "DisabledMods"), modFileName, MelonLoaderModInfoType.Version);
            Color color = Color.Lime;
            Image cloudIcon = null;
            string toolTip = "Unknown";
            Image modImage = null;
            string ModAuthor = null;
            bool modFound = false;
            bool outdated = false;

            if (modNameFromMapping != null)
            {
                foreach (var kvp in ModCache.ModsByPage)
                {
                    foreach (var mod in kvp.Value)
                    {
                        if (mod.Name.Replace("_", " ") == modNameFromMapping && !mod.isDeprecated)
                        {
                            modFound = true;
                            ModAuthor = mod.Author;
                            modImage = mod.ModImage;

                            if (!string.IsNullOrEmpty(modVersionStr))
                            {
                                int modVersion = int.Parse(modVersionStr.Replace(".", ""));
                                int modVersionCache = int.Parse(mod.Version.Replace(".", ""));

                                if (modVersion < modVersionCache)
                                {
                                    color = Color.Red;
                                    toolTip = "Out of Date";
                                    cloudIcon = Properties.Resources.UpdateIcon;
                                    outdated = true;
                                }
                                else if (modVersion == modVersionCache)
                                {
                                    color = Color.Lime;
                                    toolTip = "Up To Date";
                                    outdated = false;
                                }
                                else
                                {
                                    color = Color.Cyan;
                                    toolTip = "Unreleased version";
                                    outdated = false;
                                }
                            }
                            else
                            {
                                modVersionStr = mod.Version;
                                toolTip = "Up To Date";
                                outdated = false;
                            }
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(ModAuthor))
            {
                ModAuthor = $"By {GetMelonLoaderModInfo(Path.Combine(rumblePath, isEnabled ? "Mods" : "DisabledMods"), modFileName, MelonLoaderModInfoType.Author)}";
            }

            if (!modFound)
            {
                toolTip = "Not Found / Unreleased";
                color = Color.Cyan;
            }

            ModPanelControl modPanel = new ModPanelControl
            {
                ModNameLabel = Path.GetFileNameWithoutExtension(modFile),
                DetailsLabel = $"v{modVersionStr} by {ModAuthor}",
                ModLabelFont = new Font(privateFonts.Families[0], 15.0F, FontStyle.Bold),
                DetailsLabelFont = new Font(privateFonts.Families[0], 15.0F, FontStyle.Regular),
                UpdateNeededImage = cloudIcon,
                UpdateColor = color,
                Outdated = outdated,
                //toolTip1Text = toolTip,
                ModImage = modImage ?? Properties.Resources.UnknownMod,
                Tag = Path.GetFileName(modFile),
                ModEnabled = isEnabled,
                ModDllPath = modFile
            };

            // Add click event
            modPanel.Click += (s, e) => ModPanel_Click(modPanel, e);

            Guna.UI2.WinForms.Guna2ImageButton updateButton = modPanel.Controls.OfType<Guna.UI2.WinForms.Guna2ImageButton>().FirstOrDefault();

            if (updateButton != null)
            {
                updateButton.Click += async (s, e) =>
                {
                    ModPanel_Click(modPanel, e);

                    while (selectedPanel == null)
                    {
                        await Task.Delay(10);
                    }

                    button1_Click(modPanel, e);
                };
            }

            return modPanel;
        }


        private void Uninstall_Button_Click(object sender, EventArgs e)
        {
            if (selectedPanel != null)
            {
                string modFilePath = selectedPanel.ModDllPath;
                string modName = Path.GetFileNameWithoutExtension(modFilePath);

                if (modFilePath == null)
                {
                    MessageBox.Show("Something went wrong. File path does not exist.");
                    return;
                }

                using (Uninstall form = new Uninstall(modName, modFilePath))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        File.Delete(modFilePath);
                        LoadMods();
                    }
                }
            }
        }

        public static object GetMelonLoaderModInfo(string path, string dllName, MelonLoaderModInfoType modEnum)
        {
            string dllPath = Path.Combine(path, dllName);

            if (!File.Exists(dllPath))
            {
                return $"Error: {dllName} not found in the specified path.";
            }

            string rumblePath = Properties.Settings.Default.RumblePath;
            string melonLoaderDllPath = Path.Combine(rumblePath, "MelonLoader", "net6", "MelonLoader.dll");

            // Setup assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyPath = Path.Combine(path, new AssemblyName(args.Name).Name + ".dll");
                if (File.Exists(assemblyPath))
                {
                    return Assembly.Load(File.ReadAllBytes(assemblyPath));
                }
                if (args.Name.Contains("MelonLoader"))
                {
                    return Assembly.Load(File.ReadAllBytes(melonLoaderDllPath));
                }
                return null;
            };

            try
            {
                // Load the assembly into memory to avoid locking the file
                byte[] assemblyBytes = File.ReadAllBytes(dllPath);
                Assembly assembly = Assembly.Load(assemblyBytes);

                // Get custom attributes of type MelonInfoAttribute
                var attributes = assembly.GetCustomAttributes(false);

                foreach (var attribute in attributes)
                {
                    string attributeName = attribute.GetType().Name;

                    // Check if the attribute is of type MelonInfoAttribute
                    if (attributeName == "MelonInfoAttribute")
                    {
                        // Use reflection to get the desired property (Name or Version)
                        switch (modEnum)
                        {
                            case MelonLoaderModInfoType.Name:
                            case MelonLoaderModInfoType.Version:
                            case MelonLoaderModInfoType.Author:
                                var property = attribute.GetType().GetProperty(modEnum.ToString());
                                if (property != null)
                                {
                                    return (string)property.GetValue(attribute);
                                }
                                break;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void AdjustFontSizeToFit(Control control)
        {
            if (string.IsNullOrEmpty(control.Text))
                return;

            // Get the initial size
            Size controlSize = control.ClientSize;

            // Create a Graphics object for the control
            using (Graphics g = control.CreateGraphics())
            {
                int minFontSize = 1;
                int maxFontSize = 1000; // Set an upper limit for font size
                int optimalFontSize = minFontSize;

                // Perform a binary search to find the optimal font size
                while (minFontSize <= maxFontSize)
                {
                    int currentFontSize = (minFontSize + maxFontSize) / 2;
                    Font testFont = new Font(control.Font.FontFamily, currentFontSize, control.Font.Style);
                    SizeF textSize = g.MeasureString(control.Text, testFont);

                    if (textSize.Width <= controlSize.Width && textSize.Height <= controlSize.Height)
                    {
                        optimalFontSize = currentFontSize;
                        minFontSize = currentFontSize + 1;
                    }
                    else
                    {
                        maxFontSize = currentFontSize - 1;
                    }
                }

                // Use the optimal font size
                control.Font = new Font(control.Font.FontFamily, optimalFontSize, control.Font.Style);
            }
        }

        private void SaveProfile(string profileName)
        {
            var profile = new ModProfile
            {
                ProfileName = profileName,
                enabledMods = new List<ModState>(),
                disabledMods = new List<ModState>()
            };

            foreach (ModPanelControl modPanel in panel2.Controls.OfType<ModPanelControl>())
            {
                ModState state = new ModState
                {
                    ModName = modPanel.ModNameLabel,
                    Outdated = modPanel.Outdated
                };

                if (modPanel.ModEnabled)
                {
                    profile.enabledMods.Add(state);
                }
                else
                {
                    profile.disabledMods.Add(state);
                }
            }

            string profileDirectory = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles");

            if (!Directory.Exists(profileDirectory))
            {
                Directory.CreateDirectory(profileDirectory);
            }

            string profilePath = Path.Combine(profileDirectory, $"{profileName}_profile.json");

            Properties.Settings.Default.PreviousLoadedProfile = Properties.Settings.Default.LastLoadedProfile;
            Properties.Settings.Default.LastLoadedProfile = profileName;
            Properties.Settings.Default.Save();

            string json = JsonConvert.SerializeObject(profile, Formatting.Indented);
            File.WriteAllText(profilePath, json);

            MessageBox.Show($"Profile '{profileName}' successfully saved!");
        }

        public static void LoadProfile(string profileName, RUMBLEModManager modManagerInstance)
        {
            string modsPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mods");
            string disabledModsPath = Path.Combine(Properties.Settings.Default.RumblePath, "DisabledMods");
            string inactiveModsPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", "InactiveMods");

            if (!Directory.Exists(inactiveModsPath))
            {
                Directory.CreateDirectory(inactiveModsPath);
            }

            MoveAllFiles(modsPath, inactiveModsPath);
            MoveAllFiles(disabledModsPath, inactiveModsPath);

            string profilePath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", $"{profileName}_profile.json");
            string json = File.ReadAllText(profilePath);
            var profile = JsonConvert.DeserializeObject<ModProfile>(json);

            Properties.Settings.Default.LastLoadedProfile = profileName;
            Properties.Settings.Default.Save();

            foreach (var mod in profile.enabledMods)
            {
                string modFilePath = Path.Combine(inactiveModsPath, $"{mod.ModName}.dll");
                if (File.Exists(modFilePath))
                {
                    File.Move(modFilePath, Path.Combine(modsPath, $"{mod.ModName}.dll"));
                }
            }

            foreach (var mod in profile.disabledMods)
            {
                string modFilePath = Path.Combine(inactiveModsPath, $"{mod.ModName}.dll");
                if (File.Exists(modFilePath))
                {
                    File.Move(modFilePath, Path.Combine(disabledModsPath, $"{mod.ModName}.dll"));
                }
            }

            if (modManagerInstance != null)
            {
                modManagerInstance.LoadMods();
            }
        }

        private static void MoveAllFiles(string sourceDir, string targetDir)
        {
            var files = Directory.GetFiles(sourceDir, "*.dll");
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(targetDir, fileName);

                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }

                File.Move(file, destinationFile);
            }
        }

        private void ModPanel_Click(object sender, EventArgs e)
        {
            if (Path.Exists(Path.Combine(Properties.Settings.Default.RumblePath, "Mods")))
            {
                ModPanelControl panel = sender as ModPanelControl;
                if (panel != null)
                {
                    // Deselect the previous panel
                    if (selectedPanel != null)
                    {
                        selectedPanel.BackColor = selectedPanel.ModEnabled ? Color.FromArgb(30, 30, 30) : Color.FromArgb(192, 0, 0);

                    }

                    if (selectedPanel != panel)
                    {
                        UninstallButton.Visible = true;
                        ModVersionLabel.Visible = true;
                        ModAuthorLabel.Visible = true;
                        ModNameLabel.Visible = true;
                        ModDescriptionLabel.Visible = true;
                        DateUpdated.Visible = true;
                        DependenciesLabel.Visible = true;
                        ModPictureDisplay.Visible = true;
                        ToggleModButton.Visible = true;
                        ToggleModLabel.Visible = true;
                        selectedPanel = panel;
                        ToggleModButton.BackgroundImage = selectedPanel.ModEnabled ? Properties.Resources.green : Properties.Resources.red;
                        ToggleModLabel.BackColor = selectedPanel.ModEnabled ? Color.FromArgb(128, 255, 128) : Color.FromArgb(192, 0, 0);
                        ToggleModLabel.ForeColor = selectedPanel.ModEnabled ? Color.Green : Color.Maroon;
                        selectedPanel.BackColor = Color.LightBlue;
                        ModNameLabel.Text = selectedPanel.ModNameLabel;
                        AdjustFontSizeToFit(ModNameLabel);

                        modMappings.TryGetValue(selectedPanel.ModNameLabel + ".dll", out string modNameFromMapping);

                        string modVersionStr = (string)GetMelonLoaderModInfo(Path.Combine(Properties.Settings.Default.RumblePath, selectedPanel.ModEnabled ? "Mods" : "DisabledMods"), selectedPanel.ModNameLabel + ".dll", MelonLoaderModInfoType.Version);

                        bool modFound = false;

                        if (ModCache.ModsByPage.Count > 0)
                        {
                            foreach (var kvp in ModCache.ModsByPage)
                            {
                                foreach (var mod in kvp.Value)
                                {
                                    if (mod.Name.Replace("_", " ") == modNameFromMapping && !mod.isDeprecated)
                                    {
                                        modVersionStr = modVersionStr ?? mod.Version;
                                        CurrentlySelectedName = selectedPanel.ModNameLabel;
                                        ModPictureDisplay.Image = mod.ModImage;
                                        CurrentlySelectedMod = mod;
                                        ModNameLabel.Text = mod.Name;
                                        DateUpdated.Text = mod.DateUpdated;

                                        var cleanedDependencies = CurrentlySelectedMod.Dependencies
                                            .Where(d => !d.StartsWith("MelonLoader", StringComparison.OrdinalIgnoreCase))
                                            .Select(d =>
                                            {
                                                var parts = d.Split('-').ToList();
                                                if (parts.Count == 2)
                                                {
                                                    var name = parts[0];
                                                    var version = parts[1];
                                                    return $"{name} v{version}";
                                                }
                                                else if (parts.Count > 2)
                                                {
                                                    var name = string.Join(" ", parts.Skip(1).Take(parts.Count - 2));
                                                    var version = parts.Last();
                                                    return $"{name} v{version}";
                                                }
                                                return d; // In case it doesn't follow the expected format
                                            })
                                            .Where(d => !d.Contains("MelonLoader", StringComparison.OrdinalIgnoreCase)) // Double check after cleaning
                                            .ToList();


                                        DependenciesLabel.Text = $"Dependencies:\n{string.Join("\n", cleanedDependencies)}";
                                        AdjustFontSizeToFit(ModNameLabel);
                                        CurrentlySelectedVersion = modVersionStr ?? mod.Version;
                                        ModAuthorLabel.Text = $"By {mod.Author}";
                                        ModDescriptionLabel.Text = mod.Description;

                                        if (modVersionStr != null)
                                        {
                                            int modVersion = int.Parse(modVersionStr.Replace(".", ""));
                                            int modVersionCache = int.Parse(mod.Version.Replace(".", ""));

                                            if (modVersion > modVersionCache)
                                            {
                                                ModVersionLabel.ForeColor = Color.Cyan;
                                            }
                                            else if (modVersion == modVersionCache)
                                            {
                                                ModVersionLabel.ForeColor = Color.Lime;
                                            }
                                            else
                                            {
                                                ModVersionLabel.ForeColor = Color.Red;
                                            }
                                        }
                                        else
                                        {
                                            ModVersionLabel.ForeColor = Color.Lime;
                                        }

                                        modFound = true;
                                        break;
                                    }
                                }
                            }
                        }

                        ModVersionLabel.Text = $"Version {modVersionStr}";

                        if (!modFound)
                        {
                            ModAuthorLabel.Text = $"By {GetMelonLoaderModInfo(Path.Combine(Properties.Settings.Default.RumblePath, selectedPanel.ModEnabled ? "Mods" : "DisabledMods"), selectedPanel.ModNameLabel + ".dll", MelonLoaderModInfoType.Author)}";
                            ModPictureDisplay.Image = Properties.Resources.UnknownMod;
                            ModVersionLabel.ForeColor = Color.Cyan;
                            DateUpdated.Text = "Unkown Last Date Updated";
                            DependenciesLabel.Text = "Dependencies:";
                            ModDescriptionLabel.Text = "This mod could not be found. It's either unreleased or downloaded externally. You can still manage it.";
                        }
                    }
                    else
                    {
                        UninstallButton.Visible = false;
                        ModNameLabel.Visible = false;
                        ModVersionLabel.Visible = false;
                        ModAuthorLabel.Visible = false;
                        ModDescriptionLabel.Visible = false;
                        DateUpdated.Visible = false;
                        DependenciesLabel.Visible = false;
                        ModPictureDisplay.Image = null;
                        ModPictureDisplay.Visible = false;
                        ToggleModButton.Visible = false;
                        ToggleModLabel.Visible = false;
                        selectedPanel = null;
                    }
                }
                else
                {
                    MessageBox.Show("Panel is null");
                }
            }
        }

        private void ThunderstoreButton_Click(object sender, EventArgs e)
        {
            if (ModCache.ModsByPage != null && ModCache.ModsByPage.Count > 0)
            {
                // Check if the instance already exists and is still open
                if (_thunderstoreModsInstance == null || _thunderstoreModsInstance.IsDisposed)
                {
                    _thunderstoreModsInstance = new ThunderstoreMods(this);
                    _thunderstoreModsInstance.FormClosed += (s, args) => _thunderstoreModsInstance = null; // Clear the instance when the form is closed
                    _thunderstoreModsInstance.Show();
                }
                else
                {
                    // Optionally, bring the existing form to the front
                    _thunderstoreModsInstance.BringToFront();
                }
            }
            else
            {
                MessageBox.Show("Please wait for mods to finish loading before accessing the Thunderstore mod page.");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (int.Parse(CurrentlySelectedVersion.Replace(".", ""), CultureInfo.InvariantCulture) < int.Parse(CurrentlySelectedMod.Version.Replace(".", ""), CultureInfo.InvariantCulture))
            {
                await ThunderstoreMods.DownloadModFromInternet(CurrentlySelectedMod, this, selectedPanel.ModEnabled, true);
            }
        }

        private void ToggleModButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Toggle the ModEnabled property and update the UI
                selectedPanel.ModEnabled = !selectedPanel.ModEnabled;
                ToggleModButton.BackgroundImage = selectedPanel.ModEnabled ? Properties.Resources.green : Properties.Resources.red;
                ToggleModLabel.BackColor = selectedPanel.ModEnabled ? Color.FromArgb(128, 255, 128) : Color.FromArgb(192, 0, 0);
                ToggleModLabel.ForeColor = selectedPanel.ModEnabled ? Color.Green : Color.Maroon;
                selectedPanel.BackColor = Color.LightBlue;

                // Determine source and destination paths based on the new ModEnabled state
                string sourcePath = selectedPanel.ModEnabled
                    ? Path.Combine(Properties.Settings.Default.RumblePath, "DisabledMods", Path.GetFileName(selectedPanel.ModDllPath))
                    : Path.Combine(Properties.Settings.Default.RumblePath, "Mods", Path.GetFileName(selectedPanel.ModDllPath));

                string destinationPath = selectedPanel.ModEnabled
                    ? Path.Combine(Properties.Settings.Default.RumblePath, "Mods")
                    : Path.Combine(Properties.Settings.Default.RumblePath, "DisabledMods");

                string destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(selectedPanel.ModDllPath));

                // Ensure the source file exists
                if (File.Exists(sourcePath))
                {

                    // Create destination directory if it doesn't exist
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    // Check if the destination file exists and delete it if it does
                    if (File.Exists(destinationFilePath))
                    {
                        MessageBox.Show("Destination file already exists. Deleting...");
                        File.Delete(destinationFilePath);
                    }

                    // Move the file
                    File.Move(sourcePath, destinationFilePath, true);
                    selectedPanel.ModDllPath = Path.Combine(destinationPath, Path.GetFileName(selectedPanel.ModDllPath));
                }
                else
                {
                    MessageBox.Show("Source file does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void CustomMapsDownloadButton_Click(object sender, EventArgs e)
        {
            // Check if the instance already exists and is still open
            if (_customMapsInstance == null || _customMapsInstance.IsDisposed)
            {
                _customMapsInstance = new CustomMaps(this);
                _customMapsInstance.FormClosed += (s, args) => _customMapsInstance = null; // Clear the instance when the form is closed
                _customMapsInstance.Show();
            }
            else
            {
                // Optionally, bring the existing form to the front
                _customMapsInstance.BringToFront();
            }
        }
    }

    public static class ModCache
    {
        public static Dictionary<int, List<ThunderstoreMods.Mod>> ModsByPage { get; set; } = new Dictionary<int, List<ThunderstoreMods.Mod>>();
    }

    public enum MelonLoaderModInfoType
    {
        Name,
        Version,
        Author
    }

    public class ModState
    {
        public string ModName { get; set; }
        public bool Outdated { get; set; }
    }

    public class ModProfile
    {
        public string ProfileName { get; set; }
        public List<ModState> enabledMods { get; set; }
        public List<ModState> disabledMods { get; set; }
    }
}
