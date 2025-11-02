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

namespace Rumble_Mod_Manager
{
    public partial class CustomMaps : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        private static readonly HttpClient client = new HttpClient();
        private ThunderstoreModDisplay selectedPanel;
        private CustomMap CurrentlySelectedMap;
        private int CurrentPage = 1;
        RUMBLEModManager RumbleManager;

        public CustomMaps(RUMBLEModManager form1)
        {
            InitializeComponent();
            LoadCustomFont();
            this.Text = "Thunderstore Mods";
            RumbleManager = form1;
            PageNumberLabel.Text = $"Page {CurrentPage}";
            InstallButton.Visible = false;
            ModDescriptionLabel.Visible = false;
            BackButton.Enabled = CurrentPage > 1;
            ForwardButton.Enabled = ModCache.ModsByPage.ContainsKey(CurrentPage + 1);

            if (CustomMapsCache.MapsByPage != null && CustomMapsCache.MapsByPage.ContainsKey(CurrentPage))
            {
                DisplayMaps(CustomMapsCache.MapsByPage[CurrentPage]);
            }
            else
            {
                UserMessage errorMessage = new UserMessage("No maps found in cache. Maps were not found when first loaded. (Try restarting the manager)", true);
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
            ModNameLabel.Font = new Font(privateFonts.Families[0], 18.0F, FontStyle.Regular);
            ModAuthorLabel.Font = new Font(privateFonts.Families[0], 24.0F, FontStyle.Regular);
            ModVersionLabel.Font = new Font(privateFonts.Families[0], 26.0F, FontStyle.Regular);
            ModDescriptionLabel.Font = new Font(privateFonts.Families[0], 15.0F, FontStyle.Regular);
            InstallButton.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            BackButton.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            ForwardButton.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
            PageNumberLabel.Font = new Font(privateFonts.Families[0], 27.0F, FontStyle.Regular);
        }

        private void DisplayMaps(List<CustomMap> maps)
        {
            if (selectedPanel != null)
            {
                selectedPanel.BackColor = Color.FromArgb(30, 30, 30);
                selectedPanel = null;
            }

            var mapPanels = new List<ThunderstoreModDisplay>();

            // Loop through each mod and create a panel
            foreach (var map in maps)
            {
                // Create a panel for each mod
                ThunderstoreModDisplay modPanel = new ThunderstoreModDisplay
                {
                    BackColor = Color.FromArgb(30, 30, 30),
                    Tag = map.downloadLink,
                    ModNameLabel = map.name,
                    CreditsLabel = $"By {map.author}",
                    ModLabelFont = new Font(privateFonts.Families[0], 12.0f, FontStyle.Bold),
                    CreditsFont = new Font(privateFonts.Families[0], 10.0f, FontStyle.Bold),
                    DescriptionFont = new Font(privateFonts.Families[0], 10.0f, FontStyle.Bold),
                    ModImage = map.mapImage,
                    DescriptionLabel = map.description
                };

                modPanel.Click += (s, e) => ModPanel_Click(modPanel, map);

                foreach (Control control in modPanel.Controls)
                {
                    control.Click += (s, e) => ModPanel_Click(modPanel, map);
                }

                mapPanels.Add(modPanel);
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

            for (int i = 0; i < mapPanels.Count; i++)
            {
                int row = i / panelsPerRow;
                int col = i % panelsPerRow;

                // Calculate the X and Y positions for each panel
                int x = wallMargin + col * (panelWidth + horizontalSpacing);
                int y = ceilingMargin + row * (panelHeight + ceilingMargin);

                mapPanels[i].Size = new Size(panelWidth, panelHeight);
                mapPanels[i].Location = new Point(x, y);
                panel2.Controls.Add(mapPanels[i]);
            }

            // Set auto scroll options
            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Maximum = 0;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;
        }

        public static async Task DownloadMapFromInternet(string ModPageUrl, RUMBLEModManager form1, bool ModEnabled)
        {
            string tempDir = Path.Combine(Properties.Settings.Default.RumblePath, "temp_mod_download");
            Directory.CreateDirectory(tempDir);

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
                UserMessage errorMessage = new UserMessage($"An error occurred while downloading or extracting the mod: {ex.Message}", true);
                errorMessage.Show();
                return;
            }
        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            return;
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

        private void ModPanel_Click(object sender, CustomMap map)
        {
            ThunderstoreModDisplay panel = sender as ThunderstoreModDisplay;

            CurrentlySelectedMap = map;
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

                    ModPictureDisplay.Image = CurrentlySelectedMap.mapImage;
                    ModAuthorLabel.Text = $" By {CurrentlySelectedMap.author}";
                    ModDescriptionLabel.Text = CurrentlySelectedMap.description;
                    ModNameLabel.Text = CurrentlySelectedMap.name;
                    ModVersionLabel.Text = $"Version {CurrentlySelectedMap.version}";
                    InstallButton.Visible = true;
                    ModDescriptionLabel.Visible = true;

                    string mapsDirectory = Path.Combine(Properties.Settings.Default.RumblePath, "UserData", "CustomMultiplayerMaps", "Maps");
                    if (File.Exists(Path.Combine(mapsDirectory, CurrentlySelectedMap.name + ".txt"))) {
                        InstallButton.Text = "Update";
                        InstallButton.BackColor = Color.FromArgb(128, 255, 255);
                        InstallButton.ForeColor = Color.Teal;

                    } else
                    {
                        InstallButton.Text = "Install";
                        InstallButton.BackColor = Color.FromArgb(128, 255, 128);
                        InstallButton.ForeColor = Color.Green;
                    }
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
                    ModDescriptionLabel.Visible = false;
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                DisplayMaps(CustomMapsCache.MapsByPage[CurrentPage]);
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
                DisplayMaps(CustomMapsCache.MapsByPage[CurrentPage]);
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
            if (CustomMapsCache.MapsByPage != null && CustomMapsCache.MapsByPage.Count > 0)
            {
                var allMods = CustomMapsCache.MapsByPage.Values.SelectMany(modList => modList).ToList();
                var filteredMods = allMods.Where(map => map.name.ToLower().Contains(searchText) ||
                                                        map.description.ToLower().Contains(searchText) ||
                                                        map.author.ToLower().Contains(searchText)).ToList();

                // Paginate filtered results
                int pageSize = 26;
                int totalPages = (int)Math.Ceiling(filteredMods.Count / (double)pageSize);

                var filteredMapsByPage = new Dictionary<int, List<CustomMap>>();
                for (int i = 0; i < totalPages; i++)
                {
                    var mapsForPage = filteredMods.Skip(i * pageSize).Take(pageSize).ToList();
                    filteredMapsByPage[i + 1] = mapsForPage;
                }

                // Update the display
                if (filteredMapsByPage.ContainsKey(1))
                {
                    CurrentPage = 1;
                    DisplayMaps(filteredMapsByPage[CurrentPage]);
                    PageNumberLabel.Text = $"Page {CurrentPage}";
                    BackButton.Enabled = CurrentPage > 1;
                    ForwardButton.Enabled = filteredMapsByPage.ContainsKey(CurrentPage + 1);
                }
                else
                {
                    PageNumberLabel.Text = "No mods found";
                    BackButton.Enabled = false;
                    ForwardButton.Enabled = false;
                }
            }
        }

        private async void InstallButton_Click_1(object sender, EventArgs e)
        {
            if (CurrentlySelectedMap != null)
            {
                string mapUrl = CurrentlySelectedMap.downloadLink;
                string rumblePath = Properties.Settings.Default.RumblePath;
                string mapsDirectory = Path.Combine(rumblePath, "UserData", "CustomMultiplayerMaps", "Maps");

                if (!Directory.Exists(mapsDirectory))
                {
                    UserMessage errorMessage = new UserMessage("The CustomMultiplayerMaps/Maps directory does not exist.", true);
                    errorMessage.Show();
                    return;
                }

                string tempDir = Path.Combine(rumblePath, "temp_map_download");
                string tempFilePath = Path.Combine(tempDir, "temp_map.txt");

                try
                {
                    Directory.CreateDirectory(tempDir);

                    // Download the map
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(mapUrl);
                        response.EnsureSuccessStatusCode();

                        // Verify the content is a text file
                        if (response.Content.Headers.ContentType.MediaType != "text/plain")
                        {
                            throw new Exception("The downloaded file is not a valid text file.");
                        }

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                            {
                                await contentStream.CopyToAsync(fileStream);
                            }
                        }
                    }

                    // Move the downloaded file to the maps directory
                    string finalFilePath = Path.Combine(mapsDirectory, $"{CurrentlySelectedMap.name}.txt");
                    bool isUpdating = File.Exists(finalFilePath);

                    File.Move(tempFilePath, finalFilePath, true);

                    // Clean up
                    Directory.Delete(tempDir, true);

                    UserMessage successMessage = new UserMessage($"Successfully {(isUpdating ? "updated" : "installed")} {CurrentlySelectedMap.name}.", true);
                    successMessage.Show();
                    DisplayMaps(CustomMapsCache.MapsByPage[CurrentPage]);
                }
                catch (Exception ex)
                {
                    UserMessage errorMessage = new UserMessage($"An error occurred while installing the map: {ex.Message}", true);
                    errorMessage.Show();
                }
            }
            else
            {
                UserMessage errorMessage = new UserMessage("No map selected for installation.", true);
                errorMessage.Show();
            }
        }
    }

    public class CustomMapsCache()
    {
        public static Dictionary<int, List<CustomMap>> MapsByPage { get; set; } = new Dictionary<int, List<CustomMap>>();
    }

    public class CustomMap()
    {
        public string name = "Unknown";
        public string description = "Unknown";
        public string downloadLink = "";
        public string version = "1.0.0";
        public Image mapImage = null;
        public string author = "Unknown";
    }
}
