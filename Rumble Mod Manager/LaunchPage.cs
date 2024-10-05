﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using TheArtOfDevHtmlRenderer.Adapters;
using static Rumble_Mod_Manager.ThunderstoreMods;
using Timer = System.Threading.Timer;

namespace Rumble_Mod_Manager
{
    public partial class LaunchPage : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        private static Settings _settingsInstance;
        private static Credits _creditsInstance;
        private static RUMBLEModManager _rumbleModManagerInstance;

        Dictionary<string, string> modMappings = new Dictionary<string, string>();

        public LaunchPage()
        {
            InitializeComponent();

            progressBar1.Visible = false;
            label1.Visible = false;
            LaunchButton.Visible = false;
            SettingsButton.Visible = false;
            CreditsButton.Visible = false;

            LoadCustomFont();

            this.Shown += new EventHandler(LaunchPage_Shown);
        }

        private async void LaunchPage_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.RumblePath))
            {
                CheckRumblePath();
            }
        }

        public async Task InstallMelonLoader()
        {
            string rumblePath = Properties.Settings.Default.RumblePath;

            string downloadUrl = "https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x64.zip";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "MelonLoader.zip");
            string gamePath = Path.Combine(Properties.Settings.Default.RumblePath, "RUMBLE.exe");

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.AllowAutoRedirect = true;

                using (HttpClient client = new HttpClient(handler) { BaseAddress = new Uri("https://github.com/") })
                {
                    HttpResponseMessage response = await client.GetAsync(downloadUrl);
                    response.EnsureSuccessStatusCode();
                    await using (FileStream fs = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fs);
                    }
                }
            }

            ZipFile.ExtractToDirectory(tempZipPath, rumblePath, true);
            File.Delete(tempZipPath);

            UserMessage errorMessage = new UserMessage($"MelonLoader has been installed successfully. The game is about to open, once it reaches the T-POSE screen, please close the game. \n\n Do not click on the console. If you do, please press enter.", true);
            errorMessage.Show();

            if (!File.Exists(gamePath))
            {
                Console.WriteLine($"Executable not found at: {gamePath}");
                return;
            }

            label1.Text = "Waiting...";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = gamePath;
            startInfo.WorkingDirectory = Properties.Settings.Default.RumblePath;

            Process gameProcess = new Process();
            gameProcess.StartInfo = startInfo;
            gameProcess.Start();

            gameProcess.WaitForExit();
        }

        public void CheckRumblePath()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.RumblePath))
            {
                PathNotFound.Visible = false;
                AutoFindButton.Visible = false;
                ManualFindButton.Visible = false;
                LaunchButton.Visible = true;
                SettingsButton.Visible = true;
                CreditsButton.Visible = true;
            }
            else
            {
                progressBar1.Visible = false;
                label1.Visible = false;
                LaunchButton.Visible = false;
                SettingsButton.Visible = false;
                CreditsButton.Visible = false;
                PathNotFound.Visible = true;
                AutoFindButton.Visible = true;
                ManualFindButton.Visible = true;
            }
        }

        private async void Continue(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            label1.Visible = true;

            LaunchButton.Visible = false;
            SettingsButton.Visible = false;
            CreditsButton.Visible = false;

            await LaunchManager();
        }

        static async Task<Dictionary<int, List<CustomMap>>> ManageMaps(Guna.UI2.WinForms.Guna2CircleProgressBar progressBar)
        {
            string repoOwner = "xLoadingx";
            string repoName = "mod-maps";
            var maps = new Dictionary<int, List<CustomMap>>();

            progressBar.Value = 0;

            string basePath = Properties.Settings.Default.RumblePath;
            string targetDirectory = Path.Combine(basePath, "UserData", "CustomMultiplayerMaps", "Maps");

            if (!Directory.Exists(targetDirectory))
            {
                return null;
            }

            var mapDetails = new List<(string name, string description, string author, string version, string imageUrl, string downloadUrl)>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.github.com/");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
                HttpResponseMessage response = await client.GetAsync($"repos/{repoOwner}/{repoName}/contents/");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray directories = JArray.Parse(json);

                    progressBar.Maximum = directories.Count;
                    progressBar.Value = 0;

                    var tasks = directories
                        .Where(directory => directory["type"].ToString() == "dir")
                        .Select(async directory =>
                        {
                            string dirUrl = directory["url"].ToString();
                            var mapDetail = await GetMapDetailsFromDirectory(dirUrl, client);
                            if (mapDetail != ("Unknown", "Unknown", "Unknown", "Unknown", "", ""))
                            {
                                progressBar.Invoke((Action)(() => progressBar.Value += 1));
                                return mapDetail;
                            }
                            return default;
                        }).ToList();

                    var results = await Task.WhenAll(tasks);

                    mapDetails.AddRange(results.Where(result => result != default));
                }
            }

            int pageIndex = 1;
            int mapCount = 0;
            foreach (var detail in mapDetails)
            {
                var customMap = new CustomMap
                {
                    name = detail.name,
                    description = detail.description,
                    author = detail.author,
                    version = detail.version,
                    mapImage = await DownloadImage(detail.imageUrl),
                    downloadLink = detail.downloadUrl
                };

                if (!maps.ContainsKey(pageIndex))
                {
                    maps[pageIndex] = new List<CustomMap>();
                }

                maps[pageIndex].Add(customMap);
                mapCount++;

                if (mapCount >= 26)
                {
                    pageIndex++;
                    mapCount = 0;
                }
            }

            return maps;
        }

        static async Task<(string name, string description, string author, string version, string imageUrl, string downloadUrl)> GetMapDetailsFromDirectory(string dirUrl, HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync(dirUrl);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JArray files = JArray.Parse(json);

                string name = "";
                string description = "";
                string author = "";
                string version = "";
                string imageUrl = "";
                string downloadUrl = "";

                foreach (var file in files)
                {
                    if (file["type"].ToString() == "file")
                    {
                        string fileName = file["name"].ToString();
                        if (fileName == "Details.txt")
                        {
                            string detailsUrl = file["download_url"].ToString();
                            (name, description, author, version) = await GetMapDetails(detailsUrl);
                        }
                        else if (fileName.EndsWith(".txt"))
                        {
                            downloadUrl = file["download_url"].ToString();
                        }
                        else if (fileName.EndsWith(".png"))
                        {
                            imageUrl = file["download_url"].ToString();
                        }
                    }
                }

                return (name, description, author, version, imageUrl, downloadUrl);
            }
            else
            {
                return ("Unknown", "Unknown", "Unknown", "Unknown", "", "");
            }
        }

        static async Task<(string name, string description, string author, string version)> GetMapDetails(string fileUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(fileUrl);
                if (response.IsSuccessStatusCode)
                {
                    string fileContent = await response.Content.ReadAsStringAsync();
                    string[] lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    string name = "";
                    string description = "";
                    string author = "";
                    string version = "";
                    bool isDescription = false;
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("Name: "))
                        {
                            name = line.Substring(6);
                        }
                        else if (line.StartsWith("Description: "))
                        {
                            description = line.Substring(13);
                            isDescription = true;
                        }
                        else if (line.StartsWith("Author: "))
                        {
                            author = line.Substring(8);
                        }
                        else if (line.StartsWith("Version: "))
                        {
                            version = line.Substring(9);
                        }
                        else if (isDescription && string.IsNullOrWhiteSpace(line))
                        {
                            break;
                        }
                    }
                    return (name, description, author, version);
                }
                else
                {
                    return ("Unknown", "Unknown", "Unknown", "Unknown");
                }
            }
        }

        private static async Task<Image> DownloadImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return null;
                }

                using (HttpClient client = new HttpClient())
                {
                    if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                    {
                        UserMessage errorMessage = new UserMessage("The URL is not well-formed: " + imageUrl, true);
                        errorMessage.Show();
                        return null;
                    }

                    HttpResponseMessage response = await client.GetAsync(imageUrl);
                    response.EnsureSuccessStatusCode();

                    using (Stream imageStream = await response.Content.ReadAsStreamAsync())
                    {
                        return Image.FromStream(imageStream);
                    }
                }
            }
            catch (Exception ex)
            {
                string detailedErrorMessage = $"An error occurred while downloading image: {ex.Message}\n\n" +
                                              $"Stack Trace:\n{ex.StackTrace}";
                UserMessage errorMessage = new UserMessage(detailedErrorMessage, true);
                errorMessage.Show();
                return null;
            }
        }

        private async Task<List<ModPanelControl>> LoadModPanels()
        {
            string modsPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mods");
            string disabledModsPath = Path.Combine(Properties.Settings.Default.RumblePath, "DisabledMods");
            string inactiveModsPath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", "InactiveMods");

            label1.Text = "Loading Mod Mappings...";
            label1.Refresh();

            string url = "https://raw.githubusercontent.com/xLoadingx/mod-maps/main/modMapping.json";
            modMappings = await RUMBLEModManager.GetModMappingsAsync(url);

            if (!Directory.Exists(disabledModsPath))
            {
                label1.Text = "Creating Disabled Directory...";
                label1.Refresh();
                Directory.CreateDirectory(disabledModsPath);
            }

            if (!Directory.Exists(inactiveModsPath))
            {
                label1.Text = "Creating Inactive Mods Directory...";
                label1.Refresh();
                Directory.CreateDirectory(inactiveModsPath);
            }

            var enabledModFiles = Directory.GetFiles(modsPath, "*.dll").OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant()).ToArray();
            var disabledModFiles = Directory.GetFiles(disabledModsPath, "*.dll").OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant()).ToArray();
            var inactiveModFiles = Directory.GetFiles(inactiveModsPath, "*.dll").OrderBy(f => Path.GetFileNameWithoutExtension(f).ToLowerInvariant()).ToArray();

            List<ModPanelControl> modPanels = new List<ModPanelControl>();

            int totalMods = enabledModFiles.Length + disabledModFiles.Length + inactiveModFiles.Length;
            if (totalMods > 0)
            {
                progressBar1.Maximum = totalMods;
                progressBar1.Value = 0;
            }
            else
            {
                label1.Text = "No mods found.";
                return modPanels;
            }

            label1.Text = "Loading Enabled Mods...";
            label1.Refresh();
            foreach (var modFile in enabledModFiles)
            {
                ModPanelControl modPanel = CreateModPanel(modFile, true, Properties.Settings.Default.RumblePath);
                modPanels.Add(modPanel);

                progressBar1.Value++;
                progressBar1.Refresh();
            }

            label1.Text = "Loading Disabled Mods...";
            label1.Refresh();
            foreach (var modFile in disabledModFiles)
            {
                ModPanelControl modPanel = CreateModPanel(modFile, false, Properties.Settings.Default.RumblePath);
                modPanels.Add(modPanel);

                progressBar1.Value++;
                progressBar1.Refresh();
            }

            label1.Text = "Loading Inactive Mods...";
            label1.Refresh();
            foreach (var modFile in inactiveModFiles)
            {
                ModPanelControl modPanel = CreateModPanel(modFile, false, Properties.Settings.Default.RumblePath);
                modPanels.Add(modPanel);

                progressBar1.Value++;
                progressBar1.Refresh();
            }

            return modPanels;
        }

        private ModPanelControl CreateModPanel(string modFile, bool isEnabled, string rumblePath)
        {
            string modFileName = Path.GetFileName(modFile);

            modMappings.TryGetValue(modFileName, out string modNameFromMapping);

            string folderPath = modFile.Contains("InactiveMods") ? "Mod_Profiles/InactiveMods" :
                    isEnabled ? "Mods" : "DisabledMods";

            string modVersionStr = (string)RUMBLEModManager.GetMelonLoaderModInfo(Path.Combine(rumblePath, folderPath), modFileName, MelonLoaderModInfoType.Version);

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
                ModAuthor = $"By {RUMBLEModManager.GetMelonLoaderModInfo(Path.Combine(rumblePath, isEnabled ? "Mods" : "DisabledMods"), modFileName, MelonLoaderModInfoType.Author)}";
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
                ModLabelFont = new Font(privateFonts.Families[1], 15.0F, FontStyle.Bold),
                DetailsLabelFont = new Font(privateFonts.Families[1], 15.0F, FontStyle.Regular),
                UpdateNeededImage = cloudIcon,
                UpdateColor = color,
                Outdated = outdated,
                ModImage = modImage ?? Properties.Resources.UnknownMod,
                Tag = Path.GetFileName(modFile),
                ModEnabled = isEnabled,
                ModDllPath = modFile
            };

            return modPanel;
        }

        private async Task LaunchManager()
        {
            try
            {
                label1.Visible = true;
                label1.Text = "Fetching Mods...";

                var modsByPage = await ThunderstoreMods.FetchThunderstoreMods(progressBar1);
                ModCache.ModsByPage = modsByPage;

                string basePath = Properties.Settings.Default.RumblePath;

                if (!Properties.Settings.Default.SkipMapLoading)
                {
                    string targetDirectory = Path.Combine(basePath, "UserData", "CustomMultiplayerMaps", "Maps");

                    if (Directory.Exists(targetDirectory))
                    {
                        label1.Text = "Fetching Maps...";
                        var mapsByPage = await ManageMaps(progressBar1);

                        int retryCount = 0;
                        int maxRetries = 20;

                        while ((mapsByPage == null || !mapsByPage.Any()) && retryCount < maxRetries)
                        {
                            retryCount++;
                            mapsByPage = await ManageMaps(progressBar1);
                        }

                        if (mapsByPage != null && mapsByPage.Any())
                        {
                            CustomMapsCache.MapsByPage = mapsByPage;
                        }
                        else
                        {
                            label1.Text = "Failed to fetch maps...";
                            await Task.Delay(500);
                        }
                    }
                }

                if (!Directory.Exists(Path.Combine(basePath, "MelonLoader", "Dependencies", "Il2CppAssemblyGenerator", "UnityDependencies")))
                {
                    label1.Text = "Installing MelonLoader...";
                    await InstallMelonLoader();
                }

                label1.Text = "Preloading Panels...";
                var preloadedPanels = await LoadModPanels();

                progressBar1.Visible = false;
                label1.Visible = false;

                if (_rumbleModManagerInstance == null || _rumbleModManagerInstance.IsDisposed)
                {
                    _rumbleModManagerInstance = new RUMBLEModManager(preloadedPanels);
                    _rumbleModManagerInstance.FormClosed += (s, args) => _rumbleModManagerInstance = null;
                    _rumbleModManagerInstance.Show();
                    this.Hide();
                    _rumbleModManagerInstance.FormClosed += (s, args) => this.Close();
                }
                else
                {
                    _rumbleModManagerInstance.BringToFront();
                }
            }
            catch (Exception ex)
            {
                string detailedErrorMessage = $"An error occurred while fetching mods from Thunderstore: {ex.Message}\n\n" +
                                              $"Stack Trace:\n{ex.StackTrace}";
                MessageBox.Show(detailedErrorMessage);
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

            RUMBLELabel.Font = new Font(privateFonts.Families[0], 72.0F, FontStyle.Regular);
            ModManagerLabel.Font = new Font(privateFonts.Families[0], 36.0F, FontStyle.Regular);
            Credits.Font = new Font(privateFonts.Families[0], 48.0F, FontStyle.Regular);
            PathNotFound.Font = new Font(privateFonts.Families[1], 34.0F, FontStyle.Regular);
            AutoFindButton.Font = new Font(privateFonts.Families[1], 24.0F, FontStyle.Regular);
            ManualFindButton.Font = new Font(privateFonts.Families[1], 24.0F, FontStyle.Regular);
            label1.Font = new Font(privateFonts.Families[1], 22.0F, FontStyle.Regular);
            LaunchButton.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            SettingsButton.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            CreditsButton.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
            progressBar1.Font = new Font(privateFonts.Families[1], 26.0F, FontStyle.Regular);
        }

        private void ManualFindButton_Click(object sender, EventArgs e)
        {
            if (_settingsInstance == null || _settingsInstance.IsDisposed)
            {
                _settingsInstance = new Settings(null, this);
                _settingsInstance.FormClosed += (s, args) => _settingsInstance = null;
                _settingsInstance.Show();
            }
            else
            {
                _settingsInstance.BringToFront();
            }
        }

        public string FindRumblePath()
        {
            string rumbleExe = "RUMBLE.exe";
            string rumblePath = null;
            int rumbleAppId = 890550;

            string steamInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string;

            if (steamInstallPath == null)
            {
                steamInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null) as string;
            }

            if (steamInstallPath != null)
            {
                string rumblePathInDefault = Path.Combine(steamInstallPath, "steamapps", "common", "RUMBLE");
                if (Directory.Exists(rumblePathInDefault) && File.Exists(Path.Combine(rumblePathInDefault, rumbleExe)))
                {
                    return rumblePathInDefault;
                }
            }

            string configPath = Path.Combine(steamInstallPath, "steamapps", "libraryfolders.vdf");
            if (File.Exists(configPath))
            {
                var lines = File.ReadAllLines(configPath);
                foreach (string line in lines)
                {
                    if (line.Contains("path"))
                    {
                        var parts = line.Split('"');
                        if (parts.Length > 3)
                        {
                            string libraryPath = parts[3].Trim();

                            string rumblePathInLibrary = Path.Combine(libraryPath, "steamapps", "common", "RUMBLE");
                            if (Directory.Exists(rumblePathInLibrary) && File.Exists(Path.Combine(rumblePathInLibrary, rumbleExe)))
                            {
                                return rumblePathInLibrary;
                            }

                            string appIdPath = Path.Combine(libraryPath, "steamapps", $"{rumbleAppId}.acf");
                            if (File.Exists(appIdPath))
                            {
                                if (Directory.Exists(rumblePathInLibrary))
                                {
                                    return rumblePathInLibrary;
                                }
                            }
                        }
                    }
                }
            }

            return "RUMBLE folder not found.";
        }

        public string FindRumbleOculusPath()
        {
            string oculusPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Oculus VR, LLC\Oculus", "Base", null) as string;

            if (oculusPath == null)
            {
                oculusPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Oculus VR, LLC\Oculus", "Base", null) as string;
            }

            if (oculusPath == null)
            {
                return null;
            } else
            {
                string gamesPath = Path.Combine(oculusPath, "Software", "Software");

                if (Directory.Exists(gamesPath))
                {
                    string gameFullPath = Path.Combine(gamesPath, "kung-fu-magic-the-game");

                    if (Directory.Exists(gameFullPath))
                    {
                        return gameFullPath;
                    }
                }
            }

            return null;
        }

        private void AutoFindButton_Click(object sender, EventArgs e)
        {
            try
            {
                string steamGameFolderName = "RUMBLE";
                string oculusGameFolderName = "kung-fu-magic-the-game";

                string gameDirectory = FindRumblePath();
                if (gameDirectory == null)
                {
                    gameDirectory = FindRumbleOculusPath();
                }
                
                if (gameDirectory != null)
                {
                    Properties.Settings.Default.RumblePath = gameDirectory;
                    Properties.Settings.Default.Save();

                    if (_settingsInstance == null || _settingsInstance.IsDisposed)
                    {
                        _settingsInstance = new Settings(null, this);
                        _settingsInstance.FormClosed += (s, args) => _settingsInstance = null;
                        _settingsInstance.Show();
                    }
                    else
                    {
                        _settingsInstance.BringToFront();
                    }

                    UserMessage successMessage = new UserMessage("Game Directory Found. Make sure it is the correct path to your game directory!", true);
                    successMessage.Show();
                    PathNotFound.Visible = false;
                    AutoFindButton.Visible = false;
                    ManualFindButton.Visible = false;
                    LaunchButton.Visible = true;
                    SettingsButton.Visible = true;
                    CreditsButton.Visible = true;
                }
            }
            catch (Exception ex)
            {
                UserMessage errorMessage = new UserMessage($"Error: {ex.Message}", true);
                errorMessage.Show();
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            if (_settingsInstance == null || _settingsInstance.IsDisposed)
            {
                _settingsInstance = new Settings(null, this);
                _settingsInstance.FormClosed += (s, args) => _settingsInstance = null;
                _settingsInstance.Show();
            }
            else
            {
                _settingsInstance.BringToFront();
            }
        }

        private void CreditsButton_Click(object sender, EventArgs e)
        {
            if (_creditsInstance == null || _creditsInstance.IsDisposed)
            {
                _creditsInstance = new Credits();
                _creditsInstance.FormClosed += (s, args) => _creditsInstance = null;
                _creditsInstance.Show();
            }
            else
            {
                _creditsInstance.BringToFront();
            }
        }
    }
}
