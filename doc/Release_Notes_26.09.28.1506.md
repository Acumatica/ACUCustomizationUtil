# Acumatica Customization Utility — New Release
**Version**: 26.09.28.1506
**Date**: September 28, 2026

---

# Overview

In this release, we tackle two separate concerns:
1. Adding support for customizations without solution and assembly. As previously all version generation logic was going through them, this required detaching the version generation into a self-contained process.
2. Tying assembly and package together by the version. This helps us tackle a case when users rename customizations on production instances. When we have to troubleshoot issues related to certain product version, this creates a confusion and another hurdle to overcome. Now, you are able to trace it back through the separate `PackageVersion` metadata attribute of the assembly.

To make both things work, `acu src build` was incorporated into the `acu src make` command. This way, we can guarantee creating a new version for each package, not relying on a user to execute both commands continuously. The added benefit of this merger is also simplification of the process from 2 steps to just 1.

---

# What's New
## Added 
- Support for versioned packages without a solution/assembly.
## Removed
- `acu src build` command. It is now a part of `acu src make` command. Build parameters are taken from the `src` section of `acu.json`.  
## Changed
- `acu src make` now build assemblies (when applied).
- Minor rewording of statuses during `acu src make` command, for consistency.  
- Package versioning for customizations with assemblies. The package version is now tied to the assembly's `AssemblyVersion` metadata property instead of `AssemblyInfo.cs` file in the project. This prevents us from picking a potentially stale version string, in case the file has been updated by 3rd party.  
- Docs were updated to better reflect the current product and its configuration.  

# Upgrade Instructions

1. Replace the `acu src build` + `acu src make` calls in your scripts with a single `acu src make`.