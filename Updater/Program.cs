using System;
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

                using (ZipFile zip = ZipFile.Read(zipFilePath))
                {
                    foreach (ZipEntry entry in zip)
                    {
                        entry.Extract(targetDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }
                }

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
    }
}
