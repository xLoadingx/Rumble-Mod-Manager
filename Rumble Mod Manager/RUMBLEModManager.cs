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
        List<ModPanelControl> preloadedModPanels = new List<ModPanelControl>();

        public RUMBLEModManager(List<ModPanelControl> preloadedPanels)
        {
            InitializeComponent();
            this.Text = "RUMBLE Mod Manager";
            SettingsButton.Controls.Add(pictureBox1);
            pictureBox1.Location = new Point(SettingsButton.Width - pictureBox1.PreferredSize.Width - 23, SettingsButton.Height - pictureBox1.PreferredSize.Height - 20);
            pictureBox1.BackColor = Color.Transparent;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ModManager_KeyDown);

            preloadedModPanels = preloadedPanels;

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
                        UserMessage successMessage = new UserMessage($"Mod '{Path.GetFileName(file)}' installed successfully!", true);
                        successMessage.Show();
                        successMessage.BringToFront();
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
                    UserMessage errorMessage = new UserMessage("Invalid link or unsupported file type. Please drop a valid Thunderstore link or a .zip file.", true);
                    errorMessage.Show();
                    errorMessage.BringToFront();
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
                    UserMessage successMessage = new UserMessage("Mod downloaded and installed successfully from URL!", true);
                    successMessage.Show();
                    successMessage.BringToFront();
                }
            }
            catch (Exception ex)
            {
                UserMessage errorMessage = new UserMessage($"An error occurred while downloading or installing the mod: {ex.Message}", true);
                errorMessage.Show();
                errorMessage.BringToFront();
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
                UserMessage errorMessage = new UserMessage($"An error occurred: {ex.Message}", true);
                errorMessage.Show();
                errorMessage.BringToFront();
                return null;
            }
        }

        private void LoadCustomFont()
        {
            using (FileStream fs = new FileStream("CRUMBLE.otf", FileMode.Open, FileAccess.Read))
            {
                privateFonts.AddFontFile(fs.Name);
            }

            using (FileStream fs = new FileStream("GoodDogPlain.ttf", FileMode.Open, FileAccess.Read))
            {
                privateFonts.AddFontFile(fs.Name);
            }

            UninstallButton.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            ToggleModLabel.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            ThunderstoreButton.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            CustomMapsDownloadButton.Font = new Font(privateFonts.Families[1], 22.0F, FontStyle.Regular);
            SettingsButton.Font = new Font(privateFonts.Families[1], 24.0F, FontStyle.Regular);
            ModNameLabel.Font = new Font(privateFonts.Families[1], 24.0F, FontStyle.Regular);
            ModVersionLabel.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            DateUpdated.Font = new Font(privateFonts.Families[1], 16.0F, FontStyle.Regular);
            ModAuthorLabel.Font = new Font(privateFonts.Families[1], 15.0F, FontStyle.Regular);
            DependenciesLabel.Font = new Font(privateFonts.Families[1], 11.0F, FontStyle.Regular);
            ModDescriptionLabel.Font = new Font(privateFonts.Families[1], 13.0F, FontStyle.Regular);
            WelcomeLabel.Font = new Font(privateFonts.Families[1], 20.0F, FontStyle.Regular);
            FormTitle.Font = new Font(privateFonts.Families[0], 15.0f, FontStyle.Regular);
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

            DisplayMods();
        }

        private void ModManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R && !isLoadingDisplay)
            {
                DisplayMods();
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

        private void DisplayMods()
        {
            isLoadingDisplay = true;

            var customMapsPath = Path.Combine(Properties.Settings.Default.RumblePath, "UserData", "CustomMultiplayerMaps", "Maps");
            if (Directory.Exists(customMapsPath) && CustomMapsCache.MapsByPage.Count > 0)
            {
                ThunderstoreButton.Font = new Font(privateFonts.Families[1], 22.0F, FontStyle.Regular);
                ThunderstoreButton.Size = new Size(161, 48);
                CustomMapsDownloadButton.Visible = true;
            }
            else
            {
                ThunderstoreButton.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
                ThunderstoreButton.Size = new Size(326, 48);
                CustomMapsDownloadButton.Visible = false;
            }

            string modsPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mods");
            string disabledModsPath = Path.Combine(Properties.Settings.Default.RumblePath, "DisabledMods");

            if (!Directory.Exists(disabledModsPath))
            {
                Directory.CreateDirectory(disabledModsPath);
            }

            var enabledModFiles = Directory.GetFiles(modsPath, "*.dll").OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant()).ToArray();
            var disabledModFiles = Directory.GetFiles(disabledModsPath, "*.dll").OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant()).ToArray();

            WelcomeLabel.Visible = enabledModFiles.Length == 0 && disabledModFiles.Length == 0;

            WelcomeLabel.Visible = false;
            panel2.Controls.Clear();

            foreach (var panel in preloadedModPanels)
            {
                string panelName = panel.ModNameLabel.ToLowerInvariant();
                bool isInEnabledMods = enabledModFiles.Any(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant() == panelName);
                bool isInDisabledMods = disabledModFiles.Any(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant() == panelName);

                if (isInEnabledMods || isInDisabledMods)
                {
                    panel.ModEnabled = isInEnabledMods;
                    panel.Click -= ModPanel_Click;
                    panel.Click += (s, e) => ModPanel_Click(panel, e);

                    var updateButton = panel.Controls.OfType<Guna.UI2.WinForms.Guna2ImageButton>().FirstOrDefault();
                    if (updateButton != null)
                    {
                        updateButton.Click -= async (s, e) => await UpdateButton_Click(panel);
                        updateButton.Click += async (s, e) => await UpdateButton_Click(panel);
                    }

                    panel2.Controls.Add(panel);
                }
            }

            int panelWidth = 580;
            int panelHeight = 84;
            int verticalMargin = 10;

            int panel2Width = panel2.ClientSize.Width;
            int totalPanelsWidth = panelWidth;
            int startX = (panel2Width - totalPanelsWidth) / 2;

            foreach (Control modPanel in panel2.Controls)
            {
                modPanel.Size = new Size(panelWidth, panelHeight);
                modPanel.Location = new Point(startX, verticalMargin + panel2.Controls.GetChildIndex(modPanel) * (panelHeight + verticalMargin));
            }

            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Maximum = 0;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;

            isLoadingDisplay = false;
        }

        private async Task UpdateButton_Click(ModPanelControl panel)
        {
            ModPanel_Click(panel, EventArgs.Empty);

            while (selectedPanel == null)
            {
                await Task.Delay(1);
            }

            button1_Click(panel, EventArgs.Empty);
        }

        private void Uninstall_Button_Click(object sender, EventArgs e)
        {
            if (selectedPanel != null)
            {
                string modFilePath = selectedPanel.ModDllPath;
                string modName = Path.GetFileNameWithoutExtension(modFilePath);

                if (modFilePath == null)
                {
                    UserMessage errorMessage = new UserMessage("Something went wrong. File path does not exist.", true);
                    errorMessage.Show();
                    errorMessage.BringToFront();
                    return;
                }

                using (Uninstall form = new Uninstall(modName, modFilePath))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        File.Delete(modFilePath);
                        LoadMods();
                        SaveProfile(Properties.Settings.Default.LastLoadedProfile, false);
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
                byte[] assemblyBytes = File.ReadAllBytes(dllPath);
                Assembly assembly = Assembly.Load(assemblyBytes);

                var attributes = assembly.GetCustomAttributes(false);

                foreach (var attribute in attributes)
                {
                    string attributeName = attribute.GetType().Name;

                    if (attributeName == "MelonInfoAttribute")
                    {
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

        public Dictionary<string, object> LoadSettings(string modName)
        {
            Dictionary<string, object> settingsDictionary = new Dictionary<string, object>();

            string settingsFilePath = Path.Combine(Properties.Settings.Default.RumblePath, "UserData", modName, "Settings.txt");

            if (File.Exists(settingsFilePath))
            {
                string[] lines = File.ReadAllLines(settingsFilePath);
                for (int i = 2; i < lines.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]) && lines[i].Contains(":"))
                    {
                        string[] keyValue = lines[i].Split(new[] { ":" }, 2, StringSplitOptions.None);
                        if (keyValue.Length == 2)
                        {
                            string key = keyValue[0].Trim();
                            string value = keyValue[1].Trim();

                            if (value.Contains(" "))
                            {
                                string[] values = value.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length > 1 && Array.TrueForAll(values, v => double.TryParse(v, out _)))
                                {
                                    settingsDictionary[key] = Array.ConvertAll(values, double.Parse);
                                }
                                else
                                {
                                    settingsDictionary[key] = value;
                                }
                            }
                            else
                            {
                                settingsDictionary[key] = value;
                            }
                        }
                    }
                }
            }

            return settingsDictionary;
        }

        private void SaveProfile(string profileName, bool showMessage = true)
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

            if (showMessage)
            {
                UserMessage successMessage = new UserMessage($"Profile '{profileName}' successfully saved!", true);
                successMessage.Show();
                successMessage.BringToFront();
            }
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

                        LoadSettings(selectedPanel.ModNameLabel);

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
                            ModDescriptionLabel.Text = "This mod could not be found in the official repository. It may be unreleased or downloaded from an external source. However, you can still manage it through this page!";
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
                    UserMessage errorMessage = new UserMessage("Panel is null", true);
                    errorMessage.Show();
                    errorMessage.BringToFront();
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
                UserMessage errorMessage = new UserMessage("Please wait for mods to finish loading before accessing the Thunderstore mod page.", true);
                errorMessage.Show();
                errorMessage.BringToFront();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int currentVersion = int.Parse(CurrentlySelectedVersion.Replace(".", ""), CultureInfo.InvariantCulture);
                int latestVersion = int.Parse(CurrentlySelectedMod.Version.Replace(".", ""), CultureInfo.InvariantCulture);

                if (currentVersion < latestVersion)
                {
                    await ThunderstoreMods.DownloadModFromInternet(CurrentlySelectedMod, this, selectedPanel.ModEnabled, true);
                    SaveProfile(Properties.Settings.Default.LastLoadedProfile, false);

                    string updatedVersionStr = (string)GetMelonLoaderModInfo(Path.Combine(Properties.Settings.Default.RumblePath, "Mods"), CurrentlySelectedMod.Name + ".dll", MelonLoaderModInfoType.Version);
                    int updatedVersion = int.Parse(updatedVersionStr.Replace(".", ""), CultureInfo.InvariantCulture);

                    if (updatedVersion < latestVersion)
                    {
                        UserMessage brokenModMessage = new UserMessage("The mod has been updated successfully, but it still shows as out of date. This might indicate the mod is broken and will continue to show as out of date until the developer fixes it.", true);
                        brokenModMessage.Show();
                        brokenModMessage.BringToFront();
                    }
                }
            }
            catch (Exception ex)
            {
                UserMessage errorMessage = new UserMessage($"An error occurred while updating the mod: {ex.Message}", true);
                errorMessage.Show();
                errorMessage.BringToFront();
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
                        UserMessage message = new UserMessage("Destination file already exists. Deleting...", true);
                        message.Show();
                        message.BringToFront();
                        File.Delete(destinationFilePath);
                    }

                    // Move the file
                    File.Move(sourcePath, destinationFilePath, true);
                    selectedPanel.ModDllPath = Path.Combine(destinationPath, Path.GetFileName(selectedPanel.ModDllPath));

                    SaveProfile(Properties.Settings.Default.LastLoadedProfile, false);
                }
                else
                {
                    UserMessage errorMessage = new UserMessage("Source file does not exist.", true);
                    errorMessage.Show();
                    errorMessage.BringToFront();
                }
            }
            catch (Exception ex)
            {
                UserMessage errorMessage = new UserMessage($"An error occurred: {ex.Message}", true);
                errorMessage.Show();
                errorMessage.BringToFront();
            }
        }

        private void CustomMapsDownloadButton_Click(object sender, EventArgs e)
        {
            if (_customMapsInstance == null || _customMapsInstance.IsDisposed)
            {
                _customMapsInstance = new CustomMaps(this);
                _customMapsInstance.FormClosed += (s, args) => _customMapsInstance = null;
                _customMapsInstance.Show();
            }
            else
            {
                _customMapsInstance.BringToFront();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
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
