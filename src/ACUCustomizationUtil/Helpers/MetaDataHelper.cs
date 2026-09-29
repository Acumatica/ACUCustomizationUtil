using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Helpers.CommonTypes;

namespace ACUCustomizationUtils.Helpers
{
    public class MetaDataHelper(IAcuConfiguration config)
    {
        private static class Keys
        {
            public const string AssemblyFileVersion = "AssemblyFileVersion";
            public const string AssemblyVersion = "AssemblyVersion";
            public const string AssemblyMetadata = "AssemblyMetadata";
            public const string PackageVersion = "PackageVersion";
        }

        private static readonly JsonSerializerOptions json_write_options = new()
        {
            WriteIndented = true,
        };
        private readonly IAcuConfiguration _config = config;
        private readonly string? _packageName;

        public MetaDataHelper(IAcuConfiguration config, string packageName)
            : this(config)
        {
            _packageName = packageName;
        }

        /// <summary>
        /// Writes AssemblyVersion and AssemblyFileVersion to AssemblyInfo.cs. For a QA/ISV make the date
        /// component is shared with <paramref name="packageVersion"/>, which is also written as
        /// [assembly: AssemblyMetadata("PackageVersion", "...")]; for other makes that attribute is removed.
        /// </summary>
        public void SetBuildVersion(PackageVersion? packageVersion)
        {
            try
            {
                SetAssemblyVersion(packageVersion);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing version to file AssemblyInfo.cs: {ex}");
            }
        }

        public void SetBuildMetadata()
        {
            try
            {
                SetAssemblyMetadata();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing metadata to file AssemblyInfo.cs: {ex}");
            }
        }

        private void SetAssemblyVersion(PackageVersion? packageVersion)
        {
            string assemblyInfoPath = GetAccemblyInfoFullPath();
            string version = GetAssemblyVersion(packageVersion);

            AddOrUpdateAssemblyMetadataAttribute(assemblyInfoPath, Keys.AssemblyVersion, null, version);
            AddOrUpdateAssemblyMetadataAttribute(assemblyInfoPath, Keys.AssemblyFileVersion, null, version);

            if (packageVersion != null)
            {
                AddOrUpdateAssemblyMetadataAttribute(assemblyInfoPath, Keys.AssemblyMetadata, Keys.PackageVersion, packageVersion.Value);
            }
            else
            {
                RemoveAssemblyMetadataAttribute(assemblyInfoPath, Keys.AssemblyMetadata, Keys.PackageVersion);
            }
        }

        private void SetAssemblyMetadata()
        {
            string assemblyInfoPath = GetAccemblyInfoFullPath();

            // Define metadata values
            Dictionary<string, string> newValues = new Dictionary<string, string>
            {
                ["GitBranch"] = GetCurrentGitBranch(),
                ["GitHash"] = GetCurrentGitHash(),
                ["BuildUser"] = Environment.UserName,
                ["BuildMachine"] = Environment.MachineName,
            };

            foreach (KeyValuePair<string, string> attr in newValues)
            {
                AddOrUpdateAssemblyMetadataAttribute(assemblyInfoPath, Keys.AssemblyMetadata, attr.Key, attr.Value);
            }
        }

        public string CreateMetadataJson()
        {
            var data = new
            {
                GitBranch = GetCurrentGitBranch(),
                GitHash = GetCurrentGitHash(),
                BuildUser = Environment.UserName,
                BuildMachine = Environment.MachineName,
                AssemblyVersion = GetAssemblyInfoAttributeValue(
                    GetAccemblyInfoFullPath(),
                    "AssemblyVersion"
                ),
                MakeMode = _config.Src.MakeMode ?? Messages.MakeModeBase,
                PackageName = _packageName,
            };

            // Serialize the object to JSON
            return JsonSerializer.Serialize(data, json_write_options);
        }

