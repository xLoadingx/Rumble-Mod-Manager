using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO.Compression;
using Rumble_Mod_Manager.Properties;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.Xml.Linq;
using System.Globalization;
using static System.Windows.Forms.DataFormats;

namespace Rumble_Mod_Manager
{
    public partial class ThunderstoreMods : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        private static readonly HttpClient client = new HttpClient();
        private ThunderstoreModDisplay selectedPanel;
        private Mod CurrentlySelectedMod;
        private int CurrentPage = 1;
        RUMBLEModManager RumbleManager;

        public ThunderstoreMods(RUMBLEModManager form1)
        {
            InitializeComponent();
            LoadCustomFont();
            this.Text = "Thunderstore Mods";
            RumbleManager = form1;
            PageNumberLabel.Text = $"Page {CurrentPage}";
            InstallButton.Visible = false;
            DependenciesLabel.Visible = false;
            ModDescriptionLabel.Visible = false;
            linkLabel1.Visible = false;
            BackButton.Enabled = CurrentPage > 1;
            ForwardButton.Enabled = ModCache.ModsByPage.ContainsKey(CurrentPage + 1);

            if (ModCache.ModsByPage != null && ModCache.ModsByPage.ContainsKey(CurrentPage))
            {
                DisplayMods(ModCache.ModsByPage[CurrentPage]);
            }
            else
            {
                UserMessage errorMessage = new UserMessage("No mods found in cache. Mods were not found when first loaded. (Contact the developer 'error_real_sir' on discord for help)", true);
                errorMessage.Show();
            }

            ModAuthorLabel.Text = string.Empty;
            ModDescriptionLabel.Text = string.Empty;
            ModNameLabel.Text = string.Empty;
            ModVersionLabel.Text = string.Empty;
        }

        private void LoadCustomFont()
        {
            using (FileStream fs = new FileStream("GoodDogPlain.ttf", FileMode.Open, FileAccess.Read))
            {
                privateFonts.AddFontFile(fs.Name);
            }
            ModNameLabel.Font = new Font(privateFonts.Families[0], 24.0F, FontStyle.Bold);
            ModAuthorLabel.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            ModVersionLabel.Font = new Font(privateFonts.Families[0], 24.0F, FontStyle.Regular);
            linkLabel1.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            DependenciesLabel.Font = new Font(privateFonts.Families[0], 11.0F, FontStyle.Regular);
            ModDescriptionLabel.Font = new Font(privateFonts.Families[0], 15.0F, FontStyle.Regular);
            InstallButton.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            BackButton.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            ForwardButton.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            PageNumberLabel.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
        }

        private void DisplayMods(List<Mod> mods)
        {
            panel2.Controls.Clear();

            List<ThunderstoreModDisplay> modPanels = new List<ThunderstoreModDisplay>();

            foreach (var mod in mods)
            {
                if (!(mod.isOutdated || mod.isDeprecated))
                {
                    ThunderstoreModDisplay modPanel = CreateModPanel(mod);
                    modPanels.Add(modPanel);
                }
            }

            int panelWidth = 171;
            int panelHeight = 265;
            int ceilingMargin = 10; // Space between the top of panel2 and the first row
            int wallMargin = 10; // Wall margin for both sides

            int availableWidth = panel2.ClientSize.Width - 2 * wallMargin;
            int panelsPerRow = Math.Max(1, availableWidth / panelWidth);

            // Calculate the total space left after fitting panels
            int totalPanelWidth = panelsPerRow * panelWidth;
            int remainingSpace = availableWidth - totalPanelWidth;

            // Distribute the remaining space as padding between panels
            int horizontalSpacing = panelsPerRow > 1 ? remainingSpace / (panelsPerRow - 1) : 0;

            for (int i = 0; i < modPanels.Count; i++)
            {
                int row = i / panelsPerRow;
                int col = i % panelsPerRow;

                // Calculate the X and Y positions for each panel
                int x = wallMargin + col * (panelWidth + horizontalSpacing);
                int y = ceilingMargin + row * (panelHeight + ceilingMargin);

                modPanels[i].Size = new Size(panelWidth, panelHeight);
                modPanels[i].Location = new Point(x, y);
                panel2.Controls.Add(modPanels[i]);
            }

            // Set auto scroll options
            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Maximum = 0;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;

        }

