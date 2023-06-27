//
// C# 
// MSILid4Exe.MsiInterop+C#Classes
// v 0.3, 27.06.2023
// https://github.com/dkxce
// en,ru,1251,utf-8
//

// https://learn.microsoft.com/en-us/windows/win32/msi/windows-installer-portal

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace MSILid4Exe
{
    #region	Constants
    /// <summary>
    /// Internal class containing constants for the <c>script</c> parameter of <see cref="MsiInterop.MsiAdvertiseProduct"/> or <see cref="MsiInterop.MsiAdvertiseProductEx"/>.
    /// This class cannot be inherited.
    /// This class cannot be instantiated directly.
    /// </summary>
    sealed internal class MsiAdvertiseProductFlag
    {
        #region	Constants (Static Fields)
        /// <summary>Set to advertise a per-machine installation of the product available to all users.</summary>
        static public readonly string MachineAssign = ((char)0).ToString();
        /// <summary>Set to advertise a per-user installation of the product available to a particular user.</summary>
        static public readonly string UserAssign = ((char)1).ToString();
        #endregion	Constants (Static Fields)

        #region	Construction / Destruction
        private MsiAdvertiseProductFlag() { }
        #endregion	Construction / Destruction
    }

    /// <summary>
    /// Internal class containing constants for an MSI database.
    /// This class cannot be inherited.
    /// This class cannot be instantiated directly.
    /// </summary>
    sealed internal class MsiDatabaseTable
    {
        #region	Constants
        /// <summary>The _Columns table is a read-only system table that contains the column catalog. It lists the columns for all the tables. You can query this table to find out if a given column exists.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string _Columns = "_Columns";

        /// <summary>The _Storages table lists embedded OLE data storages. This is a temporary table, created only when referenced by a SQL statement.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string _Storages = "_Storages";

        /// <summary>The _Streams table lists embedded OLE data streams. This is a temporary table, created only when referenced by a SQL statement.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string _Streams = "_Streams";

        /// <summary>The _Tables table is a read-only system table that lists all the tables in the database. Query this table to find out if a table exists.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string _Tables = "_Tables";

        /// <summary>This is a read-only temporary table used to view transforms with the transform view mode. This table is never persisted by the installer.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string _TransformView = "_TransformView";

        /// <summary>The _Validation table is a system table that contains the column names and the column values for all of the tables in the database. It is used during the database validation process to ensure that all columns are accounted for and have the correct values. This table is not shipped with the installer database.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string _Validation = "_Validation";

        /// <summary>The ActionText table contains text to be displayed in a progress dialog box and written to the log for actions that take a long time to execute. The text displayed consists of the action description and optionally formatted data from the action.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ActionText = "ActionText";

        /// <summary>The AdminExecuteSequence table lists actions that the installer calls in sequence when the top-level ADMIN action is executed.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string AdminExecuteSequence = "AdminExecuteSequence";

        /// <summary>The AdminUISequence table lists actions that the installer calls in sequence when the top-level ADMIN action is executed and the internal user interface level is set to full UI or reduced UI. The installer skips the actions in this table if the user interface level is set to basic UI or no UI.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string AdminUISequence = "AdminUISequence";

        /// <summary>The AdvtExecuteSequence table lists actions the installer calls when the top-level ADVERTISE action is executed.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string AdvtExecuteSequence = "AdvtExecuteSequence";

        /// <summary>The installer does not use this table. The AdvtUISequence table should not exist in the installation database or it should be left empty.</summary>
        public const string AdvtUISequence = "AdvtUISequence";

        /// <summary>The AppId table or the <see cref="Registry"/> table specifies that the installer configure and register DCOM servers to do one of the following during an installation.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string AppId = "AppId";

        /// <summary>The AppSearch table contains properties needed to search for a file having a particular file signature. The AppSearch table can also be used to set a property to the existing value of a registry or .ini file entry.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string AppSearch = "AppSearch";

        /// <summary>The BBControl table lists the controls to be displayed on each billboard.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string BBControl = "BBControl";

        /// <summary>The Billboard table lists the Billboard controls displayed in the full user interface.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Billboard = "Billboard";

        /// <summary>The Binary table holds the binary data for items such as bitmaps, animations, and icons. The binary table is also used to store data for custom actions.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Binary = "Binary";

        /// <summary>The BindImage table contains information about each executable or DLL that needs to be bound to the DLLs imported by it.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string BindImage = "BindImage";

        /// <summary>The CCPSearch table contains the list of file signatures used for the Compliance Checking Program (CCP). At least one of these files needs to be present on a user's computer for the user to be in compliance with the program.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string CCPSearch = "CCPSearch";

        /// <summary>The CheckBox table lists the values for the check boxes.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string CheckBox = "CheckBox";

        /// <summary>The Class table contains COM server-related information that must be generated as a part of the product advertisement. Each row may generate a set of registry keys and values. The associated ProgId information is included in this table.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Class = "Class";

        /// <summary>The lines of a combo box are not treated as individual controls; they are part of a single combo box that functions as a control. This table lists the values for each combo box.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ComboBox = "ComboBox";

        /// <summary>The CompLocator table holds the information needed to find a file or a directory using the installer configuration data.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string CompLocator = "CompLocator";

        /// <summary>The Complus table contains information needed to install COM+ applications.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Complus = "Complus";

        /// <summary>The Component table lists components.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Component = "Component";

        /// <summary>The Condition table can be used to modify the selection state of any entry in the <see cref="Feature"/> table based on a conditional expression.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Condition = "Condition";

        /// <summary>The Control table defines the controls that appear on each dialog box.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Control = "Control";

        /// <summary>The ControlCondition table enables an author to specify special actions to be applied to controls based on the result of a conditional statement. For example, using this table the author could choose to hide a control based on the VersionNT property.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ControlCondition = "ControlCondition";

        /// <summary>The ControlEvent table allows the author to specify the Control Events started when a user interacts with a PushButton Control, CheckBox Control, or SelectionTree Control. These are the only controls users can use to initiate control events. Each control can publish multiple control events.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ControlEvent = "ControlEvent";

        /// <summary>The CreateFolder table contains references to folders that need to be created explicitly for a particular component.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string CreateFolder = "CreateFolder";

        /// <summary>The CustomAction table provides the means of integrating custom code and data into the installation. The source of the code that is executed can be a stream contained within the database, a recently installed file, or an existing executable file.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string CustomAction = "CustomAction";

        /// <summary>The Dialog table contains all the dialogs that appear in the user interface in both the full and reduced modes.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Dialog = "Dialog";

        /// <summary>The Directory table specifies the directory layout for the product. Each row of the table indicates a directory both at the source and the target.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Directory = "Directory";

        /// <summary>The DrLocator table holds the information needed to find a file or directory by searching the directory tree.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string DrLocator = "DrLocator";

        /// <summary>The DuplicateFile table contains a list of files that are to be duplicated, either to a different directory than the original file or to the same directory but with a different name. The original file must be a file installed by the InstallFiles action.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string DuplicateFile = "DuplicateFile";

        /// <summary>The Environment table is used to set the values of environment variables.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Environment = "Environment";

        /// <summary>The Error table is used to look up error message formatting templates when processing errors with an error code set but without a formatting template set (this is the normal situation).</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Error = "Error";

        /// <summary>The EventMapping table lists the controls that subscribe to some control event and lists the attribute to be changed when the event is published by another control or the installer.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string EventMapping = "EventMapping";

        /// <summary>The Extension table contains information about file name extension servers that must be generated as a part of product advertisement. Each row generates a set of registry keys and values.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Extension = "Extension";

        /// <summary>The Feature table defines the logical tree structure of features.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Feature = "Feature";

        /// <summary>The FeatureComponents table defines the relationship between features and components. For each feature, this table lists all the components that make up that feature.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string FeatureComponents = "FeatureComponents";

        /// <summary>The File table contains a complete list of source files with their various attributes, ordered by a unique, non-localized, identifier. Files can be stored on the source media as individual files or compressed within a cabinet file.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string File = "File";

        /// <summary>The FileSFPCatalog table associates specified files with the catalog files used by Windows Millennium Edition for Windows File Protection.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string FileSFPCatalog = "FileSFPCatalog";

        /// <summary>The Font table contains the information for registering font files with the system.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Font = "Font";

        /// <summary>This table contains the icon files. Each icon from the table is copied to a file as a part of product advertisement to be used for advertised shortcuts and OLE servers.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Icon = "Icon";

        /// <summary>The IniFile table contains the .ini information that the application needs to set in an .ini file.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string IniFile = "IniFile";

        /// <summary>The IniLocator table holds the information needed to search for a file or directory using an .ini file or to search for a particular .ini entry itself. The .ini file must be present in the default Microsoft Windows directory.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string IniLocator = "IniLocator";

        /// <summary>The InstallExecuteSequence table lists actions that are executed when the top-level INSTALL action is executed.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string InstallExecuteSequence = "InstallExecuteSequence";

        /// <summary>The InstallUISequence table lists actions that are executed when the top-level INSTALL action is executed and the internal user interface level is set to full UI or reduced UI. The installer skips the actions in this table if the user interface level is set to basic UI or no UI.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string InstallUISequence = "InstallUISequence";

        /// <summary>Each record of the IsolatedComponent table associates the component specified in the Component_Application column (commonly an .exe) with the component specified in the Component_Shared column (commonly a shared DLL). The IsolateComponents action installs a copy of Component_Shared into a private location for use by Component_Application. This isolates the Component_Application from other copies of Component_Shared that may be installed to a shared location on the computer.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string IsolatedComponent = "IsolatedComponent";

        /// <summary>The LaunchCondition table is used by the LaunchConditions action. It contains a list of conditions that all must be satisfied for the installation to begin.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string LaunchCondition = "LaunchCondition";

        /// <summary>The lines of a list box are not treated as individual controls, but they are part of a list box that functions as a control. The ListBox table defines the values for all list boxes.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ListBox = "ListBox";

        /// <summary>The lines of a listview are not treated as individual controls, but they are part of a listview that functions as a control. The ListView table defines the values for all listviews.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ListView = "ListView";

        /// <summary>The LockPermissions table is used to secure individual portions of your application in a locked-down environment. It can be used with the installation of files, registry keys, and created folders.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string LockPermissions = "LockPermissions";

        /// <summary>The Media table describes the set of disks that make up the source media for the installation.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Media = "Media";

        /// <summary>The MIME table associates a MIME content type with a file extension or a CLSID to generate the extension or COM server information required for advertisement of the MIME (Multipurpose Internet Mail Extensions) content.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string MIME = "MIME";

        /// <summary>For merge modules, a merge tool evaluates the ModuleAdminUISequence table and then inserts the calculated actions into the <see cref="AdminUISequence"/> table with a correct sequence number.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleAdminUISequence = "ModuleAdminUISequence";

        /// <summary>For merge modules, a merge tool evaluates the ModuleAdminExecuteSequence table and then inserts the calculated actions into the <see cref="AdminExecuteSequence"/> table with a correct sequence number.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleAdminExecuteSequence = "ModuleAdminExecuteSequence";

        /// <summary>For merge modules, a merge tool evaluates the ModuleAdvtExecuteSequence table and then inserts the calculated actions into the <see cref="AdvtExecuteSequence"/> table with a correct sequence number.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleAdvtExecuteSequence = "ModuleAdvtExecuteSequence";

        /// <summary>For merge modules, the ModuleComponents table contains a list of the components found in the merge module.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleComponents = "ModuleComponents";

        /// <summary>For merge modules, the ModuleConfiguration table identifies the configurable attributes of the module. This table is not merged into the database.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleConfiguration = "ModuleConfiguration";

        /// <summary>For merge modules, the ModuleDependency table keeps a list of other merge modules that are required for this merge module to operate properly. This table enables a merge or verification tool to ensure that the necessary merge modules are in fact included in the user's installer database. The tool checks by cross referencing this table with the <see cref="ModuleSignature"/> table in the installer database.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleDependency = "ModuleDependency";

        /// <summary>For merge modules, the ModuleExclusion table keeps a list of other merge modules that are incompatible in the same installer database. This table enables a merge or verification tool to check that conflicting merge modules are not merged in the user's installer database. The tool checks by cross-referencing this table with the <see cref="ModuleSignature"/> table in the installer database.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleExclusion = "ModuleExclusion";

        /// <summary>For merge modules, if a table in the merge module is listed in the ModuleIgnoreTable table, it is not merged into the .msi file. If the table already exists in the .msi file, it is not modified by the merge. The tables in the ModuleIgnoreTable can therefore contain data that is unneeded after the merge.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModeleIgnore = "ModeleIgnore";

        /// <summary>For merge modules, a merge tool evaluates the ModuleInstallExecuteSequence table and then inserts the calculated actions into the <see cref="InstallExecuteSequence"/> table with a correct sequence number.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleInstallExecuteSequence = "ModuleInstallExecuteSequence";

        /// <summary>For merge modules, a merge tool evaluates the ModuleInstallUISequence table and then inserts the calculated actions into the <see cref="InstallUISequence"/> table with a correct sequence number.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleInstallUISequence = "ModuleInstallUISequence";

        /// <summary>For merge modules, the ModuleSignature Table is a required table. It contains all the information necessary to identify a merge module. The merge tool adds this table to the .msi file if one does not already exist. The ModuleSignature table in a merge module has only one row containing the ModuleID, Language, and Version. However, the ModuleSignature table in an .msi file has a row containing this information for each .msm file that has been merged into it.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleSignature = "ModuleSignature";

        /// <summary>For merge modules, the ModuleSubstitution table specifies the configurable fields of a module database and provides a template for the configuration of each field. The user or merge tool may query this table to determine what configuration operations are to take place. This table is not merged into the target database.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ModuleSubstitution = "ModuleSubstitution";

        /// <summary>This table contains a list of files to be moved or copied from a specified source directory to a specified destination directory.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string MoveFile = "MoveFile";

        /// <summary>The MsiAssembly table specifies Windows Installer settings for Microsoft .NET Framework assemblies and Win32 assemblies.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string MsiAssembly = "MsiAssembly";

        /// <summary>The MsiAssembly table and MsiAssemblyName table specify Windows Installer settings for common language runtime assemblies and Win32 assemblies.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string MsiAssemblyName = "MsiAssemblyName";

        /// <summary>The MsiDigitalCertificate table stores certificates in binary stream format and associates each certificate with a primary key. The primary key is used to share certificates among multiple digitally signed objects. A digital certificate is a credential that provides a means to verify identity.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string MsiDigitalCertificate = "MsiDigitalCertificate";

        /// <summary>The MsiDigitalSignature table contains the signature information for every digitally signed object in the installation database.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string MsiDigitalSignature = "MsiDigitalSignature";

        /// <summary>The MsiFileHash table is used to store a 128-bit hash of a source file provided by the Windows Installer package. The hash is split into four 32-bit values and stored in separate columns of the table.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string MsiFileHash = "MsiFileHash";

        /// <summary>The MsiPatchHeaders table holds the binary patch header streams used for patch validation. A patch containing a populated MsiPatchHeaders table can only be applied using Windows Installer version 2.0 or later.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string MsiPatchHeaders = "MsiPatchHeaders";

        /// <summary>The ODBCAttribute table contains information about the attributes of ODBC drivers and translators.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ODBCAttribute = "ODBCAttribute";

        /// <summary>The ODBCDataSource table lists the data sources belonging to the installation.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ODBCDataSource = "ODBCDataSource";

        /// <summary>The ODBCDriver table lists the ODBC drivers belonging to the installation.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ODBCDriver = "ODBCDriver";

        /// <summary>The ODBCSourceAttribute table contains information about the attributes of data sources.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ODBCSourceAttribute = "ODBCSourceAttribute";

        /// <summary>The ODBCTranslator table lists the ODBC translators belonging to the installation.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ODBCTranslator = "ODBCTranslator";

        /// <summary>The Patch table specifies the file that is to receive a particular patch and the physical location of the patch files on the media images.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Patch = "Patch";

        /// <summary>The PatchPackage table describes all patch packages that have been applied to this product. For each patch package, the unique identifier for the patch is provided along with information about the media image the on which the patch is located.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string PatchPackage = "PatchPackage";

        /// <summary>The ProgId table contains information for program IDs and version independent program IDs that must be generated as a part of the product advertisement.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ProgId = "ProgId";

        /// <summary>The Property table contains the property names and values for all defined properties in the installation. Properties with Null values are not present in the table.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Property = "Property";

        /// <summary>The PublishComponent table associates components listed in the <see cref="Component"/> table with a qualifier text-string and a category ID GUID. Components with parallel functionality that have been grouped together in this way are referred to as qualified components.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string PublishComponent = "PublishComponent";

        /// <summary>Radio buttons are not treated as individual controls, but they are part of a radio button group that functions as a RadioButtonGroup control. The RadioButton table lists the buttons for all the groups.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string RadioButton = "RadioButton";

        /// <summary>The Registry table holds the registry information that the application needs to set in the system registry.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Registry = "Registry";

        /// <summary>The RegLocator table holds the information needed to search for a file or directory using the registry, or to search for a particular registry entry itself.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string RegLocator = "RegLocator";

        /// <summary>The RemoveFile table contains a list of files to be removed by the RemoveFiles action. Setting the FileName column of this table to Null supports the removal of empty folders.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string RemoveFile = "RemoveFile";

        /// <summary>The RemoveIniFile table contains the information an application needs to delete from a .ini file.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string RemoveIniFile = "RemoveIniFile";

        /// <summary>The RemoveRegistry table contains the registry information the application needs to delete from the system registry.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string RemoveRegistry = "RemoveRegistry";

        /// <summary>The ReserveCost table is an optional table that allows the author to reserve an amount of disk space in any directory that depends on the installation state of a component.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ReserveCost = "ReserveCost";

        /// <summary>The SelfReg table contains information about modules that need to be self registered. The installer calls the DllRegisterServer function during installation of the module; it calls DllUnregisterServer during uninstallation of the module. The installer does not self register EXE files.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string SelfReg = "SelfReg";

        /// <summary>The ServiceControl table is used to control installed or uninstalled services.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ServiceControl = "ServiceControl";

        /// <summary>The ServiceInstall table is used to install a service.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string ServiceInstall = "ServiceInstall";

        /// <summary>The SFPCatalog table contains the catalogs used by Windows Millennium Edition for Windows File Protection on Windows Millennium Edition.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string SFPCatalog = "SFPCatalog";

        /// <summary>The Shortcut table holds the information the application needs to create shortcuts on the user's computer.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Shortcut = "Shortcut";

        /// <summary>The Signature table holds the information that uniquely identifies a file signature.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Signature = "Signature";

        /// <summary>The TextStyle table lists different font styles used in controls having text.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string TextStyle = "TextStyle";

        /// <summary>The TypeLib table contains the information that needs to be placed in the registry registration of type libraries.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string TypeLib = "TypeLib";

        /// <summary>The UIText table contains the localized versions of some of the strings used in the user interface. These strings are not part of any other table. The UIText table is for strings that have no logical place in any other table.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string UIText = "UIText";

        /// <summary>The Upgrade table contains information required during major upgrades. To fully enable the installer's upgrade capabilities, every package should have an UpgradeCode property and an Upgrade table. Each record in the Upgrade table gives a characteristic combination of upgrade code, product version, and language information used to identify a set of products affected by the upgrade.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Upgrade = "Upgrade";

        /// <summary>The Verb table contains command-verb information associated with file extensions that must be generated as a part of product advertisement. Each row generates a set of registry keys and values.</summary>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        public const string Verb = "Verb";
        #endregion	Constants

        #region	Construction / Destruction
        private MsiDatabaseTable() { }
        #endregion	Construction / Destruction
    }

    /// <summary>
    /// Internal class containing constants for MSI installer properties.
    /// This class cannot be inherited.
    /// This class cannot be instantiated directly.
    /// </summary>
    sealed internal class MsiInstallerProperty
    {
        #region	Constants (Static Fields)
        #region	Component Location
        /// <summary>The installer sets the OriginalDatabase property to the launched-from database, the database on the source, or the cached database.</summary>
        public const string OriginalDatabase = "OriginalDatabase";

        /// <summary>Root directory containing the source files.</summary>
        public const string SourceDir = "SourceDir";

        /// <summary>Specifies the root destination directory for the installation. During an administrative installation this property is the location to copy the installation package.</summary>
        public const string TARGETDIR = "TARGETDIR";
        #endregion	Component Location

        #region	Configuration Properties
        /// <summary>Initial action called after the installer is initialized.</summary>
        public const string ACTION = "ACTION";

        /// <summary>Determines where configuration information will be stored.</summary>
        public const string ALLUSERS = "ALLUSERS";

        /// <summary>URL of the update channel for the application.</summary>
        public const string ARPAUTHORIZEDCDFPREFIX = "ARPAUTHORIZEDCDFPREFIX";

        /// <summary>Provides Comments for the Add or Remove Programs on Control Panel.</summary>
        public const string ARPCOMMENTS = "ARPCOMMENTS";

        /// <summary>Provides Contact for the Add or Remove Programs on Control Panel.</summary>
        public const string ARPCONTACT = "ARPCONTACT";

        /// <summary>Fully qualified path to the application's primary folder.</summary>
        public const string ARPINSTALLLOCATION = "ARPINSTALLLOCATION";

        /// <summary>Disables functionality that would modify the product.</summary>
        public const string ARPNOMODIFY = "ARPNOMODIFY";

        /// <summary>Disables functionality that would remove the product.</summary>
        public const string ARPNOREMOVE = "ARPNOREMOVE";

        /// <summary>Disables the Repair button in the Programs wizard.</summary>
        public const string ARPNOREPAIR = "ARPNOREPAIR";

        /// <summary>Specifies the primary icon for the installation package.</summary>
        public const string ARPPRODUCTICON = "ARPPRODUCTICON";

        /// <summary>Provides a ReadMe for the Add or Remove Programs on Control Panel.</summary>
        public const string ARPREADME = "ARPREADME";

        /// <summary>Estimated size of the application in kilobytes.</summary>
        public const string ARPSIZE = "ARPSIZE";

        /// <summary>Prevents display of application in the Add or Remove Programs list.</summary>
        public const string ARPSYSTEMCOMPONENT = "ARPSYSTEMCOMPONENT";

        /// <summary>URL for an application's home page.</summary>
        public const string ARPURLINFOABOUT = "ARPURLINFOABOUT";

        /// <summary>URL for application-update information.</summary>
        public const string ARPURLUPDATEINFO = "ARPURLUPDATEINFO";

        /// <summary>Registry space in xilobytes required by the application. Used by AllocateRegistrySpace action.</summary>
        public const string AVAILABLEFREEREG = "AVAILABLEFREEREG";

        /// <summary>The root path for any of the qualifying products for CCP.</summary>
        public const string CCP_DRIVE = "CCP_DRIVE";

        /// <summary>Default font style used for controls.</summary>
        public const string DefaultUIFont = "DefaultUIFont";

        /// <summary>Set to disable the generation certain shortcuts supporting installation-on-demand.</summary>
        public const string DISABLEADVTSHORTCUTS = "DISABLEADVTSHORTCUTS";

        /// <summary>Prevents the installer from registering media sources, such as a CD-ROMs, as valid sources for the product.</summary>
        public const string DISABLEMEDIA = "DISABLEMEDIA";

        /// <summary>Disables rollback for the current configuration.</summary>
        public const string DISABLEROLLBACK = "DISABLEROLLBACK";

        /// <summary>Top-level action initiated by the ExecuteAction action.</summary>
        public const string EXECUTEACTION = "EXECUTEACTION";

        /// <summary>Mode of execution performed by the installer.  (None, Script [default])</summary>
        public const string EXECUTEMODE = "EXECUTEMODE";

        /// <summary>Improves installation performance under specific OEM scenarios.</summary>
        public const string FASTOEM = "FASTOEM";

        /// <summary>Initial "level" at which features will be installed.  (1 - 32767)</summary>
        public const string INSTALLLEVEL = "INSTALLLEVEL";

        /// <summary>UI level capped as Basic.</summary>
        public const string LIMITUI = "LIMITUI";

        /// <summary>List of action names that will be logged (separated only by semicolons and with no spaces.)</summary>
        public const string LOGACTION = "LOGACTION";

        /// <summary>This property must be set to the relative path if the installation package is not located at the root of the CD-ROM.</summary>
        public const string MEDIAPACKAGEPATH = "MEDIAPACKAGEPATH";

        /// <summary>Set to prevent the installer from setting the DISABLEMEDIA property. Available with Windows Installer version 1.0.</summary>
        public const string MSINODISABLEMEDIA = "MSINODISABLEMEDIA";

        /// <summary>Allows the author to designate a "primary" folder for the installation. Used to determine the values for the PrimaryVolumePath, PrimaryVolumeSpaceAvailable, PrimaryVolumeSpaceRequired, and PrimaryVolumeSpaceRemaining properties.</summary>
        public const string PRIMARYFOLDER = "PRIMARYFOLDER";

        /// <summary>Runs an installation with elevated privileges.</summary>
        public const string Privileged = "Privileged";

        /// <summary>Action if there is insufficient disk space for the installation.  (P - prompt, D - disable, F - fail)</summary>
        public const string PROMPTROLLBACKCOST = "PROMPTROLLBACKCOST";

        /// <summary>Forces or suppresses restarting.  (Force, Suppress, ReallySuppress)</summary>
        public const string REBOOT = "REBOOT";

        /// <summary>Suppresses the display of prompts for restarts to the user. Any restarts that are needed happen automatically.  (S or Suppress)</summary>
        public const string REBOOTPROMPT = "REBOOTPROMPT";

        /// <summary>Default drive for the installation.  (Must end in '\')</summary>
        public const string ROOTDRIVE = "ROOTDRIVE";

        /// <summary>A table having the sequence table schema.</summary>
        public const string SEQUENCE = "SEQUENCE";

        /// <summary>Causes short file names to be used.</summary>
        public const string SHORTFILENAMES = "SHORTFILENAMES";

        /// <summary>List of transforms to be applied to the database.</summary>
        public const string Transforms = "Transforms";

        /// <summary>Informs the installer that the transforms for the product reside at the source.</summary>
        public const string TRANSFORMSATSOURCE = "TRANSFORMSATSOURCE";

        /// <summary>Setting the TRANSFORMSECURE property to 1 informs the installer that transforms are to be cached locally on the user's computer in a location where the user does not have write access.</summary>
        public const string TRANSFORMSSECURE = "TRANSFORMSSECURE";
        #endregion	Configuration Properties

        #region	Date / Time
        /// <summary>The current date.</summary>
        public const string Date = "Date";

        /// <summary>The current time.</summary>
        public const string Time = "Time";
        #endregion	Date / Time

        #region	Feature Installation Options
        /// <summary>List of features (delimited by commas) to be installed in their default configuration.</summary>
        public const string ADDDEFAULT = "ADDDEFAULT";

        /// <summary>List of features (delimited by commas) to be installed locally.</summary>
        public const string ADDLOCAL = "ADDLOCAL";

        /// <summary>List of features (delimited by commas) to be run from source.</summary>
        public const string ADDSOURCE = "ADDSOURCE";

        /// <summary>List of features (delimited by commas) to be advertised.</summary>
        public const string ADVERTISE = "ADVERTISE ";

        /// <summary>List of component IDs (delimited by commas) to be installed locally.</summary>
        public const string COMPADDLOCAL = "COMPADDLOCAL ";

        /// <summary>List of component IDs (delimited by commas) to run from source media.</summary>
        public const string COMPADDSOURCE = "COMPADDSOURCE ";

        /// <summary>List of file keys of files (delimited by commas) that are to be installed in their default configuration.</summary>
        public const string FILEADDDEFAULT = "FILEADDDEFAULT ";

        /// <summary>List of file keys of the files (delimited by commas) to be run locally.</summary>
        public const string FILEADDLOCAL = "FILEADDLOCAL ";

        /// <summary>List of file keys (delimited by commas)to be run from the source media.</summary>
        public const string FILEADDSOURCE = "FILEADDSOURCE ";

        /// <summary>Setting this property applies a patch.</summary>
        public const string PATCH = "PATCH";

        /// <summary>List of features (delimited by commas) to be reinstalled.</summary>
        public const string REINSTALL = "REINSTALL";

        /// <summary>A string containing letters that specify the type of reinstall to perform.</summary>
        public const string REINSTALLMODE = "REINSTALLMODE";

        /// <summary>List of features (delimited by commas) to be removed.</summary>
        public const string REMOVE = "REMOVE";
        #endregion	Feature Installation Options

        #region	Hardware
        /// <summary>Numeric processor level if running on an Alpha processor. </summary>
        public const string Alpha = "Alpha";

        /// <summary>The width, in pixels, of the window borders.</summary>
        public const string BorderSide = "BorderSide";

        /// <summary>The height, in pixels, of the window borders.</summary>
        public const string BorderTop = "BorderTop";

        /// <summary>Height, in pixels, of normal caption area.</summary>
        public const string CaptionHeight = "CaptionHeight";

        /// <summary>Number of adjacent color bits for each pixel.</summary>
        public const string ColorBits = "ColorBits";

        /// <summary>Numeric processor level if running on an Intel processor.</summary>
        public const string Intel = "Intel";

        /// <summary>Numeric processor level if running on an Itanium processor.</summary>
        public const string Intel64 = "Intel64";

        /// <summary>Size of the installed RAM in megabytes.</summary>
        public const string PhysicalMemory = "PhysicalMemory";

        /// <summary>Width, in pixels, of the screen.</summary>
        public const string ScreenX = "ScreenX";

        /// <summary>Height, in pixels, of the screen.</summary>
        public const string ScreenY = "ScreenY";

        /// <summary>The height of characters in logical units.</summary>
        public const string TextHeight = "TextHeight";

        /// <summary>Amount of available page file space in megabytes.</summary>
        public const string VirtualMemory = "VirtualMemory";
        #endregion	Hardware

        #region	Installation Status
        /// <summary>Indicates current installation follows a reboot invoked by the ForceReboot action.</summary>
        public const string AFTERREBOOT = "AFTERREBOOT";

        /// <summary>Indicates whether disk space costing has completed.</summary>
        public const string CostingComplete = "CostingComplete";

        /// <summary>Indicates that a product is already installed.</summary>
        public const string Installed = "Installed";

        /// <summary>The installer does a CRC on files only if the MSICHECKCRCS property is set.</summary>
        public const string MSICHECKCRCS = "MSICHECKCRCS";

        /// <summary>Suppresses the automatic setting of the COMPANYNAME property.</summary>
        public const string NOCOMPANYNAME = "NOCOMPANYNAME";

        /// <summary>Suppresses the automatic setting of the USERNAME property.</summary>
        public const string NOUSERNAME = "NOUSERNAME";

        /// <summary>Insufficient disk space to accommodate the installation.</summary>
        public const string OutOfDiskSpace = "OutOfDiskSpace";

        /// <summary>Insufficient disk space with rollback turned off.</summary>
        public const string OutOfNoRbDiskSpace = "OutOfNoRbDiskSpace";

        /// <summary>Features are already selected.</summary>
        public const string Preselected = "Preselected";

        /// <summary>The Installer sets the value of this property to the path of the volume designated by the PRIMARYFOLDER property.</summary>
        public const string PrimaryVolumePath = "PrimaryVolumePath";

        /// <summary>The Installer sets the value of this property to a string representing the total number of bytes available on the volume referenced by the PrimaryVolumePath property.</summary>
        public const string PrimaryVolumeSpaceAvailable = "PrimaryVolumeSpaceAvailable";

        /// <summary>The Installer sets the value of this property to a string representing the total number of bytes remaining on the volume referenced by the PrimaryVolumePath property if all the currently selected features were installed.</summary>
        public const string PrimaryVolumeSpaceRemaining = "PrimaryVolumeSpaceRemaining";

        /// <summary>The Installer sets the value of this property to a string representing the total number of bytes required by all currently selected features on the volume referenced by the PrimaryVolumePath property.</summary>
        public const string PrimaryVolumeSpaceRequired = "PrimaryVolumeSpaceRequired";

        /// <summary>Numeric language identifier (LANGID) for the database. (REQUIRED)</summary>
        public const string ProductLanguage = "ProductLanguage";

        /// <summary>Set if the installer installs over a file that is being held in use.</summary>
        public const string ReplacedInUseFiles = "ReplacedInUseFiles";

        /// <summary>Resumed installation.</summary>
        public const string RESUME = "RESUME";

        /// <summary>The installer sets this property whenever rollback is disabled.</summary>
        public const string RollbackDisabled = "RollbackDisabled";

        /// <summary>Indicates the user interface level.</summary>
        public const string UILevel = "UILevel";

        /// <summary>Set when changes to the system have begun for this installation.</summary>
        public const string UpdateStarted = "UpdateStarted";

        /// <summary>Set by the installer when an upgrade removes an application. Available with Windows Installer version 1.1 or later.</summary>
        public const string UPGRADINGPRODUCTCODE = "UPGRADINGPRODUCTCODE";

        /// <summary>The installer sets this property to the version of Windows Installer run during the installation.</summary>
        public const string VersionMsi = "VersionMsi";
        #endregion	Installation Status

        #region	Operating System
        /// <summary>Set on Microsoft Windows NT/Windows 2000 if the user has administrator privileges.</summary>
        public const string AdminUser = "AdminUser";

        /// <summary>Computer name of the current system.</summary>
        public const string ComputerName = "ComputerName";

        /// <summary>Indicates the Windows product type. Only available with Windows Installer version 2.0 and later versions.</summary>
        public const string MsiNTProductType = "MsiNTProductType";

        /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Microsoft BackOffice components are installed. Only available with Windows Installer version 2.0 and later versions.</summary>
        public const string MsiNTSuiteBackOffice = "MsiNTSuiteBackOffice";

        /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Windows 2000 DataCenter Server is installed. Only available with Windows Installer version 2.0 and later versions.</summary>
        public const string MsiNTSuiteDataCenter = "MsiNTSuiteDataCenter";

        /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Windows 2000 Advanced Server is installed. Only available with Windows Installer version 2.0 and later versions.</summary>
        public const string MsiNTSuiteEnterprise = "MsiNTSuiteEnterprise";

        /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Microsoft Small Business Server is installed. Only available with Windows Installer version 2.0 and later versions.</summary>
        public const string MsiNTSuiteSmallBusiness = "MsiNTSuiteSmallBusiness";

        /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Microsoft Small Business Server is installed with the restrictive client license. Only available with Windows Installer version 2.0 and later.</summary>
        public const string MsiNTSuiteSmallBusinessRestricted = "MsiNTSuiteSmallBusinessRestricted";

        /// <summary>On Windows 2000 and later operating systems, the installer sets the MsiNTSuiteWebServer property to 1 if the web edition of the Windows  2003 Server family is installed. Only available with the Windows Server 2003 family release of the Windows Installer.</summary>
        public const string MsiNTSuiteWebServer = "MsiNTSuiteWebServer";

        /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if the operating system is Workstation Personal (not Professional). Only available with Windows Installer version 2.0 and later versions.</summary>
        public const string MsiNTSuitePersonal = "MsiNTSuitePersonal";

        /// <summary>On systems that support common language runtime assemblies, the installer sets the value of this property to the file version of fusion.dll. The installer does not set this property if the operating system does not support common language runtime assemblies. Only available with Windows Installer version 2.0 and later.</summary>
        public const string MsiNetAssemblySupport = "MsiNetAssemblySupport";

        /// <summary>On systems that support Win32 assemblies, the installer sets the value of this property to the file version of sxs.dll. The installer does not set this property if the operating system does not support Win32 assemblies. Only available with Windows Installer version 2.0 and later.</summary>
        public const string MsiWin32AssemblySupport = "MsiWin32AssemblySupport";

        /// <summary>Set if OLE supports the Windows Installer.</summary>
        public const string OLEAdvtSupport = "OLEAdvtSupport";

        /// <summary>The installer sets the RedirectedDLLSupport property if the system performing the installation supports Isolated Components.</summary>
        public const string RedirectedDLLSupport = "RedirectedDLLSupport";

        /// <summary>The installer sets the RemoteAdminTS property when the system is a remote administration server using Terminal Services.</summary>
        public const string RemoteAdminTS = "RemoteAdminTS";

        /// <summary>The version number of the operating system service pack.</summary>
        public const string ServicePackLevel = "ServicePackLevel";

        /// <summary>The minor version number of the operating system service pack.</summary>
        public const string ServicePackLevelMinor = "ServicePackLevelMinor";

        /// <summary>Set when the system is operating as Shared Windows.</summary>
        public const string SharedWindows = "SharedWindows";

        /// <summary>Set if the shell supports feature advertising.</summary>
        public const string ShellAdvtSupport = "ShellAdvtSupport";

        /// <summary>Default language identifier for the system.</summary>
        public const string SystemLanguageID = "SystemLanguageID";

        /// <summary>Set when the system is a server with Windows Terminal Server.</summary>
        public const string TerminalServer = "TerminalServer";

        /// <summary>Indicates if the operating system supports using .TTC (true type font collections) files.</summary>
        public const string TTCSupport = "TTCSupport";

        /// <summary>Version number for the Windows operating system.</summary>
        public const string Version9X = "Version9X";

        /// <summary>Numeric database version of the current installation.</summary>
        public const string VersionDatabase = "VersionDatabase";

        /// <summary>Version number for the Windows NT/Windows 2000 operating system.</summary>
        public const string VersionNT = "VersionNT";

        /// <summary>Version number for the Windows NT/Windows 2000 operating system if the system is running on a 64-bit computer.</summary>
        public const string VersionNT64 = "VersionNT64";

        /// <summary>Build number of the operating system.</summary>
        public const string WindowsBuild = "WindowsBuild";
        #endregion	Operating System

        #region	Product Information
        /// <summary>Internet address, or URL, for technical support.</summary>
        public const string ARPHELPLINK = "ARPHELPLINK";

        /// <summary>Technical support phone numbers.</summary>
        public const string ARPHELPTELEPHONE = "ARPHELPTELEPHONE";

        /// <summary>String displayed by a message box prompting for a disk.</summary>
        public const string DiskPrompt = "DiskPrompt";

        /// <summary>HelpLink</summary>
        public const string HelpLink = "HelpLink";

        /// <summary>HelpTelephone</summary>
        public const string HelpTelephone = "HelpTelephone";

        /// <summary>InstallDate</summary>
        public const string InstallDate = "InstallDate";

        /// <summary>InstallLocation</summary>
        public const string InstallLocation = "InstallLocation";

        /// <summary>InstallSource</summary>
        public const string InstallSource = "InstallSource";

        /// <summary>InstalledProductName</summary>
        public const string InstalledProductName = "InstalledProductName";

        /// <summary>InstanceType</summary>
        public const string InstanceType = "InstanceType";

        /// <summary>Set to 1 if the current installation is running from a package created through an administrative installation.</summary>
        public const string IsAdminPackage = "IsAdminPackage";

        /// <summary>Language</summary>
        public const string Language = "Language";

        /// <summary>LocalPackage</summary>
        public const string LocalPackage = "LocalPackage";

        /// <summary>Places units to the left of the number.</summary>
        public const string LeftUnit = "LeftUnit";

        /// <summary>Name of the application's manufacturer. (Required.)</summary>
        public const string Manufacturer = "Manufacturer";

        /// <summary>The installer sets this property to 1 when the installation uses a media source, such as a CD-ROM.</summary>
        public const string MediaSourceDir = "MediaSourceDir";

        /// <summary>The presence of this property indicates that a product code changing transform is registered to the product.</summary>
        public const string MSIINSTANCEGUID = "MSIINSTANCEGUID";

        /// <summary>This property indicates the installation of a new instance of a product with instance transforms.</summary>
        public const string MSINEWINSTANCE = "MSINEWINSTANCE";

        /// <summary>String used as a template for the PIDKEY property.</summary>
        public const string PIDTemplate = "PIDTemplate";

        /// <summary>A unique identifier for the particular product release. (Required.)</summary>
        public const string ProductCode = "ProductCode";

        /// <summary>ProductIcon</summary>
        public const string ProductIcon = "ProductIcon";

        /// <summary>Human-readable name of the application. (Required.)</summary>
        public const string ProductName = "ProductName";

        /// <summary>Set to the installed state of a product.  (-1 unknown, 1 advertised, 2 absent, 5 default)</summary>
        public const string ProductState = "ProductState";

        /// <summary>String format of the product version as a numeric value. (Required.)</summary>
        public const string ProductVersion = "ProductVersion";

        /// <summary>Publisher</summary>
        public const string Publisher = "Publisher";

        /// <summary>A GUID representing a related set of products.</summary>
        public const string UpgradeCode = "UpgradeCode";

        /// <summary>URLInfoAbout</summary>
        public const string URLInfoAbout = "URLInfoAbout";

        /// <summary>URLUpdateInfo</summary>
        public const string URLUpdateInfo = "URLUpdateInfo";

        /// <summary>Version</summary>
        public const string Version = "Version";

        /// <summary>VersionString</summary>
        public const string VersionString = "VersionString";

        /// <summary>VersionMajor</summary>
        public const string VersionMajor = "VersionMajor";

        /// <summary>VersionMinor</summary>
        public const string VersionMinor = "VersionMinor";
        #endregion	Product Information

        #region	Summary Information Update
        /// <summary>The value of this property is written to the Revision Number Summary Property.</summary>
        public const string PATCHNEWPACKAGECODE = "PATCHNEWPACKAGECODE";

        /// <summary>The value of this property is written to the Comments Summary Property.</summary>
        public const string PATCHNEWSUMMARYCOMMENTS = "PATCHNEWSUMMARYCOMMENTS";

        /// <summary>The value of this property is written to the Subject Summary Property.</summary>
        public const string PATCHNEWSUMMARYSUBJECT = "PATCHNEWSUMMARYSUBJECT";
        #endregion	Summary Information Update

        #region	System Folders
        /// <summary>Full path to the directory containing administrative tools for an individual user.</summary>
        public const string AdminToolsFolder = "AdminToolsFolder";

        /// <summary>Full path to the Application Data folder for the current user.</summary>
        public const string AppDataFolder = "AppDataFolder";

        /// <summary>Full path to application data for all users.</summary>
        public const string CommonAppDataFolder = "CommonAppDataFolder";

        /// <summary>Full path to the predefined 64-bit Common Files folder.</summary>
        public const string CommonFiles64Folder = "CommonFiles64Folder";

        /// <summary>Full path to the Common Files folder for the current user.</summary>
        public const string CommonFilesFolder = "CommonFilesFolder";

        /// <summary>Full path to the Desktop folder.</summary>
        public const string DesktopFolder = "DesktopFolder";

        /// <summary>Full path to the Favorites folder.</summary>
        public const string FavoritesFolder = "FavoritesFolder";

        /// <summary>Full path to the Fonts folder.</summary>
        public const string FontsFolder = "FontsFolder";

        /// <summary>Full path to directory that serves as a data repository for local (nonroaming) applications.</summary>
        public const string LocalAppDataFolder = "LocalAppDataFolder";

        /// <summary>Full path to the My Pictures folder.</summary>
        public const string MyPicturesFolder = "MyPicturesFolder";

        /// <summary>Full path to the Personal folder for the current user.</summary>
        public const string PersonalFolder = "PersonalFolder";

        /// <summary>Full path of the predefined 64-bit Program Files folder.</summary>
        public const string ProgramFiles64Folder = "ProgramFiles64Folder";

        /// <summary>Full path of the predefined 32-bit Program Files folder.</summary>
        public const string ProgramFilesFolder = "ProgramFilesFolder";

        /// <summary>Full path to the Program Menu folder.</summary>
        public const string ProgramMenuFolder = "ProgramMenuFolder";

        /// <summary>Full path to the SendTo folder for the current user.</summary>
        public const string SendToFolder = "SendToFolder";

        /// <summary>Full path to the Start Menu folder.</summary>
        public const string StartMenuFolder = "StartMenuFolder";

        /// <summary>Full path to the Startup folder.</summary>
        public const string StartupFolder = "StartupFolder";

        /// <summary>Full path to folder for 16-bit system DLLs.</summary>
        public const string System16Folder = "System16Folder";

        /// <summary>Full path to folder for 64-bit system DLLs.</summary>
        public const string System64Folder = "System64Folder";

        /// <summary>Full path to folder for 32-bit system DLLs.</summary>
        public const string SystemFolder = "SystemFolder";

        /// <summary>Full path to the Temp folder.</summary>
        public const string TempFolder = "TempFolder";

        /// <summary>Full path to the Template folder for the current user.</summary>
        public const string TemplateFolder = "TemplateFolder";

        /// <summary>Full path to the Windows folder.</summary>
        public const string WindowsFolder = "WindowsFolder";

        /// <summary>The volume of the Windows folder.</summary>
        public const string WindowsVolume = "WindowsVolume";
        #endregion	System Folders

        #region	User Information
        /// <summary>List of properties (separated by semicolons) set during an administration installation.</summary>
        public const string AdminProperties = "AdminProperties";

        /// <summary>Organization of user performing the installation.</summary>
        public const string COMPANYNAME = "COMPANYNAME";

        /// <summary>User name for the user currently logged on.</summary>
        public const string LogonUser = "LogonUser";

        /// <summary>List of properties (separated by semicolonsthat are prevented from being written into the log.</summary>
        public const string MsiHiddenProperties = "MsiHiddenProperties";

        /// <summary>Part of the Product ID entered by user.</summary>
        public const string PIDKEY = "PIDKEY";

        /// <summary>Full Product ID after a successful validation.</summary>
        public const string ProductID = "ProductID";

        /// <summary>Default language identifier of the current user.</summary>
        public const string UserLanguageID = "UserLanguageID";

        /// <summary>User performing the installation.</summary>
        public const string USERNAME = "USERNAME";

        /// <summary>Set by the installer to the user's security identifier (SID).</summary>
        public const string UserSID = "UserSID";
        #endregion	User Information

        /// <summary>AssignmentType</summary>
        public const string AssignmentType = "AssignmentType";

        /// <summary>PackageCode</summary>
        public const string PackageCode = "PackageCode";
        #endregion	Constants (Static Fields)

        #region	Construction / Destruction
        private MsiInstallerProperty() { }
        #endregion	Construction / Destruction
    }
    #endregion	Constants

    #region	Enumerations
    /// <summary>Bit-flags of extra advertisment options.</summary>
    internal enum MsiAdvertismentOptions : uint
    {
        /// <summary>No options.</summary>
        None = 0,
        /// <summary>Multiple instances through product code changing transform support flag.  (Windows Server 2003 family and later and Windows XP Service Pack 1 and later.)</summary>
        Instance = 1,
    }

    /// <summary>Enumeration of MSI assembly types.</summary>
    internal enum MsiAssemblyInfo : uint
    {
        /// <summary>.Net assemblies</summary>
        NetAssembly = 0,
        /// <summary>Win32 assemblies</summary>
        Win32Assembly = 1,
    }

    /// <summary>Enumeration of database persistence modes.</summary>
    internal enum MsiDbPersistMode
    {
        /// <summary>database open read-only, no persistent changes</summary>
        ReadOnly = 0,
        /// <summary>database read/write in transaction mode</summary>
        Transact = 1,
        /// <summary>database direct read/write without transaction</summary>
        Direct = 2,
        /// <summary>create new database, transact mode read/write</summary>
        Create = 3,
        /// <summary>create new database, direct mode read/write</summary>
        CreateDirect = 4,
        /// <summary>database open read-only, no persistent changes</summary>
        PatchFile = 8,
    }

    /// <summary>Enumeration of view column types to return from <see cref="MsiInterop.MsiViewGetColumnInfo"/>.</summary>
    internal enum MsiColInfoType : int
    {
        /// <summary>Column names are returned.</summary>
        Names = 0,
        /// <summary>Definitions are returned.</summary>
        Types = 1,
    }

    /// <summary>Enumeration of MSI conditions.</summary>
    internal enum MsiCondition : int
    {
        /// <summary>expression evaluates to False</summary>
        False = 0,
        /// <summary>expression evaluates to True</summary>
        True = 1,
        /// <summary>no expression present</summary>
        None = 2,
        /// <summary>syntax error in expression</summary>
        Error = 3,
    }


    /// <summary>Bitflags for MSI control attributes.</summary>
    /// <remarks>Please refer to the MSDN Windows Installer documentation for more information.</remarks>
    [Flags]
    internal enum MsiControlAttribute : int
    {
        /// <summary>If the Visible Control bit is set, the control is visible on the dialog box. If this bit is not set, the control is hidden on the dialog box. The visible or hidden state of the Visible control attribute can be later changed by a Control Event.</summary>
        Visible = 0x00000001,
        /// <summary>This attribute specifies if the given control is enabled or disabled. Most controls appear gray when disabled.</summary>
        Enabled = 0x00000002,
        /// <summary>If this bit is set, the control is displayed with a sunken, three dimensional look. The effect of this style bit is different on different controls and versions of Windows. On some controls it has no visible effect. If the system does not support the Sunken control attribute, the control is displayed in the default visual style. If this bit is not set, the control is displayed with the default visual style.</summary>
        Sunken = 0x00000004,
        /// <summary>The Indirect control attribute specifies whether the value displayed or changed by this control is referenced indirectly. If this bit is set, the control displays or changes the value of the property that has the identifier listed in the Property column of the Control table. If this bit is not set, the control displays or changes the value of the property in the Property column of the Control table.</summary>
        Indirect = 0x00000008,
        /// <summary>If this bit is set on a control, the associated property specified in the Property column of the Control table is an integer. If this bit is not set, the property is a string value.</summary>
        Integer = 0x00000010,
        /// <summary>If this style bit is set the text in the control is displayed in a right-to-left reading order.</summary>
        RTLRO = 0x00000020,
        /// <summary>If this style bit is set, text in the control is aligned to the right.</summary>
        RightAligned = 0x00000040,
        /// <summary>If this bit is set, the scroll bar is located on the left side of the control. If this bit is not set, the scroll bar is on the right side of the control.</summary>
        LeftScroll = 0x00000080,
        /// <summary>This is a combination of the right-to-left reading order <see cref="RTLRO"/>, the <see cref="RightAligned"/>, and <see cref="LeftScroll"/> attributes.</summary>
        BiDi = RTLRO | RightAligned | LeftScroll,

        //	text controls

        /// <summary>If the Transparent Control bit is set on a text control, the control is displayed transparently with the background showing through the control where there are no characters. If this bit is not set the text control is opaque.</summary>
        Transparent = 0x00010000,
        /// <summary>If this bit is set on a text control, the occurrence of the ampersand character in a text string is displayed as itself. If this bit is not set, then the character following the ampersand in the text string is displayed as an underscored character.</summary>
        NoPrefix = 0x00020000,
        /// <summary>If this bit is set the text in the control is displayed on a single line. If the text extends beyond the control's margins it is truncated and an ellipsis ("...") is inserted at the end to indicate the truncation. If this bit is not set, text wraps.</summary>
        NoWrap = 0x00040000,
        /// <summary>If this bit is set for a static text control, the control will automatically attempt to format the displayed text as a number representing a count of bytes. For proper formatting, the control's text must be set to a string representing a number expressed in units of 512 bytes. The displayed value will then be formatted in terms of kilobytes (KB), megabytes (MB), or gigabytes (GB), and displayed with the appropriate string representing the units.  Kb = Less than 20480.  Mb = Less than 20971520.  Gb = Less than 10737418240</summary>
        FormatSize = 0x00080000,
        /// <summary>If this bit flag is set, fonts are created using the user's default UI code page. If the bit flag is not set, fonts are created using the database code page.</summary>
        UsersLanguage = 0x00100000,

        //	edit controls

        /// <summary>If this bit is set on an Edit control, the installer creates a multiple line edit control with a vertical scroll bar.</summary>
        MultiLine = 0x00001000,
        /// <summary>PasswordInput</summary>
        PasswordInput = 0x00200000,

        //	progress bar

        /// <summary>If this bit is set on a ProgressBar control, the bar is drawn as a series of small rectangles in Microsoft Windows 95-style. If this bit is not set, the progress indicator bar is drawn as a single continuous rectangle.</summary>
        Progress95 = 0x00001000,

        //	volume select combo and directory combo

        /// <summary>If this bit is set, the control shows all the volumes involved in the current installation plus all the removable volumes. If this bit is not set, the control lists volumes in the current installation.</summary>
        RemovableVolume = 0x00010000,
        /// <summary>If the FixedVolume Control bit is set, the control shows all the volumes involved in the current installation plus all the fixed internal hard drives. If this bit is not set, the control lists the volumes in the current installation.</summary>
        FixedVolume = 0x00020000,
        /// <summary>If this bit is set, the control shows all the volumes involved in the current installation plus all the remote volumes. If this bit is not set, the control lists volumes in the current installation.</summary>
        RemoteVolume = 0x00040000,
        /// <summary>If the CDROMVolume Control bit is set, the control shows all the volumes in the current installation plus all the CD-ROM volumes. If this bit is not set, the control shows all the volumes in the current installation.</summary>
        CDRomVolume = 0x00080000,
        /// <summary>If this bit is set, the control shows all the volumes involved in the current installation plus all the RAM disk volumes. If this bit is not set the control lists volumes in the current installation.</summary>
        RAMDiskVolume = 0x00100000,
        /// <summary>If the FloppyVolume Control bit is set, the control shows all the volumes involved in the current installation plus all the floppy volumes. If this bit is not set, the control lists volumes in the current installation. </summary>
        FloppyVolume = 0x00200000,

        //	volume list controls

        /// <summary>ShowRollbackCost</summary>
        ShowRollbackCost = 0x00400000,

        //	list box / combo box
        /// <summary>If this bit is set, the items listed in the control are displayed in a specified order. If the bit is not set, items are displayed in alphabetical order.</summary>
        Sorted = 0x00010000,
        /// <summary>If the ComboList Control bit is set on a combo box, the edit field is replaced by a static text field. This prevents a user from entering a new value and requires the user to choose only one of the predefined values. If this bit is not set, the combo box has an edit field.</summary>
        ComboList = 0x00020000,

        //	picture buttons

        /// <summary>ImageHandle</summary>
        ImageHandle = 0x00010000,
        /// <summary>If this bit is set on a check box or a radio button group, the button is drawn with the appearance of a push button, but its logic stays the same. If the bit is not set, the controls are drawn in their usual style.</summary>
        PushLike = 0x00020000,
        /// <summary>If the Bitmap Control bit is set, the text in the control is replaced by a bitmap image. The Text column in the Control table is a foreign key into the Binary table. If this bit is not set, the text in the control is specified in the Text column of the Control table.</summary>
        Bitmap = 0x00040000,
        /// <summary>If this bit is set, text is replaced by an icon image and the Text column in the Control table is a foreign key into the Binary table. If this bit is not set, text in the control is specified in the Text column of the Control table.</summary>
        Icon = 0x00080000,
        /// <summary>If the FixedSize Control bit is set, the picture is cropped or centered in the control without changing its shape or size. If this bit is not set the picture is stretched to fit the control.</summary>
        FixedSize = 0x00100000,
        /// <summary>The first 16x16 image is loaded.</summary>
        IconSize16 = 0x00200000,
        /// <summary>The first 32x32 image is loaded.</summary>
        IconSize32 = 0x00400000,
        /// <summary>The first 48x48 image is loaded.</summary>
        IconSize48 = 0x00600000,

        //	radio buttons
        /// <summary>If this bit is set, the RadioButtonGroup has text and a border displayed around it. If the style bit is not set, the border is not displayed and no text is displayed on the group.</summary>
        HasBorder = 0x01000000,
    }

    /// <summary>Enumeration of MSI feature cost tree options.</summary>
    internal enum MsiCostTree : int
    {
        /// <summary>The feature only is included in the cost.</summary>
        SelfOnly = 0,
        /// <summary>The children of the indicated feature are included in the cost.</summary>
        Children = 1,
        /// <summary>The parent features of the indicated feature are included in the cost.</summary>
        Parents = 2,
        /// <summary>Reserved for future use.</summary>
        Reserved = 3,
    }

    /// <summary>Enumeration of custom action types.</summary>
    internal enum MsiCustomActionType : int
    {
        // executable types

        /// <summary>Target = entry point name</summary>
        Dll = 0x00000001,
        /// <summary>Target = command line args</summary>
        Exe = 0x00000002,
        /// <summary>Target = text string to be formatted and set into property</summary>
        TextData = 0x00000003,
        /// <summary>Target = entry point name, null if none to call</summary>
        JScript = 0x00000005,
        /// <summary>Target = entry point name, null if none to call</summary>
        VBScript = 0x00000006,
        /// <summary>Target = property list for nested engine initialization</summary>
        Install = 0x00000007,

        // source of code

        /// <summary>Source = Binary.Name, data stored in stream</summary>
        BinaryData = 0x00000000,
        /// <summary>Source = File.File, file part of installation</summary>
        SourceFile = 0x00000010,
        /// <summary>Source = Directory.Directory, folder containing existing file</summary>
        Directory = 0x00000020,
        /// <summary>Source = Property.Property, full path to executable</summary>
        Property = 0x00000030,

        // return processing default is syncronous execution, process return code

        /// <summary>ignore action return status, continue running</summary>
        Continue = 0x00000040,
        /// <summary>run asynchronously</summary>
        Async = 0x00000080,

        // execution scheduling flags  default is execute whenever sequenced

        /// <summary>skip if UI sequence already run</summary>
        FirstSequence = 0x00000100,
        /// <summary>skip if UI sequence already run in same process</summary>
        OncePerProcess = 0x00000200,
        /// <summary>run on client only if UI already run on client</summary>
        ClientRepeat = 0x00000300,
        /// <summary>queue for execution within script</summary>
        InScript = 0x00000400,
        /// <summary>in conjunction with InScript: queue in Rollback script</summary>
        Rollback = 0x00000100,
        /// <summary>in conjunction with InScript: run Commit ops from script on success</summary>
        Commit = 0x00000200,

        // security context flag, default to impersonate as user, valid only if InScript

        /// <summary>no impersonation, run in system context</summary>
        NoImpersonate = 0x00000800,
        /// <summary>impersonate for per-machine installs on TS machines</summary>
        TSAware = 0x00004000,

        // script requires 64bit process

        /// <summary>script should run in 64bit process</summary>
        Type64BitScript = 0x00001000,
        /// <summary>don't record the contents of the Target field in the log file.</summary>
        HideTarget = 0x00002000,
    }

    /// <summary>Enumeration of MSI database errors.</summary>
    internal enum MsiDbError : int
    {
        /// <summary>invalid argument</summary>
        InvalidArg = -3,
        /// <summary>buffer too small</summary>
        MoreData = -2,
        /// <summary>function error</summary>
        FunctionError = -1,
        /// <summary>no error</summary>
        NoError = 0,
        /// <summary>new record duplicates primary keys of existing record in table</summary>
        DuplicateKey = 1,
        /// <summary>non-nullable column, no null values allowed</summary>
        Required = 2,
        /// <summary>corresponding record in foreign table not found</summary>
        BadLink = 3,
        /// <summary>data greater than maximum value allowed</summary>
        Overflow = 4,
        /// <summary>data less than minimum value allowed</summary>
        Underflow = 5,
        /// <summary>data not a member of the values permitted in the set</summary>
        NotInSet = 6,
        /// <summary>invalid version string</summary>
        BadVersion = 7,
        /// <summary>invalid case, must be all upper-case or all lower-case</summary>
        BadCase = 8,
        /// <summary>invalid GUID</summary>
        BadGUID = 9,
        /// <summary>invalid wildcardfilename or use of wildcards</summary>
        BadWildcard = 10,
        /// <summary>bad identifier</summary>
        BadIdentifier = 11,
        /// <summary>bad language Id(s)</summary>
        BadLanguage = 12,
        /// <summary>bad filename</summary>
        BadFilename = 13,
        /// <summary>bad path</summary>
        BadPath = 14,
        /// <summary>bad conditional statement</summary>
        BadCondition = 15,
        /// <summary>bad format string</summary>
        BadFormatted = 16,
        /// <summary>bad template string</summary>
        BadTemplate = 17,
        /// <summary>bad string in DefaultDir column of Directory table</summary>
        BadDefaultDir = 18,
        /// <summary>bad registry path string</summary>
        BadRegPath = 19,
        /// <summary>bad string in CustomSource column of CustomAction table</summary>
        BadCustomSource = 20,
        /// <summary>bad property string</summary>
        BadProperty = 21,
        /// <summary>_Validation table missing reference to column</summary>
        MissingData = 22,
        /// <summary>Category column of _Validation table for column is invalid</summary>
        BadCategory = 23,
        /// <summary>table in KeyTable column of _Validation table could not be found/loaded</summary>
        BadKeyTable = 24,
        /// <summary>value in MaxValue column of _Validation table is less than value in MinValue column</summary>
        BadMaxMinValues = 25,
        /// <summary>bad cabinet name</summary>
        BadCabinet = 26,
        /// <summary>bad shortcut target</summary>
        BadShortcut = 27,
        /// <summary>string overflow (greater than length allowed in column def)</summary>
        StringOverflow = 28,
        /// <summary>invalid localization attribute (primary keys cannot be localized)</summary>
        BadLocalizedAttrib = 29,
    }

    /// <summary>Enumeration of MSI database states.</summary>
    internal enum MsiDbState : int
    {
        /// <summary>invalid database handle</summary>
        Error = -1,
        /// <summary>database open read-only, no persistent changes</summary>
        Read = 0,
        /// <summary>database readable and updatable</summary>
        Write = 1,
    }

    /// <summary>Bitflags for MSI dialogs.</summary>
    /// <remarks>Please refer to the MSDN Windows Installer documentation for more information.</remarks>
    [Flags]
    internal enum MsiDialogStyle : int
    {
        /// <summary>If this bit is set the dialog is originally created as visible, otherwise it is hidden.</summary>
        Visible = 1,
        /// <summary>If this bit is set, the dialog box is modal, other dialogs of the same application cannot be put on top of it, and the dialog keeps the control while it is running. If this bit is not set, the dialog is modeless, other dialogs of the same application may be moved on top of it. After a modeless dialog is created and displayed, the user interface returns control to the installer. The installer then calls the user interface periodically to update the dialog and to give it a chance to process the messages. As soon as this is done, the control is returned to the installer.  <b>Note</b>  There should be no modeless dialogs in a wizard sequence, since this would return control to the installer, ending the wizard sequence prematurely.</summary>
        Modal = 2,
        /// <summary>If this bit is set, the dialog box can be minimized. This bit is ignored for modal dialog boxes, which cannot be minimized.</summary>
        Minimize = 4,
        /// <summary>If this style bit is set, the dialog box will stop all other applications and no other applications can take the focus. This state remains until the SysModal dialog is dismissed.</summary>
        SysModal = 8,
        /// <summary>Normally, when this bit is not set and a dialog box is created through DoAction, all other (typically modeless) dialogs are destroyed. If this bit is set, the other dialogs stay alive when this dialog box is created.</summary>
        KeepModeless = 16,
        /// <summary>If this bit is set, the dialog box periodically calls the installer. If the property changes, it notifies the controls on the dialog. This style can be used if there is a control on the dialog indicating disk space. If the user switches to another application, adds or removes files, or otherwise modifies available disk space, you can quickly implement the change using this style. Any dialog box relying on the OutOfDiskSpace property to determine whether to bring up a dialog must set the TrackDiskSpace Dialog Style Bit for the dialog to dynamically update space on the target volumes.</summary>
        TrackDiskSpace = 32,
        /// <summary>If this bit is set, the pictures on the dialog box are created with the custom palette (one per dialog received from the first control created). If the bit is not set, the pictures are rendered using a default palette.</summary>
        UseCustomPalette = 64,
        /// <summary>If this style bit is set the text in the dialog box is displayed in right-to-left-reading order.</summary>
        RTLRO = 128,
        /// <summary>If this style bit is set, the text is aligned on the right side of the dialog box.</summary>
        RightAligned = 256,
        /// <summary>If this style bit is set, the scroll bar is located on the left side of the dialog box.</summary>
        LeftScroll = 512,
        /// <summary>This is a combination of the right to left reading order <see cref="RTLRO"/>, the <see cref="RightAligned"/>, and the <see cref="LeftScroll"/> dialog style bits.</summary>
        BiDi = 896,
        /// <summary>If this bit is set, the dialog box is an error dialog.</summary>
        Error = 65536,
    }

    /// <summary>Internal enumeration of Win32/MSI errors.</summary>
    internal enum MsiError : uint
    {
        /// <summary>No error occured.</summary>
        NoError = 0,
        /// <summary>The operation was successful.</summary>
        Success = 0,
        /// <summary>The system cannot find the file specified.</summary>
        FileNotFound = 2,
        /// <summary>Access is denied.</summary>
        AccessDenied = 5,
        /// <summary>The handle is invalid.</summary>
        InvalidHandle = 6,
        /// <summary>Not enough storage is available to process this command.</summary>
        NotEnoughMemory = 8,
        /// <summary>The data is invalid.</summary>
        InvalidData = 13,
        /// <summary>Not enough storage is available to complete this operation.</summary>
        OutOfMemory = 14,
        /// <summary>The parameter is incorrect.</summary>
        InvalidParameter = 87,
        /// <summary>The system cannot open the device or file specified.</summary>
        OpenFailed = 110,
        /// <summary>There is not enough space on the disk.</summary>
        DiskFull = 112,
        /// <summary>This function is not available for this platform. It is only available on Windows 2000 and Windows XP with Window Installer version 2.0.</summary>
        CallNotImplemented = 120,
        /// <summary>The specified path is invalid.</summary>
        BadPathName = 161,
        /// <summary>No data is available.</summary>
        NoData = 232,
        /// <summary>More data is available.</summary>
        MoreData = 234,
        /// <summary>No more data is available.</summary>
        NoMoreItems = 259,
        /// <summary>The directory name is invalid.</summary>
        Directory = 267,
        /// <summary>The volume for a file has been externally altered so that the opened file is no longer valid.</summary>
        FileInvalid = 1006,
        /// <summary>This error code only occurs when using Windows Installer version 2.0 and Windows XP. If Windows Installer determines a product may be incompatible with the current operating system, it displays a dialog box informing the user and asking whether to try to install anyway. This error code is returned if the user chooses not to try the installation.</summary>
        AppHelpBlock = 1259,
        /// <summary>The Windows Installer service could not be accessed.</summary>
        InstallServiceFailure = 1601,
        /// <summary>The user cancels installation.</summary>
        InstallUserExit = 1602,
        /// <summary>A fatal error occurred during installation.</summary>
        InstallFailure = 1603,
        /// <summary>Installation suspended, incomplete.</summary>
        InstallSuspend = 1604,
        /// <summary>This action is only valid for products that are currently installed.</summary>
        UnknownProduct = 1605,
        /// <summary>The feature identifier is not registered.</summary>
        UnknownFeature = 1606,
        /// <summary>The component identifier is not registered.</summary>
        UnknownComponent = 1607,
        /// <summary>This is an unknown property.</summary>
        UnknownProperty = 1608,
        /// <summary>The handle is in an invalid state.</summary>
        InvalidHandleState = 1609,
        /// <summary>The configuration data for this product is corrupt.</summary>
        BadConfiguration = 1610,
        /// <summary>The component qualifier not present.</summary>
        IndexAbsent = 1611,
        /// <summary>The installation source for this product is not available. Verify that the source exists and that you can access it.</summary>
        InstallSourceAbsent = 1612,
        /// <summary>This installation package cannot be installed by the Windows Installer service. You must install a Windows service pack that contains a newer version of the Windows Installer service.</summary>
        InstallPackageVersion = 1613,
        /// <summary>The product is uninstalled.</summary>
        ProductUninstalled = 1614,
        /// <summary>The SQL query syntax is invalid or unsupported.</summary>
        BadQuerySyntax = 1615,
        /// <summary>The record field does not exist.</summary>
        InvalidField = 1616,
        /// <summary>Another installation is already in progress.</summary>
        InstallAlreadyRunning = 1618,
        /// <summary>This installation package could not be opened. Verify that the package exists and is accessible, or contact the application vendor to verify that this is a valid Windows Installer package.</summary>
        InstallPackageOpenFailed = 1619,
        /// <summary>This installation package could not be opened. Contact the application vendor to verify that this is a valid Windows Installer package.</summary>
        InstallPackageInvalid = 1620,
        /// <summary>There was an error starting the Windows Installer service user interface. </summary>
        InstallUIFailure = 1621,
        /// <summary>There was an error opening installation log file. Verify that the specified log file location exists and is writable.</summary>
        InstallLogFailure = 1622,
        /// <summary>This language of this installation package is not supported by your system.</summary>
        InstallLanguageUnsupported = 1623,
        /// <summary>There was an error applying transforms. Verify that the specified transform paths are valid.</summary>
        InstallTransformFailure = 1624,
        /// <summary>This installation is forbidden by system policy.</summary>
        InstallPackageRejected = 1625,
        /// <summary>The function could not be executed.</summary>
        FunctionNotCalled = 1626,
        /// <summary>The function failed during execution.</summary>
        FunctionFailed = 1627,
        /// <summary>An invalid or unknown table was specified.</summary>
        InvalidTable = 1628,
        /// <summary>The data supplied is the wrong type.</summary>
        DatatypeMismatch = 1629,
        /// <summary>Data of this type is not supported.</summary>
        UnsupportedType = 1630,
        /// <summary>The Windows Installer service failed to start.</summary>
        CreateFailed = 1631,
        /// <summary>The Temp folder is either full or inaccessible. Verify that the Temp folder exists and that you can write to it.</summary>
        InstallTempUnwritable = 1632,
        /// <summary>This installation package is not supported on this platform.</summary>
        InstallPlatformUnsupported = 1633,
        /// <summary>Component is not used on this machine.</summary>
        InstallNotUsed = 1634,
        /// <summary>This patch package could not be opened.</summary>
        PatchPackageOpenFailed = 1635,
        /// <summary>This patch package could not be opened</summary>
        PatchPackageInvalid = 1636,
        /// <summary>This patch package cannot be processed by the Windows Installer service.</summary>
        PatchPackageUnsupported = 1637,
        /// <summary>Another version of this product is already installed. Installation of this version cannot continue. To configure or remove the existing version of this product, use Add/Remove Programs in Control Panel.</summary>
        ProductVersion = 1638,
        /// <summary>Invalid command line argument. Consult the Windows Installer SDK for detailed command-line help.</summary>
        InvalidCommandLine = 1639,
        /// <summary>Installation from a Terminal Server client session is not permitted for the current user.</summary>
        InstallRemoteDisallowed = 1640,
        /// <summary>The installer has initiated a restart. This error code is not available on Windows Installer version 1.0.</summary>
        SuccessRebootInitiated = 1641,
        /// <summary>The installer cannot install the upgrade patch because the program being upgraded may be missing or the upgrade patch updates a different version of the program. Verify that the program to be upgraded exists on your computer and that you have the correct upgrade patch. This error code is not available on Windows Installer version 1.0.</summary>
        PatchTargetNotFound = 1642,
        /// <summary>The patch package is not permitted by system policy. This error code is available with Windows Installer versions 2.0.</summary>
        InstallTransformRejected = 1643,
        /// <summary>One or more customizations are not permitted by system policy. This error code is available with Windows Installer versions 2.0.</summary>
        InstallRemoteProhibited = 1644,
        /// <summary>The specified datatype is invalid.</summary>
        InvalidDataType = 1804,
        /// <summary>The specified username is invalid.</summary>
        BadUserName = 2202,
        /// <summary>A restart is required to complete the install. This does not include installs where the ForceReboot action is run. This error code is not available on Windows Installer version 1.0.</summary>
        SucessRebootRequired = 3010,
        /// <summary>Unspecified error.</summary>
        E_Fail = 0x80004005,
    }

    /// <summary>Bit flags install feature attributes enumeration.</summary>
    [Flags]
    internal enum MsiInstallFeatureAttribute : int
    {
        /// <summary>FavorLocal</summary>
        FavorLocal = (1 << 0),
        /// <summary>FavorSource</summary>
        FavorSource = (1 << 1),
        /// <summary>FollowParent</summary>
        FollowParent = (1 << 2),
        /// <summary>FavorAdvertise</summary>
        FavorAdvertise = (1 << 3),
        /// <summary>DisallowAdvertise</summary>
        DisallowAdvertise = (1 << 4),
        /// <summary>NounSupportedAdvertise</summary>
        NounSupportedAdvertise = (1 << 5),
        /// <summary>All attributes.</summary>
        All = FavorLocal | FavorSource | FollowParent | FavorAdvertise | DisallowAdvertise,
    }

    /// <summary>
    /// Bit-flags defining an MSI feature's install state.
    /// </summary>
    [Flags]
    internal enum MsiFeatureInstallState : int
    {
        /// <summary>The feature can be advertised.</summary>
        Advertised = 2,
        /// <summary>The feature can be absent.</summary>
        Absent = 4,
        /// <summary>The feature can be installed on the local drive.</summary>
        Local = 8,
        /// <summary>The feature can be configured to run from source, CD-ROM, or network.</summary>
        Source = 16,
        /// <summary>The feature can be configured to use the default location: local or source.</summary>
        Default = 32,
    }

    /// <summary>Enumeration of MSI install levels.</summary>
    internal enum MsiInstallLevel : int
    {
        /// <summary>install authored default</summary>
        Default = 0,
        /// <summary>install only required features</summary>
        Minimum = 1,
        /// <summary>install all features</summary>
        Maximum = 0xffff,
    }

    /// <summary>Bit flags for use with <see cref="MsiInterop.MsiEnableLog"/> and <see cref="MsiInterop.MsiSetExternalUI"/>.</summary>
    [Flags]
    internal enum MsiInstallLogMode : uint
    {
        /// <summary>None.</summary>
        None = 0,
        /// <summary>Logs out of memory or fatal exit information.</summary>
        FatalExit = (1 << (int)(MsiInstallMessage.FatalExit >> 24)),
        /// <summary>Logs the error messages.</summary>
        Error = (1 << (int)(MsiInstallMessage.Error >> 24)),
        /// <summary>Logs the warning messages.</summary>
        Warning = (1 << (int)(MsiInstallMessage.Warning >> 24)),
        /// <summary>Logs the user requests.</summary>
        User = (1 << (int)(MsiInstallMessage.User >> 24)),
        /// <summary>Logs the status messages that are not displayed.</summary>
        Info = (1 << (int)(MsiInstallMessage.Info >> 24)),
        /// <summary>Request to determine a valid source location.</summary>
        ResolveSource = (1 << (int)(MsiInstallMessage.ResolveSource >> 24)),
        /// <summary>Indicates insufficient disk space.</summary>
        OutOfDiskSpace = (1 << (int)(MsiInstallMessage.OutOfDiskSpace >> 24)),
        /// <summary>Logs the start of new installation actions.</summary>
        ActionStart = (1 << (int)(MsiInstallMessage.ActionStart >> 24)),
        /// <summary>Logs the data record with the installation action.</summary>
        ActionData = (1 << (int)(MsiInstallMessage.ActionData >> 24)),
        /// <summary>Logs the parameters for user-interface initialization.</summary>
        CommonData = (1 << (int)(MsiInstallMessage.CommonData >> 24)),
        /// <summary>Logs the property values at termination.</summary>
        PropertyDump = (1 << (int)(MsiInstallMessage.Progress >> 24)),
        /// <summary>Sends large amounts of information to a log file not generally useful to users. May be used for technical support.</summary>
        Verbose = (1 << (int)(MsiInstallMessage.Initialize >> 24)),
        /// <summary>Sends extra debugging information, such as handle creation information, to the log file. <b>Windows XP/2000/98/95:  This feature is not supported.</b></summary>
        ExtraDebug = (1 << (int)(MsiInstallMessage.Terminate >> 24)),
        /// <summary>external handler only</summary>
        Progress = (1 << (int)(MsiInstallMessage.Progress >> 24)),
        /// <summary>external handler only</summary>
        Initialize = (1 << (int)(MsiInstallMessage.Initialize >> 24)),
        /// <summary>external handler only</summary>
        Terminate = (1 << (int)(MsiInstallMessage.Terminate >> 24)),
        /// <summary>external handler only</summary>
        ShowDialog = (1 << (int)(MsiInstallMessage.ShowDialog >> 24)),
        /// <summary>All modes.</summary>
        ExternalUI = FatalExit | Error | Warning | User | ActionStart | ActionData |
            CommonData | Progress | ShowDialog,
    }

    /// <summary>
    /// <para>Install message type for callback is a combination of the following:</para>
    /// <para>A message box style:  MB_*, where MB_OK is the default</para>
    /// <para>A message box icon type:  MB_ICON*, where no icon is the default</para>
    /// <para>A default button:  MB_DEFBUTTON?, where MB_DEFBUTTON1 is the default</para>
    /// <para>One of these flags an install message, no default.</para>
    /// </summary>
    internal enum MsiInstallMessage : long
    {
        /// <summary>premature termination, possibly fatal OOM</summary>
        FatalExit = 0x00000000,
        /// <summary>formatted error message</summary>
        Error = 0x01000000,
        /// <summary>formatted warning message</summary>
        Warning = 0x02000000,
        /// <summary>user request message</summary>
        User = 0x03000000,
        /// <summary>informative message for log</summary>
        Info = 0x04000000,
        /// <summary>list of files in use that need to be replaced</summary>
        FilesInUse = 0x05000000,
        /// <summary>request to determine a valid source location</summary>
        ResolveSource = 0x06000000,
        /// <summary>insufficient disk space message</summary>
        OutOfDiskSpace = 0x07000000,
        /// <summary>start of action: action name and description</summary>
        ActionStart = 0x08000000,
        /// <summary>formatted data associated with individual action item</summary>
        ActionData = 0x09000000,
        /// <summary>progress gauge info: units so far, total</summary>
        Progress = 0x0a000000,
        /// <summary>product info for dialog: language Id, dialog caption</summary>
        CommonData = 0x0b000000,
        /// <summary>sent prior to UI initialization, no string data</summary>
        Initialize = 0x0c000000,
        /// <summary>sent after UI termination, no string data</summary>
        Terminate = 0x0d000000,
        /// <summary>sent prior to display or authored dialog or wizard</summary>
        ShowDialog = 0x0e000000,
    }

    /// <summary>Enumeration of installation modes.</summary>
    internal enum MsiInstallMode : int
    {
        /// <summary>skip source resolution</summary>
        NoSourceResolution = -3,
        /// <summary>skip detection</summary>
        NoDetection = -2,
        /// <summary>provide, if available</summary>
        Existing = -1,
        /// <summary>install, if absent</summary>
        Default = 0
    }

    /// <summary>Enumeration of MSI install states.</summary>
    internal enum MsiInstallState : int
    {
        /// <summary>component disabled</summary>
        NotUsed = -7,
        /// <summary>configuration data corrupt</summary>
        BadConfig = -6,
        /// <summary>installation suspended or in progress</summary>
        Incomplete = -5,
        /// <summary>run from source, source is unavailable</summary>
        SourceAbsent = -4,
        /// <summary>return buffer overflow</summary>
        MoreData = -3,
        /// <summary>invalid function argument</summary>
        InvalidArg = -2,
        /// <summary>unrecognized product or feature</summary>
        Unknown = -1,
        /// <summary>broken</summary>
        Broken = 0,
        /// <summary>advertised feature</summary>
        Advertised = 1,
        /// <summary>component being removed (action state, not settable)</summary>
        Removed = 1,
        /// <summary>uninstalled (or action state absent but clients remain)</summary>
        Absent = 2,
        /// <summary>installed on local drive</summary>
        Local = 3,
        /// <summary>run from source, CD or net</summary>
        Source = 4,
        /// <summary>use default, local or source</summary>
        Default = 5,
    }

    /// <summary>Enumeration of various MSI install types.</summary>
    internal enum MsiInstallType : int
    {
        /// <summary>set to indicate default behavior</summary>
        Default = 0,
        /// <summary>set to indicate network install</summary>
        NetworkImage = 1,
        /// <summary>set to indicate a particular instance</summary>
        SingleInstance = 2,
    }

    /// <summary>Enumeration of internal MSI install UI levels.</summary>
    internal enum MsiInstallUILevel : uint
    {
        /// <summary>UI level is unchanged</summary>
        NoChange = 0,
        /// <summary>default UI is used</summary>
        Default = 1,
        /// <summary>completely silent installation</summary>
        None = 2,
        /// <summary>simple progress and error handling</summary>
        Basic = 3,
        /// <summary>authored UI, wizard dialogs suppressed</summary>
        Reduced = 4,
        /// <summary>authored UI with wizards, progress, errors</summary>
        Full = 5,
        /// <summary>display success/failure dialog at end of install</summary>
        EndDialog = 0x80,
        /// <summary>display only progress dialog</summary>
        ProgressOnly = 0x40,
        /// <summary>do not display the cancel button in basic UI</summary>
        HideCancel = 0x20,
        /// <summary>force display of source resolution even if quiet</summary>
        SourceResOnly = 0x100,
    }

    /// <summary>Flag attributes for <see cref="MsiInterop.MsiEnableLog"/>.</summary>
    internal enum MsiLogAttribute : int
    {
        /// <summary>If this value is set, the installer appends the existing log.</summary>
        Append = (1 << 0),
        /// <summary>Forces the log buffer to be flushed after each line.</summary>
        FlushEachLine = (1 << 1),
    }

    /// <summary>Enumeration of modification modes for <see cref="MsiInterop.MsiViewModify"/>.</summary>
    internal enum MsiModify : int
    {
        /// <summary>reposition to current record primary key</summary>
        Seek = -1,
        /// <summary>refetch current record data</summary>
        Refresh = 0,
        /// <summary>insert new record, fails if matching key exists</summary>
        Insert = 1,
        /// <summary>update existing non-key data of fetched record</summary>
        Update = 2,
        /// <summary>insert record, replacing any existing record</summary>
        Assign = 3,
        /// <summary>update record, delete old if primary key edit</summary>
        Replace = 4,
        /// <summary>fails if record with duplicate key not identical</summary>
        Merge = 5,
        /// <summary>remove row referenced by this record from table</summary>
        Delete = 6,
        /// <summary>insert a temporary record</summary>
        InsertTemporary = 7,
        /// <summary>validate a fetched record</summary>
        Validate = 8,
        /// <summary>validate a new record</summary>
        ValidateNew = 9,
        /// <summary>validate field(s) of an incomplete record</summary>
        ValidateField = 10,
        /// <summary>validate before deleting record</summary>
        ValidateDelete = 11,
    }

    /// <summary>
    /// Possible flags for the <c>options</c> parameter of <see cref="MsiInterop.MsiOpenPackageEx"/>.
    /// </summary>
    internal enum MsiOpenPackageFlags : uint
    {
        /// <summary>No options.</summary>
        None = 0,
        /// <summary>Ignore the computer state when creating the product handle.</summary>
        IgnoreMachineState = 1,
    }

    /// <summary>Bit-flags for reinstallation.</summary>
    [Flags]
    internal enum MsiReinstallMode : uint
    {
        /// <summary>Reserved bit - currently ignored</summary>
        Repair = 0x00000001,
        /// <summary>Reinstall only if file is missing</summary>
        FileMissing = 0x00000002,
        /// <summary>Reinstall if file is missing, or older version</summary>
        FileOlderVersion = 0x00000004,
        /// <summary>Reinstall if file is missing, or equal or older version</summary>
        FileEqualVersion = 0x00000008,
        /// <summary>Reinstall if file is missing, or not exact version</summary>
        FileExact = 0x00000010,
        /// <summary>checksum executables, reinstall if missing or corrupt</summary>
        FileVerify = 0x00000020,
        /// <summary>Reinstall all files, regardless of version</summary>
        FileReplace = 0x00000040,
        /// <summary>insure required machine reg entries</summary>
        MachineData = 0x00000080,
        /// <summary>insure required user reg entries</summary>
        UserData = 0x00000100,
        /// <summary>validate shortcuts items</summary>
        Shortcut = 0x00000200,
        /// <summary>use re-cache source install package</summary>
        Package = 0x00000400,
    }

    /// <summary>Enumeration of MSI run modes.</summary>
    internal enum MsiRunMode : int
    {
        /// <summary>admin mode install, else product install</summary>
        Admin = 0,
        /// <summary>installing advertisements, else installing or updating product</summary>
        Advertise = 1,
        /// <summary>modifying an existing installation, else new installation</summary>
        Maintenance = 2,
        /// <summary>rollback is enabled</summary>
        RollbackEnabled = 3,
        /// <summary>log file active, enabled prior to install session</summary>
        LogEnabled = 4,
        /// <summary>spooling execute operations, else in determination phase</summary>
        Operations = 5,
        /// <summary>reboot needed after successful installation (settable)</summary>
        RebootAtEnd = 6,
        /// <summary>reboot needed to continue installation (settable)</summary>
        RebootNow = 7,
        /// <summary>installing files from cabinets and files using Media table</summary>
        Cabinet = 8,
        /// <summary>source LongFileNames suppressed via PID_MSISOURCE summary property</summary>
        SourceShortNames = 9,
        /// <summary>target LongFileNames suppressed via SHORTFILENAMES property</summary>
        TargetShortNames = 10,
        /// <summary>future use</summary>
        Reserved11 = 11,
        /// <summary>operating systems is Windows9?, else Windows NT</summary>
        Windows9x = 12,
        /// <summary>operating system supports demand installation</summary>
        ZawEnabled = 13,
        /// <summary>future use</summary>
        Reserved14 = 14,
        /// <summary>future use</summary>
        Reserved15 = 15,
        /// <summary>custom action call from install script execution</summary>
        Scheduled = 16,
        /// <summary>custom action call from rollback execution script</summary>
        Rollback = 17,
        /// <summary>custom action call from commit execution script</summary>
        Commit = 18,
    }

    /// <summary>Bit-flag error conditions that should be suppressed when the transform is applied.</summary>
    [Flags]
    internal enum MsiTransformError : int
    {
        /// <summary>None of the following conditions.</summary>
        None = 0x00000000,
        /// <summary>Adding a row that already exists.</summary>
        AddExistingRow = 0x00000001,
        /// <summary>Deleting a row that doesn't exist.</summary>
        DelMissingRow = 0x00000002,
        /// <summary>Adding a table that already exists.</summary>
        AddExistingTable = 0x00000004,
        /// <summary>Deleting a table that doesn't exist.</summary>
        DelMissingTable = 0x00000008,
        /// <summary>Updating a row that doesn't exist.</summary>
        UpdateMissingRow = 0x00000010,
        /// <summary>Transform and database code pages do not match and neither code page is neutral.</summary>
        ChangeCodePage = 0x00000020,
        /// <summary>Create the temporary _TransformView table.</summary>
        ViewTransform = 0x00000100,
    }

    /// <summary>Enumeration of MSI user states.</summary>
    internal enum MsiUserInfoState : int
    {
        /// <summary>return buffer overflow</summary>
        MoreData = -3,
        /// <summary>invalid function argument</summary>
        InvalidArg = -2,
        /// <summary>unrecognized product</summary>
        Unknown = -1,
        /// <summary>user info and PID not initialized</summary>
        Absent = 0,
        /// <summary>user info and PID initialized</summary>
        Present = 1,
    }

    /// <summary>Bit-flags for MSI validation.</summary>
    [Flags]
    internal enum MsiValidationFlag : int
    {
        /// <summary>Validate no properties.</summary>
        None = 0x00000000,
        /// <summary>Default language must match base database.</summary>
        Language = 0x00000001,
        /// <summary>Product must match base database.</summary>
        Product = 0x00000002,
        /// <summary>Check major version only.</summary>
        MajorVersion = 0x00000008,
        /// <summary>Check major and minor versions only.</summary>
        MinorVersion = 0x00000010,
        /// <summary>Check major, minor, and update versions.</summary>
        UpdateVersion = 0x00000020,
        /// <summary>Installed version &lt; base version.</summary>
        NewLessBaseVersion = 0x00000040,
        /// <summary>Installed version &lt;= base version.</summary>
        NewLessEqualBaseVersion = 0x00000080,
        /// <summary>Installed version = base version.</summary>
        NewEqualBaseVersion = 0x00000100,
        /// <summary>Installed version &gt;= base version.</summary>
        NewGreaterEqualBaseVersion = 0x00000200,
        /// <summary>Installed version &gt; base version.</summary>
        NewGreaterBaseVersion = 0x00000400,
        /// <summary>UpgradeCode must match base database.</summary>
        UpgradeCode = 0x00000800,
    }

    /// <summary>Bit-flags of platform architectures.</summary>
    [Flags]
    internal enum PlatformArchitecture : uint
    {
        /// <summary>The current platform.</summary>
        Current = 0,
        /// <summary>The x86 platform.</summary>
        X86 = 1,
        /// <summary>The ia64 platform.</summary>
        IA64 = 2,
    }

    /// <summary>Enumeration of MSI summary stream information property ids.</summary>
    internal enum SummaryInformationStreamProperty : int
    {
        /// <summary>Codepage</summary>
        Codepage = 1,
        /// <summary>Title</summary>
        Title = 2,
        /// <summary>Subject</summary>
        Subject = 3,
        /// <summary>Author</summary>
        Author = 4,
        /// <summary>Keywords</summary>
        Keywords = 5,
        /// <summary>Comments</summary>
        Comments = 6,
        /// <summary>Template</summary>
        Template = 7,
        /// <summary>LastSavedBy</summary>
        LastSavedBy = 8,
        /// <summary>RevisionNumber</summary>
        RevisionNumber = 9,
        /// <summary>LastPrinted</summary>
        LastPrinted = 11,
        /// <summary>CreateTime</summary>
        CreateTime = 12,
        /// <summary>LastSaveTime</summary>
        LastSaveTime = 13,
        /// <summary>PageCount</summary>
        PageCount = 14,
        /// <summary>WordCount</summary>
        WordCount = 15,
        /// <summary>CharacterCount</summary>
        CharacterCount = 16,
        /// <summary>CreatingApplication</summary>
        CreatingApplication = 18,
        /// <summary>Security</summary>
        Security = 19,
    }

    /// <summary>
    /// Enumeration of variant types.
    /// </summary>
    internal enum VariantType : uint
    {
        Empty = 0,
        Null = 1,
        I2 = 2,
        I4 = 3,
        R4 = 4,
        R8 = 5,
        Currency = 6,
        Date = 7,
        BStr = 8,
        Error = 10,
        Bool = 11,
        Variant = 12,
        Decimal = 14,
        I1 = 16,
        UI1 = 17,
        UI2 = 18,
        UI4 = 19,
        I8 = 20,
        UI8 = 21,
        Int = 22,
        UInt = 23,
        LPStr = 30,
        LPWStr = 31,
        Filetime = 64,
        Blob = 65,
        Stream = 66,
        Storage = 67,
        StreamObject = 68,
        StoredObject = 69,
        BlobObject = 70,
        ClipFormat = 71,
        CLSID = 72,
        Vector = 0x1000,
        Array = 0x2000,
        ByRef = 0x4000,
        TypeMask = 0xfff,
    }
    #endregion	Enumerations

    #region	Structures
    /// <summary>The <c>FILETIME</c> structure is a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC).</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct FILETIME
    {
        /// <summary>Low-order part of the file time.</summary>
        public uint LowDateTime;
        /// <summary>High-order part of the file time.</summary>
        public uint HighDateTime;
    }
    #endregion	Structures

    #region	Delegates
    /// <summary>The <c>INSTALLUI_HANDLER</c> delegate defines a callback function that the installer calls for progress notification and error messages.</summary>
    /// <remarks>
    /// The <c>messageType</c> parameter specifies a combination of one message box style, one message box icon type, one default button, and one installation message type.
    /// </remarks>
    internal delegate int MsiInstallUIHandler(Int32 context,
        uint messageType, [MarshalAs(UnmanagedType.LPTStr)] string message);
    #endregion	Delegates

    #region Interop

    /// <summary>
    /// Internal class supporting direct MSI API.
    /// This class cannot be inherited.
    /// This class cannot be instantiated directly.
    /// </summary>
    /// <remarks>
    /// <para>Supports the Windows Installer API 2.0.</para>
    /// <para><b>Note</b>:  The following are omitted from this version:
    /// Hashing and digital signature functions.</para>
    /// <para>Please refer to the MSDN documention on the Windows Installer
    /// for more information about the Windows Installer API.</para>
    /// </remarks>
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal sealed class MsiInterop
    {
        #region	Constants
        private const string MSI_LIB = "msi.dll";

        /// <summary>Maximum chars in feature name (same as string GUID)</summary>
        public const int MaxFeatureChars = 38;

        /// <summary>
        /// Type mask to extract the <see cref="MsiInstallMessage"/> in
        /// <see cref="MsiInstallUIHandler"/>s.
        /// </summary>
        public const uint MessageTypeMask = 0xff000000;

        /// <summary>Represents a "null" integer.</summary>
        public const int MsiNullInteger = Int32.MinValue;   //	0x80000000
        #endregion	Constants

        #region	Construction / Destruction
        private MsiInterop() { }
        #endregion	Construction / Destruction

        #region	Interop Methods
        #region	Installer Functions
        /// <summary>The <c>MsiAdvertiseProduct</c> function generates an advertise script or advertises a product to the computer. The <c>MsiAdvertiseProduct</c> function enables the installer to write to a script the registry and shortcut information used to assign or publish a product. The script can be written to be consistent with a specified platform by using <see cref="MsiAdvertiseProductEx"/>.</summary>
        /// <param name="path">The full path to the package of the product being advertised.</param>
        /// <param name="script">The full path to script file that will be created with the advertise information. To advertise the product locally to the computer, set <see cref="MsiAdvertiseProductFlag.MachineAssign"/> or <see cref="MsiAdvertiseProductFlag.UserAssign"/>.</param>
        /// <param name="transforms">A semicolon-delimited list of transforms to be applied. The list of transforms can be prefixed with the @ or | character to specify the secure caching of transforms. The @ prefix specifies secure-at-source transforms and the | prefix indicates secure full path transforms.  This parameter may be <c>null</c>.</param>
        /// <param name="langId">The installation language to use if the source supports multiple languages.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.CallNotImplemented"/>  This error is returned if an attempt is made to generate an advertise script on any platform other than Windows 2000 or Windows XP. Advertisement to the local computer is supported on all platforms.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiAdvertiseProduct(string path,
            string script, string transforms, UInt16 langId);

        /// <summary>The <c>MsiAdvertiseProductEx</c> function generates an advertise script or advertises a product to the computer. The <c>MsiAdvertiseProductEx</c> function enables the installer to write to a script the registry and shortcut information used to assign or publish a product. Provides the same functionality as <see cref="MsiAdvertiseProduct"/>. The script can be written to be consistent with a specified platform by using <c>MsiAdvertiseProductEx</c>.</summary>
        /// <param name="path">The full path to the package of the product being advertised.</param>
        /// <param name="script">The full path to script file that will be created with the advertise information. To advertise the product locally to the computer, set <see cref="MsiAdvertiseProductFlag.MachineAssign"/> or <see cref="MsiAdvertiseProductFlag.UserAssign"/>.</param>
        /// <param name="transforms">A semicolon-delimited list of transforms to be applied. The list of transforms can be prefixed with the @ or | character to specify the secure caching of transforms. The @ prefix specifies secure-at-source transforms and the | prefix indicates secure full path transforms.  This parameter may be <c>null</c>.</param>
        /// <param name="langId">The installation language to use if the source supports multiple languages.</param>
        /// <param name="platform">Bit flags that control for which platform the installer should create the script. This parameter is ignored if <c>script</c> is <c>null</c>. If <c>platform</c> is <see cref="PlatformArchitecture.Current"/>, then the script is created based on the current platform. This is the same functionality as <see cref="MsiAdvertiseProduct"/>. If <c>platform</c> is <see cref="PlatformArchitecture.X86"/> or <see cref="PlatformArchitecture.IA64"/>, the installer creates script for the specified platform.</param>
        /// <param name="options">Bit flags that specify extra advertisement options. Nonzero value is only available in Windows Installer versions shipped with Windows Server 2003 family and later and Windows XP Service Pack 1 and later.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.CallNotImplemented"/>  This error is returned if an attempt is made to generate an advertise script on any platform other than Windows 2000 or Windows XP. Advertisement to the local computer is supported on all platforms.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiAdvertiseProductEx(string path,
            string script, string transforms, UInt16 langId,
            PlatformArchitecture platform, MsiAdvertismentOptions options);

        /// <summary>For each product listed by the patch package as eligible to receive the patch, the MsiApplyPatch function invokes an installation and sets the PATCH property to the path of the patch package.</summary>
        /// <param name="patchPackage">A null-terminated string specifying the full path to the patch package. </param>
        /// <param name="installPackage">
        /// <para>If <c>installtype</c> is set to <see cref="MsiInstallType.NetworkImage"/>, this parameter is a null-terminated string that specifies a path to the product that is to be patched. The installer applies the patch to every eligible product listed in the patch package if <c>installPackage</c> is set to <c>null</c> and <c>installType</c> is set to <see cref="MsiInstallType.Default"/>.</para>
        /// <para>If <c>installtype</c> is <see cref="MsiInstallType.SingleInstance"/>, the installer applies the patch to the product specified by <c>installPackage</c>. In this case, other eligible products listed in the patch package are ignored and the <c>installPackage</c> parameter contains the null-terminated string representing the product code of the instance to patch. This type of installation requires the installer running Windows .NET Server 2003 family or Windows XP SP1.</para>
        /// </param>
        /// <param name="installType">
        /// <para>This parameter specifies the type of installation to patch.</para>
        /// <para><see cref="MsiInstallType.NetworkImage"/>  Specifies an administrative installation. In this case, <c>installPackage</c> must be set to a package path. A value of 1 for <see cref="MsiInstallType.NetworkImage"/> sets this for an administrative installation.</para>
        /// <para><see cref="MsiInstallType.Default"/>  Searches system for products to patch. In this case, szInstallPackage must be <c>null</c>.</para>
        /// <para><see cref="MsiInstallType.SingleInstance"/>  Patch the product specified by szInstallPackage. <c>installPackage</c> is the product code of the instance to patch. This type of installation requires the installer running Windows .NET Server 2003 family or Windows XP SP1.</para>
        /// </param>
        /// <param name="commandLine">A null-terminated string that specifies command line property settings.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.PatchPackageOpenFailed"/>  Patch package could not be opened.</para>
        /// <para><see cref="MsiError.PatchPackageInvalid"/>  The patch package is invalid.</para>
        /// <para><see cref="MsiError.PatchPackageUnsupported"/>  The patch package is not unsupported.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiApplyPatch(string patchPackage,
            string installPackage, MsiInstallType installType, string commandLine);

        /// <summary>The <c>MsiCloseAllHandles</c> function closes all open installation handles allocated by the current thread. This is a diagnostic function and should not be used for cleanup.</summary>
        /// <returns>This function returns 0 if all handles are closed. Otherwise, the function returns the number of handles open prior to its call.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB)]
        extern static public uint MsiCloseAllHandles();

        /// <summary>The <c>MsiCloseHandle</c> function closes an open installation handle.</summary>
        /// <param name="handle">Specifies any open installation handle.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid handle was passed to the function.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB)]
        extern static public MsiError MsiCloseHandle(Int32 handle);

        /// <summary>The <c>MsiCollectUserInfo</c> function obtains and stores the user information and product ID from an installation wizard.</summary>
        /// <param name="product">Specifies the product code of the product for which the user information is collected.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiCollectUserInfo(string product);

        /// <summary>The <c>MsiConfigureFeature</c> function configures the installed state for a product feature.</summary>
        /// <param name="product">Specifies the product code for the product to be configured.</param>
        /// <param name="feature">Specifies the feature ID for the feature to be configured.</param>
        /// <param name="installState">Specifies the installation state (<see cref="MsiInstallState"/>) for the feature.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiConfigureFeature(string product,
            string feature, MsiInstallState installState);

        /// <summary>The <c>MsiConfigureProduct</c> function installs or uninstalls a product.</summary>
        /// <param name="product">Specifies the product code for the product to be configured.</param>
        /// <param name="level">Specifies how much of the product should be installed when installing the product to its default state. The <c>level</c> parameter will be ignored, and all features will be installed, if the <c>installState</c> parameter is set to any other value than <see cref="MsiInstallState.Default"/>.</param>
        /// <param name="installState">Specifies the installation state for the product.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiConfigureProduct(string product,
            MsiInstallLevel level, MsiInstallState installState);

        /// <summary>The <c>MsiConfigureProductEx</c> function installs or uninstalls a product. A product command line may also be specified.</summary>
        /// <param name="product">Specifies the product code for the product to be configured.</param>
        /// <param name="level">Specifies how much of the product should be installed when installing the product to its default state. The <c>level</c> parameter will be ignored, and all features will be installed, if the <c>installState</c> parameter is set to any other value than <see cref="MsiInstallState.Default"/>.</param>
        /// <param name="installState">Specifies the installation state for the product.</param>
        /// <param name="commandLine">Specifies the command line property settings. This should be a list of the format <i>Property=Setting Property=Setting</i>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiConfigureProductEx(string product,
            MsiInstallLevel level, MsiInstallState installState, string commandLine);

        /// <summary>The <c>MsiEnableLog</c> function sets the log mode for all subsequent installations that are initiated in the calling process.</summary>
        /// <param name="logMode">Specifies the log mode.  Can be a combination of <see cref="MsiInstallLogMode"/> flags.</param>
        /// <param name="logFile">Specifies the string that holds the full path to the log file. Entering a <c>null</c> disables logging, in which case <c>logMode</c> is ignored. If a path is supplied, then <c>logMode</c> must not be zero</param>
        /// <param name="logAttributes">Specifies how frequently the log buffer is to be flushed.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnableLog(MsiInstallLogMode logMode,
            string logFile, MsiLogAttribute logAttributes);

        /// <summary>The <c>MsiEnumClients</c> function enumerates the clients for a given installed component. The function retrieves one product code each time it is called.</summary>
        /// <param name="component">Specifies the component whose clients are to be enumerated.</param>
        /// <param name="index">Specifies the index of the client to retrieve. This parameter should be zero for the first call to the <c>MsiEnumClients</c> function and then incremented for subsequent calls. Because clients are not ordered, any new client has an arbitrary index. This means that the function can return clients in any order.</param>
        /// <param name="product">Pointer to a buffer that receives the product code. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
        /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
        /// <para><see cref="MsiError.UnknownComponent"/>  The specified component is unknown.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnumClients(string component,
            uint index, string product);

        /// <summary>The <c>MsiEnumComponentQualifiers</c> function enumerates the advertised qualifiers for the given component. This function retrieves one qualifier each time it is called.</summary>
        /// <param name="component">Specifies component whose qualifiers are to be enumerated.</param>
        /// <param name="index">Specifies the index of the qualifier to retrieve. This parameter should be zero for the first call to the <c>MsiEnumComponentQualifiers</c> function and then incremented for subsequent calls. Because qualifiers are not ordered, any new qualifier has an arbitrary index. This means that the function can return qualifiers in any order.</param>
        /// <param name="qualifier">Pointer to a buffer that receives the qualifier code.</param>
        /// <param name="qualifierSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>qualifier</c> parameter. On input, this size should include the terminating <c>null</c> character. On return, the value does not include the <c>null</c> character.</param>
        /// <param name="appData">Pointer to a buffer that receives the application registered data for the qualifier. This parameter can be <c>null</c>.</param>
        /// <param name="appDataSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>appData</c> parameter. On input, this size should include the terminating <c>null</c> character. On return, the value does not include the <c>null</c> character. This parameter can be <c>null</c> only if the <c>appData</c> parameter is <c>null</c>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  A buffer is to small to hold the requested data.</para>
        /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
        /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
        /// <para><see cref="MsiError.UnknownComponent"/>  The specified component is unknown.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnumComponentQualifiers(string component,
            uint index, StringBuilder qualifier, ref uint qualifierSize,
            StringBuilder appData, ref int appDataSize);

        /// <summary>The <c>MsiEnumComponents</c> function enumerates the installed components for all products. This function retrieves one component code each time it is called.</summary>
        /// <param name="index">Specifies the index of the component to retrieve. This parameter should be zero for the first call to the <c>MsiEnumComponents</c> function and then incremented for subsequent calls. Because components are not ordered, any new component has an arbitrary index. This means that the function can return components in any order.</param>
        /// <param name="component">Pointer to a buffer that receives the component code. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating <c>null</c> character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
        /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnumComponents(uint index, string component);

        /// <summary>The <c>MsiEnumFeatures</c> function enumerates the published features for a given product. This function retrieves one feature ID each time it is called.</summary>
        /// <param name="product">Null-terminated string specifying the product code of the product whose features are to be enumerated.</param>
        /// <param name="index">Specifies the index of the feature to retrieve. This parameter should be zero for the first call to the <c>MsiEnumFeatures</c> function and then incremented for subsequent calls. Because features are not ordered, any new feature has an arbitrary index. This means that the function can return features in any order.</param>
        /// <param name="feature">Pointer to a buffer that receives the feature ID. This parameter must be sized to hold a string of length <c><see cref="MaxFeatureChars"/> + 1</c>.</param>
        /// <param name="featureParent">Pointer to a buffer that receives the feature ID of the parent of the feature. This parameter must be sized to hold a string of length <c><see cref="MaxFeatureChars"/> + 1</c>. </param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The specified product is unknown.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnumFeatures(string product, uint index,
            string feature, string featureParent);

        /// <summary>The <c>MsiEnumPatches</c> function enumerates all of the patches that have been applied to a product. The function returns the patch code GUID for each patch that has been applied to the product and returns a list of transforms from each patch that apply to the product. Note that patches may have many transforms only some of which are applicable to a particular product. The list of transforms are returned in the same format as the value of the <c>TRANSFORMS</c> property.</summary>
        /// <param name="product">Specifies the product for which patches are to be enumerated.</param>
        /// <param name="index">Specifies the index of the patch to retrieve. This parameter should be zero for the first call to the <c>MsiEnumPatches</c> function and then incremented for subsequent calls. </param>
        /// <param name="patch">Pointer to a buffer that receives the patch's GUID. This argument is required.</param>
        /// <param name="transform">Pointer to a buffer that receives the list of transforms in the patch that are applicable to the product. This argument is required and cannot be <c>null</c>.</param>
        /// <param name="transformSize">Set to the number of characters copied to <c>transform</c> upon an unsuccessful return of the function. Not set for a successful return. On input, this is the full size of the buffer, including a space for a terminating <c>null</c> character. If the buffer passed in is too small, the count returned does not include the terminating <c>null</c> character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
        /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnumPatches(string product, uint index,
            string patch, StringBuilder transform, ref uint transformSize);

        /// <summary>The <c>MsiEnumProducts</c> function enumerates through all the products currently advertised or installed. Both per-user and per-machine installations and advertisements are enumerated.</summary>
        /// <param name="index">Specifies the index of the product to retrieve. This parameter should be zero for the first call to the <c>MsiEnumProducts</c> function and then incremented for subsequent calls. Because products are not ordered, any new product has an arbitrary index. This means that the function can return products in any order.</param>
        /// <param name="product">Pointer to a buffer that receives the product code. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
        /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnumProducts(uint index, string product);

        /// <summary>The <c>MsiEnumRelatedProducts</c> function enumerates products with a specified upgrade code. This function lists the currently installed and advertised products that have the specified UpgradeCode property in their Property table.</summary>
        /// <param name="upgradeCode">The null-terminated string specifying the upgrade code of related products that the installer is to enumerate.</param>
        /// <param name="reserved">This parameter is reserved and must be 0.</param>
        /// <param name="index">The zero-based index into the registered products.</param>
        /// <param name="product">A buffer to receive the product code GUID. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating <c>null</c> character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
        /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnumRelatedProducts(string upgradeCode,
            uint reserved, uint index, string product);

        /// <summary>The <c>MsiGetComponentPath</c> function returns the full path to an installed component. If the key path for the component is a registry key then the registry key is returned.</summary>
        /// <param name="product">Specifies the product code for the client product.</param>
        /// <param name="component">Specifies the component ID of the component to be located.</param>
        /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>. If the component is a registry key, the registry roots are represented numerically. If this is a registry subkey path, there is a backslash at the end of the Key Path. If this is a registry value key path, there is no backslash at the end. For example, a registry path of "HKEY_CURRENT_USER\SOFTWARE\Microsoft" would be returned as "01:\SOFTWARE\Microsoft\".</param>
        /// <param name="pathSize">
        /// <para>Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating <c>null</c> character.</para>
        /// <para>If <c>path</c> is <c>null</c>, <c>pathSize</c> can be <c>null</c>.</para>
        /// </param>
        /// <returns>The <see cref="MsiInstallState"/>.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiInstallState MsiGetComponentPath(
            string product, string component, StringBuilder path, ref uint pathSize);

        /// <summary>The <c>MsiGetFeatureInfo</c> function returns descriptive information for a feature.</summary>
        /// <param name="productHandle">Handle to the product that owns the feature. This handle is obtained from <see cref="MsiOpenProduct"/>.</param>
        /// <param name="feature">Specifies the feature code for the feature about which information should be returned.</param>
        /// <param name="attributes">Specifies the attribute flags.</param>
        /// <param name="title">Pointer to a buffer to receive the localized descriptive name of the feature.</param>
        /// <param name="titleSize">As input, the size of <c>title</c>. As output, the number of characters returned in <c>title</c>. On input, this is the full size of the buffer, including a space for a terminating <c>null</c> character. If the buffer passed in is too small, the count returned does not include the terminating <c>null</c> character.</param>
        /// <param name="help">Pointer to a buffer to receive the localized descriptive name of the feature.</param>
        /// <param name="helpSize">As input, the size of <c>help</c>. As output, the number of characters returned in <c>help</c>. On input, this is the full size of the buffer, including a space for a terminating <c>null</c> character. If the buffer passed in is too small, the count returned does not include the terminating <c>null</c> character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  The product handle is invalid.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  The feature is not known.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetFeatureInfo(Int32 productHandle,
            string feature, MsiInstallFeatureAttribute attributes,
            StringBuilder title, ref uint titleSize, string help, ref uint helpSize);

        /// <summary>The <c>MsiGetFeatureUsage</c> returns the usage metrics for a product feature.</summary>
        /// <param name="product">Specifies the product code for the product containing the feature.</param>
        /// <param name="feature">Specifies the feature code for the feature for which metrics are to be returned.</param>
        /// <param name="useCount">Indicates the number of times the feature has been used.</param>
        /// <param name="dateUsed">Specifies the date that the feature was last used. The date is in the MS-DOS date format.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  No usage information is available or the product or feature is invalid.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetFeatureUsage(string product, string feature,
            out uint useCount, out UInt16 dateUsed);

        /// <summary>The <c>MsiGetFileVersion</c> returns the version string and language string in the format that the installer expects to find them in the database. If you just want version information, set <c>language</c> to <c>null</c> and <c>languageSize</c> to zero. If you just want language information, set <c>version</c> to <c>null</c> and <c>versionSize</c> to zero.</summary>
        /// <param name="path">Specifies the path to the file.</param>
        /// <param name="version">Returns the file version. Set to <c>null</c> for language information only.</param>
        /// <param name="versionSize">In and out buffer byte count. Set to 0 for language information only. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
        /// <param name="language">Returns the file language. Set to <c>null</c> for version information only.</param>
        /// <param name="languageSize">In and out buffer byte count. Set to 0 for version information only. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.AccessDenied"/>  File could not be opened to get version information.</para>
        /// <para><see cref="MsiError.E_Fail"/>  Unexpected error.</para>
        /// <para><see cref="MsiError.FileInvalid"/>  File does not contain version information.</para>
        /// <para><see cref="MsiError.FileNotFound"/>  File does not exist.</para>
        /// <para><see cref="MsiError.InvalidData"/>  The version information is invalid.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetFileVersion(string path, StringBuilder version,
            ref uint versionSize, StringBuilder language, ref uint languageSize);

        /// <summary>The <c>MsiGetPatchInfo</c> function returns information about a patch.</summary>
        /// <param name="patch">Specifies the patch code for the patch package.</param>
        /// <param name="attribute">Specifies the attribute to be retrieved.  (See <see cref="MsiInstallerProperty.LocalPackage"/>)</param>
        /// <param name="value">Pointer to a buffer that receives the property value. This parameter can be <c>null</c>.</param>
        /// <param name="valueSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>value</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The patch package is not installed.</para>
        /// <para><see cref="MsiError.UnknownProperty"/>  The property is unrecognized.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetPatchInfo(string patch, string attribute,
            StringBuilder value, ref uint valueSize);

        /// <summary>The <c>MsiGetProductCode</c> function returns the product code of an application by using the component code of an installed or advertised component of the application. During initialization, an application must determine under which product code it has been installed or advertised.</summary>
        /// <param name="component">This parameter specifies the component code of a component that has been installed by the application. This will be typically the component code of the component containing the executable file of the application.</param>
        /// <param name="product">Pointer to a buffer that receives the product code. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The product code could not be determined.</para>
        /// <para><see cref="MsiError.UnknownComponent"/>  The specified component is unknown.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetProductCode(string component, string product);

        /// <summary>The <c>MsiGetProductInfo</c> function returns product information for published and installed products.</summary>
        /// <param name="product">Specifies the product code for the product.</param>
        /// <param name="property">Specifies the property to be retrieved. The properties in the following list can only be retrieved from applications that are already installed. Note that required properties are guaranteed to be available, but other properties are available only if that property has been set. See the indicated links to the installer properties for information about how each property is set.</param>
        /// <param name="value">Pointer to a buffer that receives the property value. This parameter can be <c>null</c>.</param>
        /// <param name="valueSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>value</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The patch package is not installed.</para>
        /// <para><see cref="MsiError.UnknownProperty"/>  The property is unrecognized.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetProductInfo(string product, string property,
            StringBuilder value, ref uint valueSize);

        /// <summary>The <c>MsiGetProductInfoFromScript</c> function returns product information for a Windows Installer script file.</summary>
        /// <param name="scriptFile">A null-terminated string specifying the full path to the script file. The script file is the advertise script that was created by calling <see cref="MsiAdvertiseProduct"/> or <see cref="MsiAdvertiseProductEx"/>.</param>
        /// <param name="product">Points to a buffer that receives the product code. The buffer must be 39 characters long. The first 38 characters are for the product code GUID, and the last character is for the terminating null character.</param>
        /// <param name="langId">Points to a variable that receives the product language.</param>
        /// <param name="version">Points to a buffer that receives the product version. </param>
        /// <param name="name">Points to a buffer that receives the product name. The buffer includes a terminating null character.</param>
        /// <param name="nameSize">Points to a variable that specifies the size, in characters, of the buffer pointed to by the <c>name</c> parameter. This size should include the terminating null character. When the function returns, this variable contains the length of the string stored in the buffer. The count returned does not include the terminating null character. If the buffer is not large enough, the function returns <see cref="MsiError.MoreData"/>, and the variable contains the size of the string in characters, without counting the null character.</param>
        /// <param name="package">Points to a buffer that receives the package name. The buffer includes the terminating null character.</param>
        /// <param name="packageSize">Points to a variable that specifies the size, in characters, of the buffer pointed to by the <c>package</c> parameter. This size should include the terminating null character. When the function returns, this variable contains the length of the string stored in the buffer. The count returned does not include the terminating null character. If the buffer is not large enough, the function returns <see cref="MsiError.MoreData"/>, and the variable contains the size of the string in characters, without counting the null character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.CallNotImplemented"/>  This function is only available on Windows 2000 and Windows XP.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  Could not get script information.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetProductInfoFromScript(string scriptFile,
            string product, ref UInt16 langId, ref uint version, StringBuilder name,
            ref uint nameSize, StringBuilder package, ref uint packageSize);

        /// <summary>The <c>MsiGetProductProperty</c> function retrieves product properties. These properties are in the product database.</summary>
        /// <param name="productHandle">Handle to the product obtained from <see cref="MsiOpenProduct"/>.</param>
        /// <param name="property">Specifies the property to retrieve. This is case-sensitive.</param>
        /// <param name="value">Pointer to a buffer that receives the property value. This parameter can be <c>null</c>.</param>
        /// <param name="valueSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>value</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  The product handle is invalid.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetProductProperty(Int32 productHandle,
            string property, StringBuilder value, ref uint valueSize);

        /// <summary>The <c>MsiGetShortcutTarget</c> function examines a shortcut and returns its product, feature name, and component if available.</summary>
        /// <param name="target">A null-terminated string specifying the full path to a shortcut.</param>
        /// <param name="product">A GUID for the product code of the shortcut. This string buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character. This parameter can be null.</param>
        /// <param name="feature">The feature name of the shortcut. The string buffer must be <c><see cref="MaxFeatureChars"/> + 1</c> characters long. This parameter can be null.</param>
        /// <param name="component">A GUID of the component code. This string buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character. This parameter can be null.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetShortcutTarget(string target,
            string product, string feature, string component);

        /// <summary>The <c>MsiGetUserInfo</c> function returns the registered user information for an installed product.</summary>
        /// <param name="product">Specifies the product code for the product to be queried.</param>
        /// <param name="user">Pointer to a variable that receives the name of the user.</param>
        /// <param name="userSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>user</c> parameter. This size should include the terminating null character.</param>
        /// <param name="org">Pointer to a buffer that receives the organization name.</param>
        /// <param name="orgSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>org</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
        /// <param name="serial">Pointer to a buffer that receives the product ID.</param>
        /// <param name="serialSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>serial</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
        /// <returns>The <see cref="MsiUserInfoState"/> result.</returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiUserInfoState MsiGetUserInfo(string product,
            StringBuilder user, ref uint userSize, StringBuilder org, ref uint orgSize,
            StringBuilder serial, ref uint serialSize);

        /// <summary>The <c>MsiInstallMissingComponent</c> function installs files that are unexpectedly missing.</summary>
        /// <param name="product">Specifies the product code for the product that owns the component to be installed.</param>
        /// <param name="component">Identifies the component to be installed.</param>
        /// <param name="state">Specifies the way the component should be installed. </param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The installation failed.</para>
        /// <para><see cref="MsiError.InstallSourceAbsent"/>  The source was unavailable.</para>
        /// <para><see cref="MsiError.InstallSuspend"/>  The installation was suspended.</para>
        /// <para><see cref="MsiError.InstallUserExit"/>  The user exited the installation.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The product code is unrecognized.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiInstallMissingComponent(string product,
            string component, MsiInstallState state);

        /// <summary>The <c>MsiInstallMissingFile</c> function installs files that are unexpectedly missing.</summary>
        /// <param name="product">Specifies the product code for the product that owns the file to be installed.</param>
        /// <param name="file">Specifies the file to be installed.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The installation failed.</para>
        /// <para><see cref="MsiError.InstallSourceAbsent"/>  The source was unavailable.</para>
        /// <para><see cref="MsiError.InstallSuspend"/>  The installation was suspended.</para>
        /// <para><see cref="MsiError.InstallUserExit"/>  The user exited the installation.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The product code is unrecognized.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiInstallMissingFile(string product, string file);

        /// <summary>The MsiInstallProduct function installs or uninstalls a product.</summary>
        /// <param name="product">A null-terminated string that specifies the path to the package.</param>
        /// <param name="commandLine">A null-terminated string that specifies the command line property settings. This should be a list of the format <i>Property=Setting Property=Setting</i>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiInstallProduct(string product, string commandLine);

        /// <summary>
        /// <para>The <c>MsiIsProductElevated</c> function checks whether the product is installed with elevated privileges. An application is called a "managed application" if elevated (system) privileges are used to install the application.</para>
        /// <para>Note that <c>MsiIsProductElevated</c> does not take into account policies such as AlwaysInstallElevated, but verifies that the local system owns the product's registry data.</para>
        /// </summary>
        /// <param name="product">The full product code GUID of the product. This parameter is required and may not be null or empty.</param>
        /// <param name="elevated">A pointer to a <see cref="bool"/> for the result. This may not be null.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.CallNotImplemented"/>  This function is only available on Windows 2000 and Windows XP.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiIsProductElevated(string product, out bool elevated);

        /// <summary>The <c>MsiLocateComponent</c> function returns the full path to an installed component without a product code. This function attempts to determine the product using <see cref="MsiGetProductCode"/>, but is not guaranteed to find the correct product for the caller. <see cref="MsiGetComponentPath"/> should always be called when possible.</summary>
        /// <param name="component">Specifies the component ID of the component to be located.</param>
        /// <param name="path">Pointer to a variable that receives the path to the component. The variable includes the terminating null character.</param>
        /// <param name="pathSize">
        /// <para>Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. Upon success of the <c>MsiLocateComponent</c> function, the variable pointed to by pcchBuf contains the count of characters not including the terminating null character. If the size of the buffer passed in is too small, the function returns <see cref="MsiInstallState.MoreData"/>.</para>
        /// <para>If <c>path</c> is <c>null</c>, pcchBuf can be 0.</para>
        /// </param>
        /// <returns>The <see cref="MsiInstallState"/>.</returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiInstallState MsiLocateComponent(string component,
            StringBuilder path, ref uint pathSize);

        /// <summary>The <c>MsiOpenPackage</c> function opens a package for use with the functions that access the product database. The <see cref="MsiCloseHandle"/> function must be called with the handle when the handle is no longer needed.</summary>
        /// <param name="path">Specifies the path to the package. </param>
        /// <param name="handle">Specifies the path to the package. </param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
        /// <para><see cref="MsiError.InstallRemoteProhibited"/>  Windows Installer does not permit installation from a Remote Desktop Connection.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiOpenPackage(string path, out Int32 handle);

        /// <summary>The <c>MsiOpenPackageEx</c> function opens a package for use with functions that access the product database. The <see cref="MsiCloseHandle"/> function must be called with the handle when the handle is no longer needed.</summary>
        /// <param name="path">Specifies the path to the package.</param>
        /// <param name="options">The <see cref="MsiOpenPackageFlags"/> option.</param>
        /// <param name="handle">Pointer to a variable that receives the product handle.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
        /// <para><see cref="MsiError.InstallRemoteProhibited"/>  Windows Installer does not permit installation from a Remote Desktop Connection.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiOpenPackageEx(string path, MsiOpenPackageFlags options,
            out Int32 handle);

        /// <summary>The <c>MsiOpenProduct</c> function opens a product for use with the functions that access the product database. The <see cref="MsiCloseHandle"/> function must be called with the handle when the handle is no longer needed.</summary>
        /// <param name="product">Specifies the product code of the product to be opened.</param>
        /// <param name="handle">Pointer to a variable that receives the product handle. </param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
        /// <para><see cref="MsiError.InstallRemoteProhibited"/>  Windows Installer does not permit installation from a Remote Desktop Connection.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiOpenProduct(string product, out Int32 handle);

        /// <summary>
        /// <para>The <c>MsiProvideAssembly</c> function returns the full path to a Windows Installer component containing an assembly. The function prompts for a source and performs any necessary installation. <c>MsiProvideAssembly</c> increments the usage count for the feature.</para>
        /// <para>This function is available starting with Windows Installer version 2.0.</para>
        /// </summary>
        /// <param name="assembly">The assembly's name as a string.</param>
        /// <param name="context">Set to <c>null</c> for global assemblies. For private assemblies, set <c>context</c> to the full path of the application configuration file (.cfg file) or to the full path of the executable file (.exe) of the application to which the assembly has been made private.</param>
        /// <param name="mode">Defines the installation mode.</param>
        /// <param name="info">Assembly information and assembly type.</param>
        /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>.</param>
        /// <param name="pathSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character. </param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.FileNotFound"/>  The feature is absent or broken.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
        /// <para><see cref="MsiError.InstallNotUsed"/>  The component being requested is disabled on the computer.</para>
        /// <para><see cref="MsiError.InstallSourceAbsent"/>  The source was unavailable.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
        /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
        /// <para><see cref="MsiError.UnknownComponent"/>  The component ID does not specify a known component.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  The feature ID does not identify a known feature.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
        /// <para><see cref="MsiInstallState.Unknown"/>  An unrecognized product or a feature name was passed to the function.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public uint MsiProvideAssembly(string assembly, string context,
            uint mode, MsiAssemblyInfo info, string path, ref int pathSize);

        /// <summary>The <c>MsiProvideComponent</c> function returns the full component path, performing any necessary installation. This function prompts for source if necessary and increments the usage count for the feature.</summary>
        /// <param name="product">Specifies the product code for the product that contains the feature with the necessary component.</param>
        /// <param name="feature">Specifies the feature ID of the feature with the necessary component.</param>
        /// <param name="component">Specifies the component code of the necessary component.</param>
        /// <param name="mode">Defines the installation mode.</param>
        /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>.</param>
        /// <param name="pathSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character. </param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.FileNotFound"/>  The feature is absent or broken.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
        /// <para><see cref="MsiError.InstallNotUsed"/>  The component being requested is disabled on the computer.</para>
        /// <para><see cref="MsiError.InstallSourceAbsent"/>  The source was unavailable.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  The feature ID does not identify a known feature.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
        /// <para><see cref="MsiInstallState.Unknown"/>  An unrecognized product or a feature name was passed to the function.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public uint MsiProvideComponent(string product, string feature,
            string component, uint mode, StringBuilder path, ref int pathSize);

        /// <summary>The <c>MsiProvideQualifiedComponent</c> function returns the full component path for a qualified component and performs any necessary installation. This function prompts for source if necessary, and increments the usage count for the feature.</summary>
        /// <param name="component">Specifies the component ID for the requested component. This may not be the GUID for the component itself, but rather a server that provides the correct functionality, as in the ComponentId column of the PublishComponent table.</param>
        /// <param name="qualifier">Specifies a qualifier into a list of advertising components (from PublishComponent Table).</param>
        /// <param name="mode">Defines the installation mode.</param>
        /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>.</param>
        /// <param name="pathSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character. </param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FileNotFound"/>  The feature is absent or broken.</para>
        /// <para><see cref="MsiError.IndexAbsent"/>  The component qualifier is invalid or absent.</para>
        /// <para><see cref="MsiError.UnknownComponent"/>  The component ID does not specify a known component.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiProvideQualifiedComponent(string component,
            string qualifier, uint mode, StringBuilder path, ref int pathSize);

        /// <summary>The <c>MsiProvideQualifiedComponent</c> function returns the full component path for a qualified component and performs any necessary installation. This function prompts for source if necessary, and increments the usage count for the feature.</summary>
        /// <param name="component">Specifies the component ID for the requested component. This may not be the GUID for the component itself, but rather a server that provides the correct functionality, as in the ComponentId column of the PublishComponent table.</param>
        /// <param name="qualifier">Specifies a qualifier into a list of advertising components (from PublishComponent Table).</param>
        /// <param name="mode">Defines the installation mode.</param>
        /// <param name="product">Specifies the product to match that has published the qualified component. If this is <c>null</c>, then this API works the same as <see cref="MsiProvideQualifiedComponent"/>.</param>
        /// <param name="unused1">Reserved. Must be zero.</param>
        /// <param name="unused2">Reserved. Must be zero.</param>
        /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>.</param>
        /// <param name="pathSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character. </param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FileNotFound"/>  The feature is absent or broken.</para>
        /// <para><see cref="MsiError.IndexAbsent"/>  The component qualifier is invalid or absent.</para>
        /// <para><see cref="MsiError.UnknownComponent"/>  The component ID does not specify a known component.</para>
        /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiProvideQualifiedComponentEx(string component,
            string qualifier, uint mode, string product, uint unused1, uint unused2,
            StringBuilder path, ref int pathSize);

        /// <summary>The <c>MsiQueryFeatureState</c> function returns the installed state for a product feature.</summary>
        /// <param name="product">Specifies the product code for the product that contains the feature of interest.</param>
        /// <param name="feature">Identifies the feature of interest.</param>
        /// <returns>The <see cref="MsiInstallState"/>.</returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiInstallState MsiQueryFeatureState(string product,
            string feature);

        /// <summary>The <c>MsiQueryProductState</c> function returns the installed state for a product.</summary>
        /// <param name="product">Specifies the product code for the product of interest.</param>
        /// <returns>The <see cref="MsiInstallState"/>.</returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiInstallState MsiQueryProductState(string product);

        /// <summary>The <c>MsiReinstallFeature</c> function reinstalls features.</summary>
        /// <param name="product">Specifies the product code for the product containing the feature to be reinstalled.</param>
        /// <param name="feature">Identifies the feature to be reinstalled.</param>
        /// <param name="mode">Specifies what to install.  (See <see cref="MsiReinstallMode"/>.)</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The installation failed.</para>
        /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
        /// <para><see cref="MsiError.InstallSuspend"/>  The installation was suspended.</para>
        /// <para><see cref="MsiError.InstallUserExit"/>  The user exited the installation.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  The feature ID does not identify a known feature.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiReinstallFeature(string product,
            string feature, MsiReinstallMode mode);

        /// <summary>The <c>MsiReinstallProduct</c> function reinstalls products.</summary>
        /// <param name="product">Specifies the product code for the product containing the feature to be reinstalled.</param>
        /// <param name="mode">Specifies what to install.  (See <see cref="MsiReinstallMode"/>.)</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The installation failed.</para>
        /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
        /// <para><see cref="MsiError.InstallSuspend"/>  The installation was suspended.</para>
        /// <para><see cref="MsiError.InstallUserExit"/>  The user exited the installation.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  The feature ID does not identify a known feature.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiReinstallProduct(string product,
            MsiReinstallMode mode);

        /// <summary>The <c>MsiSetExternalUI</c> function enables an external user-interface handler. This external UI handler is called before the normal internal user-interface handler. The external UI handler has the option to suppress the internal UI by returning a non-zero value to indicate that it has handled the messages.</summary>
        /// <param name="handler">The <see cref="MsiInstallUIHandler"/> handler delegate.</param>
        /// <param name="filter">Specifies which messages to handle using the external message handler. If the external handler returns a non-zero result, then that message will not be sent to the UI, instead the message will be logged if logging has been enabled. See <see cref="MsiEnableLog"/>.</param>
        /// <param name="context">Pointer to an application context that is passed to the callback function. This parameter can be used for error checking.</param>
        /// <returns>The return value is the previously set external handler, or <c>null</c> if there was no previously set handler.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiInstallUIHandler MsiSetExternalUI(
            [MarshalAs(UnmanagedType.FunctionPtr)]
            MsiInstallUIHandler handler, MsiInstallLogMode filter,
            Int32 context);

        /// <summary>The <c>MsiSetInternalUI</c> function enables the installer's internal user interface. Then this user interface is used for all subsequent calls to user-interface-generating installer functions in this process.</summary>
        /// <param name="level">Specifies the level of complexity of the user interface.</param>
        /// <param name="parentWindow">Pointer to a window. This window becomes the owner of any user interface created. A pointer to the previous owner of the user interface is returned. If this parameter is <c>null</c>, the owner of the user interface does not change.</param>
        /// <returns>The previous user interface level is returned. If an invalid <c>level</c> is passed, then <see cref="MsiInstallUILevel.NoChange"/> is returned.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiInstallUILevel MsiSetInternalUI(MsiInstallUILevel level,
            ref Int32 parentWindow);

        /// <summary>The <c>MsiSourceListAddSource</c> function adds to the list of valid network sources in the source list.</summary>
        /// <param name="product">Specifies the product code.</param>
        /// <param name="user">User name for per-user installation; null or empty string for per-machine installation. On Windows NT, Windows 2000, or Windows XP, the user name should always be in the format of DOMAIN\USERNAME (or MACHINENAME\USERNAME for a local user).</param>
        /// <param name="reserved">Reserved for future use. This value must be set to 0.</param>
        /// <param name="source">Pointer to the string specifying the source.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.AccessDenied"/>  The user does not have the ability to add a source.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.BadUserName"/>  Could not resolve the user name.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSourceListAddSource(string product,
            string user, uint reserved, string source);

        /// <summary>The <c>MsiSourceListClearAll</c> function removes all network sources from the source list.</summary>
        /// <param name="product">Specifies the product code.</param>
        /// <param name="user">User name for per-user installation; null or empty string for per-machine installation. On Windows NT, Windows 2000, or Windows XP, the user name should always be in the format of DOMAIN\USERNAME (or MACHINENAME\USERNAME for a local user).</param>
        /// <param name="reserved">Reserved for future use. This value must be set to 0.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.AccessDenied"/>  The user does not have the ability to add a source.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.BadUserName"/>  Could not resolve the user name.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSourceListClearAll(string product,
            string user, uint reserved);

        /// <summary>The <c>MsiSourceListForceResolution</c> function forces the installer to search the source list for a valid product source the next time a source is needed. For example, when the installer performs an installation or reinstallation, or when it needs the path for a component that is set to run from source.</summary>
        /// <param name="product">Specifies the product code.</param>
        /// <param name="user">User name for per-user installation; null or empty string for per-machine installation. On Windows NT, Windows 2000, or Windows XP, the user name should always be in the format of DOMAIN\USERNAME (or MACHINENAME\USERNAME for a local user).</param>
        /// <param name="reserved">Reserved for future use. This value must be set to 0.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.AccessDenied"/>  The user does not have the ability to add a source.</para>
        /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
        /// <para><see cref="MsiError.BadUserName"/>  Could not resolve the user name.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSourceListForceResolution(string product,
            string user, uint reserved);

        /// <summary>The <c>MsiUseFeature</c> function increments the usage count for a particular feature and indicates the installation state for that feature. This function should be used to indicate an application's intent to use a feature.</summary>
        /// <param name="product">Specifies the product code for the product that owns the feature to be used.</param>
        /// <param name="feature">Identifies the feature to be used.</param>
        /// <returns>The <see cref="MsiInstallState"/>.</returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiInstallState MsiUseFeature(string product, string feature);

        /// <summary>The <c>MsiUseFeatureEx</c> function increments the usage count for a particular feature and indicates the installation state for that feature. This function should be used to indicate an application's intent to use a feature.</summary>
        /// <param name="product">Specifies the product code for the product that owns the feature to be used.</param>
        /// <param name="feature">Identifies the feature to be used.</param>
        /// <param name="mode">This can be <see cref="MsiInstallMode.NoDetection"/>.</param>
        /// <param name="reserved">Reserved for future use. This value must be set to 0. </param>
        /// <returns>The <see cref="MsiInstallState"/>.</returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiInstallState MsiUseFeatureEx(string product, string feature,
            MsiInstallMode mode, uint reserved);

        /// <summary>The <c>MsiVerifyPackage</c> function verifies that the given file is an installation package.</summary>
        /// <param name="path">Specifies the path and file name of the package.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The file is a package.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.PatchPackageInvalid"/>  The file is not a valid package.</para>
        /// <para><see cref="MsiError.PatchPackageOpenFailed"/>  The file could not be opened.</para>
        /// </returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiVerifyPackage(string path);
        #endregion	Installer Functions

        #region	Database Functions
        /// <summary>The <c>MsiCreateRecord</c> function creates a new record object with the specified number of fields. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <param name="count">Specifies the number of fields the record will have. The maximum number of fields in a record is limited to 65535.</param>
        /// <returns>
        /// <para>If the function succeeds, the return value is handle to a new record object.</para>
        /// <para>If the function fails, the return value is <c>IntPtr.Zero</c>.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB)]
        extern static public Int32 MsiCreateRecord(uint count);

        /// <summary>The <c>MsiCreateTransformSummaryInfo</c> function creates summary information of an existing transform to include validation and error conditions. Execution of this function sets the error record, accessible through <see cref="MsiGetLastErrorRecord"/>.</summary>
        /// <param name="database">Handle to the database that contains the new database Summary Information.</param>
        /// <param name="databaseRef">Handle to the database that contains the original Summary Information.</param>
        /// <param name="transformFile">The name of the transform to which the Summary Information is added.</param>
        /// <param name="errorConditions">The error conditions that should be suppressed when the transform is applied.</param>
        /// <param name="validation">Specifies those properties to be validated to verify the transform can be applied to the database.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InstallPackageInvalid"/>  Reference to an invalid Windows Installer package.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.OpenFailed"/>  The transform storage file could not be opened.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiCreateTransformSummaryInfo(Int32 database,
            Int32 databaseRef, string transformFile, MsiTransformError errorConditions,
            MsiValidationFlag validation);

        /// <summary>The <c>MsiDatabaseApplyTransform</c> function applies a transform to a database.</summary>
        /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/> to the transform.</param>
        /// <param name="transformFile">Specifies the name of the transform file to apply.</param>
        /// <param name="errorConditions">Error conditions that should be suppressed.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InstallTransformFailure"/>  Reference to an invalid Windows Installer package.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.OpenFailed"/>  The transform storage file could not be opened.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseApplyTransform(Int32 database,
            string transformFile, MsiTransformError errorConditions);

        /// <summary>The <c>MsiDatabaseCommit</c> function commits changes to a database.</summary>
        /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseCommit(Int32 database);

        /// <summary>The <c>MsiDatabaseExport</c> function exports an installer table from an open database to a text archive file.</summary>
        /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
        /// <param name="table">Specifies the name of the table to export.</param>
        /// <param name="folder">Specifies the name of the folder that contains archive files.</param>
        /// <param name="fileName">Specifies the name of the exported table archive file.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadPathName"/>  An invalid path was passed to the function.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseExport(Int32 database,
            string table, string folder, string fileName);

        /// <summary>The <c>MsiDatabaseGenerateTransform</c> function generates a transform file of differences between two databases. A transform is a way of recording changes to a database without altering the original database. You can also use <c>MsiDatabaseGenerateTransform</c> to test whether two databases are identical without creating a transform.</summary>
        /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/> that includes the changes.</param>
        /// <param name="databaseRef">Handle to the database obtained from <see cref="MsiOpenDatabase"/> that does not include the changes.</param>
        /// <param name="transformFile">A null-terimated string specifying the name of the transform file being generated. This parameter can be <c>null</c>. If <c>transformFile</c> is <c>null</c>, you can use <c>MsiDatabaseGenerateTransform</c> to test whether two databases are identical without creating a transform. If the databases are identical, the function returns <see cref="MsiError.NoData"/>. If the databases are different the function returns <see cref="MsiError.NoError"/>.</param>
        /// <param name="reserved1">This is a reserved argument and must be set to 0.</param>
        /// <param name="reserved2">This is a reserved argument and must be set to 0.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.CreateFailed"/>  The storage for the transform file could not be created.</para>
        /// <para><see cref="MsiError.NoData"/>  If <c>transformFile</c> is <c>null</c>, this value is returned if the two databases are identical. No transform file is generated.</para>
        /// <para><see cref="MsiError.NoError"/>  If <c>transformFile</c> is <c>null</c>, this is returned if the two databases are different.</para>
        /// <para><see cref="MsiError.InstallTransformFailure"/>  The transform could not be generated.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseGenerateTransform(Int32 database,
            Int32 databaseRef, string transformFile, int reserved1, int reserved2);

        /// <summary>The <c>MsiDatabaseGetPrimaryKeys</c> function returns a record containing the names of all the primary key columns for a specified table. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <param name="database">Handle to the database.</param>
        /// <param name="table">Specifies the name of the table from which to obtain primary key names.</param>
        /// <param name="record">Pointer to the handle of the record that holds the primary key names.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.InvalidTable"/>  An invalid table was passed to the function.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseGetPrimaryKeys(Int32 database,
            string table, out Int32 record);

        /// <summary>The <c>MsiDatabaseImport</c> function imports an installer text archive table into an open database.</summary>
        /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
        /// <param name="folder">Specifies the path to the folder containing archive files.</param>
        /// <param name="fileName">Specifies the name of the file to import.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadPathName"/>  An invalid path was passed to the function.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseImport(Int32 database,
            string folder, string fileName);

        /// <summary>The <c>MsiDatabaseIsTablePersistent</c> function returns an enumeration describing the state of a particular table.</summary>
        /// <param name="database">Handle to the database to which the relevant table belongs.</param>
        /// <param name="table">Specifies the name of the relevant table.</param>
        /// <returns>The <see cref="MsiCondition"/> of the table.</returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiCondition MsiDatabaseIsTablePersistent(Int32 database,
            string table);

        /// <summary>The <c>MsiDatabaseMerge</c> function merges two databases together, allowing duplicate rows.</summary>
        /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
        /// <param name="merge">Handle to the database obtained from <see cref="MsiOpenDatabase"/> to merge into the base database.</param>
        /// <param name="table">Specifies the name of the table to receive merge conflict information.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.DatatypeMismatch"/>  Schema difference between the two databases.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidTable"/>  An invalid table was passed to the function.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseMerge(Int32 database,
            Int32 merge, string table);

        /// <summary>The <c>MsiDatabaseOpenView</c> function prepares a database query and creates a view object. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <param name="database">Handle to the database to which you want to open a view object.</param>
        /// <param name="query">Specifies a SQL query string for querying the database.</param>
        /// <param name="view">Pointer to a handle for the returned view.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadQuerySyntax"/>  An invalid SQL query string was passed to the function.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseOpenView(Int32 database,
            string query, out Int32 view);

        /// <summary>The MsiDoAction function executes a built-in action, custom action, or user-interface wizard action.</summary>
        /// <param name="database">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>. </param>
        /// <param name="action">Specifies the action to execute.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.FunctionNotCalled"/>  The action was not found.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The action failed.</para>
        /// <para><see cref="MsiError.InstallSuspend"/>  The user suspended the installation.</para>
        /// <para><see cref="MsiError.InstallUserExit"/>  The user canceled the action.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  The action indicates that the remaining actions should be skipped.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiDoAction(Int32 database, string action);

        /// <summary>The <c>MsiEnableUIPreview</c> function enables preview mode of the user interface to facilitate authoring of user-interface dialog boxes. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <param name="database">Handle to the database.</param>
        /// <param name="preview">Pointer to a returned handle for user-interface preview capability.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.OutOfMemory"/>  Out of memory.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnableUIPreview(Int32 database,
            out Int32 preview);

        /// <summary>The <c>MsiEnumComponentCosts</c> function enumerates the disk-space per drive required to install a component. This information is needed to display the disk-space cost required for all drives in the user interface. The returned disk-space costs are expressed in multiples of 512 bytes.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="component">A null-terminated string specifying the component's name as it is listed in the Component column of the Component table. This parameter can be <c>null</c>. If <c>component</c> is <c>null</c> or an empty string, <c>MsiEnumComponentCosts</c> enumerates the total disk-space per drive used during the installation. In this case, <c>state</c> is ignored. The costs of the installer include those costs for caching the database in the secure folder as well as the cost to create the installation script. Note that the total disk-space used during the installation may be larger than the space used after the component is installed.</param>
        /// <param name="index">0-based index for drives. This parameter should be zero for the first call to the <c>MsiEnumComponentCosts</c> function and then incremented for subsequent calls.</param>
        /// <param name="state">Requested component state to be enumerated. If <c>component</c> is passed as <c>null</c> or an empty string, the installer ignores the <c>state</c> parameter.</param>
        /// <param name="drive">Buffer that holds the drive name including the null terminator. This is an empty string in case of an error.</param>
        /// <param name="driveSize">Pointer to a variable that specifies the size, in TCHARs, of the buffer pointed to by the <c>drive</c> parameter. This size should include the terminating null character. If the buffer provided is too small, the variable pointed to by <c>driveSize</c> contains the count of characters not including the null terminator.</param>
        /// <param name="cost">Cost of the component per drive expressed in multiples of 512 bytes. This value is 0 if an error has occurred. The value returned in <c>cost</c> is final disk-space used by the component after installation. If <c>component</c> is passed as <c>null</c> or an empty string, the installer sets the value at <c>cost</c> to 0.</param>
        /// <param name="tempCost">The component cost per drive for the duration of the installation, or 0 if an error occurred. The value in <c>tempCost</c> represents the temporary space requirements for the duration of the installation. This temporary space requirement is space needed only for the duration of the installation. This does not affect the final disk space requirement.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionNotCalled"/>  Costing is not complete.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  Buffer not large enough for the drive name.</para>
        /// <para><see cref="MsiError.NoMoreItems"/>  There are no more drives to return.</para>
        /// <para><see cref="MsiError.UnknownComponent"/>  The component is missing.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnumComponentCosts(Int32 install,
            string component, uint index, MsiInstallState state, string drive,
            ref uint driveSize, out int cost, out int tempCost);

        /// <summary>The <c>MsiEvaluateCondition</c> function evaluates a conditional expression containing property names and values.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="condition">Specifies the conditional expression. This parameter must not be <c>null</c>.</param>
        /// <returns>The <see cref="MsiCondition"/>.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiCondition MsiEvaluateCondition(Int32 install,
            string condition);

        /// <summary>The <c>MsiFormatRecord</c> function formats record field data and properties using a format string.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="record">Handle to the record to format. The template string must be stored in record field 0 followed by referenced data parameters.</param>
        /// <param name="result">Pointer to the buffer that receives the null terminated formatted string. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>result</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>resultSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>resultSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
        /// <param name="resultSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>result</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>result</c>, not including the terminating null character. If <c>result</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>resultSize</c>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  Buffer not large enough for the drive name.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiFormatRecord(Int32 install, Int32 record,
            StringBuilder result, ref uint resultSize);

        /// <summary>The <c>MsiGetActiveDatabase</c> function returns the active database for the installation. This function returns a read-only handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <returns>If the function succeeds, it returns a read-only handle to the database currently in use by the installer. If the function fails, the function returns <c>IntPtr.Zero</c>.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public Int32 MsiGetActiveDatabase(Int32 install);

        /// <summary>The <c>MsiGetComponentState</c> function obtains the state of a component.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="component">A null-terminated string specifying the component name within the product.</param>
        /// <param name="state">Receives the current installed state.</param>
        /// <param name="action">Receives the action taken during the installation.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.UnknownComponent"/>  An unknown component was requested.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetComponentState(Int32 install,
            string component, out MsiInstallState state, out MsiInstallState action);

        /// <summary>The <c>MsiGetDatabaseState</c> function returns the state of the database.</summary>
        /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
        /// <returns>The <see cref="MsiDbState"/>.</returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiDbState MsiGetDatabaseState(Int32 database);

        /// <summary>The <c>MsiGetFeatureCost</c> function returns the disk space required by a feature and its selected children and parent features.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="feature">Specifies the name of the feature.</param>
        /// <param name="costTree">Specifies the value the function uses to determine disk space requirements.</param>
        /// <param name="state">Specifies the installation state.</param>
        /// <param name="cost">Receives the disk space requirements in units of 512 bytes. This parameter must not be null.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  An unknown feature was requested.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetFeatureCost(Int32 install, string feature,
            MsiCostTree costTree, MsiInstallState state, out int cost);

        /// <summary>The <c>MsiGetFeatureState</c> function gets the requested state of a feature.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="feature">Specifies the feature name within the product.</param>
        /// <param name="state">Receives the current installed state.</param>
        /// <param name="action">Receives the action taken during the installation.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  An unknown feature was requested.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetFeatureState(Int32 install,
            string feature, out MsiInstallState state, out MsiInstallState action);

        /// <summary>The <c>MsiGetFeatureValidStates</c> function returns a valid installation state.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="feature">Specifies the feature name within the product.</param>
        /// <param name="state">Receives the location to hold the valid installation states.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  An unknown feature was requested.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetFeatureValidStates(Int32 install,
            string feature, out MsiFeatureInstallState state);

        /// <summary>The <c>MsiGetLanguage</c> function returns the numeric language of the installation that is currently running.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <returns>If the function succeeds, the return value is the numeric LANGID for the install.  Can be <see cref="MsiError.InvalidHandle"/> if the function fails, or zero if the installation is not running.</returns>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public UInt16 MsiGetLanguage(Int32 install);

        /// <summary>The <c>MsiGetLastErrorRecord</c> function returns the error record that was last returned for the calling process. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <returns>A handle to the error record. If the last function was successful, <c>MsiGetLastErrorRecord</c> returns an <c>IntPtr.Zero</c>.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public Int32 MsiGetLastErrorRecord();

        /// <summary>The <c>MsiGetMode</c> function is used to determine whether the installer is currently running in a specified mode.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="mode">Specifies the run mode.</param>
        /// <returns><c>true</c> if the mode matches requested,</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public bool MsiGetMode(Int32 install, MsiRunMode mode);

        /// <summary>The MsiGetProperty function gets the value for an installer property.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="name">A null-terminated string that specifies the name of the property.</param>
        /// <param name="value">Pointer to the buffer that receives the null terminated string containing the value of the property. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>value</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>valueSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>valueSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
        /// <param name="valueSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>value</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>value</c>, not including the terminating null character. If <c>value</c> is not not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>valueSize</c>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  The provided buffer was too small to hold the entire value.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetProperty(Int32 install, string name,
            StringBuilder value, ref uint valueSize);

        /// <summary>The <c>MsiGetSourcePath</c> function returns the full source path for a folder in the Directory table.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="folder"> null-terminated string that specifies a record of the Directory table. If the directory is a root directory, this can be a value from the DefaultDir column. Otherwise it must be a value from the Directory column.</param>
        /// <param name="path">Pointer to the buffer that receives the null terminated full source path. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>path</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>pathSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>pathSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
        /// <param name="pathSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>path</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>path</c>, not including the terminating null character. If <c>path</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>pathSize</c>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.Directory"/>  The directory specified was not found in the Directory table.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  The provided buffer was too small to hold the entire value.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetSourcePath(Int32 install, string folder,
            StringBuilder path, ref uint pathSize);

        /// <summary>The <c>MsiGetSummaryInformation</c> function obtains a handle to the _SummaryInformation stream for an installer database. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="path">Specifies the path to the database. </param>
        /// <param name="updateCount">Specifies the maximum number of updated values.</param>
        /// <param name="summaryInfo">Pointer to the location from which to receive the summary information handle.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InstallPackageInvalid"/>  The installation package is invalid.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetSummaryInformation(Int32 install,
            string path, uint updateCount, out Int32 summaryInfo);

        /// <summary>The <c>MsiGetTargetPath</c> function returns the full target path for a folder in the Directory table.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="folder"> null-terminated string that specifies a record of the Directory table. If the directory is a root directory, this can be a value from the DefaultDir column. Otherwise it must be a value from the Directory column.</param>
        /// <param name="path">Pointer to the buffer that receives the null terminated full source path. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>path</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>pathSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>pathSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
        /// <param name="pathSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>path</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>path</c>, not including the terminating null character. If <c>path</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>pathSize</c>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.Directory"/>  The directory specified was not found in the Directory table.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  The provided buffer was too small to hold the entire value.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetTargetPath(Int32 install, string folder,
            StringBuilder path, ref uint pathSize);

        /// <summary>The <c>MsiOpenDatabase</c> function opens a database file for data access. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <param name="path">Specifies the full path or relative path to the database file.</param>
        /// <param name="persist">Receives the full path to the file or the persistence mode.  You can use one of the constants from <see cref="MsiDbOpenPersistMode"/>.</param>
        /// <param name="handle">Pointer to the location of the returned database handle.</param>
        /// <returns>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiOpenDatabase(string path, MsiDbPersistMode persist,
            out Int32 handle);

        /// <summary>The <c>MsiPreviewBillboard</c> function displays a billboard with the host control in the displayed dialog box.</summary>
        /// <param name="preview">Handle to the preview.</param>
        /// <param name="name">Specifies the name of the host control.</param>
        /// <param name="billboard">Specifies the name of the billboard to display.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiPreviewBillboard(Int32 preview,
            string name, string billboard);

        /// <summary>The <c>MsiPreviewDialog</c> function displays a dialog box as modeless and inactive.</summary>
        /// <param name="preview">Handle to the preview.</param>
        /// <param name="dialog">Specifies the name of the dialog box to preview.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.FunctionNotCalled"/>  The function was not called.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The action failed.</para>
        /// <para><see cref="MsiError.InstallSuspend"/>  The user suspended the installation.</para>
        /// <para><see cref="MsiError.InstallUserExit"/>  The user canceled the action.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiPreviewDialog(Int32 preview,
            string dialog);

        /// <summary>The <c>MsiProcessMessage</c> function sends an error record to the installer for processing.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="type">The <see cref="MsiInstallMessage"/>.</param>
        /// <param name="record">Handle to a record containing message format and data.</param>
        /// <returns>
        /// <para><b>-1</b> An invalid parameter or handle was supplied.</para>
        /// <para><b>0</b> No action was taken.</para>
        /// <para>A <see cref="DialogResult"/>.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public int MsiProcessMessage(Int32 install,
            MsiInstallMessage type, Int32 record);

        /// <summary>The <c>MsiRecordClearData</c> function sets all fields in a record to <c>null</c>.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordClearData(Int32 record);

        /// <summary>The <c>MsiRecordDataSize</c> function returns the length of a record field. The count does not include the terminating null character.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <param name="field">Specifies a field of the record.</param>
        /// <returns>
        /// <para>The <c>MsiRecordDataSize</c> function returns 0 if the field is null, nonexistent, or an internal object pointer. The function also returns 0 if the handle is not a valid record handle.</para>
        /// <para>If the data is in integer format, the function returns sizeof(int).</para>
        /// <para>If the data is in string format, the function returns the character count (not including the null character).</para>
        /// <para>If the data is in stream format, the function returns the byte count.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public uint MsiRecordDataSize(Int32 record, uint field);

        /// <summary>The <c>MsiRecordGetFieldCount</c> function returns the number of fields in a record.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <returns>The count returned by the <c>MsiRecordGetFieldCount</c> parameter does not include field 0. Read access to fields beyond this count returns null values. Write access fails.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public uint MsiRecordGetFieldCount(Int32 record);

        /// <summary>The <c>MsiRecordGetInteger</c> function returns the integer value from a record field.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <param name="field">Specifies a field of the record.</param>
        /// <returns>The MsiRecordGetInteger function returns <see cref="MsiNullInteger"/> if the field is null or if the field is a string that cannot be converted to an integer.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public int MsiRecordGetInteger(Int32 record, uint field);

        /// <summary>The <c>MsiRecordGetString</c> function returns the string value of a record field.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <param name="field">Specifies a field of the record.</param>
        /// <param name="value">Pointer to the buffer that receives the null terminated string containing the value of the record field. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>value</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>valueSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>valueSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
        /// <param name="valueSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>value</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>value</c>, not including the terminating null character. If <c>value</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>valueSize</c>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  The provided buffer was too small to hold the entire value.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordGetString(Int32 record, uint field,
            StringBuilder value, ref uint valueSize);

        /// <summary>The <c>MsiRecordIsNull</c> function reports whether a record field is <c>null</c>.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <param name="field">Specifies a field of the record.</param>
        /// <returns><c>true</c>, if the function succeeded and the field is null or the field does not exist; otherwise, The function succeeded and the field is not null or the record handle is invalid.</returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public bool MsiRecordIsNull(Int32 record, uint field);

        /// <summary>The <c>MsiRecordReadStream</c> function reads bytes from a record stream field into a buffer.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <param name="field">Specifies a field of the record.</param>
        /// <param name="buffer">A buffer to receive the stream field. You should ensure the destination buffer is the same size or larger than the source buffer.</param>
        /// <param name="bufferSize">Specifies the in and out buffer count. On input, this is the full size of the buffer. On output, this is the number of bytes that were actually written to the buffer.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidDataType"/>  The field is not a stream column.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordReadStream(Int32 record, uint field,
            StringBuilder buffer, ref uint bufferSize);

        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordReadStream(Int32 record, uint field,
            IntPtr buffer, ref uint bufferSize);

        /// <summary>The MsiRecordSetInteger function sets a record field to an integer field.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <param name="field">Specifies a field of the record.</param>
        /// <param name="value">Specifies the value to which to set the field.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidField"/>  An invalid field of the record was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordSetInteger(Int32 record, uint field,
            int value);

        /// <summary>The <c>MsiRecordSetStream</c> function sets a record stream field from a file. Stream data cannot be inserted into temporary fields.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <param name="field">Specifies a field of the record.</param>
        /// <param name="path">Specifies the path to the file containing the stream.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.BadPathName"/>  An invalid path was supplied.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordSetStream(Int32 record, uint field,
            string path);

        /// <summary>The <c>MsiRecordSetString</c> function copies a string into the designated field.</summary>
        /// <param name="record">Handle to the record.</param>
        /// <param name="field">Specifies a field of the record.</param>
        /// <param name="value">Specifies the string value of the field.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordSetString(Int32 record, uint field,
            string value);

        /// <summary>The <c>MsiSequence</c> function executes another action sequence, as described in the specified table.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="table">Specifies the name of the table containing the action sequence.</param>
        /// <param name="mode">This parameter is currently unimplemented. It is reserved for future use and must be 0.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.FunctionNotCalled"/>  The function was not called.</para>
        /// <para><see cref="MsiError.InstallFailure"/>  The action failed.</para>
        /// <para><see cref="MsiError.InstallSuspend"/>  The user suspended the installation.</para>
        /// <para><see cref="MsiError.InstallUserExit"/>  The user canceled the action.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSequence(Int32 install, string table,
            int mode);

        /// <summary>The <c>MsiSetComponentState</c> function sets a component to the requested state.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="component">Specifies the name of the component.</param>
        /// <param name="state">Specifies the state to set.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InstallUserExit"/>  The user canceled the action.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.UnknownComponent"/>  An unknown component was requested.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSetComponentState(Int32 install, string component,
            MsiInstallState state);

        /// <summary>The <c>MsiSetFeatureAttributes</c> function can modify the default attributes of a feature at runtime. Note that the default attributes of features are authored in the Attributes column of the Feature table.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="feature">Specifies the feature name within the product.</param>
        /// <param name="attributes">Feature attributes specified at run time.  (Bit flags)</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  The feature is not known.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSetFeatureAttributes(Int32 install,
            string feature, MsiInstallFeatureAttribute attributes);

        /// <summary>The <c>	MsiSetFeatureState</c> function sets a feature to a specified state.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="feature">Specifies the feature name within the product.</param>
        /// <param name="state">Specifies the state to set.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownFeature"/>  The feature is not known.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSetFeatureState(Int32 install, string feature,
            MsiInstallState state);

        /// <summary>The <c>MsiSetInstallLevel</c> function sets the installation level for a full product installation.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="level">Specifies the installation level.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSetInstallLevel(Int32 install, int level);

        /// <summary>The <c>MsiSetMode</c> function sets an internal engine Boolean state.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="mode">The <see cref="MsiRunMode"/>.  Only <see cref="MsiRunMode.RebootAtEnd"/> and <see cref="MsiRunMode.RebootNow"/> are supported.</param>
        /// <param name="state">Specifies the state to set to <c>true</c> or <c>false</c>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.AccessDenied"/>  Access was denied.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSetMode(Int32 install, MsiRunMode mode,
            bool state);

        /// <summary>The <c>MsiSetProperty</c> function sets the value for an installation property.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="name">Specifies the name of the property.</param>
        /// <param name="value">Specifies the value of the property.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSetProperty(Int32 install, string name,
            string value);

        /// <summary>The <c>MsiSetTargetPath</c> function sets the full target path for a folder in the Directory table.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <param name="folder">Specifies the folder identifier. This is a primary key in the Directory table.</param>
        /// <param name="path">Specifies the full path for the folder, ending in a directory separator.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.Directory"/>  The directory specified was not found in the Directory table.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSetTargetPath(Int32 install, string folder,
            string path);

        /// <summary>The <c>MsiSummaryInfoGetProperty</c> function gets a single property from the summary information.</summary>
        /// <param name="summaryInfo">Handle to summary information.</param>
        /// <param name="id">Specifies the property ID.</param>
        /// <param name="type">Receives the returned property type.</param>
        /// <param name="intValue">Receives the returned integer property data.</param>
        /// <param name="fileTime">Pointer to a file value.</param>
        /// <param name="value">Pointer to the buffer that receives the null terminated summary information property value. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>value</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>valueSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>valueSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character. This parameter is an empty string if there are no errors.</param>
        /// <param name="valueSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>value</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>value</c>, not including the terminating null character. If <c>value</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>valueSize</c>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.MoreData"/>  The buffer passed in was too small to hold the entire value. </para>
        /// <para><see cref="MsiError.UnknownProperty"/>  The property is unknown.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSummaryInfoGetProperty(Int32 summaryInfo,
            uint id, out VariantType type, out int intValue, out FILETIME fileTime,
            StringBuilder value, ref int valueSize);

        /// <summary>The <c>MsiSummaryInfoGetPropertyCount</c> function returns the number of existing properties in the summary information stream.</summary>
        /// <param name="summaryInfo">Handle to summary information.</param>
        /// <param name="count">Location to receive the total property count.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSummaryInfoGetPropertyCount(Int32 summaryInfo,
            out int count);

        /// <summary>The <c>MsiSummaryInfoPersist</c> function writes changed summary information back to the summary information stream.</summary>
        /// <param name="summaryInfo">Handle to summary information.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSummaryInfoPersist(Int32 summaryInfo);

        /// <summary>The <c>MsiSummaryInfoSetProperty</c> function sets a single summary information property.</summary>
        /// <param name="summaryInfo">Handle to summary information.</param>
        /// <param name="id">Specifies the property to set.</param>
        /// <param name="type">Specifies the type of property to set.</param>
        /// <param name="intValue">Specifies the integer value.</param>
        /// <param name="fileTime">Specifies the file-time value.</param>
        /// <param name="value">Specifies the text value. </param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.DatatypeMismatch"/>  The data types were mismatched.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// <para><see cref="MsiError.UnknownProperty"/>  The property is unknown.</para>
        /// <para><see cref="MsiError.UnsupportedType"/>  The type is unsupported.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSummaryInfoSetProperty(Int32 summaryInfo,
            uint id, VariantType type, int intValue, FILETIME fileTime, string value);

        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiSummaryInfoSetProperty(Int32 summaryInfo,
            uint id, VariantType type, int intValue, IntPtr fileTime, string value);

        /// <summary>The <c>MsiVerifyDiskSpace</c> function checks to see if sufficient disk space is present for the current installation.</summary>
        /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.DiskFull"/>  The disk is full.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiVerifyDiskSpace(Int32 install);

        /// <summary>The <c>MsiViewClose</c> function releases the result set for an executed view. </summary>
        /// <param name="view">Handle to a view that is set to release.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewClose(Int32 view);

        /// <summary>The <c>MsiViewExecute</c> function executes a SQL view query and supplies any required parameters. The query uses the question mark token to represent parameters as described in SQL Syntax. The values of these parameters are passed in as the corresponding fields of a parameter record.</summary>
        /// <param name="view">Handle to the view upon which to execute the query.</param>
        /// <param name="record">Handle to a record that supplies the parameters. This parameter contains values to replace the parameter tokens in the SQL query. It is optional, so hRecord can be <c>IntPtr.Zero</c>.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewExecute(Int32 view, Int32 record);

        /// <summary>The <c>MsiViewFetch</c> function fetches the next sequential record from the view. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <param name="view">Handle to the view to fetch from.</param>
        /// <param name="record">Pointer to the handle for the fetched record.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewFetch(Int32 view, ref Int32 record);

        /// <summary>The <c>MsiViewGetColumnInfo</c> function returns a record containing column names or definitions. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
        /// <param name="view">Handle to the view from which to obtain column information.</param>
        /// <param name="type">Specifies a flag indicating what type of information is needed.</param>
        /// <param name="record">Pointer to a handle to receive the column information data record.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewGetColumnInfo(Int32 view,
            MsiColInfoType type, out Int32 record);

        /// <summary>The <c>MsiViewGetError</c> function returns the error that occurred in the <see cref="MsiViewModify"/> function.</summary>
        /// <param name="view">Handle to the view.</param>
        /// <param name="columnNames">Pointer to the buffer that receives the null-terminated column name. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>columnNames</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiDbError.MoreData"/> and <c>columnNamesSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiDbError.NoError"/>, <c>columnNamesSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character. This parameter is an empty string if there are no errors.</param>
        /// <param name="columnNamesSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>columnNames</c>. When the function returns <see cref="MsiDbError.NoError"/>, this variable contains the size of the data copied to <c>columnNames</c>, not including the terminating null character. If <c>columnNames</c> is not large enough, the function returns <see cref="MsiDbError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>columnNamesSize</c>.</param>
        /// <returns>The <see cref="MsiDbError"/></returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiDbError MsiViewGetError(Int32 view,
            StringBuilder columnNames, ref uint columnNamesSize);

        /// <summary>The MsiViewModify function updates a fetched record.</summary>
        /// <param name="view">Handle to a view.</param>
        /// <param name="mode">Specifies the modify mode.</param>
        /// <param name="record">Handle to the record to modify.</param>
        /// <returns>
        /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
        /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
        /// <para><see cref="MsiError.InvalidData"/>  A validation was requested and the data did not pass. For more information, call <see cref="MsiViewGetError"/>.</para>
        /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
        /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
        /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
        /// </returns>
        /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
        [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewModify(Int32 view, MsiModify mode,
            Int32 record);
        #endregion	Database Functions
        #endregion	Interop Methods
    }

    #endregion Interop

    #region C# Classes

    public interface MSIDBFile
    {
        // https://learn.microsoft.com/en-us/windows/win32/msi/specifying-properties
        Dictionary<string, string> GetProperties();
        void SetProperties(Dictionary<string, string> props);

        Dictionary<string, string> GetSummaryProperties();
        void SetSummaryProperties(Dictionary<string, string> info);

        Dictionary<string, object> GetSummaryObjects();
        void SetSummaryObjects(Dictionary<string, object> values);

        byte[] GetBinaryData(string field);
        void SetBinaryData(string field, byte[] data);

        void SaveBinaryDataToFile(string recordName, string fileName);
        void LoadBinaryDataFromFile(string recordName, string fileName);
    }

    /// <summary>
    ///     MSI Database File using MsiInterop (msi.dll)
    /// </summary>
    public sealed class MSIDatabaseFile
    {
        private string fileName = string.Empty;
        private int dbHandle = 0;

        public MSIDatabaseFile() { }
        public MSIDatabaseFile(string fileName) { this.fileName = fileName; }

        public bool IsOpened { get { return dbHandle > 0; } }
        public string FileName { get { return fileName; } }

        public void Open() { Open(this.fileName); }

        public void Open(string fileName)
        {
            dbHandle = 0;
            MsiError err = MsiInterop.MsiOpenDatabase(fileName, MsiDbPersistMode.Direct, out dbHandle);
            if (err != MsiError.Success) throw new Exception($"{err}");
            if (dbHandle == 0) throw new Exception("Unknown Error");
        }

        public void Close()
        {
            if (dbHandle > 0) MsiInterop.MsiCloseHandle(dbHandle);
            dbHandle = 0;
        }

        public string GetTableValue(string tableName, string whereName, string whereValue, uint fieldNumber = 2)
        {
            if (dbHandle == 0) throw new Exception("Database not open");
            string query = $"SELECT * FROM {tableName} WHERE {whereName}='{whereValue}'";

            MsiError err = MsiInterop.MsiDatabaseOpenView(dbHandle, query, out int view);
            if (err != MsiError.Success) throw new Exception($"{err}");
            if (view == 0) throw new Exception("Unknown Error");

            err = MsiInterop.MsiViewExecute(view, 0);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            int record = 0;
            err = MsiInterop.MsiViewFetch(view, ref record);
            if (err != MsiError.Success) throw new Exception($"{err}");
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            uint bs = 1024 * 1024; // 1MB
            StringBuilder sb = new StringBuilder((int)bs);
            err = MsiInterop.MsiRecordGetString(record, fieldNumber, sb, ref bs);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };
            string res = sb.ToString();

            MsiInterop.MsiCloseHandle(record);
            MsiInterop.MsiCloseHandle(view);

            return res;
        }

        public void SetTableValue(string tableName, string whereName, string whereValue, string newValue, uint fieldNumber = 2)
        {
            if (dbHandle == 0) throw new Exception("Database not open");
            string query = $"SELECT * FROM {tableName} WHERE {whereName}='{whereValue}'";

            MsiError err = MsiInterop.MsiDatabaseOpenView(dbHandle, query, out int view);
            if (err != MsiError.Success) throw new Exception($"{err}");
            if (view == 0) throw new Exception("Unknown Error");

            err = MsiInterop.MsiViewExecute(view, 0);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            int record = 0;
            err = MsiInterop.MsiViewFetch(view, ref record);
            if (err != MsiError.Success) throw new Exception($"{err}");
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            err = MsiInterop.MsiRecordSetString(record, fieldNumber, newValue);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };
            err = MsiInterop.MsiViewModify(view, MsiModify.Update, record);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };
            err = MsiInterop.MsiDatabaseCommit(dbHandle);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            MsiInterop.MsiCloseHandle(record);
            MsiInterop.MsiCloseHandle(view);
        }

        public void SaveBinaryDataToFile(string recordName, string fileName)
        {
            if (dbHandle == 0) throw new Exception("Database not open");
            string query = $"SELECT * FROM Binary WHERE Name='{recordName}'";

            MsiError err = MsiInterop.MsiDatabaseOpenView(dbHandle, query, out int view);
            if (err != MsiError.Success) throw new Exception($"{err}");
            if (view == 0) throw new Exception("Unknown Error");

            err = MsiInterop.MsiViewExecute(view, 0);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            int record = 0;
            err = MsiInterop.MsiViewFetch(view, ref record);
            if (err != MsiError.Success) throw new Exception($"{err}");
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            uint bs = 16 * 1024 * 1024; // 16 MB
            IntPtr buff = IntPtr.Zero;
            try { buff = Marshal.AllocHGlobal((int)bs); } catch (Exception ex) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw ex; };
            err = MsiInterop.MsiRecordReadStream(record, 2, buff, ref bs);
            if (err != MsiError.Success) { Marshal.FreeHGlobal(buff); MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            if (bs > 0)
            {
                byte[] data = new byte[bs];
                Marshal.Copy(buff, data, 0, (int)bs);
                Marshal.FreeHGlobal(buff);
                File.WriteAllBytes(fileName, data);
            }
            else
                File.WriteAllBytes(fileName, new byte[0]);

            MsiInterop.MsiCloseHandle(record);
            MsiInterop.MsiCloseHandle(view);
        }

        public void LoadBinaryDataFromFile(string recordName, string fileName)
        {
            if (dbHandle == 0) throw new Exception("Database not open");
            string query = $"SELECT * FROM Binary WHERE Name='{recordName}'";

            MsiError err = MsiInterop.MsiDatabaseOpenView(dbHandle, query, out int view);
            if (err != MsiError.Success) throw new Exception($"{err}");
            if (view == 0) throw new Exception("Unknown Error");

            err = MsiInterop.MsiViewExecute(view, 0);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            int record = 0;
            err = MsiInterop.MsiViewFetch(view, ref record);
            if (err != MsiError.Success) throw new Exception($"{err}");
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            err = MsiInterop.MsiRecordSetStream(record, 2, fileName);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };
            err = MsiInterop.MsiViewModify(view, MsiModify.Update, record);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };
            err = MsiInterop.MsiDatabaseCommit(dbHandle);
            if (err != MsiError.Success) { MsiInterop.MsiCloseHandle(record); MsiInterop.MsiCloseHandle(view); throw new Exception($"{err}"); };

            MsiInterop.MsiCloseHandle(record);
            MsiInterop.MsiCloseHandle(view);
        }

        public void SetProperties(Dictionary<string, string> props)
        {
            if (dbHandle == 0)
                throw new Exception("Database not open");

            foreach (KeyValuePair<string, string> kvp in props)
                this.SetTableValue("Property", "Property", kvp.Key, kvp.Value);
        }

        internal static void Test()
        {
            string dir = "C:\\Downloads\\";
            string prop = "Property";

            MSIDatabaseFile msi = new MSIDatabaseFile($"{dir}TEST_TEST_TEST.msi");
            msi.Open();
            msi.SaveBinaryDataToFile("_D7D112F049BA1A655B5D9A1D0702DEE5", $"{dir}to_read.exe");
            msi.SetTableValue(prop, prop, "ARPCONTACT", "info@nodomain.com");
            msi.SetTableValue(prop, prop, "ARPHELPLINK", "http://nodomain.com");
            msi.SetTableValue(prop, prop, "ARPURLINFOABOUT", "http://nodomain.com");
            msi.SetTableValue(prop, prop, "Manufacturer", "nodomain.com");
            msi.SetTableValue(prop, prop, "ProductName", "nodomain.com");
            msi.SetTableValue(prop, prop, "ProductVersion", "1.0.0.0");
            msi.SetTableValue(prop, prop, "ProductCode", "{8F41CB95-0000-0000-0000-FD8C1BA7FF01}");
            msi.SetTableValue(prop, prop, "UpgradeCode", "{8F41CB95-0000-0000-0000-FD8C1BA7FF02}");
            msi.SetTableValue(prop, prop, "ARPPRODUCTICON", "install.ico");
            msi.SetTableValue(prop, prop, "DISPLAYLANGUAGE", "EN");
            msi.LoadBinaryDataFromFile("_D7D112F049BA1A655B5D9A1D0702DEE5", $"{dir}to_write.exe");
            msi.Close();
        }
    }
    
    /// <summary>
    ///     MSI Database File using Microsoft.Deployment.WindowsInstaller.dll
    /// </summary>
    public class MSIQDatabaseFile: IDisposable, MSIDBFile
    {
        private string fileName = null;
        public MSIQDatabaseFile(string msiFile) { fileName = msiFile; }

        #region Executable

        public string ExeActionName
        {
            get
            {
                string res = null;
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    View v = db.OpenView("SELECT `CustomAction`.`Action`, `CustomAction`.`Type` FROM `CustomAction`");
                    v.Execute();
                    foreach (Record r in v)
                    {
                        string customActionName = r.GetString("Action");
                        int type = r.GetInteger("Type") & 0x00000007;
                        CustomActionTypes customActionType = (CustomActionTypes)type;
                        if (customActionType == CustomActionTypes.Exe)
                            res = customActionName;
                        r.Close();
                    };
                    v.Close();
                };
                return res;
            }
        }

        public string ExeSourceName
        {
            get
            {
                string res = null;
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    View v = db.OpenView("SELECT `CustomAction`.`Source`, `CustomAction`.`Type` FROM `CustomAction`");
                    v.Execute();
                    foreach (Record r in v)
                    {
                        string customSourceName = r.GetString("Source");
                        int type = r.GetInteger("Type") & 0x00000007;
                        CustomActionTypes customActionType = (CustomActionTypes)type;
                        if (customActionType == CustomActionTypes.Exe)
                            res = customSourceName;
                        r.Close();
                    };
                    v.Close();
                };
                return res;
            }
        }

        public string ExeActionArgs
        {
            get
            {
                string res = null;
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    View v = db.OpenView("SELECT `CustomAction`.`Target`, `CustomAction`.`Type` FROM `CustomAction`");
                    v.Execute();
                    foreach (Record r in v)
                    {
                        string customActionTarget = r.GetString("Target");
                        int type = r.GetInteger("Type") & 0x00000007;
                        CustomActionTypes customActionType = (CustomActionTypes)type;
                        if (customActionType == CustomActionTypes.Exe)
                            res = customActionTarget;
                        r.Close();
                    };
                    v.Close();
                };
                return res;
            }
            set
            {
                string action = null;
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    View v = db.OpenView("SELECT `CustomAction`.`Action`, `CustomAction`.`Type` FROM `CustomAction`");
                    v.Execute();
                    foreach (Record r in v)
                    {
                        string customActionName = r.GetString("Action");
                        int type = r.GetInteger("Type") & 0x00000007;
                        CustomActionTypes customActionType = (CustomActionTypes)type;
                        if (customActionType == CustomActionTypes.Exe)
                            action = customActionName;
                        r.Close();
                    };
                    v.Close();
                    if (!string.IsNullOrEmpty(action))
                    {
                        db.Execute($"UPDATE `CustomAction` SET `Target` = '{value}' WHERE `Action` = '{action}'");
                    };
                };               
            }
        }

        #endregion Executable

        #region SummaryProperties

        public Dictionary<string, object> SummaryAsObjects
        {
            get
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    Type myType = db.SummaryInfo.GetType();
                    foreach(PropertyInfo pi in myType.GetProperties())
                    res.Add(pi.Name, pi.GetValue(db.SummaryInfo));
                };
                return res;
            }

            set
            {
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    Type myType = db.SummaryInfo.GetType();
                    foreach (KeyValuePair<string, object> kvp in value)
                    {
                        FieldInfo fi = myType.GetField(kvp.Key);
                        PropertyInfo pi = myType.GetProperty(kvp.Key);
                        if (fi != null) fi.SetValue(db.SummaryInfo, kvp.Value);
                        if (pi != null) pi.SetValue(db.SummaryInfo, kvp.Value);
                    };
                };
            }
        }

        public Dictionary<string, string> SummaryProperties
        {
            get
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    Type myType = db.SummaryInfo.GetType();
                    foreach (PropertyInfo pi in myType.GetProperties())
                    {
                        object val = pi.GetValue(db.SummaryInfo);
                        if(val is string) res.Add(pi.Name, (string)val);
                    };
                };
                return res;
            }

            set
            {
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    db.SummaryInfo.CreatingApp = GetCreatorApp();
                    db.SummaryInfo.CreateTime = DateTime.Now;
                    db.SummaryInfo.LastPrintTime = DateTime.Now;
                    db.SummaryInfo.LastSaveTime = DateTime.Now;
                    
                    Type myType = db.SummaryInfo.GetType();
                    foreach (KeyValuePair<string, string> kvp in value)
                    {
                        FieldInfo fi = myType.GetField(kvp.Key);
                        PropertyInfo pi = myType.GetProperty(kvp.Key);
                        if (fi != null) fi.SetValue(db.SummaryInfo, kvp.Value);
                        if (pi != null) pi.SetValue(db.SummaryInfo, kvp.Value);
                    };                    
                };
            }
        }

        #endregion SummaryProperties

        #region Properties

        public Dictionary<string, string> Properties
        {
            get
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    View v = db.OpenView("SELECT `Property`.`Property`, `Property`.`Value` FROM `Property`");
                    v.Execute();
                    foreach (Record r in v)
                    {
                        res.Add(r.GetString("Property"), r.GetString("Value"));
                        r.Close();
                    };
                    v.Close();
                };
                return res;
            }
            set
            {
                using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
                {
                    foreach (KeyValuePair<string, string> kvp in value)
                    {
                        string sql = $"UPDATE `Property` SET `Value` = '{kvp.Value}' WHERE `Property` = '{kvp.Key}'";
                        try { db.Execute(sql); } catch (Exception ex) { };
                        sql = $"INSERT INTO `Property` (`Property`, `Value`) VALUES ('{kvp.Key}', '{kvp.Value}')";
                        try { db.Execute(sql); } catch (Exception ex) { };
                    };
                };
            }
        }

        #endregion Properties

        #region Methods

        public byte[] GetBinaryData(string field)
        {
            byte[] res = null;
            using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
            {
                View v = db.OpenView($"SELECT `Binary`.`Data`, `Binary`.`Name` FROM `Binary` WHERE `Name` = '{field}'");
                v.Execute();
                foreach (Record r in v)
                {
                    Stream str = r["Data"] as Stream;
                    res = ReadToEnd(str);
                    if(str != null) str.Close();
                    r.Close();
                    break;
                };
                v.Close();
            };
            return res;
        }

        public void SetBinaryData(string field, byte[] data)
        {
            using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
            {
                View v = db.OpenView($"SELECT `Binary`.`Data`, `Binary`.`Name` FROM `Binary` WHERE `Name` = '{field}'");
                v.Execute();
                foreach (Record r in v)
                {
                    MemoryStream ms = new MemoryStream(data);
                    ms.Position = 0;
                    r.SetStream("Data", ms);
                    v.Modify(ViewModifyMode.Update, r);
                    r.Close();
                    ms.Close();
                    break;
                };
                v.Close();
                db.Commit();
            };
        }

        public void SaveBinaryDataToFile(string recordName, string file)
        {
            using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
            {
                View v = db.OpenView($"SELECT `Binary`.`Data`, `Binary`.`Name` FROM `Binary` WHERE `Name` = '{recordName}'");
                v.Execute();
                foreach (Record r in v)
                {                    
                    r.GetStream("Data", file);
                    r.Close();
                    break;
                };
                v.Close();
            };
        }

        public void LoadBinaryDataFromFile(string recordName, string file)
        {
            using (Database db = new Database(fileName, DatabaseOpenMode.Direct))
            {
                View v = db.OpenView($"SELECT `Binary`.`Data`, `Binary`.`Name` FROM `Binary` WHERE `Name` = '{recordName}'");
                v.Execute();
                foreach (Record r in v)
                {
                    r.SetStream("Data", file);                    
                    v.Modify(ViewModifyMode.Update, r);
                    r.Close();
                    break;
                };
                v.Close();
                db.Commit();
            };
        }

        public Dictionary<string, string> GetSummaryProperties() { return SummaryProperties; }
        public void SetSummaryProperties(Dictionary<string, string> info) { SummaryProperties = info; }

        public Dictionary<string, object> GetSummaryObjects() { return SummaryAsObjects; }
        public void SetSummaryObjects(Dictionary<string, object> values) { SummaryAsObjects = values; }

        public void SetProperties(Dictionary<string, string> props) { Properties = props; }

        public Dictionary<string, string> GetProperties() { return Properties; }

        #endregion Methods

        #region Private

        private static byte[] ReadToEnd(Stream input)
        {
            if (input == null) return null;
            if (input.Length == 0) return new byte[0];
            byte[] res = new byte[input.Length];
            input.Read(res, 0, res.Length);
            return res;
        }

        private static string GetCreatorApp()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string fileVersion = fvi.FileVersion;
            string description = fvi.Comments;
            string subVersion = fvi.LegalTrademarks;
            return $"{description} v{fileVersion} {subVersion}";
        }

        #endregion Private

        public void Dispose() { }
    }

    /// <summary>
    ///     MSI Database File SummaryProperties using MsiInterop (msi.dll)
    ///     (READ ONLY WORKS FINE)
    /// </summary>
    public class MSISummaryInformation : IDisposable
    {
        public enum Properties
        {
            CodePage = 1,
            Title = 2,
            Subject = 3,
            Author = 4,
            Keywords = 5,
            Comments = 6,
            Template = 7,
            LastAuthor = 8,
            Revision = 9,
            Printed = 11,
            Created = 12,
            Saved = 13,
            Pages = 14,
            Words = 15,
            Characters = 16,
            Application = 18,
            Security = 19
        }

        [Flags]
        public enum SourceType
        {
            ShortFileNames = 0x00000001,
            Compressed = 0x00000002,
            AdminImage = 0x00000004
        }

        private int siHandle;

        public MSISummaryInformation(int db, uint count)
        {
            MsiError err = MsiInterop.MsiGetSummaryInformation(db, null, count, out siHandle);
            if (err != MsiError.Success) throw new Exception($"{err}");
        }

        public MSISummaryInformation(string path, uint count)
        {
            MsiError err = MsiInterop.MsiGetSummaryInformation(0, path, count, out siHandle);
            if (err != MsiError.Success) throw new Exception($"{err}");
        }

        public void Persist()
        {
            MsiError err = MsiInterop.MsiSummaryInfoPersist(siHandle);
            if (err != MsiError.Success) throw new Exception($"{err}");
        }

        #region GetProperty
        public string GetProperty(Properties which)
        {
            int integerData;
            FILETIME dateTimeData;
            VariantType dataType;
            int stringDataLength = 0;
            MsiError err = MsiInterop.MsiSummaryInfoGetProperty(siHandle, (uint)which,
                out dataType, out integerData, out dateTimeData, null,
                ref stringDataLength);
            if (err != MsiError.Success) throw new Exception($"{err}");
            StringBuilder stringData = new StringBuilder(stringDataLength + 1);
            stringDataLength = stringData.Capacity;
            err = MsiInterop.MsiSummaryInfoGetProperty(siHandle, (uint)which,
                out dataType, out integerData, out dateTimeData, stringData,
                ref stringDataLength);
            if (err != MsiError.Success) throw new Exception($"{err}");

            return stringData.ToString();
        }

        public int GetIntegerProperty(Properties which)
        {
            int integerData;
            FILETIME dateTimeData;
            int stringDataLength = 0;
            VariantType dataType;
            MsiError err = MsiInterop.MsiSummaryInfoGetProperty(siHandle, (uint)which,
                out dataType, out integerData, out dateTimeData, null,
                ref stringDataLength);
            if (err != MsiError.Success) throw new Exception($"{err}");
            return integerData;
        }

        public DateTime GetDateTimeProperty(Properties which)
        {
            int integerData;
            FILETIME dateTimeData;
            int stringDataLength = 0;
            VariantType dataType;
            MsiError err = MsiInterop.MsiSummaryInfoGetProperty(siHandle, (uint)which,
                out dataType, out integerData, out dateTimeData, null,
                ref stringDataLength);
            if (err != MsiError.Success) throw new Exception($"{err}");
            return ToDateTime(dateTimeData);
        }

        #endregion

        #region SetProperty
        public void SetProperty(Properties which, string data)
        {
            MsiError err = MsiInterop.MsiSummaryInfoSetProperty(siHandle, (uint)which,
                VariantType.LPStr, 0, IntPtr.Zero, data);
            if (err != MsiError.Success) throw new Exception($"{err}");
        }

        public void SetIntegerProperty(Properties which, int data)
        {
            MsiError err = MsiInterop.MsiSummaryInfoSetProperty(siHandle, (uint)which,
                VariantType.I4, data, IntPtr.Zero, null);
            if (err != MsiError.Success) throw new Exception($"{err}");
        }

        public void SetDateTimeProperty(Properties which, DateTime data)
        {
            FILETIME ft = FromDateTime(data);
            MsiError err = MsiInterop.MsiSummaryInfoSetProperty(siHandle, (uint)which,
                VariantType.Filetime, 0, ft, null);
            if (err != MsiError.Success) throw new Exception($"{err}");
        }

        #endregion

        private static FILETIME FromDateTime(DateTime time)
        {
            FILETIME val = new FILETIME();
            long ticks = time.ToFileTime();
            val.LowDateTime = (uint)(ticks & 0xFFFFFFFFL);
            val.HighDateTime = (uint)(ticks >> 32);
            return val;
        }
        private static DateTime ToDateTime(FILETIME time)
        {
            return new DateTime(
                (((long)time.HighDateTime) << 32) + time.LowDateTime);
        }

        #region SynthesizedProperties

        public int PropertyCount
        {
            get
            {
                int count;
                MsiError err = MsiInterop.MsiSummaryInfoGetPropertyCount(siHandle, out count);
                if (err != MsiError.Success) throw new Exception($"{err}");
                return count;
            }
        }

        public int CodePage
        {
            get { return GetIntegerProperty(Properties.CodePage); }
            set { SetIntegerProperty(Properties.CodePage, value); }
        }

        public string Title
        {
            get { return GetProperty(Properties.Title); }
            set { SetProperty(Properties.Title, value); }
        }

        public string Subject
        {
            get { return GetProperty(Properties.Subject); }
            set { SetProperty(Properties.Subject, value); }
        }

        public string Author
        {
            get { return GetProperty(Properties.Author); }
            set { SetProperty(Properties.Author, value); }
        }

        public string Keywords
        {
            get { return GetProperty(Properties.Keywords); }
            set { SetProperty(Properties.Keywords, value); }
        }

        public string Comments
        {
            get { return GetProperty(Properties.Comments); }
            set { SetProperty(Properties.Comments, value); }
        }

        public string Template
        {
            get { return GetProperty(Properties.Template); }
            set { SetProperty(Properties.Template, value); }
        }

        public string LastAuthor
        {
            get { return GetProperty(Properties.LastAuthor); }
            set { SetProperty(Properties.LastAuthor, value); }
        }

        public string Revision
        {
            get { return GetProperty(Properties.Revision); }
            set { SetProperty(Properties.Revision, value); }
        }

        public DateTime Printed
        {
            get { return GetDateTimeProperty(Properties.Printed); }
            set { SetDateTimeProperty(Properties.Printed, value); }
        }

        public DateTime Created
        {
            get { return GetDateTimeProperty(Properties.Created); }
            set { SetDateTimeProperty(Properties.Created, value); }
        }

        public DateTime Saved
        {
            get { return GetDateTimeProperty(Properties.Saved); }
            set { SetDateTimeProperty(Properties.Saved, value); }
        }

        public int Pages
        {
            get { return GetIntegerProperty(Properties.Pages); }
            set { SetIntegerProperty(Properties.Pages, value); }
        }

        public int Words
        {
            get { return GetIntegerProperty(Properties.Words); }
            set { SetIntegerProperty(Properties.Words, value); }
        }

        public bool ShortFileNames
        {
            get { return 0 != (Words & (int)SourceType.ShortFileNames); }
        }

        public bool Compressed
        {
            get { return 0 != (Words & (int)SourceType.Compressed); }
        }

        public bool AdministrativeImage
        {
            get { return 0 != (Words & (int)SourceType.AdminImage); }
        }

        public int Characters
        {
            get { return GetIntegerProperty(Properties.Characters); }
            set { SetIntegerProperty(Properties.Characters, value); }
        }

        public string Application
        {
            get { return GetProperty(Properties.Application); }
            set { SetProperty(Properties.Application, value); }
        }

        public string Security
        {
            get { return GetProperty(Properties.Security); }
            set { SetProperty(Properties.Security, value); }
        }

        #endregion SynthesizedProperties

        #region IDisposable Members
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
                MsiInterop.MsiCloseHandle(siHandle);
            };
        }
        public void Dispose()
        {
            Dispose(true);
        }
        ~MSISummaryInformation()
        {
            Dispose(false);
        }
        #endregion
    }


    #endregion C# Classes
}
