using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.ACU;

namespace ACUCustomizationUtils.Helpers
{
    public class MetaDataHelper(IAcuConfiguration config)
    {
        public const string MetadataFileName = "manifest.json";
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

        public void SetBuildVersion()
        {
            try
            {
                SetAssemblyVersion();
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

        private void SetAssemblyVersion()
        {
            string assemblyInfoPath = GetAccemblyInfoFullPath();
            string version = GetAssemblyVersion();

            if (version != null)
            {
                AddOrUpdateAssemblyMetadataAttribute(
                    assemblyInfoPath,
                    "AssemblyVersion",
                    null,
                    version
                );
                AddOrUpdateAssemblyMetadataAttribute(
                    assemblyInfoPath,
                    "AssemblyFileVersion",
                    null,
                    version
                );
            }
            else
            {
                throw new ArgumentNullException(nameof(version), "Version is null");
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
                AddOrUpdateAssemblyMetadataAttribute(
                    assemblyInfoPath,
                    "AssemblyMetadata",
                    attr.Key,
                    attr.Value
                );
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

        public string GetAssemblyVersion()
        {
            string datePart = GetDateVersion();
            string isvPart = GetISVVersion();

            string majorPart = $"{_config.Erp.ErpVersion?[..6]}";
            string makeMode = _config.Src.MakeMode ?? Messages.MakeModeBase;
            string minorPart = makeMode switch
            {
                Messages.MakeModeBase => datePart,
                Messages.MakeModeQA => datePart,
                Messages.MakeModeISV => isvPart,
                _ => datePart,
            };

            string version = $"{majorPart}.{minorPart}";
            return version;
        }

        private static string GetDateVersion()
        {
            DateTime firstDate = new DateTime(DateTime.Now.Year, 1, 1);
            string days = Math.Truncate((DateTime.Now - firstDate).TotalDays).ToString("000");
            return $"{DateTime.Now:yy}{days}.{DateTime.Now:HHmm}";
        }

        private static string GetISVVersion() => DateTime.Now.ToString("yyyy.MM.dd");

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
