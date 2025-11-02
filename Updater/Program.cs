using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Ionic.Zip;

namespace Updater
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Invalid arguments passed to the updater.");
                return;
            }

            string zipFilePath = args[0];
            string targetDirectory = args[1];
            string mainAppPath = args[2];

            try
            {
                Console.WriteLine("Updating the application. Please wait...");

                string processName = Path.GetFileNameWithoutExtension(mainAppPath);
                CloseApplication(processName);

                string tempExtractPath = Path.Combine(Path.GetTempPath(), "UpdaterTemp");
                Directory.CreateDirectory(tempExtractPath);

                using (ZipFile zip = ZipFile.Read(zipFilePath))
                {
                    foreach (ZipEntry entry in zip)
                    {
                        entry.Extract(tempExtractPath, ExtractExistingFileAction.OverwriteSilently);
                    }
                }

                foreach (var file in Directory.GetFiles(tempExtractPath))
                {
                    string destFile = Path.Combine(targetDirectory, Path.GetFileName(file));

                    if (File.Exists(destFile))
                    {
                        WaitForUnlock(destFile);
                        File.Delete(destFile);
                    }

                    try
                    {
                        if (File.Exists(destFile))
                            File.Delete(destFile);

                        File.Move(file, destFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping file '{file}': {ex.Message}");
                    }
                }

                Directory.Delete(tempExtractPath, true);
                File.Delete(zipFilePath);

                Console.WriteLine("Update completed successfully!");

                if (File.Exists(mainAppPath))
                {
                    Console.WriteLine("Relaunching application...");
                    Process.Start(mainAppPath);
                }
                else
                {
                    Console.WriteLine("Main application not found. Update completed, but app not restarted.");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update failed: {ex.Message}");
                Console.ReadKey();
            }
        }

        static void CloseApplication(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch (Exception ex) {
                    Console.WriteLine($"Failed to close application: {ex.Message}");
                }
            }
        }

        static bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            } catch (Exception ex)
            {
                return true;
            }
        }

        static void WaitForUnlock(string filePath)
        {
            for (int i = 0; i < 10; i++)
            {
                if (!IsFileLocked(filePath)) return;
                Thread.Sleep(300);
            }
        }
    }
}
