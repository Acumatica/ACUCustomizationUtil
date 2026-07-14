# Acumatica Customization Util (ACU)

## Command Reference (version 26.07.14.1557))

| **Command** | **Commands** | **Global Options**      | **Description**                                                       
|-------------|--------------|-------------------------|-----------------------------------------------------------------------
| acu         |              |                         |                                                                       
|             |              | --config                | An option of path to configuration file [default: acu.json]           
|             |              | --user-config           | An option of path to user configuration file [default: acu.json.user] 
|             |              | --version               | Show version information                                              
|             |              | -?                      | Show help and usage information                                       
|             |              | -h                      |                                                                       
|             |              | --help                  |                                                                       
|             | erp          |                         | Work with a Acumatica ERP                                             
|             | site         |                         | Work with a Acumatica instance                                        
|             | src          |                         | Work with a source code of customization                              
|             | pkg          |                         | Work with a customization package                                     
| **Command** | **Commands** | **Options**             | **Description**                                                       
| erp         |              |                         |                                                                       
|             | download     |                         | Download ERP installation                                             
|             |              | --erp-version           | ERP version                                                           
|             |              | --destination-directory | Base directory for install ERP                                        
|             |              | --installer-name        | Name of ERP installer file [default: AcumaticaERPInstall.msi]         
|             |              | --url                   | ERP installer download url                                            
|             | install      |                         | Install ERP                                                           
|             |              | --erp-version           | ERP version                                                           
|             |              | --destination-directory | Base directory for install ERP                                        
|             |              | --installer-name        | Name of ERP installer file [default: AcumaticaERPInstall.msi]         
|             | delete       |                         | Delete ERP                                                            
|             |              | --erp-version           | ERP version                                                           
|             |              | --destination-directory | Base directory for install ERP                                        
|             |              | --installer-name        | Name of ERP installer file [default: AcumaticaERPInstall.msi]         
| **Command** | **Commands** | **Options**             | **Description**                                                           
| site        |              |                         |                                                                       
|             | install      |                         | Install Acumatica instance                                            
|             |              | --sqlServerName         | Database server for Acumatica database [default: localhost]           
|             |              | --dbName                | Acumatica database name                                               
|             |              | --dbProvider            | Database provider: mssql or mysql [default: mssql]                    
|             |              | --dbUser                | Database user name (required for mysql)                               
|             |              | --dbPassword            | Database user password (required for mysql)                           
|             |              | --instanceName          | Acumatica instance name                                               
|             |              | --instansePath          | Acumatica instanse physical path                                      
|             |              | --acuAdminName          | Acumatica instance admin name                                         
|             |              | --acuAdminPassword      | Acumatica instance admin password                                     
|             |              | --acuToolPath           | Acumatica ac.exe tool path                                            
|             | update       |                         | Update Acumatica instance                                             
|             |              | instance                | Update site                                                           
|             |              | database                | Update database                                                       
|             |              | --dbProvider            | Database provider: mssql or mysql [default: mssql] (database)         
|             |              | --dbUser                | Database user name (required for mysql) (database)                    
|             |              | --dbPassword            | Database user password (required for mysql) (database)                
|             |              | --acuToolPath           | Acumatica ac.exe tool path: version for update                        
|             | delete       |                         | Delete Acumatica instance                                             
|             |              | --instanceName          | Acumatica instance name                                               
|             |              | --acuToolPath           | Acumatica ac.exe tool path: current version                           
| **Command** | **Commands** | **Options**             | **Description**                                                           
| src         |              |                         |                                                                       
|             | get          |                         | Get customization project source                                      
|             |              | --packageName           | Package name                                                          
|             |              | --dbConnectionString    | Database connection string                                            
|             |              | --dbProvider            | Database provider: mssql or mysql [default: mssql]                    
|             |              | --dbUser                | Database user name (required for mysql)                               
|             |              | --dbPassword            | Database user password (required for mysql)                           
|             |              | --sitePath              | Acumatica instance physical path                                      
|             |              | --sourceDirectory       | Customization source items directory                                  
|             | make         |                         | Create customization package from source code                         
|             |              | --sourceDirectory       | Customization source items directory                                  
|             |              | --packageName           | Package name
|             |              | --pkgSuffix             | Package name suffix (to add the task number to the package name)                
|             |              | --packageDirectory      | Package destination directory                                         
|             |              | --makeMode              | Mode for create package QA or ISV                                     
|             | build        |                         | Compile external library source code                                  
|             |              | --solutionFile          | External code solution file full name                                 
|             |              | --targetDirectory       | External code build target directory
|             |              | --msBuildPath           | MSBuild full path
|             |              | --assemblyInfoPath      | External code assembly info file full name
|             |              |                         | 
| **Command** | **Commands** | **Options**             | **Description**                                                           
| pkg         |              |                         |                                                                       
|             |              | --url                   | Acumatica instance url                                                
|             |              | --login                 | User login                                                            
|             |              | --password              | User password                                                         
|             |              | --tenant                | Tenant to login
|             |              | --branch                | Branch to login                                                        
|             | get          |                         | Get package content.                                                  
|             |              | --packageName           | Package name                                                          
|             |              | --packageDir            | Package directory                                                     
|             | publish      |                         | Publish package(s)                                                    
|             |              | --packageName           | Package name                                                          
|             | upload       |                         | Upload package                                                        
|             |              | --packageName           | Package name                                                          
|             |              | --packageDir            | Package directory                                                     
|             | unpublish    |                         | Unpublish all packages                                                









