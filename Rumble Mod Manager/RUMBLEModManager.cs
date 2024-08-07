namespace Rumble_Mod_Manager
{
    using Microsoft.Windows.Themes;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Diagnostics;
    using System.Drawing.Text;
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

        public RUMBLEModManager()
        {
            InitializeComponent();
            this.Text = "RUMBLE Mod Manager";
            SettingsButton.Controls.Add(pictureBox1);
            pictureBox1.Location = new Point(SettingsButton.Width - pictureBox1.PreferredSize.Width - 23, SettingsButton.Height - pictureBox1.PreferredSize.Height - 20);
            pictureBox1.BackColor = Color.Transparent;

            UpdateButton.Controls.Add(pictureBox2);
            pictureBox2.Location = new Point(UpdateButton.Width - pictureBox2.PreferredSize.Width - 23, UpdateButton.Height - pictureBox2.PreferredSize.Height - 20);
            pictureBox2.BackColor = Color.Transparent;

            LoadCustomFont();
            LoadMods();
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
            UpdateButton.Font = new Font(privateFonts.Families[0], 22.0F, FontStyle.Regular);
            DependenciesLabel.Font = new Font(privateFonts.Families[0], 11.0F, FontStyle.Regular);
            ModDescriptionLabel.Font = new Font(privateFonts.Families[0], 15.0F, FontStyle.Regular);
        }

        private void Settings_Button_Click(object sender, EventArgs e)
        {
            Settings settingsForm = new Settings(this, null);
            settingsForm.Show();
        }

        public void LoadMods()
        {
            UninstallButton.Visible = false;
            ModNameLabel.Visible = false;
            ModVersionLabel.Visible = false;
            ModAuthorLabel.Visible = false;
            UpdateButton.Visible = false;
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

            if (Directory.Exists(Path.Combine(rumblePath, "UserData", "CustomMultiplayerMaps", "Maps")))
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
        }

        private void DisplayMods(string rumblePath)
        {
            string modsPath = Path.Combine(rumblePath, "Mods");
            string disabledModsPath = Path.Combine(rumblePath, "DisabledMods");

            // Check if DisabledMods folder exists, if not create it
            if (!Directory.Exists(disabledModsPath))
            {
                Directory.CreateDirectory(disabledModsPath);
            }

            // Get mod files from both Mods and DisabledMods folders
            string[] enabledModFiles = Directory.GetFiles(modsPath, "*.dll");
            string[] disabledModFiles = Directory.GetFiles(disabledModsPath, "*.dll");
            string[] allModFiles = enabledModFiles.Concat(disabledModFiles).ToArray();

            // Clear existing controls
            panel2.Controls.Clear();

            // Define the number of columns
            int columnCount = 1;
            int rowCount = (allModFiles.Length + columnCount - 1) / columnCount;

            // Define the size of each mod panel and margin
            int panelWidth = panel2.Width / columnCount;
            int panelHeight = 84; // Set this to the desired height of your mod panels
            int verticalMargin = 10;

            for (int i = 0; i < allModFiles.Length; i++)
            {
                string modFile = allModFiles[i];
                bool isEnabled = enabledModFiles.Contains(modFile);
                string modVersionStr = GetMelonLoaderModInfo(Path.Combine(rumblePath, isEnabled ? "Mods" : "DisabledMods"), Path.GetFileName(modFile));
                Color color = Color.Lime;
                Image cloudIcon = null;
                string toolTip = "Unknown";
                Image modImage = null;
                string ModAuthor = null;

                foreach (var kvp in ModCache.ModsByPage)
                {
                    foreach (var mod in kvp.Value)
                    {
                        if (IsFuzzyMatch(mod.Name.Replace(" ", "").Replace("_", ""), Path.GetFileNameWithoutExtension(modFile)))
                        {
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
                                }
                                else if (modVersion == modVersionCache)
                                {
                                    color = Color.Lime;
                                    toolTip = "Up To Date";
                                }
                                else
                                {
                                    color = Color.Cyan;
                                    toolTip = "Unreleased version";
                                }
                            }
                            else
                            {
                                modVersionStr = mod.Version;
                                toolTip = "Up To Date";
                            }
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(ModAuthor))
                {
                    ModAuthor = "Unknown";
                }

                // Create the mod panel
                ModPanelControl modPanel = new ModPanelControl
                {
                    ModName = Path.GetFileNameWithoutExtension(modFile),
                    DetailsLabel = $"v{modVersionStr} by {ModAuthor}",
                    ModLabelFont = new Font(privateFonts.Families[0], 15.0F, FontStyle.Bold),
                    DetailsLabelFont = new Font(privateFonts.Families[0], 15.0F, FontStyle.Regular),
                    UpdateNeededImage = cloudIcon,
                    UpdateColor = color,
                    toolTip1Text = toolTip,
                    ModImage = modImage ?? Properties.Resources.UnknownMod,
                    Tag = Path.GetFileName(modFile),
                    ModEnabled = isEnabled, // Set the ModEnabled property
                    Size = new Size(panelWidth, panelHeight), // Adjust size for vertical margin
                    ModDllPath = modFile
                };

                // Add click event
                modPanel.Click += (s, e) => ModPanel_Click(modPanel, e);

                // Calculate position with vertical margin
                int row = i / columnCount;
                int column = i % columnCount;
                modPanel.Location = new Point(column * panelWidth, row * (panelHeight + verticalMargin));

                // Add the mod panel to the parent panel
                panel2.Controls.Add(modPanel);
            }

            // Make the parent panel scrollable and hide scroll bars
            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Maximum = 0;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;
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

                Uninstall uninstallForm = new Uninstall(modName, modFilePath, this);
                uninstallForm.Show();
            }
        }

        public static string GetMelonLoaderModInfo(string path, string dllName, bool getName = false)
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
                    // Check if the attribute is of type MelonInfoAttribute
                    if (attribute.GetType().Name == "MelonInfoAttribute")
                    {
                        // Use reflection to get the desired property (Name or Version)
                        string propertyName = getName ? "Name" : "Version";
                        var property = attribute.GetType().GetProperty(propertyName);
                        if (property != null)
                        {
                            var value = property.GetValue(attribute) as string;
                            return value;
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
                        UpdateButton.Visible = true;
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
                        ModNameLabel.Text = selectedPanel.ModName;
                        AdjustFontSizeToFit(ModNameLabel);


                        string modVersionStr = GetMelonLoaderModInfo(Path.Combine(Properties.Settings.Default.RumblePath, selectedPanel.ModEnabled ? "Mods" : "DisabledMods"), selectedPanel.ModName + ".dll");

                        bool modFound = false;

                        if (ModCache.ModsByPage.Count > 0)
                        {
                            foreach (var kvp in ModCache.ModsByPage)
                            {
                                foreach (var mod in kvp.Value)
                                {
                                    if (IsFuzzyMatch(mod.Name.Replace(" ", "").Replace("_", ""), selectedPanel.ModName))
                                    {
                                        modVersionStr = modVersionStr ?? mod.Version;
                                        CurrentlySelectedName = selectedPanel.ModName;
                                        ModPictureDisplay.Image = mod.ModImage;
                                        CurrentlySelectedMod = mod;
                                        ModNameLabel.Text = mod.Name;
                                        DateUpdated.Text = mod.DateUpdated;
                                        var cleanedDependencies = CurrentlySelectedMod.Dependencies
                                        .Where(d => !d.StartsWith("MelonLoader", StringComparison.OrdinalIgnoreCase))
                                        .Select(d => d.Replace(" icon", "").Trim())
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
                                                UpdateButton.BackgroundImage = Properties.Resources.blue;
                                                ModVersionLabel.Location = new Point(207, 63);
                                                UpdateButton.Visible = false;
                                                ModVersionLabel.ForeColor = Color.Cyan;
                                                pictureBox2.Visible = false;
                                            }
                                            else if (modVersion == modVersionCache)
                                            {
                                                UpdateButton.BackgroundImage = Properties.Resources.green;
                                                ModVersionLabel.Location = new Point(207, 63);
                                                UpdateButton.Visible = false;
                                                ModVersionLabel.ForeColor = Color.Lime;
                                                pictureBox2.Visible = false;
                                            }
                                            else
                                            {
                                                ModVersionLabel.Location = new Point(229, 65);
                                                UpdateButton.BackgroundImage = Properties.Resources.red;
                                                UpdateButton.Visible = true;
                                                ModVersionLabel.ForeColor = Color.Red;
                                                pictureBox2.Visible = true;
                                            }
                                        }
                                        else
                                        {
                                            UpdateButton.BackgroundImage = Properties.Resources.green;
                                            ModVersionLabel.ForeColor = Color.Lime;
                                            pictureBox2.Visible = false;
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
                            ModAuthorLabel.Text = "By Unknown";
                            ModPictureDisplay.Image = Properties.Resources.UnknownMod;
                            UpdateButton.BackgroundImage = null;
                            ModVersionLabel.Location = new Point(207, 63);
                            pictureBox2.Visible = false;
                            ModVersionLabel.ForeColor = Color.FromArgb(64, 64, 64);
                            DateUpdated.Text = "Unkown Last Date Updated";
                            DependenciesLabel.Text = "Dependencies:";
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
                        UpdateButton.Visible = false;
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

        public static bool IsFuzzyMatch(string modName, string localModName)
        {
            return modName.Contains(localModName, StringComparison.OrdinalIgnoreCase) ||
                   localModName.Contains(modName, StringComparison.OrdinalIgnoreCase);
        }

        private void ThunderstoreButton_Click(object sender, EventArgs e)
        {
            if (ModCache.ModsByPage != null && ModCache.ModsByPage.Count > 0)
            {
                ThunderstoreMods thunderstoreForum = new ThunderstoreMods(this);
                thunderstoreForum.Show();
            }
            else
            {
                MessageBox.Show("Please wait for mods to finish loading before accessing the Thunderstore mod page.");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (CurrentlySelectedVersion != CurrentlySelectedMod.Version)
            {
                await ThunderstoreMods.DownloadModFromInternet(CurrentlySelectedMod.ModPageUrl, this, selectedPanel.ModEnabled);
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
            CustomMaps customMaps = new CustomMaps(this);
            customMaps.Show();
        }
    }

    public static class ModCache
    {
        public static Dictionary<int, List<ThunderstoreMods.Mod>> ModsByPage { get; set; } = new Dictionary<int, List<ThunderstoreMods.Mod>>();
    }
}
