using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Windows.Navigation;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tulpep.NotificationWindow;

namespace Rumble_Mod_Manager
{
    public partial class LaunchPage : PersistentForm
    {
        private static PrivateFontCollection privateFonts = new PrivateFontCollection();
        private static Settings _settingsInstance;
        private static Credits _creditsInstance;
        private static RUMBLEModManager _rumbleModManagerInstance;

        private static Dictionary<string, string> modMappings = new Dictionary<string, string>();

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

                Task.Run(() => CheckForInternetConnectionBackground());

                await CheckForUpdates();
            }
        }

        private async Task CheckForInternetConnectionBackground()
        {
            while (true)
            {
                if (!IsConnectedToInternet())
                {
                    LaunchButton.Enabled = false;
                    UserMessage.ShowDialog("You are not connected to the internet. Please connect to the internet.", "Connection Error", true);

                    while (!IsConnectedToInternet())
                    {
                        await Task.Delay(2000);
                    }

                    LaunchButton.Enabled = true;
                }

                await Task.Delay(5000);
            }
        }

        public static bool IsConnectedToInternet()
        {
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                {
                    return true;
                }
            }
            return false;
        }

        public static void ShowNotification(Image image, string titleText, string contentText)
        {
            Font textFont = new Font(privateFonts.Families[1], 15.0F, FontStyle.Regular);
            PopupNotifier popup = new PopupNotifier();
            popup.Image = image;
            popup.ImagePadding = new Padding(0, 5, 0, 0);
            popup.ImageSize = new Size(80, 80);
            popup.BodyColor = Color.White;
            popup.TitleText = titleText;
            popup.TitlePadding = new Padding(0, 5, 0, 0);
            popup.TitleColor = Color.Black;
            popup.TitleFont = textFont;

            popup.ContentText = contentText;
            popup.ContentColor = Color.Black;
            popup.ContentFont = textFont;
            popup.ContentPadding = new Padding(0, 10, 0, 0);
            popup.Popup();
        }

        public async Task InstallMelonLoader(bool launchGame = true)
        {
            string rumblePath = Properties.Settings.Default.RumblePath;

            string baseDownloadUrl = "https://github.com/LavaGang/MelonLoader/releases/latest/download/";
            string architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            string downloadUrl = $"{baseDownloadUrl}MelonLoader.{architecture}.zip";
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

            if (launchGame)
            {
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
            try
            {
                string repoOwner = "xLoadingx";
                string repoName = "mod-maps";
                var maps = new Dictionary<int, List<CustomMap>>();
                progressBar.Value = 0;

                string basePath = Properties.Settings.Default.RumblePath;
                string targetDirectory = Path.Combine(basePath, "UserData", "CustomMultiplayerMaps", "Maps");

                if (!Directory.Exists(targetDirectory))
                {
                    return new Dictionary<int, List<CustomMap>>();
                }

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.github.com/");
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("request");

                    var response = await client.GetAsync($"repos/{repoOwner}/{repoName}/contents/");
                    if (!response.IsSuccessStatusCode)
                    {
                        return new Dictionary<int, List<CustomMap>>();
                    }

                    var directoriesJson = await response.Content.ReadAsStringAsync();
                    var directories = JArray.Parse(directoriesJson)
                        .Where(d => d["type"]?.ToString() == "dir")
                        .ToList();

                    progressBar.Maximum = directories.Count;

                    var allFiles = await Task.WhenAll(directories.Select(async dir =>
                    {
                        var dirResponse = await client.GetAsync(dir["url"]?.ToString() ?? string.Empty);
                        return dirResponse.IsSuccessStatusCode
                            ? JArray.Parse(await dirResponse.Content.ReadAsStringAsync())
                            : null;
                    }));

                    var mapDetails = allFiles
                        .Where(files => files != null)
                        .SelectMany(files => files ?? Enumerable.Empty<JToken>())
                        .GroupBy(file => file["path"]?.ToString()?.Split('/')[1])
                        .Select(group =>
                        {
                            string name = "Unknown", description = "Unknown", author = "Unknown", version = "Unknown";
                            string imageUrl = "", downloadUrl = "";

                            foreach (var file in group)
                            {
                                string fileName = file["name"]?.ToString();
                                if (fileName == "Details.txt")
                                {
                                    var detailsResponse = client.GetAsync(file["download_url"]?.ToString() ?? string.Empty).Result;
                                    if (detailsResponse.IsSuccessStatusCode)
                                    {
                                        var lines = detailsResponse.Content.ReadAsStringAsync().Result.Split(
                                            new[] { "\r\n", "\r", "\n" },
                                            StringSplitOptions.None
                                        );
                                        foreach (var line in lines)
                                        {
                                            if (line.StartsWith("Name: ")) name = line.Substring(6);
                                            if (line.StartsWith("Description: ")) description = line.Substring(13);
                                            if (line.StartsWith("Author: ")) author = line.Substring(8);
                                            if (line.StartsWith("Version: ")) version = line.Substring(9);
                                        }
                                    }
                                }
                                else if (fileName?.EndsWith(".png") == true)
                                {
                                    imageUrl = file["download_url"]?.ToString();
                                }
                                else if (fileName?.EndsWith(".txt") == true)
                                {
                                    downloadUrl = file["download_url"]?.ToString();
                                }
                            }
                            return (name, description, author, version, imageUrl, downloadUrl);
                        }).ToList();

                    int pageIndex = 1, mapCount = 0;

                    foreach (var detail in mapDetails)
                    {
                        if (string.IsNullOrEmpty(detail.name) || string.IsNullOrEmpty(detail.downloadUrl))
                        {
                            continue; // Skip invalid entries
                        }

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

                        progressBar.Invoke((Action)(() => progressBar.Value += 1));
                    }
                }

                return maps;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in ManageMaps: {ex.Message}\n{ex.StackTrace}");
                return new Dictionary<int, List<CustomMap>>();
            }
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
                if (string.IsNullOrEmpty(imageUrl)) throw new ArgumentException("Image URL cannot be null or empty.", nameof(imageUrl)); ;

                using (HttpClient client = new HttpClient())
                {
                    if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                    {
                        UserMessage errorMessage = new UserMessage("The URL is not well-formed: " + imageUrl, true);
                        errorMessage.Show();
                        throw new ArgumentException("The URL is not well-formed", nameof(imageUrl));
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
                throw new ArgumentException($"An error occurred while downloading image: {ex.Message}", nameof(imageUrl));
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

        public static ModPanelControl CreateModPanel(string modFile, bool isEnabled, string rumblePath)
        {
            string modFileName = Path.GetFileName(modFile);

            modMappings.TryGetValue(modFileName, out string modNameFromMapping);

            string folderPath = modFile.Contains("InactiveMods") ? "Mod_Profiles/InactiveMods" :
                    isEnabled ? "Mods" : "DisabledMods";

            string modVersionStr = (string)RUMBLEModManager.GetMelonLoaderModInfo(Path.Combine(rumblePath, folderPath), modFileName, MelonLoaderModInfoType.Version);

            long fileSize = new FileInfo(Path.Combine(rumblePath, folderPath, modFileName)).Length;
            double displaySize;
            string fileText = "MB";

            if (fileSize >= 1024 * 1024)
                displaySize = fileSize / (1024.0 * 1024.0);
            else if (fileSize >= 1024)
            {
                displaySize = fileSize / 1024.0;
                fileText = "KB";
            } else {
                displaySize = fileSize;
                fileText = "bytes";
            }
                

            Color color = Color.Lime;
            Image cloudIcon = null;
            Image modImage = null;
            string ModAuthor = null;
            bool modFound = false;
            bool outdated = false;
            ThunderstoreMods.Mod panelMod = null;

            if (modNameFromMapping != null)
            {
                foreach (var kvp in ModCache.ModsByPage)
                {
                    foreach (var mod in kvp.Value)
                    {
                        if ((mod.Name.Replace("_", " ") == modNameFromMapping || string.Equals(mod.Name.Replace("_", " "), Path.GetFileNameWithoutExtension(modFile), StringComparison.OrdinalIgnoreCase)) && !mod.isDeprecated)
                        {
                            modFound = true;
                            ModAuthor = mod.Author;
                            modImage = mod.ModImage;
                            panelMod = mod;

                            if (!string.IsNullOrEmpty(modVersionStr))
                            {
                                int modVersion = int.Parse(modVersionStr.Replace(".", ""));
                                int modVersionCache = int.Parse(mod.Version.Replace(".", ""));

                                if (modVersion < modVersionCache)
                                {
                                    color = Color.Red;
                                    cloudIcon = Properties.Resources.UpdateIcon;
                                    outdated = true;
                                }
                                else if (modVersion == modVersionCache)
                                {
                                    color = Color.Lime;
                                    outdated = false;
                                }
                                else
                                {
                                    color = Color.Cyan;
                                    outdated = false;
                                }
                            }
                            else
                            {
                                modVersionStr = mod.Version;
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
                color = Color.Cyan;
            }

            ModPanelControl modPanel = new ModPanelControl
            {
                ModNameLabel = Path.GetFileNameWithoutExtension(modFile),
                DetailsLabel = $"v{modVersionStr} by {ModAuthor}",
                ModLabelFont = new Font(privateFonts.Families[1], 15.0F, FontStyle.Bold),
                DetailsLabelFont = new Font(privateFonts.Families[1], 15.0F, FontStyle.Regular),
                FileSizeLabel = $"{displaySize.ToString("0.##")} {fileText}",
                FileSizeFont = new Font(privateFonts.Families[1], 15.0F, FontStyle.Regular),
                UpdateNeededImage = cloudIcon,
                UpdateColor = color,
                Outdated = outdated,
                ModImage = modImage ?? Properties.Resources.UnknownMod,
                Tag = Path.GetFileName(modFile),
                ModEnabled = isEnabled,
                ModDllPath = modFile,
                Mod = panelMod,
                VersionString = modVersionStr,
                OnlineModLink = panelMod?.OnlinePageUrl,
                FileSize = fileSize
            };

            modPanel.label1.Left = modPanel.label2.Right;
            modPanel.label1.Top = modPanel.label2.Top;

            return modPanel;
        }

        public static async Task CheckForUpdates(bool showScreen = false)
        {
            if (!IsConnectedToInternet()) return; 
            int currentVersion = 146;

            var client = new HttpClient();

            var url = "https://raw.githubusercontent.com/xLoadingx/Rumble-Mod-Manager/main/update/updates.json";
            url += "?nocache=" + DateTime.Now.Ticks;
            var response = await client.GetStringAsync(url);
            dynamic updateInfo = JsonConvert.DeserializeObject(response);
            string versionString = (string)updateInfo["version"];

            if (currentVersion < int.Parse(versionString.Replace(".", "")))
            {
                UserMessage updateMessage = new UserMessage($"A new version of the manager is available! Do you want to install? \n\n {string.Join(".", currentVersion.ToString().ToCharArray())} => {updateInfo.version.ToString()}", false, true);
                if (updateMessage.ShowDialog() == DialogResult.Yes)
                {
                    string tempZipPath = Path.Combine(Path.GetTempPath(), "Manager.zip");
                    using (var download = await client.GetStreamAsync(updateInfo.url.ToString()))
                    using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                    {
                        await download.CopyToAsync(fileStream);
                    }

                    string updaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.exe");
                    if (File.Exists(updaterPath))
                    {
                        Process.Start(updaterPath, $"\"{tempZipPath}\" \"{AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\')}\" \"{Environment.ProcessPath}\"");
                        Application.Exit();
                    }
                    else
                    {
                        UserMessage error = new UserMessage("Updater not found. Please reinstall the application.");
                        error.Show();
                    }
                }
            } else if (showScreen)
            {
                string message = "You have the latest version of Rumble Mod Manager!";

                if (Environment.MachineName == "Desktop-49CDQ") message = "You have the latest version. \n You are the developer idiot.";

                UserMessage upToDate = new UserMessage(message);
                upToDate.Show();
            }
        }

        private async Task LaunchManager()
        {
            try
            {
                label1.Visible = true;
                label1.Text = "Fetching Mods...";

                var modsByPage = await ThunderstoreMods.FetchThunderstoreMods(progressBar1);
                if (modsByPage == null || !modsByPage.Any())
                {
                    label1.Text = "Failed to fetch mods...";
                    await Task.Delay(500);
                    return;
                }

                ModCache.ModsByPage = modsByPage;

                string basePath = Properties.Settings.Default.RumblePath;

                if (false /*!Properties.Settings.Default.SkipMapLoading*/) // Custom maps is being rebuilt
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
                if (preloadedPanels == null)
                {
                    throw new InvalidOperationException("Failed to preload mod panels.");
                }

                progressBar1.Visible = false;
                label1.Visible = false;

                if (Directory.Exists(Path.Combine(basePath, "MelonLoader", "Dependencies", "Il2CppAssemblyGenerator", "UnityDependencies")))
                {
                    string melonLoaderPath = Path.Combine(Properties.Settings.Default.RumblePath, "MelonLoader", "net6", "MelonLoader.dll");

                    if (File.Exists(melonLoaderPath))
                    {
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(melonLoaderPath);
                        string currentMelonVersionString = fileVersionInfo.FileVersion;

                        string[] versionParts = currentMelonVersionString.Split('.');
                        while (versionParts.Length < 3)
                        {
                            currentMelonVersionString += ".0";
                        }

                        Version currentMelonVersion = new Version(currentMelonVersionString);

                        string latestMelonVersionString = await GetLatestMelonLoaderVersionAsync();
                        Version latestMelonVersion = new Version(latestMelonVersionString);

                        if (currentMelonVersion < latestMelonVersion)
                        {
                            UserMessage updatePrompt = new UserMessage($"Your current MelonLoader version is out of date. Would you like to update? \n {currentMelonVersion} -> {latestMelonVersion}", false, true);
                            DialogResult result = updatePrompt.ShowDialog();

                            if (result == DialogResult.Yes)
                            {
                                UserMessage updatingPrompt = new UserMessage("Updating MelonLoader...", false);
                                updatingPrompt.Show();

                                await InstallMelonLoader(false);

                                updatingPrompt.UpdateStatusMessage("MelonLoader updated successfully!");
                                updatingPrompt.ShowButtons(true);
                            }
                        }
                    }
                }

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

        public static async Task<string> GetLatestMelonLoaderVersionAsync()
        {
            string latestReleaseUrl = "https://github.com/LavaGang/MelonLoader/releases/latest";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(latestReleaseUrl);
                    response.EnsureSuccessStatusCode();

                    string redirectedUrl = response.RequestMessage.RequestUri.ToString();

                    string version = redirectedUrl.Split('/').Last().TrimStart('v');
                    return version;
                }
            }
            catch (Exception ex)
            {
                return $"Error fetching latest version: {ex.Message}";
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
