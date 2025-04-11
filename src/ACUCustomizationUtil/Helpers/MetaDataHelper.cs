using ACUCustomizationUtils.Configuration.ACU;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ACUCustomizationUtils.Helpers
{
    public static class MetaDataHelper
    {
        public const string MetadataFileName = "metadata.json";
        public class MetaDataInfo
        {
            public string? Branch { get; set; }
            public string? MachineName { get; set; }
            public string? User { get; set; }
        }

        public static void SetBuildMetadata(IAcuConfiguration config)
        {
            string projectRoorPath = GetProjectRootPath(config);
            string filePath = GetAccemblyInfoFullPath(projectRoorPath);
            try
            {
                if (!File.Exists(filePath))
                    CreateAssemblyInfoFile(filePath);

                string content = File.ReadAllText(filePath);
                string descriptionPattern = @"(\[assembly:\s*AssemblyTitle\("")(.*?)(?=""\)\])";
                string updatedContent = Regex.Replace(content, descriptionPattern, $"$1{CreateMetadataContent()}");

                File.WriteAllText(filePath, updatedContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing AssemblyInfo.cs: {ex.Message}");
            }
        }

        public static void SetAdditionalZipInfo(string zipPath)
        {            
            using (FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.ReadWrite))
            using (ZipFile zipFile = new ZipFile(fs))
            {
                zipFile.BeginUpdate();
                zipFile.SetComment(CreateMetadataContent());
                zipFile.CommitUpdate();
            }
        }

        public static string CreateMetadataContent()
        {
            var info = GetMetaDataInfo();
            return $"Branch: {info.Branch}; " +
                   $"MachineName: {info.MachineName}; " +
                   $"User: {info.User}";
        }

        public static string CreateMetadataJson()
        {
            return JsonSerializer.Serialize(GetMetaDataInfo());            
        }

        private static MetaDataInfo GetMetaDataInfo()
        {
            return new MetaDataInfo()
            {
                Branch = GetCurrentGitBranch(),
                MachineName = Environment.MachineName,
                User = Environment.UserName
            };
        }

        private static void CreateAssemblyInfoFile(string filePath)
        {
            string assemblyInfoContent = @"
        using System.Reflection;
        using System.Runtime.CompilerServices;
        using System.Runtime.InteropServices;

        [assembly: AssemblyTitle("""")]
        [assembly: AssemblyDescription("""")]
        [assembly: AssemblyConfiguration("""")]
        [assembly: AssemblyCompany("""")]
        [assembly: AssemblyProduct("""")]
        [assembly: AssemblyCopyright("""")]
        [assembly: AssemblyTrademark("""")]
        [assembly: AssemblyCulture("""")]
        ";

            string? directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath != null)
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.WriteAllText(filePath, assemblyInfoContent);
        }

        private static string GetAccemblyInfoFullPath(string projectRoorPath)
        {
            const string ProperiesFolder = "Properties";
            const string AssemblyInfoFileName = "AssemblyInfo.cs";
            return Path.Combine(projectRoorPath, ProperiesFolder, AssemblyInfoFileName);
        }

        private static string GetProjectRootPath(IAcuConfiguration config)
        {
            return Directory.GetParent(
                Directory.GetParent(config.Src.MsBuildTargetDirectory ?? string.Empty)
                ?.FullName ?? string.Empty)
                ?.FullName ?? string.Empty;
        }

        private static string GetCurrentGitBranch()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "rev-parse --abbrev-ref HEAD",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();
                return output;
            }
        }
    }
}
