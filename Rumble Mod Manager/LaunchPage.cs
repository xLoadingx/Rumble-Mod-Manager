using System;
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

            MessageBox.Show($"MelonLoader has been installed successfully. The game is about to open, once it reaches the T-POSE screen, please close the game. \n\n Do not click on the console. If you do, please press enter.");

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
            // Start the swipe animations with shorter delays
            SwipeControlOut(LaunchButton, 0);
            SwipeControlOut(SettingsButton, 100); // 250ms delay after the first button
            SwipeControlOut(CreditsButton, 200); // 250ms delay after the second button

            await Task.Delay(250);

            // Show the progress bar and label
            progressBar1.Visible = true;
            label1.Visible = true;

            // Animate the progress bar and label to slide in from left to right
            AnimateControlIn(progressBar1, progressBar1.Location.X);
            await AnimateControlIn(label1, label1.Location.X);

            LaunchManager();
        }

        private async Task SwipeControlOut(Control control, int delay)
        {
            control.Enabled = false;
            await Task.Delay(delay);

            int targetX = -control.Width; // Target X position (completely out of view to the left)
            int initialPosition = control.Left;
            int distance = initialPosition - targetX;

            for (int i = 0; i <= 50; i++)
            {
                double t = i / 50.0;
                double easeInValue = t * t; // Ease-in quadratic formula

                int newLeft = initialPosition - (int)(easeInValue * distance);

                // Ensure UI updates are done on the main thread
                control.Invoke((Action)(() =>
                {
                    control.Left = newLeft;
                    control.Invalidate(); // Force a redraw to prevent artifacts
                }));

                await Task.Delay(10); // Adjust this value to control the smoothness
            }

            // Ensure it ends exactly at the target position
            control.Invoke((Action)(() =>
            {
                control.Left = targetX;
                control.Visible = false;
            }));
        }

        private async Task AnimateControlIn(Control control, int targetX)
        {
            int initialPosition = this.Width; // Start off-screen to the right
            control.Left = initialPosition;

            // Manually animate the control to slide in with ease-out
            for (int i = 0; i <= 100; i++)
            {
                double t = i / 100.0;
                double easeOutValue = 1 - (1 - t) * (1 - t); // Ease-out quadratic formula

                control.Left = (int)(initialPosition + easeOutValue * (targetX - initialPosition));
                await Task.Delay(10); // Adjust this value to control the smoothness
            }

            control.Left = targetX; // Ensure it ends exactly at the target position
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

            List<(string name, string description, string author, string version, string imageUrl, string downloadUrl)> mapDetails = new List<(string name, string description, string author, string version, string imageUrl, string downloadUrl)>();

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
                        MessageBox.Show("The URL is not well-formed: " + imageUrl, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(detailedErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private async void LaunchManager()
        {
            try
            {
                label1.Visible = true;
                label1.Text = "Fetching Mods...";

                var modsByPage = await ThunderstoreMods.FetchThunderstoreMods(progressBar1);
                ModCache.ModsByPage = modsByPage;

                string basePath = Properties.Settings.Default.RumblePath;
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
                        MessageBox.Show($"Failed to fetch maps after {maxRetries} attempts. The manager has probably been opened a lot in a short period of time, if so, please try again later.");
                    }
                }

                if (!Directory.Exists(Path.Combine(basePath, "MelonLoader", "Dependencies", "Il2CppAssemblyGenerator", "UnityDependencies")))
                {
                    label1.Text = "Installing MelonLoader...";
                    await InstallMelonLoader();
                }

                progressBar1.Visible = false;
                label1.Visible = false;

                if (_rumbleModManagerInstance == null || _rumbleModManagerInstance.IsDisposed)
                {
                    _rumbleModManagerInstance = new RUMBLEModManager();
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
                MessageBox.Show(detailedErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void AutoFindButton_Click(object sender, EventArgs e)
        {
            try
            {
                string steamGameFolderName = "RUMBLE";
                string oculusGameFolderName = "kung-fu-magic-the-game";

                List<string> steamDirectories = FindSteamAppsDirectories();
                List<string> oculusDirectories = FindOculusSoftwareDirectories();

                string gameDirectory = FindGameDirectory(steamDirectories, steamGameFolderName);
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

                    MessageBox.Show("Game Directory Found. Make sure it is the correct path to your game directory!");
                    PathNotFound.Visible = false;
                    AutoFindButton.Visible = false;
                    ManualFindButton.Visible = false;
                    LaunchButton.Visible = true;
                    SettingsButton.Visible = true;
                    CreditsButton.Visible = true;
                }
                else
                {
                    string oculusGameDirectory = FindGameDirectory(oculusDirectories, oculusGameFolderName);
                    if (oculusGameDirectory != null)
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

                        MessageBox.Show("Game Directory Found. Make sure it is the correct path to your game directory!");
                        PathNotFound.Visible = false;
                        AutoFindButton.Visible = false;
                        ManualFindButton.Visible = false;
                        LaunchButton.Visible = true;
                        SettingsButton.Visible = true;
                        CreditsButton.Visible = true;
                    }
                    else
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

                        MessageBox.Show("Game directory not found. Please set it manually.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static List<string> FindSteamAppsDirectories()
        {
            List<string> foundDirectories = new List<string>();

            // Default Steam installation path
            string defaultSteamPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam");
            string libraryFoldersFile = Path.Combine(defaultSteamPath, "steamapps", "libraryfolders.vdf");

            if (File.Exists(libraryFoldersFile))
            {
                // Read and parse the libraryfolders.vdf file
                string[] lines = File.ReadAllLines(libraryFoldersFile);
                Regex pathRegex = new Regex(@"\""path\""\s*\""(.*?)\""");

                foreach (var line in lines)
                {
                    var match = pathRegex.Match(line);
                    if (match.Success)
                    {
                        string libraryPath = match.Groups[1].Value;
                        string commonPath = Path.Combine(libraryPath, "steamapps", "common");

                        if (Directory.Exists(commonPath))
                        {
                            foundDirectories.Add(commonPath);
                        }
                    }
                }
            }

            // Add the default steamapps/common directory just in case
            string defaultCommonPath = Path.Combine(defaultSteamPath, "steamapps", "common");
            if (Directory.Exists(defaultCommonPath))
            {
                foundDirectories.Add(defaultCommonPath);
            }

            return foundDirectories;
        }

        static List<string> FindOculusSoftwareDirectories()
        {
            List<string> possibleDirectories = new List<string>
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oculus", "Software", "Software"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Oculus", "Software", "Software"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Oculus", "Software", "Software"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Oculus", "Software", "Software")
            };

            possibleDirectories.Add(@"D:\Oculus\Software\Software");
            possibleDirectories.Add(@"E:\OculusLibrary\Software\Software");

            List<string> foundDirectories = new List<string>();

            foreach (var dir in possibleDirectories)
            {
                if (Directory.Exists(dir))
                {
                    foundDirectories.Add(dir);
                }
            }

            return foundDirectories;
        }

        static string FindGameDirectory(List<string> directories, string gameFolderName)
        {
            foreach (var dir in directories)
            {
                string gamePath = Path.Combine(dir, gameFolderName);
                if (Directory.Exists(gamePath))
                {
                    return gamePath;
                }
            }

            return null;
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
