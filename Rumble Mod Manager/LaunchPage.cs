using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Rumble_Mod_Manager
{
    public partial class LaunchPage : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

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

        private void Continue(object sender, EventArgs e)
        {
            PathNotFound.Visible = false;
            AutoFindButton.Visible = false;
            ManualFindButton.Visible = false;
            progressBar1.Visible = true;
            label1.Visible = true;
            LaunchButton.Visible = false;
            SettingsButton.Visible = false;
            CreditsButton.Visible = false;

            FindThunderstoreMods();
        }

        static async Task<Dictionary<int, List<CustomMap>>> ManageMaps(ProgressBar progressBar)
        {
            string repoOwner = "xLoadingx"; // Replace with your GitHub username
            string repoName = "mod-maps"; // Replace with your repository name
            var maps = new Dictionary<int, List<CustomMap>>();

            // Retrieve the RumblePath from settings
            string basePath = Properties.Settings.Default.RumblePath; // Assuming RumblePath is set in your settings
            string targetDirectory = Path.Combine(basePath, "UserData", "CustomMultiplayerMaps", "Maps");

            // Check if the directory exists
            if (!Directory.Exists(targetDirectory))
            {
                MessageBox.Show($"The directory {targetDirectory} does not exist.", "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            // List maps with name, description, author, version, and image
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
                    foreach (var directory in directories)
                    {
                        if (directory["type"].ToString() == "dir")
                        {
                            string dirName = directory["name"].ToString();
                            string dirUrl = directory["url"].ToString();
                            var mapDetail = await GetMapDetailsFromDirectory(dirUrl, client);
                            progressBar.Value += 1;
                            mapDetails.Add((mapDetail.name, mapDetail.description, mapDetail.author, mapDetail.version, mapDetail.imageUrl, mapDetail.downloadUrl));
                        }
                    }
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
                MessageBox.Show($"Failed to get directory details: {response.ReasonPhrase}");
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
                    MessageBox.Show($"Failed to get map details: {response.ReasonPhrase}");
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
                    // Ensure the URL is an absolute URI
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

        private async void FindThunderstoreMods()
        {
            try
            {
                label1.Visible = true;
                label1.Text = "Fetching Mods...";
                var modsByPage = await ThunderstoreMods.FetchThunderstoreMods(progressBar1);
                ModCache.ModsByPage = modsByPage;
                label1.Text = "Fetching Maps...";
                var mapsByPage = await ManageMaps(progressBar1);
                CustomMapsCache.MapsByPage = mapsByPage;
                progressBar1.Visible = false;
                label1.Visible = false;

                RUMBLEModManager rumbleModManager = new RUMBLEModManager();
                rumbleModManager.Show();
                this.Hide();

                rumbleModManager.FormClosed += (s, args) => this.Close();
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
        }

        private void ManualFindButton_Click(object sender, EventArgs e)
        {
            Settings settingsForm = new Settings(null, this);
            settingsForm.Show();
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

                    Settings settingsForm = new Settings(null, this);
                    settingsForm.Show();
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

                        Settings settings = new Settings(null, this);
                        settings.Show();
                        MessageBox.Show("Game Directory Found. Make sure it is the correct path to your game directory!");
                        PathNotFound.Visible = false;
                        AutoFindButton.Visible = false;
                        ManualFindButton.Visible = false;
                        LaunchButton.Visible = true;
                        SettingsButton.Visible = true;
                        CreditsButton.Visible = true;
                    } else
                    {
                        Settings settingsForm = new Settings(null, this);
                        settingsForm.Show();
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
            List<string> possibleDirectories = new List<string>
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam", "steamapps", "common"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Steam", "steamapps", "common"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam", "steamapps", "common"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".local", "share", "Steam", "steamapps", "common")
        };

            possibleDirectories.Add(@"D:\SteamLibrary\steamapps\common");
            possibleDirectories.Add(@"E:\Games\SteamLibrary\steamapps\common");

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

        static List<string> FindOculusSoftwareDirectories()
        {
            List<string> possibleDirectories = new List<string>
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oculus", "Software", "Software"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Oculus", "Software", "Software"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Oculus", "Software", "Software"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Oculus", "Software", "Software")
        };

            // Add custom library folders (example paths, customize as needed)
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
            Settings settings = new Settings(null, this);
            settings.Show();
        }

        private void CreditsButton_Click(object sender, EventArgs e)
        {
            Credits credits = new Credits();
            credits.Show();
        }
    }
}