        private ThunderstoreModDisplay CreateModPanel(Mod mod)
        {
            ThunderstoreModDisplay modPanel = new ThunderstoreModDisplay
            {
                ModImage = mod.ModImage,
                ModNameLabel = mod.Name,
                DescriptionLabel = mod.Description,
                CreditsLabel = $"By {mod.Author}",
                DescriptionFont = new Font(privateFonts.Families[0], 10.0f, FontStyle.Regular),
                ModLabelFont = new Font(privateFonts.Families[0], 12.0f, FontStyle.Bold),
                CreditsFont = new Font(privateFonts.Families[0], 10.0f, FontStyle.Regular),
                OnlineModLink = mod.OnlinePageUrl
            };

            modPanel.Click += (s, e) => ModPanel_Click(modPanel, mod);

            foreach (Control control in modPanel.Controls)
            {
                control.Click += (s, e) => ModPanel_Click(modPanel, mod);
            }

            return modPanel;
        }

        public static async Task DownloadModFromInternet(Mod mod, RUMBLEModManager form1, bool ModEnabled, bool initialMod, UserMessage installingMessage = null, ModPanelControl panel = null, bool showMessage = true)
        {
            string tempDir = Path.Combine(Properties.Settings.Default.RumblePath, $"temp_mod_download_{Guid.NewGuid()}");
            string tempZipPath = Path.Combine(tempDir, "temp_mod.zip");

            try
            {
                if (installingMessage == null && showMessage)
                {
                    installingMessage = new UserMessage(string.Empty, false);
                    installingMessage.Show();
                }

                installingMessage?.UpdateStatusMessage($"Starting installation for '{mod.Name}'");

                if (mod.Dependencies != null && mod.Dependencies.Any(dependency => !dependency.Contains("LavaGang-MelonLoader", StringComparison.OrdinalIgnoreCase)))
                {
                    installingMessage?.UpdateStatusMessage($"Starting installation of dependencies for '{mod.Name}'");

                    foreach (var dependency in mod.Dependencies)
                    {
                        if (dependency.Contains("LavaGang-MelonLoader", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var parts = dependency.Split('-');
                        if (parts.Length >= 2)
                        {
                            string author = parts[0];
                            string name = parts[1];

                            installingMessage?.UpdateStatusMessage($"Attempting to install dependency: {author}-{name}");

                            var dependentMod = FindModByAuthorAndName(author, name);
                            if (dependentMod != null)
                            {
                                await DownloadModFromInternet(dependentMod, form1, ModEnabled, false, installingMessage, showMessage: showMessage);
                                installingMessage?.UpdateStatusMessage($"Successfully installed: {author}-{name}");
                            }
                            else
                            {
                                installingMessage?.UpdateStatusMessage($"Dependency {author}-{name} not found.");
                            }
                        }
                    }

                    installingMessage?.UpdateStatusMessage("Dependency installation process completed");
                }

                if (!Directory.Exists(tempDir))
                {
                    try
                    {
                        Directory.CreateDirectory(tempDir);
                    }
                    catch (Exception ex)
                    {
                        UserMessage errorMessage = new UserMessage($"Failed to create temp directory: {ex.Message}", true);
                        errorMessage.Show();
                        installingMessage?.Close();
                        return;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(mod.ModPageUrl);
                    response.EnsureSuccessStatusCode();

                    using (Stream zipStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (FileStream fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                        {
                            await zipStream.CopyToAsync(fileStream);
                        }
                    }
                }

                if (!File.Exists(tempZipPath))
                {
                    UserMessage errorMessage = new UserMessage("Downloaded mod zip file could not be found. Please check if the download was successful.", true);
                    errorMessage.Show();
                    return;
                }

                await InstallMod(tempZipPath, ModEnabled, form1, installingMessage, initialMod);

                if (initialMod)
                {
                    installingMessage?.UpdateStatusMessage($"'{mod.Name}' successfully installed");
                    installingMessage?.ShowButtons(true);

                    string profilePath = Path.Combine(Properties.Settings.Default.RumblePath, "Mod_Profiles", $"{Properties.Settings.Default.LastLoadedProfile}_profile.json");
                    string json = File.ReadAllText(profilePath);
                    var profile = JsonConvert.DeserializeObject<ModProfile>(json);

                    form1.LoadMods(profile);
                }
            }
            catch (Exception ex)
            {
                UserMessage errorMessage = new UserMessage($"An error occurred while downloading or extracting the mod: {ex.Message}", true);
                errorMessage.Show();
            }
            finally
            {
                // Cleanup: delete the temp files and folder
                try
                {
                    if (File.Exists(tempZipPath))
                    {
                        File.Delete(tempZipPath);
                    }

                    if (Directory.Exists(tempDir) && Directory.GetFiles(tempDir).Length == 0)
                    {
                        Directory.Delete(tempDir);
                    }
                }
                catch (Exception cleanupEx)
                {
                    UserMessage errorMessage = new UserMessage($"An error occurred during cleanup: {cleanupEx.Message}", true);
                    errorMessage.Show();
                }
            }
        }

        private static Mod FindModByAuthorAndName(string author, string name)
        {
            foreach (var page in ModCache.ModsByPage.Values)
            {
                foreach (var mod in page)
                {
                    if (mod.Author.Equals(author, StringComparison.OrdinalIgnoreCase) &&
                        mod.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return mod;
                    }
                }
            }

            return null; // If the mod is not found
        }

        private async void InstallButton_Click(object sender, EventArgs e)
        {
            await DownloadModFromInternet(CurrentlySelectedMod, RumbleManager, true, true);
        }

        private static bool IsValidMod(string extractedDir)
        {
            return Directory.GetFiles(extractedDir, "*.dll", System.IO.SearchOption.AllDirectories).Any();
        }

        public static async Task<ModPanelControl> InstallMod(string zipPath, bool ModEnabled, RUMBLEModManager form1 = null, UserMessage message = null, bool initialMod = false)
        {
            bool UpdatingMod = false;
            string tempDir = Path.Combine(Properties.Settings.Default.RumblePath, "temp_mod_download");

            try
            {
                Directory.CreateDirectory(tempDir);

                ZipFile.ExtractToDirectory(zipPath, tempDir);

                if (IsValidMod(tempDir))
                {
                    string modsFolder = Path.Combine(Properties.Settings.Default.RumblePath, ModEnabled ? "Mods" : "DisabledMods");

                    Directory.CreateDirectory(modsFolder);

                    var allowedDirs = new[] { "Mods", "UserLibs", "UserData" };

                    string dllFilePath = Directory.GetFiles(Path.Combine(tempDir, "Mods"))[0];
                    string modPath = Path.Combine(modsFolder, Path.GetFileName(dllFilePath));

                    if (form1 != null)
                    {
                        foreach (var panel in form1.preloadedModPanels)
                        {
                            if (string.Equals(Path.GetFileNameWithoutExtension(dllFilePath), panel.ModNameLabel, StringComparison.OrdinalIgnoreCase))
                            {
                                if (message != null)
                                {
                                    message.UpdateStatusMessage("Mod already installed. Updating existing mod...");

                                    await Task.Delay(100);
                                }
                                
                                if (File.Exists(modPath))
                                {
                                    File.Delete(modPath);
                                }
                                File.Copy(dllFilePath, modPath, true);
                            }
                        }
                    }

                    foreach (var subDir in Directory.EnumerateDirectories(tempDir))
                    {
                        string directoryName = Path.GetFileName(subDir);
                        string destinationDir;

                        if (allowedDirs.Contains(directoryName, StringComparer.OrdinalIgnoreCase))
                        {
                            if (directoryName.Equals("UserLibs", StringComparison.OrdinalIgnoreCase))
                            {
                                destinationDir = Path.Combine(Properties.Settings.Default.RumblePath, "UserLibs");
                            }
                            else if (directoryName.Equals("UserData", StringComparison.OrdinalIgnoreCase))
                            {
                                destinationDir = Path.Combine(Properties.Settings.Default.RumblePath, "UserData");
                            }
                            else 
                            {
                                destinationDir = modsFolder;
                            }

                            MergeDirectories(subDir, destinationDir, ref UpdatingMod);
                        }
                    }

                    if (form1 != null)
                    {
                        ModPanelControl newModPanel = LaunchPage.CreateModPanel(modPath, ModEnabled, Properties.Settings.Default.RumblePath);
                        newModPanel.Click += form1.ModPanel_Click;
                        
                        form1.AddOrUpdateModPanel(newModPanel);
                        form1.AdjustPanelLocations();
                        form1.SaveProfile(Properties.Settings.Default.LastLoadedProfile, false);

                        return newModPanel;
                    }

                } else
                {
                    UserMessage errorMessage = new UserMessage("This ZIP file does not contain a valid mod.", true);
                    errorMessage.ShowDialog();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                UserMessage errorMessage = new UserMessage($"Access denied: {ex.Message}", true);
                errorMessage.Show();
            }
            catch (DirectoryNotFoundException ex)
            {
                UserMessage errorMessage = new UserMessage($"Directory not found: {ex.Message}", true);
                errorMessage.Show();
            }
            catch (IOException ex)
            {
                UserMessage errorMessage = new UserMessage($"IO error: {ex.Message}", true);
                errorMessage.Show();
            }
            catch (Exception ex)
            {
                UserMessage errorMessage = new UserMessage($"Failed to install mod: {ex.Message}", true);
                errorMessage.Show();
            }
            finally
            {
                Directory.Delete(tempDir, true);
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
            }

            return null;
        }

        private static void MergeDirectories(string sourceDir, string targetDir, ref bool updatingMod)
        {
            // Create all directories first
            foreach (var dirPath in Directory.EnumerateDirectories(sourceDir, "*", System.IO.SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(sourceDir, dirPath);
                string destDirPath = Path.Combine(targetDir, relativePath);

                if (!Directory.Exists(destDirPath))
                {
                    Directory.CreateDirectory(destDirPath);
                }
            }

            // Copy all files, overwrite if exists
            foreach (var filePath in Directory.EnumerateFiles(sourceDir, "*.*", System.IO.SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(sourceDir, filePath);
                string destFilePath = Path.Combine(targetDir, relativePath);

                // If the file already exists in the destination, it means we're updating
                if (File.Exists(destFilePath))
                {
                    updatingMod = true;
                }

                RetryFileOperation(() => File.Copy(filePath, destFilePath, true));
                File.Delete(filePath);
            }
        }

        private static void RetryFileOperation(Action fileOperation, int maxRetries = 3, int delayMilliseconds = 1000)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    fileOperation();
                    return;
                }
                catch when (i < maxRetries - 1)
                {
                    Thread.Sleep(delayMilliseconds);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings settingsForm = new Settings(RumbleManager, null);
            settingsForm.Show();
        }

        public static async Task<Dictionary<int, List<Mod>>> FetchThunderstoreMods(Guna.UI2.WinForms.Guna2CircleProgressBar progressBar)
        {
            var modsByPage = new Dictionary<int, List<Mod>>();
            var modList = new ConcurrentBag<Mod>();
            var pinnedModList = new ConcurrentBag<Mod>();

            try
            {
                string url = "https://thunderstore.io/c/rumble/api/v1/package/?page=1";

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://thunderstore.io/");
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("request");

                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return modsByPage;
                    }
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var mods = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseBody);

                    if (mods == null || mods.Count == 0)
                    {
                        return modsByPage;
                    }

                    progressBar.Maximum = mods.Count;
                    progressBar.Value = 0;

                    using (var semaphore = new SemaphoreSlim(20))
                    {
                        var tasks = mods.Select(async modDict =>
                        {
                            await semaphore.WaitAsync();
                            try
                            {
                                var modName = modDict.GetValueOrDefault("name")?.ToString();
                                var dateUpdated = modDict.GetValueOrDefault("date_updated")?.ToString();

                                if (modName != null && (modName.Equals("MelonLoader", StringComparison.OrdinalIgnoreCase) || modName.Equals("GaleModManager", StringComparison.OrdinalIgnoreCase)))
                                {
                                    return;
                                }

                                var versions = modDict.GetValueOrDefault("versions") as JArray;

                                if (versions != null && versions.Count > 0)
                                {
                                    var latestVersion = versions[0] as JObject;

                                    bool isDeprecated = modDict.GetValueOrDefault("is_deprecated")?.ToString().ToLower() == "true";

                                    var dependencies = latestVersion?.GetValue("dependencies")?.ToObject<List<string>>();
                                    bool isOutdated = dependencies != null && dependencies.Any(dep => dep.Contains("MelonLoader-0.5.7", StringComparison.OrdinalIgnoreCase));

                                    string name = modName;
                                    string description = latestVersion?.GetValue("description")?.ToString();
                                    string author = modDict.GetValueOrDefault("owner")?.ToString();
                                    string imageUrl = latestVersion?.GetValue("icon")?.ToString();
                                    string version = latestVersion?.GetValue("version_number")?.ToString();

                                    var mod = new Mod
                                    {
                                        Name = name,
                                        Description = description,
                                        Author = author,
                                        ImageUrl = imageUrl,
                                        Version = version,
                                        Dependencies = dependencies,
                                        DateUpdated = dateUpdated,
                                        ModPageUrl = $"https://thunderstore.io/package/download/{author}/{name}/{version}",
                                        OnlinePageUrl = $"https://thunderstore.io/c/rumble/p/{author}/{name}",
                                        isOutdated = isOutdated,
                                        isDeprecated = isDeprecated
                                    };

                                    if (!string.IsNullOrEmpty(mod.ImageUrl))
                                    {
                                        string fullImageUrl = mod.ImageUrl.StartsWith("http") ? mod.ImageUrl : $"https://thunderstore.io{mod.ImageUrl}";

                                        try
                                        {
                                            using (HttpResponseMessage imageResponse = await client.GetAsync(fullImageUrl))
                                            {
                                                imageResponse.EnsureSuccessStatusCode();
                                                using (Stream imageStream = await imageResponse.Content.ReadAsStreamAsync())
                                                {
                                                    mod.ModImage = Image.FromStream(imageStream);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            // Assign a placeholder image or log the issue if the image download fails
                                            mod.ModImage = Properties.Resources.UnknownMod;
                                        }
                                    }

                                    if (modDict.GetValueOrDefault("is_pinned")?.ToString().ToLower() == "true")
                                    {
                                        pinnedModList.Add(mod);
                                    }
                                    else
                                    {
                                        modList.Add(mod);
                                    }
                                }

                                if (progressBar.Value < progressBar.Maximum)
                                {
                                    progressBar.Invoke(new Action(() =>
                                    {
                                        progressBar.Value += 1;
                                    }));
                                }
                            }
                            catch (Exception ex)
                            {
                                UserMessage errorMessage = new UserMessage($"An error occurred while processing a mod: {ex.Message}", true);
                                errorMessage.Show();
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }).ToList();

                        await Task.WhenAll(tasks);
                    }

                    var reversedModList = modList.Reverse().ToList();
                    reversedModList.RemoveAll(mod => mod.isOutdated || mod.isDeprecated);

                    int pageSize = 24;
                    int totalPages = (int)Math.Ceiling(reversedModList.Count / (double)pageSize);

                    var sortedModList = reversedModList
                        .OrderByDescending(mod => DateTime.Parse(mod.DateUpdated))
                        .ToList();

                    for (int i = 0; i < totalPages; i++)
                    {
                        var modsForPage = sortedModList.Skip(i * pageSize).Take(pageSize).ToList();
                        modsByPage[i + 1] = modsForPage;
                    }

                    if (modsByPage.ContainsKey(1))
                    {
                        modsByPage[1] = pinnedModList.Concat(modsByPage[1]).Take(pageSize).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                UserMessage errorMessage = new UserMessage($"An error occurred: {ex.Message}", true);
                errorMessage.Show();
            }

            return modsByPage;
        }

        public class Mod
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public string ModPageUrl { get; set; }
            public string OnlinePageUrl { get; set; }
            public string ImageUrl { get; set; }
            public string Version { get; set; }
            public Image ModImage { get; set; }
            public string DateUpdated { get; set; }
            public List<string> Dependencies { get; set; } = new List<string>();
            public bool isOutdated { get; set; } = true;
            public bool isDeprecated { get; set; } = false;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                DisplayMods(ModCache.ModsByPage[CurrentPage]);
                PageNumberLabel.Text = $"Page {CurrentPage}";
                BackButton.Enabled = CurrentPage > 1;
                ForwardButton.Enabled = ModCache.ModsByPage.ContainsKey(CurrentPage + 1);
            }
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            if (ModCache.ModsByPage.ContainsKey(CurrentPage + 1))
            {
                CurrentPage++;
                DisplayMods(ModCache.ModsByPage[CurrentPage]);
                PageNumberLabel.Text = $"Page {CurrentPage}";
                BackButton.Enabled = CurrentPage > 1;
                ForwardButton.Enabled = ModCache.ModsByPage.ContainsKey(CurrentPage + 1);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (debounceTimer != null)
            {
                debounceTimer.Stop();
                debounceTimer.Dispose(); // Dispose of the old timer before creating a new one
            }

            debounceTimer = new System.Windows.Forms.Timer();
            debounceTimer.Interval = 300; // Adjust debounce delay (300ms here)
            debounceTimer.Tick += (s, ev) =>
            {
                debounceTimer.Stop();
                debounceTimer.Dispose(); // Clean up timer
                string searchText = textBox1.Text.ToLower();
                FilterMods(searchText);
            };

            debounceTimer.Start();
        }

        private void FilterMods(string searchText)
        {
            if (ModCache.ModsByPage != null && ModCache.ModsByPage.Count > 0)
            {
                var allMods = ModCache.ModsByPage.Values.SelectMany(modList => modList).ToList();
                var filteredMods = allMods.Where(mod => mod.Name.ToLower().Contains(searchText.Replace(" ", "_")) ||
                                                        mod.Description.ToLower().Contains(searchText) ||
                                                        mod.isOutdated ||
                                                        mod.isDeprecated ||
                                                        mod.Author.ToLower().Contains(searchText)).ToList();

                // Paginate filtered results
                int pageSize = 26;
                int totalPages = (int)Math.Ceiling(filteredMods.Count / (double)pageSize);

                var filteredModsByPage = new Dictionary<int, List<Mod>>();
                for (int i = 0; i < totalPages; i++)
                {
                    var modsForPage = filteredMods.Skip(i * pageSize).Take(pageSize).ToList();
                    filteredModsByPage[i + 1] = modsForPage;
                }

                // Update the display
                if (filteredModsByPage.ContainsKey(1))
                {
                    CurrentPage = 1;
                    DisplayMods(filteredModsByPage[CurrentPage]);
                    PageNumberLabel.Text = $"Page {CurrentPage}";
                    BackButton.Enabled = CurrentPage > 1;
                    ForwardButton.Enabled = filteredModsByPage.ContainsKey(CurrentPage + 1);
                }
                else
                {
                    PageNumberLabel.Text = "No mods found";
                    BackButton.Enabled = false;
                    ForwardButton.Enabled = false;
                }
            }
        }

        private void ModPanel_Click(object sender, Mod mod)
        {
            ThunderstoreModDisplay panel = sender as ThunderstoreModDisplay;

            CurrentlySelectedMod = mod;
            if (panel != null)
            {
                // Deselect the previous panel
                if (selectedPanel != null)
                {
                    selectedPanel.BackColor = Color.FromArgb(30, 30, 30);
                }

                if (selectedPanel != panel)
                {
                    // Select the new panel
                    selectedPanel = panel;
                    selectedPanel.BackColor = Color.LightBlue;

                    ModPictureDisplay.Image = CurrentlySelectedMod.ModImage;
                    ModAuthorLabel.Text = $" By {CurrentlySelectedMod.Author}";
                    ModDescriptionLabel.Text = CurrentlySelectedMod.Description;
                    ModNameLabel.Text = CurrentlySelectedMod.Name;
                    ModVersionLabel.Text = $"Version {CurrentlySelectedMod.Version}";
                    InstallButton.Visible = true;
                    ModDescriptionLabel.Visible = true;
                    DependenciesLabel.Visible = true;
                    linkLabel1.Visible = true;

                    linkLabel1.Click -= LinkLabel1_Click;
                    linkLabel1.Click += LinkLabel1_Click;

                    var cleanedDependencies = CurrentlySelectedMod.Dependencies
                        .Where(d => !d.StartsWith("MelonLoader", StringComparison.OrdinalIgnoreCase))
                        .Select(d => d.Replace(" icon", "").Trim())
                        .ToList();

                    DependenciesLabel.Text = $"Dependencies:\n{string.Join("\n", cleanedDependencies)}";
                }
                else
                {
                    selectedPanel = null;
                    ModPictureDisplay.Image = null;
                    ModAuthorLabel.Text = string.Empty;
                    ModDescriptionLabel.Text = string.Empty;
                    ModNameLabel.Text = string.Empty;
                    ModVersionLabel.Text = string.Empty;
                    InstallButton.Visible = false;
                    DependenciesLabel.Visible = false;
                    ModDescriptionLabel.Visible = false;
                    linkLabel1.Visible = false;
                    DependenciesLabel.Text = string.Empty;
                }
            }
        }

        private void LinkLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = selectedPanel.OnlineModLink,
                UseShellExecute = true
            });
        }
    }
}
