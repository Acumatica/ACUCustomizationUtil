# Acumatica Customization Util (ACU)

## Project Configuration Guide (version 23.7.12.23868)

### Введение

В данном руководстве будут рассмотрены два способа организации проектов. Назовем их условно "классический" и "современный".
Классический проект - это проект, организованный по принципу именования каталогов в соотвествии с конвенцией, при этом каждое наименование папки жестко привязано к контенту, который храниться в ней. Использовался совместно со средством автоматизации разработки CstCustomizationUtil.
Современный проект - проект, организованный в виде папок, именованых в соотвествии с представлениями разработчика о здравом смысле. Такие проекты автоматизируются средством ACUCustomizationUtil. Конфигурация ACUCustomizationUtil позволяет настраивать конфигурацию под любую организацию проекта, максимально гибко, без каких-либо правил или конвеннций. 

Основной проблемой в организации любого проекта кастомизации является наличие в нем зависимостей (подключаемые библиотеки, пути установки ERP и инстанса Акуматики и т.д). При командной работе такие ограничения можно решить тремя  способами:

	- каталоги проекта и пути к подключаемым ресурсам, а так же к ERP & инстансу Acumatica одинаковы и жестко заданы.
	- каталоги проекта жеско заданы, пути к подключаемым ресурсам, к ERP & инстансу Acumatica сконфигурированы как относительные (относительно основного каталога проекта).
	- каталоги проекта, а так же пути к подключаемым ресурсам, к ERP & инстансу Acumatica конфигурируются как вычисляемые параметры.
Первый способ самый простой, но и абсолютно негибкий. Просто отметим это и перейдем к рассмотрению оставшихся двух.
Второй способ довольно работоспособоен, требует минимальной конфигурации. Однако основаная проблема такого подхода - невозможность создавать и использовать инстансы Acumatica как разделяемые ресурсы между проектами. Таким образом, для каждого проекта кастомизации было необходимо раворачиать отдельный инстанс Acumatica, что является избыточным. Такой способ использует "классический" способ.
Третий подход видиться наиболее перспективным. Единственная проблемы - кажущаяся на первых порах сложность конфигурации. Преимущества же - гибкость и минимализм конфигурации, возможность использовать ранее установленные инстансы Acumatica как общие ресурсы между проектами.

> Если проект создается "с нуля", то должен создаваться проект в современном стиле.

> Проекты в классическом стиле должны быть преобразованы в соотвествии с этим руководством.

### Структура каталогов и файлов

* Классический проект
```powershell
ProjectName                         root folder
    ├───ProjectName                 project C#, extension library
    ├───ProjectName.package         customization project package
    └───ProjectName.source          customization project source code
	.gitignore                      ignore rules for Git
	acu.json                        ACUCustomizationUtil config file
	Directory.Build.props           shared parameters for all solution projects
	ProjectName.sln                 solution file
	README.md                       solution information
```

* Современный проект
```powershell
ProjectName
├───cst                             customization project  source code
├───pkg                             directory for customization packages
└───src                             project C#, extension library
    └───ProjectName
	.gitignore						ignore rules for Git
	acu.json						ACUCustomizationUtil config file
	Directory.Build.props			shared parameters for all solution projects
	ProjectName.sln					solution file
	README.md						solution information
```

### Изменения в проекте
1. Для проекта создается корневая папка (root). Има папки как правило совпадает с именем проекта.
2. В каталоге ProjectName или src создается проект типа Class Library (.NET Framework)
![CreateNewProject](img/CreateNewProject.png)
3. В корневую папку копируется файл **Directory.Build.props**, в котором определена переменная **SiteDir**. Значение этой переменной должно указывать на инстанс Acumatica, для которого разрабатывается кастомизация. Значение переменной **SiteDir** используется в файле проекта, как заменитель пути к инстансу Acumatica.

_Directory.Build.props_
```xml
<Project>
    <PropertyGroup>
        <TargetFramework>net48</TargetFramework>
        <SiteDir>C:\Acumatica\instance\23.105.0016\Site</SiteDir>   <--- Variable SiteDir. 
    </PropertyGroup>
</Project>
```
3. В файл проекта вносятся такие изменения:

 * все пути в библиотекам, которые подключаются из инстанса Acumatica записываются с использованием переменной $(SiteDir):
	
_Project File_
```xml
  <ItemGroup>
    <Reference Include="PX.Common, Version=1.0.0.0, Culture=neutral">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>$(SiteDir)\Bin\PX.Common.dll</HintPath>
    </Reference>
    <Reference Include="PX.Data, Version=1.0.0.0, Culture=neutral">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>$(SiteDir)\Bin\PX.Data.dll</HintPath>
    </Reference>
    <Reference Include="PX.Objects, Version=1.0.0.0, Culture=neutral">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>$(SiteDir)\Bin\PX.Objects.dll</HintPath>
    </Reference>
  </ItemGroup>
```
 * Добавляется правило BeforeBuild с инструкциями, которые позволяют задавать версию сборки при использовании MSBuild:
 
 _Project File_
```xml
<Target Name="BeforeBuild">
	<ItemGroup>
		<AssemblyAttributes Include="AssemblyVersion">
			<_Parameter1>$(Version)</_Parameter1>
		</AssemblyAttributes>
	</ItemGroup>
	<MakeDir Directories="$(IntermediateOutputPath)" />
	<WriteCodeFragment Language="C#" OutputFile="$(IntermediateOutputPath)Version.cs"  AssemblyAttributes="@(AssemblyAttributes)" />
	<ItemGroup>
		<Compile Include="$(IntermediateOutputPath)Version.cs" />
	</ItemGroup>
</Target>
```

 * Добавляется секция PostBuildEvent с инструкциями, которые копируют сборку проекта в каталог Bin Acumatica instance после каждой удачной сборки проекта:
 
 _Project File_
```xml
<PropertyGroup>
	<PostBuildEvent>
			xcopy /F /Y $(TargetPath) $(SiteDir)\Bin\
			xcopy /F /Y $(TargetDir)$(TargetName).pdb $(SiteDir)\Bin\
	</PostBuildEvent>
</PropertyGroup>
```

4. Файл Solution (ProjectName.sln) необходимо перенести в корневую папку проекта и отредактировать путь к проекту Extension Library.

### Преобразование классического проекта
1. Определить используемую версию ERP Acumatica
2. Удалить каталог CstCustomizationUtil со всем его содержимым.
3. Удалить ссылку на инстанс Acumatica из файла solution
4. Выполнить действия, описанные в разделе "Изменения в проекте" этого руководства
5. Добавить файл конфигурации acu.json и сконфигурировать параметры для текущего проекта кастомизации.
Если необходимо переключить проект на использование общего инстанса Акуматика
6. С помощью утилиты Acumatica соотвествующей версии удалить инстанс Акуматика, с которым работает проект
7. Сделать изменения в файле acu.json: разделы erp & site
8. Удалить каталог ProjectName.webapp
