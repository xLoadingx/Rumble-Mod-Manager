namespace Rumble_Mod_Manager
{
    using Guna.UI2.WinForms;
    using Microsoft.VisualBasic;
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Drawing.Text;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using static Rumble_Mod_Manager.ThunderstoreMods;
    using static System.Runtime.InteropServices.JavaScript.JSType;
    using Control = Control;

    public partial class RUMBLEModManager : PersistentForm
    {
        private List<ModPanelControl> selectedPanels = new List<ModPanelControl>();
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        private bool isLoadingDisplay = false;

        private static CustomMaps _customMapsInstance;
        private static ThunderstoreMods _thunderstoreModsInstance;
        private static Settings _settingsInstance;

        private Process gameProcess;

        Dictionary<string, string> modMappings = new Dictionary<string, string>();
        public List<ModPanelControl> preloadedModPanels = new List<ModPanelControl>();
        public List<ModPanelControl> allMods = new List<ModPanelControl>();

        public RUMBLEModManager(List<ModPanelControl> preloadedPanels)
        {
            InitializeComponent();
            this.Text = "RUMBLE Mod Manager";
            SettingsButton.Controls.Add(pictureBox1);
            pictureBox1.Location = new Point(SettingsButton.Width - pictureBox1.PreferredSize.Width - 23, SettingsButton.Height - pictureBox1.PreferredSize.Height - 20);
            pictureBox1.BackColor = Color.Transparent;
            gameCheckTimer.Start();

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

            await LoadProfile(Properties.Settings.Default.LastLoadedProfile, this);

            if (Properties.Settings.Default.AutoModUpdating)
                CheckForModUpdates();

            panel2.AllowDrop = true;
            panel2.DragEnter += new DragEventHandler(panel2_DragEnter);
            panel2.DragLeave += new EventHandler(panel2_DragLeave);
            panel2.DragDrop += new DragEventHandler(panel2_DragDrop);
        }

        private async void CheckForModUpdates()
        {
            List<ModPanelControl> currentMods = new List<ModPanelControl>(allMods);

            for (int i = 0; i < currentMods.Count; i++)
            {
                ModPanelControl mod = currentMods[i];

                if (mod != null && mod.Outdated)
                {
                    await DownloadModFromInternet(mod.Mod, this, mod.Enabled, true, showMessage: false);
                    mod.Outdated = false;
                    mod.ImageButton.Image = null;
                    mod.ImageButton.BackColor = Color.Lime;
                }
            }

            SaveProfile(Properties.Settings.Default.LastLoadedProfile, false);
        }

        private void panel2_DragLeave(object sender, EventArgs e)
        {
            panel2.BackColor = Color.FromArgb(40, 40, 40);
        }

        private void panel2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0 && files[0].EndsWith(".zip"))
                {
                    e.Effect = DragDropEffects.Copy;
                    panel2.BackColor = Color.LightBlue;
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
                        await InstallMod(file, true, this);
                        UserMessage successMessage = new UserMessage($"Mod '{Path.GetFileNameWithoutExtension(file)}' installed successfully!", true);
                        successMessage.Show();
                        successMessage.BringToFront();
                    }
                }

                panel2.BackColor = Color.FromArgb(40, 40, 40);
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string url = (string)e.Data.GetData(DataFormats.Text);
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute) && url.Contains("thunderstore.io/package/download/"))
                {
                    await DownloadModFromURL(url, this);
                }
                else
                {
                    UserMessage errorMessage = new UserMessage("Invalid link or unsupported file type. Please drop a valid Thunderstore link or a .zip file.", true, showCopyDialog: true);
                    errorMessage.Show();
                    errorMessage.BringToFront();
                }

                panel2.BackColor = Color.FromArgb(40, 40, 40);
            }

            LoadProfile(Properties.Settings.Default.LastLoadedProfile, this);
        }

        public static async Task DownloadModFromURL(string url, RUMBLEModManager manager)
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

                    ModPanelControl panel = await InstallMod(tempZipPath, true, manager);

                    var updateButton = panel.Controls.OfType<Guna2ImageButton>().FirstOrDefault();
                    if (updateButton != null)
                    {
                        updateButton.Click -= async (s, e) => await manager.UpdateButton_Click(panel);
                        updateButton.Click += async (s, e) => await manager.UpdateButton_Click(panel);
                    }

                    UserMessage successMessage = new UserMessage("Mod downloaded and installed successfully from URL!", true);
                    successMessage.Show();
                    successMessage.BringToFront();
                }
            }
            catch (Exception ex)
            {
                UserMessage errorMessage = new UserMessage($"An error occurred while downloading or installing the mod: {ex.Message}", true, showCopyDialog: true);
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
                UserMessage errorMessage = new UserMessage($"An error occurred: {ex.Message}", true, showCopyDialog: true);
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
            LaunchGame.Font = new Font(privateFonts.Families[1], 12.0f, FontStyle.Regular);
            ToggleModLabel.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            ThunderstoreButton.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            CustomMapsDownloadButton.Font = new Font(privateFonts.Families[1], 22.0F, FontStyle.Regular);
            SettingsButton.Font = new Font(privateFonts.Families[1], 24.0F, FontStyle.Regular);
            ModNameLabel.Font = new Font(privateFonts.Families[1], 24.0F, FontStyle.Regular);
            ModVersionLabel.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            linkLabel1.Font = new Font(privateFonts.Families[1], 18.0F, FontStyle.Regular);
            ModAuthorLabel.Font = new Font(privateFonts.Families[1], 15.0F, FontStyle.Regular);
            DependenciesLabel.Font = new Font(privateFonts.Families[1], 11.0F, FontStyle.Regular);
            ModDescriptionLabel.Font = new Font(privateFonts.Families[1], 13.0F, FontStyle.Regular);
            WelcomeLabel.Font = new Font(privateFonts.Families[1], 20.0F, FontStyle.Regular);
            FormTitle.Font = new Font(privateFonts.Families[0], 15.0f, FontStyle.Regular);
            button1.Font = new Font(privateFonts.Families[1], 12.0f, FontStyle.Regular);
            button2.Font = new Font(privateFonts.Families[1], 12.0f, FontStyle.Regular);
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

        public void AddOrUpdateModPanel(ModPanelControl modPanel)
        {
            var existingPanel = allMods.FirstOrDefault(mod => mod.ModNameLabel == modPanel.ModNameLabel);
            if (existingPanel != null)
            {
                existingPanel.ModEnabled = modPanel.ModEnabled;
                existingPanel.Outdated = modPanel.Outdated;
                existingPanel.FavoritedStar.Checked = modPanel.FavoritedStar.Checked;
            }
            else
            {
                allMods.Add(modPanel);
                panel2.Controls.Add(modPanel);
            }
        }

        public void AdjustPanelLocations()
        {
            int panelWidth = 580;
            int panelHeight = 84;
            int verticalMargin = 10;

            int panel2Width = panel2.ClientSize.Width;
            int totalPanelsWidth = panelWidth;
            int startX = (panel2Width - totalPanelsWidth) / 2;

            panel2.AutoScroll = false;

            var sortedPanels = panel2.Controls.OfType<ModPanelControl>()
                .OrderByDescending(p => p.FavoritedStar.Checked)
                .ThenBy(p => p.ModNameLabel.ToLowerInvariant())
                .ToList();

            int currentIndex = 0;

            foreach (var modPanel in sortedPanels)
            {
                panel2.Controls.SetChildIndex(modPanel, currentIndex);

                modPanel.Size = new Size(panelWidth, panelHeight);
                modPanel.Location = new Point(startX, verticalMargin + currentIndex * (panelHeight + verticalMargin));
                currentIndex++;
            }

            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Maximum = 0;
            panel2.VerticalScroll.Visible = false;

            panel2.AutoScrollPosition = new Point(0, 0);
            panel2.AutoScroll = true;
        }

        public async Task LoadMods(ModProfile loadedProfile)
        {
            ToggleModDisplay(false);
            if (selectedPanels.Count != 0)
            {
                foreach (var panel in selectedPanels)
                {
                    panel.BackColor = Color.FromArgb(30, 30, 30);
                }

                selectedPanels.Clear();
            }

            await DisplayMods(loadedProfile);
        }

        private void ModManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R && !isLoadingDisplay)
            {
                LoadProfile(Properties.Settings.Default.LastLoadedProfile, this);
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

        public async Task DisplayMods(ModProfile loadedProfile)
        {
            isLoadingDisplay = true;

            var customMapsPath = Path.Combine(Properties.Settings.Default.RumblePath, "UserData", "CustomMultiplayerMaps", "Maps");
            if (false /*Directory.Exists(customMapsPath) && CustomMapsCache.MapsByPage.Count > 0*/ ) // Custom Maps is being rebuilt
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

            var enabledModFilesTask = Task.Run(() =>
                Directory.GetFiles(modsPath, "*.dll")
                    .OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant())
                    .ToArray());

            var disabledModFilesTask = Task.Run(() =>
                Directory.GetFiles(disabledModsPath, "*.dll")
                    .OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant())
                    .ToArray());

            var enabledModFiles = await enabledModFilesTask;
            var disabledModFiles = await disabledModFilesTask;

            WelcomeLabel.Visible = enabledModFiles.Length == 0 && disabledModFiles.Length == 0;

            preloadedModPanels = preloadedModPanels.OrderBy(p => p.ModNameLabel.ToLowerInvariant()).ToList();

            foreach (var panel in preloadedModPanels)
            {
                string panelName = panel.ModNameLabel.ToLowerInvariant();
                bool isInEnabledMods = enabledModFiles.Any(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant() == panelName);
                bool isInDisabledMods = disabledModFiles.Any(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant() == panelName);


                ModState profileMod = null;
                if (loadedProfile != null)
                {
                    profileMod = loadedProfile.enabledMods
                    .Concat(loadedProfile.disabledMods)
                    .FirstOrDefault(mod => mod.ModName.ToLowerInvariant() == panelName);

                    if (isInEnabledMods || isInDisabledMods)
                    {
                        panel.ModEnabled = isInEnabledMods;

                        if (!panel2.Controls.Contains(panel))
                        {
                            panel.Click -= ModPanel_Click;
                            panel.Click += ModPanel_Click;
                            if (profileMod != null)
                            {
                                panel.FavoritedStar.MouseUp += (sender, e) =>
                                {
                                    SaveProfile(Properties.Settings.Default.LastLoadedProfile, false);
                                    AddOrUpdateModPanel(panel);
                                    LoadProfile(Properties.Settings.Default.LastLoadedProfile, this);
                                };
                            }
                            else
                            {
                                panel.FavoritedStar.Visible = false;
                            }

                            var updateButton = panel.Controls.OfType<Guna2ImageButton>().FirstOrDefault();
                            if (updateButton != null)
                            {
                                updateButton.Click -= async (s, e) => await UpdateButton_Click(panel);
                                updateButton.Click += async (s, e) => await UpdateButton_Click(panel);
                            }

                            if (profileMod != null)
                            {
                                panel.FavoritedStar.Checked = profileMod.Favorited;
                            }

                            AddOrUpdateModPanel(panel);
                        }
                    }

                    bool existsInAnyList = loadedProfile.enabledMods
                        .Concat(loadedProfile.disabledMods)
                        .Any(mod => mod.ModName == panel.ModNameLabel);

                    if (!existsInAnyList && panel2.Controls.Contains(panel))
                    {
                        panel2.Controls.Remove(panel);
                        allMods.Remove(panel);
                    }
                }
                else
                {
                    if (isInEnabledMods || isInDisabledMods)
                    {
                        panel.ModEnabled = isInEnabledMods;

                        if (!panel2.Controls.Contains(panel))
                        {
                            panel.Click -= ModPanel_Click;
                            panel.Click += ModPanel_Click;

                            var updateButton = panel.Controls.OfType<Guna2ImageButton>().FirstOrDefault();
                            if (updateButton != null)
                            {
                                updateButton.Click -= async (s, e) => await UpdateButton_Click(panel);
                                updateButton.Click += async (s, e) => await UpdateButton_Click(panel);
                            }

                            panel.FavoritedStar.Visible = false;

                            AddOrUpdateModPanel(panel);
                        }
                    }
                }
            }

            AdjustPanelLocations();
            allMods = panel2.Controls.OfType<ModPanelControl>().ToList();

            isLoadingDisplay = false;
        }

        private async Task UpdateButton_Click(ModPanelControl panel)
        {
            await button1_Click(panel, EventArgs.Empty);
        }

        private void Uninstall_Button_Click(object sender, EventArgs e)
        {
            if (selectedPanels.Count != 0)
            {
                List<string> modFilePaths = new List<string>();
                List<string> modNames = new List<string>();

                foreach (var selectedPanel in selectedPanels)
                {
                    if (selectedPanel.ModDllPath != null)
                    {
                        modFilePaths.Add(selectedPanel.ModDllPath);
                        if (selectedPanel.Mod != null)
                        {
                            modNames.Add(selectedPanel.Mod.Name);
                        }
                        else
                        {
                            modNames.Add(Path.GetFileNameWithoutExtension(selectedPanel.ModDllPath));
                        }
                    }
                    else
                    {
                        UserMessage errorMessage = new UserMessage("Something went wrong. A file path does not exist.", true, showCopyDialog: true);
                        errorMessage.Show();
                        errorMessage.BringToFront();
                        return;
                    }
                }

                using (Uninstall form = new Uninstall(modNames, modFilePaths))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var selectedPanel in selectedPanels)
                        {
                            string modFilePath = selectedPanel.ModDllPath;

                            if (File.Exists(modFilePath))
                            {
                                File.Delete(modFilePath);
                            }

                            preloadedModPanels.Remove(selectedPanel);
                            allMods.Remove(selectedPanel);
                            panel2.Controls.Remove(selectedPanel);
                        }

                        SaveProfile(Properties.Settings.Default.LastLoadedProfile, false);
                        LoadProfile(Properties.Settings.Default.LastLoadedProfile, this);
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
            catch
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

        public void SaveProfile(string profileName, bool showMessage = true)
        {
            var profile = new ModProfile
            {
                ProfileName = profileName,
                enabledMods = new List<ModState>(),
                disabledMods = new List<ModState>()
            };

            foreach (ModPanelControl modPanel in allMods)
            {
                ModState state = new ModState
                {
                    ModName = modPanel.ModNameLabel,
                    Outdated = modPanel.Outdated,
                    Favorited = modPanel.FavoritedStar.Checked
                };

                if (modPanel.Mod != null)
                {
                    state.DownloadLink = modPanel.Mod.ModPageUrl;
                }

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

        public static async Task LoadProfile(string profileName, RUMBLEModManager modManagerInstance)
        {
            string modsPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mods");
            string disabledModsPath = Path.Combine(Properties.Settings.Default.RumblePath, "DisabledMods");
            string inactiveModsPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", "InactiveMods");

            if (!Directory.Exists(inactiveModsPath))
            {
                Directory.CreateDirectory(inactiveModsPath);
            }

            if (string.IsNullOrEmpty(Properties.Settings.Default.LastLoadedProfile) || !File.Exists(Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", $"{Properties.Settings.Default.LastLoadedProfile}_profile.json")))
            {
                if (modManagerInstance != null)
                {
                    await modManagerInstance.LoadMods(null);
                }
            }
            else
            {
                string profilePath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", $"{profileName}_profile.json");
                string json = await File.ReadAllTextAsync(profilePath);
                var profile = JsonConvert.DeserializeObject<ModProfile>(json);

                MoveAllFiles(modsPath, inactiveModsPath, profile);
                MoveAllFiles(disabledModsPath, inactiveModsPath, profile);

                Properties.Settings.Default.LastLoadedProfile = profileName;
                Properties.Settings.Default.Save();

                foreach (var mod in profile.enabledMods)
                {
                    string modFilePath = Path.Combine(inactiveModsPath, $"{mod.ModName}.dll");
                    if (File.Exists(modFilePath))
                    {
                        await Task.Run(() => File.Move(modFilePath, Path.Combine(modsPath, $"{mod.ModName}.dll")));
                    }
                }

                foreach (var mod in profile.disabledMods)
                {
                    string modFilePath = Path.Combine(inactiveModsPath, $"{mod.ModName}.dll");
                    if (File.Exists(modFilePath))
                    {
                        await Task.Run(() => File.Move(modFilePath, Path.Combine(disabledModsPath, $"{mod.ModName}.dll")));
                    }
                }

                if (modManagerInstance != null)
                {
                    await modManagerInstance.LoadMods(profile);
                }
            }
        }

        private static void MoveAllFiles(string sourceDir, string targetDir, ModProfile profile)
        {
            foreach (var mod in profile.enabledMods.Concat(profile.disabledMods))
            {
                string fileName = mod.ModName + ".dll";
                string sourceFile = Path.Combine(sourceDir, fileName);
                string destinationFile = Path.Combine(targetDir, fileName);

                if (File.Exists(sourceFile))
                {
                    if (File.Exists(destinationFile))
                        File.Delete(destinationFile);

                    File.Move(sourceFile, destinationFile);
                }
            }
        }

        public void ModPanel_Click(object sender, EventArgs e)
        {
            if (Path.Exists(Path.Combine(Properties.Settings.Default.RumblePath, "Mods")))
            {
                ModPanelControl panel = sender as ModPanelControl;
                if (panel != null)
                {
                    if (selectedPanels.Count == 1 && selectedPanels.Contains(panel))
                    {
                        panel.BackColor = panel.ModEnabled ? Color.FromArgb(30, 30, 30) : Color.FromArgb(192, 0, 0);
                        panel.label1.ForeColor = panel.ModEnabled ? Color.DimGray : Color.White;
                        selectedPanels.Clear();

                        ToggleModDisplay(false);

                        return;
                    }

                    bool shiftHeld = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                    bool ctrlHeld = (Control.ModifierKeys & Keys.Control) == Keys.Control;

                    if (shiftHeld && selectedPanels.Count > 0)
                    {
                        var firstPanel = selectedPanels[0];
                        int firstIndex = allMods.IndexOf(firstPanel);
                        int clickedIndex = allMods.IndexOf(panel);

                        int startIndex = Math.Min(firstIndex, clickedIndex);
                        int endIndex = Math.Max(firstIndex, clickedIndex);

                        foreach (var p in allMods.Skip(startIndex).Take(endIndex - startIndex + 1))
                        {
                            if (!selectedPanels.Contains(p))
                            {
                                selectedPanels.Add(p);
                                panel.label1.ForeColor = Color.White;
                                p.BackColor = Color.LightBlue;
                            }
                        }
                    }
                    else if (ctrlHeld)
                    {
                        if (selectedPanels.Contains(panel))
                        {
                            panel.BackColor = panel.ModEnabled ? Color.FromArgb(30, 30, 30) : Color.FromArgb(192, 0, 0);
                            selectedPanels.Remove(panel);
                        }
                        else
                        {
                            panel.label1.ForeColor = Color.White;
                            panel.BackColor = Color.LightBlue;
                            selectedPanels.Add(panel);
                        }
                    }
                    else
                    {
                        foreach (var selectedPanel in selectedPanels)
                        {
                            selectedPanel.BackColor = selectedPanel.ModEnabled ? Color.FromArgb(30, 30, 30) : Color.FromArgb(192, 0, 0);
                            panel.label1.ForeColor = panel.ModEnabled ? Color.DimGray : Color.White;
                        }

                        selectedPanels.Clear();

                        panel.label1.ForeColor = Color.White;
                        panel.BackColor = Color.LightBlue;
                        selectedPanels.Add(panel);
                    }

                    if (selectedPanels.Count > 0)
                    {
                        ToggleModDisplay(true);

                        ToggleModLabel.Visible = true;

                        ToggleModButton.BackgroundImage = selectedPanels.All(p => p.ModEnabled) ? Properties.Resources.green : Properties.Resources.red;
                        ToggleModLabel.BackColor = selectedPanels.All(p => p.ModEnabled) ? Color.FromArgb(128, 255, 128) : Color.FromArgb(192, 0, 0);
                        ToggleModLabel.ForeColor = selectedPanels.All(p => p.ModEnabled) ? Color.Green : Color.Maroon;

                        if (selectedPanels.Count == 1)
                        {
                            var modPanel = selectedPanels[0];
                            ModNameLabel.Text = modPanel.ModNameLabel;

                            string versionString = modPanel.label2.Text;
                            string pattern = @"\d+\.\d+\.\d+";

                            Match match = Regex.Match(versionString, pattern);

                            if (match.Success)
                            {
                                ModVersionLabel.Text = $"Version {match.Value}";
                            }
                            else
                            {
                                ModVersionLabel.Text = $"Unknown Version";
                            }

                            string modAuthor = modPanel.Mod?.Author ?? (string)GetMelonLoaderModInfo(
                                Path.Combine(
                                    Properties.Settings.Default.RumblePath,
                                    modPanel.ModEnabled ? "Mods" : "DisabledMods"),
                                modPanel.ModNameLabel + ".dll",
                                MelonLoaderModInfoType.Author) ?? "Unknown Author";

                            ModAuthorLabel.Text = $"By {modAuthor}";
                            ModDescriptionLabel.Text = modPanel.Mod?.Description ?? "Description not available.";

                            if (modPanel.Mod != null)
                            {
                                var dependencies = modPanel.Mod.Dependencies
                                    .Where(d => !d.StartsWith("MelonLoader", StringComparison.OrdinalIgnoreCase))
                                    .Select(d =>
                                    {
                                        var parts = d.Split('-').ToList();
                                        if (parts.Count == 2) return $"{parts[0]} v{parts[1]}";
                                        if (parts.Count > 2) return $"{string.Join(" ", parts.Skip(1).Take(parts.Count - 2))} v{parts.Last()}";
                                        return d;
                                    });

                                DependenciesLabel.Text = $"Dependencies:\n{string.Join("\n", dependencies)}";
                            }
                            else
                            {
                                DependenciesLabel.Text = "Dependencies: None";
                            }

                            ModPictureDisplay.Image = modPanel.Mod?.ModImage ?? Properties.Resources.UnknownMod;
                        }
                        else
                        {
                            ModNameLabel.Text = $"{selectedPanels.Count} mods selected";

                            var authors = selectedPanels.Select(p =>
                                p.Mod?.Author ?? (string)GetMelonLoaderModInfo(
                                    Path.Combine(
                                        Properties.Settings.Default.RumblePath,
                                        p.ModEnabled ? "Mods" : "DisabledMods"),
                                    p.ModNameLabel + ".dll",
                                    MelonLoaderModInfoType.Author) ?? "Unknown Author").Distinct();

                            var versions = selectedPanels.Select(p =>
                                (string)GetMelonLoaderModInfo(
                                    Path.Combine(
                                        Properties.Settings.Default.RumblePath,
                                        p.ModEnabled ? "Mods" : "DisabledMods"),
                                    p.ModNameLabel + ".dll",
                                    MelonLoaderModInfoType.Version) ?? p.Mod?.Version ?? "Unknown").Distinct();

                            ModVersionLabel.Text = versions.Count() == 1 ? $"Version {versions.First()}" : "Multiple versions";
                            ModAuthorLabel.Text = authors.Count() == 1 ? $"By {authors.First()}" : "Multiple authors";
                            ModDescriptionLabel.Text = "Multiple descriptions";

                            DependenciesLabel.Text = "Multiple dependencies";
                            ModPictureDisplay.Image = Properties.Resources.UnknownMod;
                        }
                    }
                    else
                    {
                        ToggleModDisplay(false);
                    }
                }
                else
                {
                    var errorMessage = new UserMessage("Panel is null", true, showCopyDialog: true);
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
                UserMessage errorMessage = new UserMessage("Please wait for mods to finish loading before accessing the Thunderstore mod page.", true, showCopyDialog: true);
                errorMessage.Show();
                errorMessage.BringToFront();
            }
        }

        private async Task button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is ModPanelControl parentControl)
                {
                    if (parentControl.Mod != null)
                    {
                        int currentVersion = int.Parse(parentControl.VersionString.Replace(".", ""), CultureInfo.InvariantCulture);
                        int latestVersion = int.Parse(parentControl.Mod.Version.Replace(".", ""), CultureInfo.InvariantCulture);

                        if (currentVersion < latestVersion)
                        {
                            await ThunderstoreMods.DownloadModFromInternet(parentControl.Mod, this, parentControl.ModEnabled, true);
                            SaveProfile(Properties.Settings.Default.LastLoadedProfile, false);

                            string updatedVersionStr = (string)GetMelonLoaderModInfo(Path.Combine(Properties.Settings.Default.RumblePath, "Mods"), parentControl.ModNameLabel + ".dll", MelonLoaderModInfoType.Version);
                            int updatedVersion = int.Parse(updatedVersionStr.Replace(".", ""), CultureInfo.InvariantCulture);

                            if (updatedVersion < latestVersion)
                            {
                                UserMessage brokenModMessage = new UserMessage("The mod has been updated successfully, but it still shows as out of date. This might indicate the mod is broken and will continue to show as out of date until the developer fixes it.", true);
                                brokenModMessage.Show();
                                brokenModMessage.BringToFront();
                            }

                            if (updatedVersion <= latestVersion)
                            {
                                parentControl.label2.Text = $"v{parentControl.Mod.Version} by {parentControl.Mod.Author}";
                                parentControl.VersionString = parentControl.Mod.Version;
                                parentControl.ImageButton.BackColor = Color.Lime;
                                parentControl.ImageButton.Image = null;
                            }

                            AddOrUpdateModPanel(parentControl);
                        }
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
                foreach (var panel in selectedPanels)
                {
                    panel.ModEnabled = !panel.ModEnabled;

                    string sourcePath = panel.ModEnabled
                        ? Path.Combine(Properties.Settings.Default.RumblePath, "DisabledMods", Path.GetFileName(panel.ModDllPath))
                        : Path.Combine(Properties.Settings.Default.RumblePath, "Mods", Path.GetFileName(panel.ModDllPath));

                    string destinationPath = panel.ModEnabled
                        ? Path.Combine(Properties.Settings.Default.RumblePath, "Mods")
                        : Path.Combine(Properties.Settings.Default.RumblePath, "DisabledMods");

                    string destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(panel.ModDllPath));

                    if (File.Exists(sourcePath))
                    {
                        if (!Directory.Exists(destinationPath))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }

                        if (File.Exists(destinationFilePath))
                        {
                            File.Delete(destinationFilePath);
                        }

                        File.Move(sourcePath, destinationFilePath, true);
                        panel.ModDllPath = Path.Combine(destinationPath, Path.GetFileName(panel.ModDllPath));
                        AddOrUpdateModPanel(panel);
                    }
                    else
                    {
                        UserMessage errorMessage = new UserMessage($"Source file for {panel.ModNameLabel} does not exist.", true, showCopyDialog: true);
                        errorMessage.Show();
                        errorMessage.BringToFront();
                    }

                    panel.BackColor = Color.LightBlue;
                }

                ToggleModButton.BackgroundImage = selectedPanels[0].ModEnabled ? Properties.Resources.green : Properties.Resources.red;
                ToggleModLabel.BackColor = selectedPanels[0].ModEnabled ? Color.FromArgb(128, 255, 128) : Color.FromArgb(192, 0, 0);
                ToggleModLabel.ForeColor = selectedPanels[0].ModEnabled ? Color.Green : Color.Maroon;

                SaveProfile(Properties.Settings.Default.LastLoadedProfile, false);
            }
            catch (Exception ex)
            {
                UserMessage errorMessage = new UserMessage($"An error occurred: {ex.Message}", true, showCopyDialog: true);
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

        private async void LaunchGameModded_Click(object sender, EventArgs e)
        {
            LaunchRumble(true);
        }

        private async void LaunchGameVanilla_Click(object sender, EventArgs e)
        {
            LaunchRumble(false);
        }

        private async void LaunchRumble(bool modded)
        {
            string steamInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string;

            string arguments = modded ? "-applaunch 890550" : "-applaunch 890550 --no-mods --melonloader.hideconsole --melonloader.disablestartscreen";

            if (steamInstallPath == null)
            {
                steamInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null) as string;
            }

            string steamExePath = Path.Combine(steamInstallPath, "steam.exe");

            if (!File.Exists(steamExePath))
            {
                UserMessage error = new UserMessage("Steam executable not found at the specified path.", true, showCopyDialog: true);
                error.Show();
                return;
            }

            try
            {
                if (gameProcess == null)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = steamExePath,
                        Arguments = arguments,
                        UseShellExecute = true
                    });
                }
                else
                {
                    if (!gameProcess.HasExited)
                    {
                        gameProcess.Kill();
                        gameProcess.WaitForExitAsync();
                    }
                    gameProcess = null;
                }
            }
            catch (Exception ex)
            {
                UserMessage error = new UserMessage($"Failed to launch or stop RUMBLE: {ex.Message}", true, showCopyDialog: true);
                error.Show();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (debounceTimer.Enabled)
            {
                debounceTimer.Stop();
            }
            debounceTimer.Start();
        }

        private void debounceTimer_Tick(object sender, EventArgs e)
        {
            debounceTimer.Stop();

            string searchText = textBox1.Text.ToLower();
            LoadProfile(Properties.Settings.Default.LastLoadedProfile, this);
            FilterMods(searchText);
        }

        private void FilterMods(string searchText)
        {
            var filteredPanels = preloadedModPanels
                  .Where(panel =>
                      panel.ModNameLabel.ToLower().Contains(searchText.ToLower()) ||
                      panel.label2.Text.ToLower().Contains(searchText.ToLower())
                  )
                  .ToList();

            panel2.Controls.Clear();

            foreach (var panel in filteredPanels)
            {
                panel2.Controls.Add(panel);
            }

            AdjustPanelLocations();
        }

        private void ToggleModDisplay(bool toggle)
        {
            UninstallButton.Visible = toggle;
            ModNameLabel.Visible = toggle;
            ModVersionLabel.Visible = toggle;
            ModAuthorLabel.Visible = toggle;
            ModDescriptionLabel.Visible = toggle;
            linkLabel1.Visible = toggle;
            DependenciesLabel.Visible = toggle;
            if (!toggle)
                ModPictureDisplay.Image = null;
            ModPictureDisplay.Visible = toggle;
            ToggleModButton.Visible = toggle;
            ToggleModLabel.Visible = toggle;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LaunchPage.CheckForUpdates(true);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void gameCheckTimer_Tick(object sender, EventArgs e)
        {
            Process existing = Process.GetProcessesByName("RUMBLE").FirstOrDefault();

            if (existing != null)
            {
                gameProcess = existing;
                LaunchGame.Text = "Stop Game";
                button2.Visible = false;
                LaunchGame.BackColor = Color.Red;
                LaunchGame.ForeColor = Color.Maroon;
            }
            else
            {
                gameProcess = null;
                LaunchGame.Text = "Launch Modded";
                button2.Visible = true;
                LaunchGame.BackColor = Color.Lime;
                LaunchGame.ForeColor = Color.Green;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (selectedPanels.Count > 1)
            {
                UserMessage msg = new UserMessage($"You have more than one mod selected ({selectedPanels.Count} Mods). Do you want to open all of them?", showYesNo: true);
                if (msg.ShowDialog() == DialogResult.No)
                {
                    return;
                }
            }

            foreach (var selectedPanel in selectedPanels.Where(p => p.OnlineModLink != null))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = selectedPanel.OnlineModLink,
                    UseShellExecute = true
                });
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
        public bool Favorited { get; set; }
        public string DownloadLink { get; set; }
    }

    public class ModProfile
    {
        public string ProfileName { get; set; }
        public List<ModState> enabledMods { get; set; }
        public List<ModState> disabledMods { get; set; }
    }
}