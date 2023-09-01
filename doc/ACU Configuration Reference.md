## ACU configuration referense

```
{
  "erp": {                                                                                                                    Parameters for erp command
    "url": "http://acumatica-builds.s3.amazonaws.com/builds/23.1/23.105.0016/AcumaticaERP/AcumaticaERPInstall.msi",           URL for download ERP installer
    "erpVersion": "23.105.0016",                                                                                              ERP Version
    "installationFileName": "AcumaticaERPInstall.msi",                                                                        Installation file name
    "destinationDirectory": "C:\\ACU\\ERPs"                                                                                   Destination directory
  },
  "site": {                                                                                                                   Parameters for site command
    "acumaticaToolPath": null,                                                                                                Path to acumatica tool "ac.exe"
    "instanceName": "AcuTestUtil",                                                                                            Instance name
    "instancePath": "C:\\ACU\\instances\\AcuTestUtil",                                                                        Full physical path of instance
    "sqlServerName": "SPRINT039",                                                                                             Name of SQL server with ACU instance databace
    "dbName": "AcuTestUtilDB",                                                                                                Instance database name
    "dbConnectionString": "Data Source=SPRINT039;Initial Catalog=AcuTestUtilDB;Integrated Security=True;Encrypt=False;",      Database connection string
    "acumaticaAdminName": "admin",                                                                                            Name of admin user
    "acumaticaAdminPassword": "123",                                                                                          Password which will be set for the  admin user
                                                                                                                              after instance was installed
    "iisAppPool": null,                                                                                                       IIS app pool name
    "iisWebSite": null                                                                                                        IIS web site name
  },
  "package": {                                                                                                                Parameters for package command
    "url": "http://localhost/AcuTestUtil/api/ServiceGate.asmx",                                                               URL of service gate
    "login": "admin",                                                                                                         Service gate login
    "password": "123",                                                                                                        Service gate passworg
    "tenant": null,                                                                                                           Tenant to connect
    "packageName": "AcuTest",                                                                                                 Package name
    "packageDirectory": "C:\\ACU\\ERPs\\pkg"                                                                                  Package directory
  },
  "code": {                                                                                                                   Parameters for code command
    "pkgSourceDirectory": "C:\\ACU\\ERPs\\src",                                                                               Directory with package sources
    "pkgDescription": null,                                                                                                   Package description
    "pkgLevel": "0",                                                                                                          Package level
    "msBuildSolutionFile": "C:\\VS\\SPFileProcessing\\SPFileProcessing\\SPFileProcessing.sln",                                External library solution file full path
    "msBuildTargetDirectory": "C:\\VS\\SPFileProcessing\\SPFileProcessing\\bin\\Release\\",                                   External library solution build target directory
    "makeMode": null,                                                                                                         Option "makeMode" (QA/IVS/):
                                                                                                                              - QA - package will be named for QA testing,
                                                                                                                              - ISV - for ISV
  }
}
```