        private static void AddOrUpdateAssemblyMetadataAttribute(
            string filePath,
            string attributeName,
            string? key,
            string value
        )
        {
            string content = File.ReadAllText(filePath);
            string attributePattern;
            string replacement;

            if (!string.IsNullOrEmpty(key))
            {
                // Example: [assembly: AssemblyMetadata("GitBranch", "main")]
                attributePattern =
                    $@"\[assembly:\s*{Regex.Escape(attributeName)}\(""{Regex.Escape(key)}"",\s*"".*?""\)\]";
                replacement = $@"[assembly: {attributeName}(""{key}"", ""{value}"")]";
            }
            else
            {
                // Example: [assembly: AssemblyTitle("MyApp")]
                attributePattern =
                    $@"\[assembly:\s*{Regex.Escape(attributeName)}\(\s*""[^""]*""\s*\)\]";
                replacement = $@"[assembly: {attributeName}(""{value}"")]";
            }

            if (Regex.IsMatch(content, attributePattern))
            {
                // Update the existing attribute
                content = Regex.Replace(content, attributePattern, replacement);
            }
            else
            {
                // Add a new attribute after the last using/attribute
                List<string> lines = content.Split([Environment.NewLine], StringSplitOptions.None).ToList();
                int insertIndex = lines.FindLastIndex(line =>
                    line.TrimStart().StartsWith("[assembly:", StringComparison.OrdinalIgnoreCase)
                    || line.TrimStart().StartsWith("using ", StringComparison.OrdinalIgnoreCase)
                );

                if (insertIndex == -1)
                    insertIndex = lines.Count - 1;

                lines.Insert(insertIndex + 1, replacement);
                content = string.Join(Environment.NewLine, lines);
            }

            File.WriteAllText(filePath, content);
        }

        private static void RemoveAssemblyMetadataAttribute(string filePath, string attributeName, string key)
        {
            string content = File.ReadAllText(filePath);

            // Example: [assembly: AssemblyMetadata("PackageVersion", "2026.09.28.1432")]
            string attributePattern =
                $@"^[ \t]*\[assembly:\s*{Regex.Escape(attributeName)}\(""{Regex.Escape(key)}"",\s*"".*?""\)\][ \t]*(\r?\n)?";
            string updated = Regex.Replace(content, attributePattern, string.Empty, RegexOptions.Multiline);

            if (updated != content)
                File.WriteAllText(filePath, updated);
        }

        private string GetAccemblyInfoFullPath()
        {
            if (_config.Src.AssemblyInfoPath != null && File.Exists(_config.Src.AssemblyInfoPath))
            {
                return _config.Src.AssemblyInfoPath;
            }

            throw new ArgumentNullException(
                nameof(_config.Src.AssemblyInfoPath),
                "Assembly name is null"
            );
        }

        private static string GetCurrentGitBranch()
        {
            ProcessStartInfo psi = new()
            {
                FileName = "git",
                Arguments = "rev-parse --abbrev-ref HEAD",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process process = new() { StartInfo = psi };
            process.Start();
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            return output;
        }

        private static string GetCurrentGitHash()
        {
            ProcessStartInfo psi = new()
            {
                FileName = "git",
                Arguments = "rev-parse HEAD",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process process = new() { StartInfo = psi };
            process.Start();
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            return output;
        }

        public string GetAssemblyVersion(PackageVersion? packageVersion)
        {
            string majorPart = $"{_config.Erp.ErpVersion?[..6]}";
            string dateVersion = packageVersion?.DateVersion ?? PackageVersion.GetDateVersion(DateTime.Now);

            return $"{majorPart}.{dateVersion}";
        }

        public string? GetAssemblyInfoAttributeValue(
            string filePath,
            string attributeName,
            string? key = null
        )
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            string fileContent = File.ReadAllText(filePath);

            // Pattern to match the attribute: [assembly: AttributeName("value")] or [assembly: AttributeName(key="value")]
            string pattern = $@"\[assembly:\s*{Regex.Escape(attributeName)}\s*\((?:[^)]*)\)\]";
            Match match = Regex.Match(fileContent, pattern);

            if (!match.Success)
            {
                return null;
            }

            string attributeContent = match.Value;

            if (string.IsNullOrEmpty(key))
            {
                // Find the first quoted value
                Match valueMatch = Regex.Match(attributeContent, @"""([^""]*)""");
                return valueMatch.Success ? valueMatch.Groups[1].Value : null;
            }

            // Find the value for the specified key
            string keyPattern = $@"{Regex.Escape(key)}\s*=\s*""([^""]*)""";
            Match keyMatch = Regex.Match(attributeContent, keyPattern);
            return keyMatch.Success ? keyMatch.Groups[1].Value : null;
        }
    }
}
