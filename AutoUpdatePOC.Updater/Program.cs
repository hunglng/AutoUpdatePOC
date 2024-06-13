using System.Diagnostics;

namespace AutoUpdatePOC.Updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("AutoUpdatePOC.Updater");
            var processes = Process.GetProcessesByName("AutoUpdatePOC.Application");
            foreach (Process proc in processes)
            {
                proc.Kill();
            }
            //wait a sec 
            Thread.Sleep(1000);
            //Update
            // Copy all file from Resources to current
            var directoryName = "Resources";
            if (Directory.Exists(directoryName))
            {
                var files = Directory.GetFiles(directoryName);
                foreach (var file in files)
                {
                    try
                    {
                        Console.WriteLine($"Processing {file}");
                        File.Copy(file, Path.GetFileName(file), true);
                        Console.WriteLine($"Completed {file}");
                    }
                    catch (Exception e)
                    {
                        // POC -> Ignore 
                        // Need Retry Strategy
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }
                }
            }

            ProcessStartInfo processStartInfo = new(".\\AutoUpdatePOC.Application.exe")
            {
                CreateNoWindow = false,
                UseShellExecute = true,
            };

            Process.Start(processStartInfo);
        }
    }
}
