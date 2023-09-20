# Acumatica Customization Util (ACU)

## Configuration Reference (version 23.9.14.32856)
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
    "sqlServerName": "localhost",                                                                                             Name of SQL server with ACU instance databace
    "dbName": "23.105.0016DB",                                                                                                Instance database name
    "acumaticaAdminName": "admin",                                                                                            Name of admin user
    "acumaticaAdminPassword": "123",                                                                                          Password which will be set for the  admin user
                                                                                                                              after instance was installed
    "acumaticaToolPath": null,                                                                                                Path to acumatica tool "ac.exe"
    "dbConnectionString": null,                                                                                               Database connection string
    "iisAppPool": null,                                                                                                       IIS app pool name
    "iisWebSite": null                                                                                                        IIS web site name
  },

  "pkg": {                                                                                                                    Parameters for package command
    "url": "http://localhost/23.105.0016/api/ServiceGate.asmx",                                                               URL of service gate
    "login": "admin",                                                                                                         Service gate login
    "password": "123",                                                                                                        Service gate passworg
    "pkgName": "ACUCustomization",                                                                                            Package name
    "pkgDirectory": "C:\\Acumatica\\projects\\ACUCustomization\\pkg"                                                          Package directory
    "tenant": null,                                                                                                           Tenant to connect
  },

  "src": {                                                                                                                    Parameters for code command
    "pkgSourceDirectory": "C:\\Acumatica\\projects\\ACUCustomization\\cst",                                                   Directory with package source code
    "pkgLevel": "0",                                                                                                          Package level
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
