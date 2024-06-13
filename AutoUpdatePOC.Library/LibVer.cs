using System.Diagnostics;
using System.Reflection;

namespace AutoUpdatePOC.Library
{
    public static class LibVer
    {
        public static string? CurrentVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}
