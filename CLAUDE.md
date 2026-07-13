# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

ACU (Acumatica Customization Util) is a single Windows CLI tool (`acu.exe`) for automating ERP Acumatica customization workflows: installing ERP/site instances, pulling customization source from a running instance, building extension libraries via MSBuild, packaging customizations into deployable `.zip` files, and uploading/publishing them through Acumatica's ServiceGate SOAP API.

Single-project repo: [src/ACUCustomizationUtil/ACUCustomizationUtil.csproj](src/ACUCustomizationUtil/ACUCustomizationUtil.csproj), targeting **net9.0**, output assembly name **`acu`**. There is no test project.

## Build, run, publish

All commands assume you are at the repo root unless noted. PowerShell on Windows.

```powershell
# Build (debug)
dotnet build ACUCustomizationUtil.sln

# Build (release). Note: Release also triggers Update-Version.ps1, which
# rewrites version strings across doc/*.md and writes doc/VERSION.
dotnet build ACUCustomizationUtil.sln -c Release

# Run the CLI directly without publishing
dotnet run --project src/ACUCustomizationUtil -- <command> <subcommand> [options]
# e.g. dotnet run --project src/ACUCustomizationUtil -- src make --mode QA

# Produce a self-contained redistributable (see doc/ACUPackageGuide.md for
# the IDE-driven flow this repo expects).
dotnet publish src/ACUCustomizationUtil/ACUCustomizationUtil.csproj -c Release -r win-x86 --self-contained true
```

Top-level CLI surface: `acu {erp|site|src|pkg} <subcommand>`. `acu -?` and `acu <command> -?` show the auto-generated help.

## Version bumping

