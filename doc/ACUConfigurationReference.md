# Acumatica Customization Util (ACU)

## Configuration Reference (version 26.08.20.1929)
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
    "instancePath": "C:\\Acumatica\\instance\\23.105.0016\\Site",                                                             Full physical path of instance
    "dbProvider": null,                                                                                                       Database provider: "mssql" (default) or "mysql"
    "sqlServerName": "localhost",                                                                                             Name of database server with ACU instance database
    "dbPort": null,                                                                                                           Database server port (mysql only, default 3306)
    "dbName": "23.105.0016DB",                                                                                                Instance database name
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
    "url": "http://localhost/23.105.0016/api/ServiceGate.asmx",                                                               URL of service gate
    "login": "admin",                                                                                                         Service gate login
    "password": "123",                                                                                                        Service gate passworg
    "pkgName": "ACUCustomization",                                                                                            Package name
    "pkgDirectory": "C:\\Acumatica\\projects\\ACUCustomization\\pkg"                                                          Package directory
    "tenant": null,                                                                                                           Tenant to login
    "branch": null											                      Branch to login	
  },

  "src": {                                                                                                                    Parameters for code command
    "pkgSourceDirectory": "C:\\Acumatica\\projects\\ACUCustomization\\cst",                                                   Directory with package source code
    "pkgLevel": "0",                                                                                                          Package level
    "msBuildPath": "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe",         MSBuild full path
    "assemblyInfoPath": "src\\ACUCustomization\\Properties\\AssemblyInfo.cs",						      External code assembly info file full name
    "msBuildSolutionFile": "C:\\Acumatica\\projects\\ACUCustomization\\ACUCustomization.sln",                                 External library solution file full path
    "msBuildTargetDirectory": "C:\\Acumatica\\projects\\ACUCustomization\\src\\ACUCustomization\\bin\\Release",               External library solution build
                                                                                                                              targetdirectory
    "msBuildAssemblyName": "ACUCustomization.dll",                                                                            Name of accembly (dll file)
    "makeMode": null,                                                                                                         Option "makeMode" (QA/IVS/):
                                                                                                                               - QA - package will be named for QA testing,
                                                                                                                               - ISV - for ISV
    "pkgDescription": null,                                                                                                   Package description
  }
}
```