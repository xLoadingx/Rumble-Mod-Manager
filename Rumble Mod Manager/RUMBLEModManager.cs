namespace Rumble_Mod_Manager
{
    using Guna.UI2.WinForms;
    using Microsoft.VisualBasic;
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Drawing.Drawing2D;
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

        protected override void OnLayoutRestored()
        {
            Control[] targets = { this, panel1, panel2 };

            ApplyAllRoundedCorners();
            this.ResizeBegin += (s, e) =>
            {
                foreach (var ctrl in targets)
                    ctrl.Region = null;

                foreach (Control ctrl in panel2.Controls)
                    ctrl.Region = null;
            };
            this.ResizeEnd += (s, e) =>
            {
                ApplyAllRoundedCorners();
            };
        }

        private void ApplyAllRoundedCorners()
        {
            Control[] targets = { this, panel1, panel2 };

            foreach (var ctrl in targets)
                ApplyRoundedCorners(ctrl);

            foreach (Control control in panel2.Controls)
            {
                if (control is ModPanelControl)
                    ApplyRoundedCorners(control);
            }
        }


        private async void StartManager()
        {
            string url = "https://raw.githubusercontent.com/xLoadingx/mod-maps/main/modMapping.json";
            modMappings = await GetModMappingsAsync(url);

            LoadCustomFont();

            if (ProfileSystem.AllProfiles.Count == 0)
            {
                var defaultProfile = new ModProfile
                {
                    Name = "Default",
                    EnabledModIds = Directory.GetFiles(ProfileSystem.ModsDirectory, "*.dll")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToList()
                };

                ProfileSystem.SaveProfile(defaultProfile);
                Properties.Settings.Default.LastLoadedProfile = defaultProfile.Name;
                Properties.Settings.Default.Save();
            }

            string name = Properties.Settings.Default.LastLoadedProfile;
            ModProfile profile = ProfileSystem.AllProfiles.FirstOrDefault(p => p.Name == name);

            if (profile != null)
                ProfileSystem.ApplyProfile(profile);

            panel2.AllowDrop = true;
            panel2.DragEnter += new DragEventHandler(panel2_DragEnter);
            panel2.DragLeave += new EventHandler(panel2_DragLeave);
            panel2.DragDrop += new DragEventHandler(panel2_DragDrop);
        }

        //private async void SyncModsFolderToProfile()
        //{
        //    if (ProfileSystem.CurrentProfile == null)
        //        return;

        //    string modsDir = ProfileSystem.ModsDirectory;

        //    foreach (string filePath in Directory.GetFiles(modsDir, "*.dll"))
        //    {
        //        string modId = Path.GetFileNameWithoutExtension(filePath);

        //        if (!ProfileSystem.CurrentProfile.EnabledModIds.Contains(modId))
        //        {
        //            ProfileSystem.CurrentProfile.EnabledModIds.Add(modId);
        //        }
        //    }

        //    ProfileSystem.SaveProfile(ProfileSystem.CurrentProfile);
        //    DisplayMods(ProfileSystem.CurrentProfile);
        //}

        //private async void CheckForModUpdates()
        //{
        //    List<ModPanelControl> currentMods = new List<ModPanelControl>(allMods);

        //    for (int i = 0; i < currentMods.Count; i++)
        //    {
        //        ModPanelControl mod = currentMods[i];

        //        if (mod != null && mod.Outdated)
        //        {
        //            await DownloadModFromInternet(mod.Mod, this, mod.Enabled, true, showMessage: false);
        //            mod.Outdated = false;
        //            mod.ImageButton.Image = null;
        //            mod.ImageButton.BackColor = Color.Lime;
        //        }
        //    }

        //    ProfileSystem.SaveProfile(ProfileSystem.CurrentProfile);
        //}

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

            DisplayMods(ProfileSystem.CurrentProfile);
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
            int panelWidth = panel2.ClientSize.Width - 20;
            int panelHeight = 84;
            int verticalMargin = 10;

            int panel2Width = panel2.ClientSize.Width;
            int totalPanelsWidth = panelWidth;
            int startX = 10;

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
                DisplayMods(ProfileSystem.CurrentProfile);
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

            string modsPath = ProfileSystem.ModsDirectory;
            string cacheDir = ProfileSystem.ModCacheDirectory;

            Directory.CreateDirectory(modsPath);
            Directory.CreateDirectory(cacheDir);

            var enabledModFilesTask = Task.Run(() =>
                Directory.GetFiles(modsPath, "*.dll")
                    .OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant())
                    .ToArray());

            var disabledModFilesTask = Task.Run(() =>
                Directory.GetFiles(cacheDir, "*.dll")
                    .OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant())
                    .ToArray());

            var enabledMods = await enabledModFilesTask;
            var disabledMods = await disabledModFilesTask;

            var enabledSet = loadedProfile.EnabledModIds.Select(x => x.ToLowerInvariant()).ToHashSet();
            var disabledSet = loadedProfile.DisabledModIds.Select(x => x.ToLowerInvariant()).ToHashSet();

            WelcomeLabel.Visible = enabledSet.Count == 0 && disabledSet.Count == 0;

            panel2.Controls.Clear();
            allMods.Clear();

            foreach (var panel in preloadedModPanels.OrderBy(p => p.ModNameLabel.ToLowerInvariant()))
            {
                string modId = panel.ModNameLabel.ToLowerInvariant();
                bool isInProfile = enabledSet.Contains(modId) || disabledSet.Contains(modId);

                if (!isInProfile)
                    continue;

                panel.ModEnabled = enabledMods.Contains(modId);

                panel.Click -= ModPanel_Click;
                panel.Click += ModPanel_Click;

                var updateButton = panel.Controls.OfType<Guna2ImageButton>().FirstOrDefault();
                if (updateButton != null)
                {
                    updateButton.Click -= async (s, e) => await UpdateButton_Click(panel);
                    updateButton.Click += async (s, e) => await UpdateButton_Click(panel);
                }

                panel.FavoritedStar.MouseUp += (sender, e) =>
                {
                    var favs = ProfileSystem.CurrentProfile.FavoritedModIds;

                    if (panel.FavoritedStar.Checked && !favs.Contains(modId))
                        favs.Add(modId);
                    else if (!panel.FavoritedStar.Checked)
                        favs.Remove(modId);

                    ProfileSystem.SaveProfile(ProfileSystem.CurrentProfile);

                    AdjustPanelLocations();
                };

                panel.FavoritedStar.Checked = loadedProfile.FavoritedModIds.Contains(modId);

                AddOrUpdateModPanel(panel);
            }

            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Maximum = 0;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;

            AdjustPanelLocations();

            allMods = panel2.Controls.OfType<ModPanelControl>().ToList();

            foreach (Control control in panel2.Controls)
            {
                if (control is ModPanelControl)
                    ApplyRoundedCorners(control);
            }

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
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK || result == DialogResult.Abort)
                    {
                        foreach (var selectedPanel in selectedPanels)
                        {
                            string modId = selectedPanel.ModNameLabel.ToLowerInvariant();
                            string modFilePath = selectedPanel.ModDllPath;

                            if (result == DialogResult.Abort)
                            {
                                if (File.Exists(modFilePath))
                                    File.Delete(modFilePath);

                                foreach (var profile in ProfileSystem.AllProfiles)
                                {
                                    profile.EnabledModIds.Remove(modId);
                                    profile.DisabledModIds.Remove(modId);
                                    profile.FavoritedModIds.Remove(modId);
                                }
                            } else
                            {
                                ProfileSystem.CurrentProfile.EnabledModIds.Remove(modId);
                                ProfileSystem.CurrentProfile.DisabledModIds.Remove(modId);
                                ProfileSystem.CurrentProfile.FavoritedModIds.Remove(modId);
                            }

                            preloadedModPanels.Remove(selectedPanel);
                            allMods.Remove(selectedPanel);
                            panel2.Controls.Remove(selectedPanel);
                        }

                        if (result == DialogResult.Abort)
                        {
                            foreach (var profile in ProfileSystem.AllProfiles)
                                ProfileSystem.SaveProfile(profile);
                        } else
                            ProfileSystem.SaveProfile(ProfileSystem.CurrentProfile);
                        DisplayMods(ProfileSystem.CurrentProfile);
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
                                linkLabel1.Visible = true;
                            }
                            else
                            {
                                DependenciesLabel.Text = "Dependencies: None";
                                linkLabel1.Visible = false;
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
                            ProfileSystem.SaveProfile(ProfileSystem.CurrentProfile);

                            string updatedVersionStr = (string)GetMelonLoaderModInfo(Path.Combine(Properties.Settings.Default.RumblePath, "Mods"), parentControl.ModNameLabel + ".dll", MelonLoaderModInfoType.Version);
                            int updatedVersion = int.Parse(updatedVersionStr.Replace(".", ""), CultureInfo.InvariantCulture);

                            if (updatedVersion < latestVersion)
                            {
                                UserMessage brokenModMessage = new UserMessage("The mod has been updated successfully, but it still shows as out of date. This might indicate the mod's versioning is broken and will continue to show as out of date until the developer fixes it.", true);
                                brokenModMessage.Show();
                                brokenModMessage.BringToFront();
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

                ProfileSystem.SaveProfile(ProfileSystem.CurrentProfile);
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
            DisplayMods(ProfileSystem.CurrentProfile);
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

        private void ApplyRoundedCorners(Control ctrl, int radius = 20)
        {
            if (ctrl.Width <= 0 || ctrl.Height <= 0)
                return;

            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(ctrl.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(ctrl.Width - radius, ctrl.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, ctrl.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();

                ctrl.Region?.Dispose();
                ctrl.Region = new Region(path);
            }
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            if (panel1.Height > 551)
            {
                // Expanded Mode
                ModDescriptionLabel.Top = 163;
                ModDescriptionLabel.Anchor = AnchorStyles.Top;
                ModDescriptionLabel.Size = new Size(376, 96);

                DependenciesLabel.Left = 17;
                DependenciesLabel.Size = new Size(376, 153);
            }
            else
            {
                // Compact Mode
                ModDescriptionLabel.Top = DependenciesLabel.Top;
                ModDescriptionLabel.Size = new Size(155, 155);
                ModDescriptionLabel.Anchor = AnchorStyles.Bottom;

                DependenciesLabel.Left = 175;
                DependenciesLabel.Size = new Size(217, 153);
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            if (m.Msg == WM_NCHITTEST)
            {
                Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                int grip = 10;

                if (pos.X < grip && pos.Y < grip)
                    m.Result = (IntPtr)HTTOPLEFT;
                else if (pos.X > Width - grip && pos.Y < grip)
                    m.Result = (IntPtr)HTTOPRIGHT;
                else if (pos.X < grip && pos.Y > Height - grip)
                    m.Result = (IntPtr)HTBOTTOMLEFT;
                else if (pos.X > Width - grip && pos.Y > Height - grip)
                    m.Result = (IntPtr)HTBOTTOMRIGHT;
                else if (pos.X < grip)
                    m.Result = (IntPtr)HTLEFT;
                else if (pos.X > Width - grip)
                    m.Result = (IntPtr)HTRIGHT;
                else if (pos.Y < grip)
                    m.Result = (IntPtr)HTTOP;
                else if (pos.Y > Height - grip)
                    m.Result = (IntPtr)HTBOTTOM;
                else
                    base.WndProc(ref m);

                return;
            }

            base.WndProc(ref m);
        }

        private void RUMBLEModManager_Resize(object sender, EventArgs e)
        {
            AdjustPanelLocations();
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
}