The assembly version is the single source of truth and lives **inline in code** at [src/ACUCustomizationUtil/Program.cs](src/ACUCustomizationUtil/Program.cs#L15) as `[assembly: AssemblyVersion("YY.MM.DD.HHMM")]`. Format is strict — 4 components, validated by regex in [Update-Version.ps1](Update-Version.ps1).

To bump: edit that one line, then build in Release. The `UpdateDocsVersion` MSBuild target runs `Update-Version.ps1`, which rewrites every `YY.MM.DD.HHMM`-shaped string across `doc/*.md` (excluding `VERSION` and `Release_Notes_*.md`) and writes `doc/VERSION`. Do not edit version numbers in docs by hand — they will be overwritten on the next Release build.

## Pre-build side effect (important)

[ACUCustomizationUtil.csproj:50-52](src/ACUCustomizationUtil/ACUCustomizationUtil.csproj#L50-L52) defines a `PreBuild` target that `xcopy`s `acu.json` into `..\..\proj\` (the test customization project at the repo's `proj/`). Editing `src/ACUCustomizationUtil/acu.json` and rebuilding will silently overwrite `proj/acu.json`. The `proj/` directory is the sample customization project used to exercise the tool end-to-end (see [doc/ACUTestProjectGuide.md](doc/ACUTestProjectGuide.md)).

## Architecture

The codebase follows a strict 4-way symmetric split: every top-level CLI verb (`erp`, `site`, `src`, `pkg`) has its own Builder + Service + Validator + Configuration section. Add new functionality by adding to all four in parallel — there is no shared command-handling code path beyond the bases.

```
Program.cs
  └─> SerilogBuilder.Build()            file sink: acu-log<date>.txt (rolling daily)
  └─> HostBuilder.Build()                MS.Extensions.Hosting + Serilog + DI registrations
  └─> RootCommandBuilder.BuildCommand()  wires the 4 sub-CommandBuilders into System.CommandLine
        ├─ ErpCommandBuilder    ─> IErpService     ─> Validators/Erp/*
        ├─ SiteCommandBuilder   ─> ISiteService    ─> Validators/Site/*
        ├─ SrcCommandBuilder    ─> ISrcService     ─> Validators/Src/*
        └─ PackageCommandBuilder─> IPackageService ─> Validators/Package/*
```

- **Command wiring**: [Builders/Commands/](src/ACUCustomizationUtil/Builders/Commands/). Each `XxxCommandBuilder` extends `CommandBuilderBase`, declares `Option<T>` objects, and binds them to a service method via a `XxxConfigurationBinder` (in `Builders/Commands/Binders/`) that produces an `IAcuConfiguration` from the option values plus the two global `--config` / `--user-config` file options.
- **DI registration**: [Builders/DI/HostBuilder.cs](src/ACUCustomizationUtil/Builders/DI/HostBuilder.cs) — services are `Transient`, command builders are `Singleton`. No auto-discovery; add new services here by hand.
- **Services**: [Services/](src/ACUCustomizationUtil/Services/) hold business logic and delegate heavy lifting to helpers. They always: log → print config → validate via `XxxValidator.ValidateForY(config)` → execute, all wrapped in a `Spectre.Console` `AnsiConsole.Status()` spinner.
- **Helpers** ([Helpers/](src/ACUCustomizationUtil/Helpers/)) are the workhorses:
  - `PackageHelper` — zips customization source into the final `.zip` with a synthesized `project.xml` manifest. Make mode (`Base`/`QA`/`ISV` from [Common/Messages.cs](src/ACUCustomizationUtil/Common/Messages.cs)) controls filename pattern.
  - `MSBuildHelper` — shells out to MSBuild via `ProcessHelper`, then copies the built `.dll` into the package's `Bin/` directory.
  - `DatabaseHelper` — Dapper + `Microsoft.Data.SqlClient` against the Acumatica DB for `src get` (pulls customization entities directly from the DB).
  - `CstEntityHelper` — serializes pulled DB entities to disk as customization source.
  - `SoapClient` / `RestClient` / `WebClient` — talk to the running Acumatica instance for `pkg` operations. The SOAP reference is generated under [Connected Services/AcuSOAP/Reference.cs](src/ACUCustomizationUtil/Connected%20Services/AcuSOAP/Reference.cs) from ServiceGate.asmx; regenerating it from a new Acumatica build requires Visual Studio's Connected Service tooling.

## Configuration model (the part that's not obvious)

There are three configuration sources, merged by **last-write-wins** priority:

1. `acu.json` (lowest, default name, `--config <path>` to override)
2. `acu.json.user` (middle, `--user-config <path>` to override)
3. CLI `--option` values for the current command (highest)

All three are deserialized into the same `IAcuConfiguration` shape and merged by [Extensions/ObjectExtentions.cs](src/ACUCustomizationUtil/Extensions/ObjectExtentions.cs) `CopyValues` — a reflection-based recursive copy that only overwrites non-null source properties. [Helpers/ConfigurationHelper.cs](src/ACUCustomizationUtil/Helpers/ConfigurationHelper.cs) `GetConfiguration` orchestrates the merge and is called from every Binder.

`IAcuConfiguration` has four sub-configs (`Erp`, `Site`, `Pkg`, `Src`). Each interface carries a `[JsonConverter]` attribute pointing to a hand-written `JsonConverter` in [JSON/](src/ACUCustomizationUtil/JSON/) because `System.Text.Json` cannot deserialize interfaces by default. **If you add a property to a sub-config interface, you must update the corresponding `XxxConfigurationConverter.Read()` switch — there is no fallback / auto-binding.** Each sub-config also implements `SetDefaultValues(IAcuConfiguration)` invoked from `OnDeserialized` to derive things like the SQL connection string from `sqlServerName` + `dbName`.

Path-valued configuration fields support Windows env var expansion (e.g. `%ACUBASEDIR%\\erp`). The known path fields are listed in [doc/ACUUserGuide.md](doc/ACUUserGuide.md) under "Configuration".

## Validation

Per-action FluentValidation validators live in [Validators/](src/ACUCustomizationUtil/Validators/). The pattern is one orchestrator per command group (e.g. `SrcValidator.ValidateForMake`, `ValidateForBuild`, `ValidateForSrc`) that fans out to multiple per-sub-config validators. Failures throw `ArgumentException` via the `OptionsValidatorBase.Validate` helper and are caught + logged at the service level (services do not propagate exceptions — they log and return).

## Code style

- `.editorconfig` is authoritative. CRLF line endings, 4-space indent, `dotnet_sort_system_directives_first = true` with `dotnet_separate_import_directive_groups = true` (note the blank line between `using System.*` and other usings in existing files — match this).
- `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>` are both on. Code uses `!` extensively on configuration access because validators guarantee non-null at the point of use.
- `<EnforceCodeStyleInBuild>True</EnforceCodeStyleInBuild>` is set, so style violations become build warnings.
