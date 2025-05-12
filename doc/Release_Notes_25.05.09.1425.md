
# Acumatica Customization Utility — New Release  
**Version**: 25.05.09.1425

---

# Overview

This release introduces several important improvements and new features to the Acumatica Customization Utility, enhancing package management, metadata handling, and build processes.

---

# What's New

- **Added REST client** for managing customization packages.
- **Extended `acu.json` configuration** with new parameters:
  - `pkg.tenant`
  - `pkg.branch`
  - `src.msBuildPath`
  - `src.assemblyInfoPath`
- **Customization package improvements**:
  - Embed metadata into the customization `*.dll`.
  - Include `metadata.json` in the customization `.zip` package.
- **Assembly metadata handling**:
  - Assembly version and metadata are now automatically written to `AssemblyInfo.cs`.
- **Build command enhancement**:
  - Added `pkgSuffix` option to the `src` make command.

---

# Upgrade Instructions

1. Update your `acu.json` file to include the new parameters as needed.
2. Adjust your project files by removing old `BeforeBuild` logic (see "Notes for Users").
3. Ensure that new metadata is correctly incorporated both into assemblies and customization packages.

---

# Notes for Users

### 1. Parameter `--url` / `pkg.url` Usage

In the CLI `--url` parameter must be specified in the following format:

```
http://localhost/<instance name>
```

In the configuration file (`acu.json`), the `pkg.url` parameter must be specified in the following format:

```
http://localhost/<instance name>
```

---

### 2. Project File Update

Remove the following `BeforeBuild` target from your project `.csproj` file to avoid conflicts with the new metadata management:

```xml
<Target Name="BeforeBuild">
  <ItemGroup>
    <AssemblyAttributes Include="AssemblyVersion">
      <_Parameter1>$(Version)</_Parameter1>
    </AssemblyAttributes>
  </ItemGroup>
  <MakeDir Directories="$(IntermediateOutputPath)" />
  <WriteCodeFragment Language="C#" OutputFile="$(IntermediateOutputPath)Version.cs" AssemblyAttributes="@(AssemblyAttributes)" />
  <ItemGroup>
    <Compile Include="$(IntermediateOutputPath)Version.cs" />
  </ItemGroup>
</Target>
```

---

### 3. Assembly Metadata

Metadata is now automatically embedded into the assembly using `AssemblyMetadata` attributes, for example:

```csharp
[assembly: AssemblyMetadata("GitBranch", "develop")]
[assembly: AssemblyMetadata("GitHash", "6e1ba91843de136967c1f7720314cd31d9272f36")]
[assembly: AssemblyMetadata("BuildUser", "********")]
[assembly: AssemblyMetadata("BuildMachine", "*********")]
```

---

### 4. Manifest Metadata (`metadata.json`)

The `metadata.json` file included in the customization `.zip` now contains detailed build information:

```json
{
  "GitBranch": "develop",
  "GitHash": "9d12753f2ae922d938ac17c0a7a3511eb1c2e742",
  "BuildUser": "******",
  "BuildMachine": "*******",
  "AssemblyVersion": "25.100.25117.1504",
  "MakeMode": "QA",
  "PackageName": "ACUCustomization_PM-001_[25.100.0054][25117.1504].zip"
}
```

---

# Additional Information

For more details on migration or any issues during upgrade, please refer to internal documentation or contact the development team.

---



