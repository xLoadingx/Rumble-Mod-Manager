using System.Text.Json;

namespace Rumble_Mod_Manager
{
    public static class ProfileSystem
    {
        public static string ProfilesDirectory => Path.Combine(Properties.Settings.Default.RumblePath, "Profiles");
        public static string ModCacheDirectory => Path.Combine(Properties.Settings.Default.RumblePath, "mod-cache");
        public static string ModsDirectory => Path.Combine(Properties.Settings.Default.RumblePath, "Mods");

        public static ModProfile CurrentProfile { get; private set; }

        public static List<ModProfile> AllProfiles
        {
            get
            {
                Directory.CreateDirectory(ProfilesDirectory);
                var profiles = new List<ModProfile>();

                foreach (var file in Directory.GetFiles(ProfilesDirectory, "*.json"))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var profile = JsonSerializer.Deserialize<ModProfile>(json);
                        if (profile != null) profiles.Add(profile);
                    }
                    catch (Exception e)
                    {
                        UserMessage error = new UserMessage($"Error loading profile: {e.StackTrace}", true, showCopyDialog: true);
                        error.ShowDialog();
                    }
                }

                return profiles;
            }
        }

        public static void CreateNewProfile(string profileName)
        {
            if (string.IsNullOrEmpty(profileName) || !IsValidFileName(profileName))
            {
                UserMessage error = new UserMessage("Name is not a valid profile name.");
                error.ShowDialog();
                return;
            }

            var newProfile = new ModProfile
            {
                Name = profileName,
                EnabledModIds = new List<string>(),
                DisabledModIds = new List<string>()
            };

            ProfileSystem.SaveProfile(newProfile);
            ProfileSystem.ApplyProfile(newProfile);
        }


        public static void SaveProfile(ModProfile profile)
        {
            Directory.CreateDirectory(ProfilesDirectory);
            string path = Path.Combine(ProfilesDirectory, profile.Name + ".json");
            string json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public static void ApplyProfile(ModProfile profile)
        {
            CurrentProfile = profile;

            Directory.CreateDirectory(ModsDirectory);
            Directory.CreateDirectory(ModCacheDirectory);

            foreach (string modId in CurrentProfile.EnabledModIds)
            {
                string sourcePath = Path.Combine(ModsDirectory, modId + ".dll");
                string targetPath = Path.Combine(ModCacheDirectory, modId + ".dll");

                if (File.Exists(sourcePath))
                    File.Move(sourcePath, targetPath);
            }

            foreach (string modId in profile.EnabledModIds)
            {
                string source = Path.Combine(ModCacheDirectory, modId + ".dll");
                string target = Path.Combine(ModsDirectory, modId + ".dll");

                if (File.Exists(source))
                    File.Move(source, target);
            }

            Properties.Settings.Default.LastLoadedProfile = profile.Name;
            Properties.Settings.Default.Save();
        }

        public static void DeleteMod(string modId, bool fromAllProfiles)
        {
            if (fromAllProfiles)
            {
                foreach (var profile in AllProfiles)
                {
                    profile.EnabledModIds.Remove(modId);
                    profile.DisabledModIds.Remove(modId);
                    SaveProfile(profile);
                }

                string[] paths =
                {
                    Path.Combine(ModsDirectory, modId + ".dll"),
                    Path.Combine(ModCacheDirectory, modId + ".dll")
                };

                foreach (string path in paths)
                    if (File.Exists(path)) File.Delete(path);
            }
            else if (CurrentProfile != null)
            {
                CurrentProfile.EnabledModIds.Remove(modId);
                CurrentProfile.DisabledModIds.Remove(modId);
                SaveProfile(CurrentProfile);

                string source = Path.Combine(ModsDirectory, modId + ".dll");
                string target = Path.Combine(ModCacheDirectory, modId + ".dll");

                if (File.Exists(source) && !File.Exists(target))
                    File.Move(source, target);
            }
        }

        private static bool IsValidFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return !fileName.Any(ch => invalidChars.Contains(ch));
        }
    }

    public class ModProfile
    {
        public string Name { get; set; } = "Unnamed";
        public List<string> EnabledModIds { get; set; } = new();
        public List<string> DisabledModIds { get; set; } = new();
        public List<string> FavoritedModIds { get; set; } = new();
    }

    public class LegacyProfile
    {
        public string ProfileName { get; set; }
        public List<LegacyModEntry> enabledMods { get; set; }
        public List<LegacyModEntry> disabledMods { get; set; }
    }

    public class LegacyModEntry
    {
        public string ModName { get; set; }
        public bool Outdated { get; set; }
        public bool Favorited { get; set; }
        public string DownloadLink { get; set; }
    }
}
