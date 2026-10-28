# Acumatica Customization Util (ACU)

## Configuration Reference (version 26.09.30.1529)
```
{
  "erp": {                                                                                                                    Parameters for erp command
    "erpVersion": "23.105.0016",                                                                                              ERP Version
    "installationFileName": "AcumaticaERPInstall.msi",                                                                        Installation file name
    "destinationDirectory": "C:\\Acumatica\\erp"                                                                              Destination directory
    "url": null,                                                                                                              URL for download ERP installer
  },

  "site": {                                                                                                                   Parameters for site command
    "instanceName": "AcuTestUtil",                                                                                            Instance name
    "instancePath": "C:\\Acumatica\\instance\\26.101.0225\\Site",                                                             Full physical path of instance
    "dbProvider": null,                                                                                                       Database provider: "mssql" (default) or "mysql"
    "sqlServerName": "localhost",                                                                                             Name of database server with ACU instance database
    "dbPort": null,                                                                                                           Database server port (mysql only, default 3306)
    "dbName": "26.101.0225DB",                                                                                                Instance database name
    "dbUser": null,                                                                                                           Database user name (required for mysql;
                                                                                                                              for mssql enables SQL auth instead of Windows auth)
    "dbPassword": null,                                                                                                       Database user password
    "acumaticaAdminName": "admin",                                                                                            Name of admin user
    "acumaticaAdminPassword": "123",                                                                                          Password which will be set for the  admin user
                                                                                                                              after instance was installed
    "acumaticaToolPath": null,                                                                                                Path to acumatica tool "ac.exe"
    "dbConnectionString": null,                                                                                               Database connection string; when null it is derived
                                                                                                                              from dbProvider/sqlServerName/dbPort/dbName/dbUser/
                                                                                                                              dbPassword; an explicit value always wins
    "iisAppPool": null,                                                                                                       IIS app pool name
    "iisWebSite": null                                                                                                        IIS web site name
  },

  "pkg": {                                                                                                                    Parameters for package command
    "url": "http://localhost/26.101.0225/",                                                                                   URL of service gate
    "login": "admin",                                                                                                         Service gate login
    "password": "123",                                                                                                        Service gate passworg
    "pkgName": "ACUCustomization",                                                                                            Package name
    "pkgDirectory": "C:\\Acumatica\\projects\\ACUCustomization\\pkg"                                                          Package directory
    "tenant": null,                                                                                                           Tenant to login
    "branch": null											                                                                  Branch to login	
  },

  "src": {                                                                                                                    Parameters for src commands
    "pkgSourceDirectory": "C:\\Acumatica\\projects\\ACUCustomization\\cst",                                                   Directory with package source code
    "pkgLevel": "0",                                                                                                          Package level
    "msBuildPath": "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe",         MSBuild full path (optional, auto-detected if not set, see "MSBuild resolution" below)
    "assemblyInfoPath": "src\\ACUCustomization\\Properties\\AssemblyInfo.cs",						                          External code assembly info file full name
    "msBuildSolutionFile": "C:\\Acumatica\\projects\\ACUCustomization\\ACUCustomization.sln",                                 External library solution file full path (optional, skipped if missing)
    "msBuildTargetDirectory": "C:\\Acumatica\\projects\\ACUCustomization\\src\\ACUCustomization\\bin\\Release",               External library solution build
                                                                                                                              targetdirectory
    "msBuildAssemblyName": "ACUCustomization.dll",                                                                            Name of accembly (dll file)
    "makeMode": null,                                                                                                         Package naming modes:
                                                                                                                               - Base (default) - package name without version,
                                                                                                                               - QA - package will be named for QA testing,
                                                                                                                               - ISV - for ISV.
    "pkgDescription": null,                                                                                                   Package description
  }
}
```

### MSBuild resolution

`acu src make` builds the external library solution (`src.msBuildSolutionFile`) with MSBuild. MSBuild.exe is resolved in the following order, the first match wins:

| # | Source | Details |
|---|--------|---------|
| 1 | `src.msBuildPath` | Value from `acu.json` / `acu.json.user`. If set, auto-detection is skipped; a path to a missing file stops the build with a validation error |
| 2 | vswhere | Latest stable Visual Studio or Build Tools with MSBuild installed (`vswhere -latest -products * -requires Microsoft.Component.MSBuild`) |
| 3 | .NET SDK | `MSBuild.exe` of the highest stable SDK listed by `dotnet --list-sdks` |
| 4 | PATH | `MSBuild.exe` found by `where MSBuild.exe` |
| 5 | Disk scan | Recursive search on drive `C:` (`where /R C:\ MSBuild.exe`). Slow (minutes), a warning is written to the log |

The resolved path and its source are written to the log, for example:

```
[INF] MSBuild resolved via vswhere: C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe
```

Recommendations:

- If several Visual Studio versions are installed, step 2 picks the newest one. To build with a specific version, set `src.msBuildPath`.
- `src.msBuildPath` is machine specific, so put it into `acu.json.user` (not tracked by git) rather than into `acu.json`.
- If the log shows `disk scan`, set `src.msBuildPath` to avoid a multi-minute search on every build.
