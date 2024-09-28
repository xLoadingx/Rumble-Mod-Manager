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
                MessageBox.Show("No mods found in cache. Mods were not found when first loaded. (Contact the developer 'error_real_sir' on discord for help");
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

        public static async Task DownloadModFromInternet(Mod mod, RUMBLEModManager form1, bool ModEnabled, bool initialMod, UserMessage installingMessage = null)
        {
            string tempDir = Path.Combine(Properties.Settings.Default.RumblePath, "temp_mod_download");
            string tempZipPath = Path.Combine(tempDir, "temp_mod.zip");

            try
            {
                // Only create a new form if one doesn't already exist (e.g., the first call)
                if (installingMessage == null)
                {
                    installingMessage = new UserMessage(string.Empty, false);
                    installingMessage.Show();
                }

                installingMessage.UpdateStatusMessage($"Starting installation for '{mod.Name}'");

                // Handle dependencies
                if (mod.Dependencies != null && mod.Dependencies.Any(dependency => !dependency.Contains("LavaGang-MelonLoader", StringComparison.OrdinalIgnoreCase)))
                {
                    installingMessage.UpdateStatusMessage($"Starting installation of dependencies for '{mod.Name}'");

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

                            installingMessage.UpdateStatusMessage($"Attempting to install dependency: {author}-{name}");

                            var dependentMod = FindModByAuthorAndName(author, name);
                            if (dependentMod != null)
                            {
                                // Pass the same installingMessage form to the recursive call
                                await DownloadModFromInternet(dependentMod, form1, ModEnabled, false, installingMessage);
                                installingMessage.UpdateStatusMessage($"Successfully installed: {author}-{name}");
                            }
                            else
                            {
                                installingMessage.UpdateStatusMessage($"Dependency {author}-{name} not found.");
                            }
                        }
                    }

                    installingMessage.UpdateStatusMessage("Dependency installation process completed");
                }

                // Create temp directory if it doesn't exist
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
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

                // Install the main mod
                InstallMod(tempZipPath, ModEnabled);

                if (initialMod)
                {
                    installingMessage.UpdateStatusMessage($"'{mod.Name}' successfully installed");
                    installingMessage.ShowCloseButton(true);
                    form1.LoadMods();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while downloading or extracting the mod: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show($"An error occurred during cleanup: {cleanupEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            bool hasDll = Directory.GetFiles(extractedDir, "*.dll", System.IO.SearchOption.AllDirectories).Any();
            bool hasManifest = Directory.GetFiles(extractedDir, "manifest.json", System.IO.SearchOption.AllDirectories).Any();

            return hasDll && hasManifest;
        }


        public static void InstallMod(string zipPath, bool ModEnabled)
        {
            bool UpdatingMod = false;
            string tempDir = Path.Combine(Properties.Settings.Default.RumblePath, "temp_mod_download");

            try
            {
                // Ensure temp directory exists
                Directory.CreateDirectory(tempDir);

                // Extract the ZIP file to the temp directory
                ZipFile.ExtractToDirectory(zipPath, tempDir);

                if (IsValidMod(tempDir))
                {
                    string modsFolder = Path.Combine(Properties.Settings.Default.RumblePath, ModEnabled ? "Mods" : "DisabledMods");

                    // Ensure mods folder exists
                    Directory.CreateDirectory(modsFolder);

                    // Define the directories we want to process
                    var allowedDirs = new[] { "Mods", "UserLibs", "UserData" };

                    foreach (var subDir in Directory.EnumerateDirectories(tempDir))
                    {
                        string directoryName = Path.GetFileName(subDir);
                        string destinationDir;

                        // Only process allowed directories
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
                            else // "Mods"
                            {
                                destinationDir = modsFolder;
                            }

                            MergeDirectories(subDir, destinationDir, ref UpdatingMod);
                        }
                    }
                } else
                {
                    MessageBox.Show("This ZIP file does not contain a valid mod.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Directory.Delete(tempDir, true);
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access denied: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show($"Directory not found: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"IO error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (ex.Data != null && ex.Data.Count > 0)
                {
                    Console.WriteLine("Exception Data:");
                    foreach (DictionaryEntry entry in ex.Data)
                    {
                        MessageBox.Show($"{entry.Key}: {entry.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("No additional data available.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to install mod: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                catch (IOException ex) when (i < maxRetries - 1)
                {
                    // Wait before retrying
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
                        // Stop if a 404 error is encountered
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
                                var DateUpdated = modDict.GetValueOrDefault("date_updated")?.ToString();

                                if (modName != null && modName.Equals("MelonLoader", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Skip MelonLoader
                                    return;
                                }

                                var versions = modDict.GetValueOrDefault("versions") as JArray;

                                if (versions != null && versions.Count > 0)
                                {
                                    var latestVersion = versions[0] as JObject;

                                    // Check if mod is deprecated
                                    bool isDeprecated = modDict.GetValueOrDefault("is_deprecated")?.ToString().ToLower() == "true";

                                    var dependencies = latestVersion?.GetValue("dependencies")?.ToObject<List<string>>();
                                    bool isOutdated = dependencies != null && dependencies.Any(dep => dep.Contains("MelonLoader-0.5.7", StringComparison.OrdinalIgnoreCase));

                                    string name = modName;
                                    string description = latestVersion?.GetValue("description")?.ToString();
                                    string author = modDict.GetValueOrDefault("owner")?.ToString();
                                    string imageurl = latestVersion?.GetValue("icon")?.ToString();
                                    string version = latestVersion?.GetValue("version_number")?.ToString();
                                    string lastUpdated = DateUpdated;

                                    var mod = new Mod
                                    {
                                        Name = name,
                                        Description = description,
                                        Author = author,
                                        ImageUrl = imageurl,
                                        Version = version,
                                        Dependencies = dependencies,
                                        DateUpdated = lastUpdated,
                                        ModPageUrl = $"https://thunderstore.io/package/download/{author}/{name}/{version}",
                                        OnlinePageUrl = $"https://thunderstore.io/c/rumble/p/{author}/{name}",
                                        isOutdated = isOutdated,
                                        isDeprecated = isDeprecated
                                    };

                                    // Fetch the mod image
                                    if (!string.IsNullOrEmpty(mod.ImageUrl))
                                    {
                                        string imageUrl = mod.ImageUrl.StartsWith("http") ? mod.ImageUrl : $"https://thunderstore.io{mod.ImageUrl}";

                                        using (HttpResponseMessage imageResponse = await client.GetAsync(imageUrl))
                                        {
                                            imageResponse.EnsureSuccessStatusCode();
                                            using (Stream imageStream = await imageResponse.Content.ReadAsStreamAsync())
                                            {
                                                mod.ModImage = Image.FromStream(imageStream);
                                            }
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

                                // Update progress bar in batches to avoid too frequent UI updates
                                if (progressBar.Value < progressBar.Maximum)
                                {
                                    progressBar.Invoke(new Action(() => progressBar.Value++));
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"An error occurred while processing a mod: {ex.Message}");
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }).ToArray();

                        await Task.WhenAll(tasks);
                    }

                    // Reverse and filter the list
                    var reversedModList = modList.Reverse().ToList();
                    reversedModList.RemoveAll(mod => mod.isOutdated || mod.isDeprecated);

                    int pageSize = 24;
                    int totalPages = (int)Math.Ceiling(reversedModList.Count / (double)pageSize);

                    // Sort the entire list by DateUpdated
                    var sortedModList = reversedModList
                        .OrderByDescending(mod => DateTime.Parse(mod.DateUpdated))
                        .ToList();

                    // Create pages from the sorted list
                    for (int i = 0; i < totalPages; i++)
                    {
                        var modsForPage = sortedModList.Skip(i * pageSize).Take(pageSize).ToList();
                        modsByPage[i + 1] = modsForPage;
                    }

                    // Ensure pinned mods are at the top of the first page
                    if (modsByPage.ContainsKey(1))
                    {
                        modsByPage[1] = pinnedModList.Concat(modsByPage[1]).Take(pageSize).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
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
            string searchText = textBox1.Text.ToLower();
            FilterMods(searchText);
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

                    linkLabel1.Click += (sender, e) =>
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = selectedPanel.OnlineModLink,
                            UseShellExecute = true
                        });
                    };

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
    }
}
