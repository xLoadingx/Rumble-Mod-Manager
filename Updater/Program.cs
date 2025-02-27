﻿using System;
using System.Diagnostics;
using System.IO;
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
                        File.Delete(destFile);
                    }

                    File.Move(file, destFile);
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

                File.Delete(zipFilePath);
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
    }
}
