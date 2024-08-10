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

namespace Rumble_Mod_Manager
{
    public partial class ThunderstoreMods : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        private static readonly HttpClient client = new HttpClient();
        private Panel selectedPanel;
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
            ModNameLabel.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            ModAuthorLabel.Font = new Font(privateFonts.Families[0], 24.0F, FontStyle.Regular);
            ModVersionLabel.Font = new Font(privateFonts.Families[0], 26.0F, FontStyle.Regular);
            DependenciesLabel.Font = new Font(privateFonts.Families[0], 11.0F, FontStyle.Regular);
            ModDescriptionLabel.Font = new Font(privateFonts.Families[0], 15.0F, FontStyle.Regular);
            InstallButton.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            BackButton.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            ForwardButton.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            PageNumberLabel.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
        }

        private void DisplayMods(List<Mod> mods)
        {
            ModDisplayGrid.Controls.Clear();
            ModDisplayGrid.ColumnCount = 4;
            ModDisplayGrid.RowCount = (mods.Count + 3) / 4;

            // Set styles for rows and columns
            ModDisplayGrid.RowStyles.Clear();
            ModDisplayGrid.ColumnStyles.Clear();

            for (int i = 0; i < ModDisplayGrid.RowCount; i++)
            {
                ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / ModDisplayGrid.RowCount));
            }

            for (int i = 0; i < ModDisplayGrid.ColumnCount; i++)
            {
                ModDisplayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / ModDisplayGrid.ColumnCount));
            }

            // Loop through each mod and create a panel
            foreach (var mod in mods)
            {
                if (mod.Show)
                {
                    // Create a panel for each mod
                    Panel modPanel = new Panel
                    {
                        Width = 140,
                        Height = 140,
                        BorderStyle = BorderStyle.Fixed3D,
                        BackColor = Color.FromArgb(30, 30, 30),
                        Tag = mod.ModPageUrl
                    };

                    Label label = new Label
                    {
                        Text = mod.Name,
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = Color.White,
                        Font = new Font(privateFonts.Families[0], 15.0F, FontStyle.Regular), // Adjusted font size
                        Dock = DockStyle.Fill,
                        MaximumSize = new Size(140, 0), // Set maximum width and enable word wrap
                        AutoEllipsis = true // Show ellipsis if text overflows
                    };

                    label.Click += (s, e) => ModPanel_Click(modPanel, mod);

                    // Add controls to panel
                    modPanel.Controls.Add(label);
                    modPanel.Click += (s, e) => ModPanel_Click(modPanel, mod);

                    // Add panel to TableLayoutPanel
                    ModDisplayGrid.Controls.Add(modPanel);
                }
            }
        }

        public static async Task DownloadModFromInternet(string ModPageUrl, RUMBLEModManager form1, bool ModEnabled)
        {
            string tempDir = Path.Combine(Properties.Settings.Default.RumblePath, "temp_mod_download");

            // Ensure the directory exists
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            string tempZipPath = Path.Combine(tempDir, "temp_mod.zip");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(ModPageUrl);
                    response.EnsureSuccessStatusCode();

                    using (Stream zipStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (FileStream fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                        {
                            await zipStream.CopyToAsync(fileStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while downloading or extracting the mod: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            InstallMod(tempZipPath, form1, ModEnabled);
        }

        private async void InstallButton_Click(object sender, EventArgs e)
        {
            await DownloadModFromInternet(CurrentlySelectedMod.ModPageUrl, RumbleManager, true);
        }

        public static void InstallMod(string zipPath, RUMBLEModManager form1, bool ModEnabled)
        {
            bool UpdatingMod = false;
            string tempDir = Path.Combine(Properties.Settings.Default.RumblePath, "temp_mod_download");

            try
            {
                // Ensure temp directory exists
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                ZipFile.ExtractToDirectory(zipPath, tempDir);

                string modsFolder = Path.Combine(Properties.Settings.Default.RumblePath, ModEnabled ? "Mods" : "DisabledMods");

                // Ensure mods folder exists
                if (!Directory.Exists(modsFolder))
                {
                    Directory.CreateDirectory(modsFolder);
                }

                foreach (var item in Directory.EnumerateFiles(tempDir, "*.dll", System.IO.SearchOption.TopDirectoryOnly))
                {
                    string destFilePath = Path.Combine(modsFolder, Path.GetFileName(item));

                    if (File.Exists(destFilePath))
                    {
                        UpdatingMod = true;
                    }

                    RetryFileOperation(() => File.Copy(item, destFilePath, true));
                    File.Delete(item);
                }

                foreach (var subDir in Directory.EnumerateDirectories(tempDir))
                {
                    foreach (var item in Directory.EnumerateFiles(subDir, "*.dll", System.IO.SearchOption.AllDirectories))
                    {
                        string destFilePath = Path.Combine(modsFolder, Path.GetFileName(item));

                        if (File.Exists(destFilePath))
                        {
                            UpdatingMod = true;
                        }

                        RetryFileOperation(() => File.Copy(item, destFilePath, true));
                        File.Delete(item);
                    }
                }

                // Copy the entire UserData folder if it exists
                string userDataSource = Path.Combine(tempDir, "UserData");
                if (Directory.Exists(userDataSource))
                {
                    string userDataDest = Path.Combine(Properties.Settings.Default.RumblePath, "UserData");
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(userDataSource, userDataDest, true);
                }

                // Copy the entire UserLibs folder if it exists
                string userLibsSource = Path.Combine(tempDir, "UserLibs");
                if (Directory.Exists(userLibsSource))
                {
                    string userLibsDest = Path.Combine(Properties.Settings.Default.RumblePath, "UserLibs");
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(userLibsSource, userLibsDest, true);
                }

                Directory.Delete(tempDir, true);

                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                string message = UpdatingMod ? "Mod update complete!" : "Mod installation complete!";
                MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                form1.LoadMods();
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



        private static void RetryFileOperation(Action fileOperation)
        {
            int retryCount = 0;
            bool successful = false;

            while (!successful && retryCount < 3)
            {
                try
                {
                    fileOperation();
                    successful = true;
                }
                catch (IOException)
                {
                    retryCount++;
                    Thread.Sleep(1000); // Wait for a second before retrying
                }
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

        public static async Task<Dictionary<int, List<Mod>>> FetchThunderstoreMods(ProgressBar progressBar)
        {
            var modsByPage = new Dictionary<int, List<Mod>>();

            var modList = new ConcurrentBag<Mod>();
            var pinnedModList = new ConcurrentBag<Mod>();
            var modNameSet = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

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

                    using (var semaphore = new SemaphoreSlim(5)) // Limit to 5 concurrent tasks
                    {
                        var tasks = mods.Select(async modDict =>
                        {
                            await semaphore.WaitAsync();
                            try
                            {
                                var modName = modDict.GetValueOrDefault("name")?.ToString();
                                var DateUpdated = modDict.GetValueOrDefault("date_updated")?.ToString();

                                bool DisplayMod = true;
                                if (modName != null && modName.Equals("MelonLoader", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Skip MelonLoader mods
                                    return;
                                }

                                var versions = modDict.GetValueOrDefault("versions") as JArray;

                                if (versions != null && versions.Count > 0)
                                {
                                    var latestVersion = versions[0] as JObject;

                                    // Check if mod is deprecated
                                    bool isDeprecated = modDict.GetValueOrDefault("is_deprecated")?.ToString().ToLower() == "true";
                                    if (isDeprecated)
                                    {
                                        DisplayMod = false;
                                    }

                                    // Check for MelonLoader dependency version 0.5.7
                                    var dependencies = latestVersion?.GetValue("dependencies")?.ToObject<List<string>>();
                                    if (dependencies != null && dependencies.Any(dep => dep.Contains("MelonLoader-0.5.7", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        DisplayMod = false;
                                    }

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
                                        Show = DisplayMod
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

                                    if (!modNameSet.TryAdd(name, true))
                                    {
                                        // Duplicate name found, skip this mod
                                        return;
                                    }

                                    if (modDict.GetValueOrDefault("is_pinned")?.ToString().ToLower() == "true")
                                    {
                                        pinnedModList.Add(mod);
                                    }
                                    else
                                    {
                                        modList.Add(mod);
                                    }

                                    // Update progress bar in batches to avoid too frequent UI updates
                                    if (progressBar.Value < progressBar.Maximum)
                                    {
                                        progressBar.Invoke(new Action(() => progressBar.Value++));
                                    }
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

                    // Combine pinned mods and regular mods for the front page
                    var combinedModList = pinnedModList.Concat(modList).ToList();

                    int pageSize = 26;
                    int totalPages = (int)Math.Ceiling(combinedModList.Count / (double)pageSize);

                    for (int i = 0; i < totalPages; i++)
                    {
                        var modsForPage = combinedModList.Skip(i * pageSize).Take(pageSize).ToList();
                        modsForPage.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)); // Sort each page alphabetically
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
            public string ImageUrl { get; set; }
            public string Version { get; set; }
            public Image ModImage { get; set; }
            public string DateUpdated { get; set; }
            public List<string> Dependencies { get; set; } = new List<string>();
            public bool Show { get; set; } = true;
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
                var filteredMods = allMods.Where(mod => mod.Name.ToLower().Contains(searchText) ||
                                                        mod.Description.ToLower().Contains(searchText) ||
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
                    ModDisplayGrid.Controls.Clear();
                    PageNumberLabel.Text = "No mods found";
                    BackButton.Enabled = false;
                    ForwardButton.Enabled = false;
                }
            }
        }

        private void ModPanel_Click(object sender, Mod mod)
        {
            Panel panel = sender as Panel;

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
                    DependenciesLabel.Text = string.Empty;
                }
            }
        }
    }
}
