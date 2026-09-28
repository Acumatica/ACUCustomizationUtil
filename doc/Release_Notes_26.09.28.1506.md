# Acumatica Customization Utility — New Release
**Version**: 26.09.28.1506
**Date**: September 28, 2026

---

# Overview

This release makes creating customization packages simpler: `acu src make` builds the external library itself, and it can create versioned packages (QA/ISV) for customizations that have no external library.

---

# What's New

- **`src make` builds the external library**:
  - If the solution (`src.msBuildSolutionFile`) exists, `src make` writes the assembly version and metadata to `AssemblyInfo.cs`, compiles the solution (Release, Rebuild), copies the dll to `<pkgSourceDirectory>\Bin` and then creates the package.
  - If there is no solution, the build is skipped and the package is created from the source items.
  - If the build fails, the package is not created.
- **Versioned packages without a solution/dll**:
  - The package version for QA/ISV mode is taken from the customization dll in `<pkgSourceDirectory>\Bin`.
  - If there is no dll, the version is generated from the current date in the same format as the external library build (`yyDDD.HHmm`).
- **Log improvements**:
  - The log shows the package version, where it came from (`Assembly` / `Generated`) and the created package file.

---

# Changes

- **Removed:** `src build` command. Its functionality is part of `src make`. Build parameters are taken from the `src` section of `acu.json`.
- **Changed:** Base mode of `src make` no longer requires a dll or `AssemblyInfo.cs`.
- **Changed:** `src make` no longer reads the package version from the `AssemblyInfo.cs` file, and `src.assemblyInfoPath` is not required for `src make` of customizations without a solution.
- **Fixed:** Build failed when the solution path contained spaces.
- **Docs:** Command reference, configuration reference and user guide are updated. `src` option names are aligned with the utility (`--pkgName`, `--pkgDirectory`, `--instancePath`, `--mode`).

---

# Upgrade Instructions

1. Replace the `acu src build` + `acu src make` calls in your scripts with a single `acu src make`.
2. Make sure `src.msBuildSolutionFile`, `src.msBuildTargetDirectory`, `src.msBuildAssemblyName` and `src.assemblyInfoPath` are set in `acu.json` for projects with an external library.
