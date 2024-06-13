using AutoUpdatePOC.Library;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Reflection;

namespace AutoUpdatePOC
{
    internal class Program
    {
        static HubConnection connection;
        private static readonly string ResourcesPath = "Resources";

        static async Task Main(string[] args)
        {
            Console.WriteLine("AutoUpdatePOC.Application");
            LogVersion();
            await SignalRConnect();

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    LogVersion();
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    Console.WriteLine("Bye!");
                    break;
                }
            }

        }

        private static void StartUpdater()
        {
            ProcessStartInfo processStartInfo = new(".\\AutoUpdatePOC.Updater.exe")
            {
                CreateNoWindow = false,
                UseShellExecute = true,
            };

            Process.Start(processStartInfo);
        }

        private static async Task SignalRConnect()
        {
            connection = new HubConnectionBuilder()
                            .WithUrl("https://localhost:7238/UpdateHub")
                            .Build();

            await connection.StartAsync();
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"{user}: {message}");

            });

            connection.On<string>("CheckUpdate", (version) =>
            {
                Console.WriteLine($"CheckUpdate: {version}");

            });

            connection.On("UpdateAndReload", () =>
            {
                StartUpdater();
            });

            connection.On<Dictionary<string, byte[]?>>("PushFiles", (dict) =>
            {
                Console.WriteLine($"PushFiles: {dict.Count} item(s)");
                var dir = Directory.CreateDirectory(ResourcesPath);

                foreach (var item in dict)
                {
                    if (item.Value != null)
                    {
                        var filePath = Path.Combine(dir.FullName, item.Key);
                        File.WriteAllBytes(filePath, item.Value); 
                    }
                }
                Console.WriteLine($"PushFiles: Completed - Start to Reload");
                StartUpdater();
            });
        }

        private static void LogVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.WriteLine($"App Version: {fileVersionInfo.ProductVersion}");

            var libVersion = LibVer.CurrentVersion();
            Console.WriteLine($"Library Version: {libVersion}");
        }
    }
}
