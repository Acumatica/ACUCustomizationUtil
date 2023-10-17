# Acumatica Customization Util (ACU)

## Command Reference (version 23.10.17.38782)

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
|             |              | --sqlServerName         | SQL Server instance for Acumatica database [default: localhost]       
|             |              | --dbName                | Acumatica database name                                               
|             |              | --instanceName          | Acumatica instance name                                               
|             |              | --instansePath          | Acumatica instanse physical path                                      
|             |              | --acuAdminName          | Acumatica instance admin name                                         
|             |              | --acuAdminPassword      | Acumatica instance admin password                                     
|             |              | --acuToolPath           | Acumatica ac.exe tool path                                            
|             | update       |                         | Update Acumatica instance                                             
|             |              | instance                | Update site                                                           
|             |              | database                | Update database                                                       
|             |              | --acuToolPath           | Acumatica ac.exe tool path: version for update                        
|             | delete       |                         | Delete Acumatica instance                                             
|             |              | --instanceName          | Acumatica instance name                                               
|             |              | --acuToolPath           | Acumatica ac.exe tool path: current version                           
| **Command** | **Commands** | **Options**             | **Description**                                                           
| src         |              |                         |                                                                       
|             | get          |                         | Get customization project source                                      
|             |              | --packageName           | Package name                                                          
|             |              | --dbConnectionString    | Database connection string                                            
|             |              | --sitePath              | Acumatica instance physical path                                      
|             |              | --sourceDirectory       | Customization source items directory                                  
|             | make         |                         | Create customization package from source code                         
|             |              | --sourceDirectory       | Customization source items directory                                  
|             |              | --packageName           | Package name                                                          
|             |              | --packageDirectory      | Package destination directory                                         
|             |              | --makeMode              | Mode for create package QA or ISV                                     
|             | build        |                         | Compile external library source code                                  
|             |              | --solutionFile          | External code solution file full name                                 
|             |              | --targetDirectory       | External code build target directory                                  
| **Command** | **Commands** | **Options**             | **Description**                                                           
| pkg         |              |                         |                                                                       
|             |              | --url                   | Acumatica instance url                                                
|             |              | --login                 | User login                                                            
|             |              | --password              | User password                                                         
|             |              | --tenant                | Tenant to login                                                       
|             | get          |                         | Get package content.                                                  
|             |              | --packageName           | Package name                                                          
|             |              | --packageDir            | Package directory                                                     
|             | publish      |                         | Publish package(s)                                                    
|             |              | --packageName           | Package name                                                          
|             | upload       |                         | Upload package                                                        
|             |              | --packageName           | Package name                                                          
|             |              | --packageDir            | Package directory                                                     
|             | unpublish    |                         | Unpublish all packages                                                
