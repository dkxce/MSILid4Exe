using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal static class NativeMethods
	{
		internal enum Error : uint
		{
			SUCCESS = 0u,
			FILE_NOT_FOUND = 2u,
			PATH_NOT_FOUND = 3u,
			ACCESS_DENIED = 5u,
			INVALID_HANDLE = 6u,
			INVALID_DATA = 13u,
			INVALID_PARAMETER = 87u,
			OPEN_FAILED = 110u,
			DISK_FULL = 112u,
			CALL_NOT_IMPLEMENTED = 120u,
			BAD_PATHNAME = 161u,
			NO_DATA = 232u,
			MORE_DATA = 234u,
			NO_MORE_ITEMS = 259u,
			DIRECTORY = 267u,
			INSTALL_USEREXIT = 1602u,
			INSTALL_FAILURE = 1603u,
			FILE_INVALID = 1006u,
			UNKNOWN_PRODUCT = 1605u,
			UNKNOWN_FEATURE = 1606u,
			UNKNOWN_COMPONENT = 1607u,
			UNKNOWN_PROPERTY = 1608u,
			INVALID_HANDLE_STATE = 1609u,
			INSTALL_SOURCE_ABSENT = 1612u,
			BAD_QUERY_SYNTAX = 1615u,
			INSTALL_PACKAGE_INVALID = 1620u,
			FUNCTION_FAILED = 1627u,
			INVALID_TABLE = 1628u,
			DATATYPE_MISMATCH = 1629u,
			CREATE_FAILED = 1631u,
			SUCCESS_REBOOT_INITIATED = 1641u,
			SUCCESS_REBOOT_REQUIRED = 3010u
		}

		internal enum SourceType
		{
			Unknown,
			Network,
			Url,
			Media
		}

		[Flags]
		internal enum STGM : uint
		{
			DIRECT = 0u,
			TRANSACTED = 0x10000u,
			SIMPLE = 0x8000000u,
			READ = 0u,
			WRITE = 1u,
			READWRITE = 2u,
			SHARE_DENY_NONE = 0x40u,
			SHARE_DENY_READ = 0x30u,
			SHARE_DENY_WRITE = 0x20u,
			SHARE_EXCLUSIVE = 0x10u,
			PRIORITY = 0x40000u,
			DELETEONRELEASE = 0x4000000u,
			NOSCRATCH = 0x100000u,
			CREATE = 0x1000u,
			CONVERT = 0x20000u,
			FAILIFTHERE = 0u,
			NOSNAPSHOT = 0x200000u,
			DIRECT_SWMR = 0x400000u
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct MsiPatchSequenceData
		{
			public string szPatchData;

			public int ePatchDataType;

			public int dwOrder;

			public uint dwStatus;
		}

		internal class MsiHandle : SafeHandle
		{
			public override bool IsInvalid => handle == IntPtr.Zero;

			public MsiHandle(IntPtr handle, bool ownsHandle)
				: base(handle, ownsHandle)
			{
			}

			public static implicit operator IntPtr(MsiHandle msiHandle)
			{
				return msiHandle.handle;
			}

			[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
			protected override bool ReleaseHandle()
			{
				return RemotableNativeMethods.MsiCloseHandle((int)handle) == 0;
			}
		}

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int DllGetVersion(uint[] dvi);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetInternalUI(uint dwUILevel, ref IntPtr phWnd);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetInternalUI(uint dwUILevel, IntPtr phWnd);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern NativeExternalUIHandler MsiSetExternalUI([MarshalAs(UnmanagedType.FunctionPtr)] NativeExternalUIHandler puiHandler, uint dwMessageFilter, IntPtr pvContext);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnableLog(uint dwLogMode, string szLogFile, uint dwLogAttributes);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetProductInfo(string szProduct, string szProperty, StringBuilder lpValueBuf, ref uint pcchValueBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetPatchInfo(string szPatch, string szAttribute, StringBuilder lpValueBuf, ref uint pcchValueBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumFeatures(string szProduct, uint iFeatureIndex, StringBuilder lpFeatureBuf, StringBuilder lpParentBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiQueryFeatureState(string szProduct, string szFeature);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiUseFeatureEx(string szProduct, string szFeature, uint dwInstallMode, uint dwReserved);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiQueryProductState(string szProduct);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetShortcutTarget(string szShortcut, StringBuilder szProductCode, StringBuilder szFeatureId, StringBuilder szComponentCode);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiProvideComponent(string szProduct, string szFeature, string szComponent, uint dwInstallMode, StringBuilder lpPathBuf, ref uint cchPathBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiProvideQualifiedComponentEx(string szComponent, string szQualifier, uint dwInstallMode, string szProduct, uint dwUnused1, uint dwUnused2, StringBuilder lpPathBuf, ref uint cchPathBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiReinstallFeature(string szFeature, string szProduct, uint dwReinstallMode);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiReinstallProduct(string szProduct, uint dwReinstallMode);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiCollectUserInfo(string szProduct);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiOpenPackageEx(string szPackagePath, uint dwOptions, out int hProduct);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiOpenProduct(string szProduct, out int hProduct);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiInstallProduct(string szPackagePath, string szCommandLine);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiConfigureProductEx(string szProduct, int iInstallLevel, int eInstallState, string szCommandLine);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiConfigureFeature(string szProduct, string szFeature, int eInstallState);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiApplyPatch(string szPatchPackage, string szInstallPackage, int eInstallType, string szCommandLine);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiOpenDatabase(string szDatabasePath, IntPtr uiOpenMode, out int hDatabase);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiOpenDatabase(string szDatabasePath, string szPersist, out int hDatabase);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiGetDatabaseState(int hDatabase);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDatabaseOpenView(int hDatabase, string szQuery, out int hView);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDatabaseMerge(int hDatabase, int hDatabaseMerge, string szTableName);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDatabaseCommit(int hDatabase);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDatabaseGetPrimaryKeys(int hDatabase, string szTableName, out int hRecord);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDatabaseIsTablePersistent(int hDatabase, string szTableName);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDatabaseExport(int hDatabase, string szTableName, string szFolderPath, string szFileName);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDatabaseImport(int hDatabase, string szFolderPath, string szFileName);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDatabaseGenerateTransform(int hDatabase, int hDatabaseReference, string szTransformFile, int iReserved1, int iReserved2);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiCreateTransformSummaryInfo(int hDatabase, int hDatabaseReference, string szTransformFile, int iErrorConditions, int iValidation);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDatabaseApplyTransform(int hDatabase, string szTransformFile, int iErrorConditions);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiViewExecute(int hView, int hRecord);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiViewFetch(int hView, out int hRecord);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiViewModify(int hView, int iModifyMode, int hRecord);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiViewGetError(int hView, StringBuilder szColumnNameBuffer, ref uint cchBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiViewGetColumnInfo(int hView, uint eColumnInfo, out int hRecord);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiCreateRecord(uint cParams);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiFormatRecord(int hInstall, int hRecord, StringBuilder szResultBuf, ref uint cchResultBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiRecordClearData(int hRecord);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiRecordGetFieldCount(int hRecord);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool MsiRecordIsNull(int hRecord, uint iField);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiRecordGetInteger(int hRecord, uint iField);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiRecordGetString(int hRecord, uint iField, StringBuilder szValueBuf, ref uint cchValueBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiRecordSetInteger(int hRecord, uint iField, int iValue);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiRecordSetString(int hRecord, uint iField, string szValue);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiRecordDataSize(int hRecord, uint iField);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiRecordReadStream(int hRecord, uint iField, byte[] szDataBuf, ref uint cbDataBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiRecordSetStream(int hRecord, uint iField, string szFilePath);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetSummaryInformation(int hDatabase, string szDatabasePath, uint uiUpdateCount, out int hSummaryInfo);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSummaryInfoGetProperty(int hSummaryInfo, uint uiProperty, out uint uiDataType, out int iValue, ref long ftValue, StringBuilder szValueBuf, ref uint cchValueBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSummaryInfoSetProperty(int hSummaryInfo, uint uiProperty, uint uiDataType, int iValue, ref long ftValue, string szValue);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSummaryInfoPersist(int hSummaryInfo);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiCloseHandle(int hAny);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetFileVersion(string szFilePath, StringBuilder szVersionBuf, ref uint cchVersionBuf, StringBuilder szLangBuf, ref uint cchLangBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetFileHash(string szFilePath, uint dwOptions, uint[] hash);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiGetActiveDatabase(int hInstall);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetProperty(int hInstall, string szName, StringBuilder szValueBuf, ref uint cchValueBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetProperty(int hInstall, string szName, string szValue);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiProcessMessage(int hInstall, uint eMessageType, int hRecord);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEvaluateCondition(int hInstall, string szCondition);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool MsiGetMode(int hInstall, uint iRunMode);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetMode(int hInstall, uint iRunMode, [MarshalAs(UnmanagedType.Bool)] bool fState);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDoAction(int hInstall, string szAction);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSequence(int hInstall, string szTable, int iSequenceMode);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetSourcePath(int hInstall, string szFolder, StringBuilder szPathBuf, ref uint cchPathBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetTargetPath(int hInstall, string szFolder, StringBuilder szPathBuf, ref uint cchPathBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetTargetPath(int hInstall, string szFolder, string szFolderPath);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetComponentState(int hInstall, string szComponent, out int iInstalled, out int iAction);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetComponentState(int hInstall, string szComponent, int iState);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetFeatureState(int hInstall, string szFeature, out int iInstalled, out int iAction);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetFeatureState(int hInstall, string szFeature, int iState);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetFeatureValidStates(int hInstall, string szFeature, out uint dwInstallState);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetInstallLevel(int hInstall, int iInstallLevel);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern ushort MsiGetLanguage(int hInstall);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumComponents(uint iComponentIndex, StringBuilder lpComponentBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumComponentsEx(string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwIndex, StringBuilder szInstalledProductCode, [MarshalAs(UnmanagedType.I4)] out UserContexts pdwInstalledContext, StringBuilder szSid, ref uint pcchSid);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumClients(string szComponent, uint iProductIndex, StringBuilder lpProductBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumClientsEx(string szComponent, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint iProductIndex, StringBuilder lpProductBuf, [MarshalAs(UnmanagedType.I4)] out UserContexts pdwInstalledContext, StringBuilder szSid, ref uint pcchSid);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiGetComponentPath(string szProduct, string szComponent, StringBuilder lpPathBuf, ref uint pcchBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiGetComponentPathEx(string szProduct, string szComponent, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, StringBuilder lpPathBuf, ref uint pcchBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumComponentQualifiers(string szComponent, uint iIndex, StringBuilder lpQualifierBuf, ref uint pcchQualifierBuf, StringBuilder lpApplicationDataBuf, ref uint pcchApplicationDataBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern int MsiGetLastErrorRecord();

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumRelatedProducts(string upgradeCode, uint dwReserved, uint iProductIndex, StringBuilder lpProductBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetProductCode(string szComponent, StringBuilder lpProductBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetFeatureUsage(string szProduct, string szFeature, out uint dwUseCount, out ushort dwDateUsed);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetFeatureCost(int hInstall, string szFeature, int iCostTree, int iState, out int iCost);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiVerifyPackage(string szPackagePath);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiIsProductElevated(string szProductCode, [MarshalAs(UnmanagedType.Bool)] out bool fElevated);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiAdvertiseProduct(string szPackagePath, IntPtr szScriptFilePath, string szTransforms, ushort lgidLanguage);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiAdvertiseProduct(string szPackagePath, string szScriptFilePath, string szTransforms, ushort lgidLanguage);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiAdvertiseProductEx(string szPackagePath, string szScriptFilePath, string szTransforms, ushort lgidLanguage, uint dwPlatform, uint dwReserved);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiAdvertiseScript(string szScriptFile, uint dwFlags, IntPtr phRegData, [MarshalAs(UnmanagedType.Bool)] bool fRemoveItems);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiProcessAdvertiseScript(string szScriptFile, string szIconFolder, IntPtr hRegData, [MarshalAs(UnmanagedType.Bool)] bool fShortcuts, [MarshalAs(UnmanagedType.Bool)] bool fRemoveItems);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetProductInfoFromScript(string szScriptFile, StringBuilder lpProductBuf39, out ushort plgidLanguage, out uint pdwVersion, StringBuilder lpNameBuf, ref uint cchNameBuf, StringBuilder lpPackageBuf, ref uint cchPackageBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiProvideAssembly(string szAssemblyName, string szAppContext, uint dwInstallMode, uint dwAssemblyInfo, StringBuilder lpPathBuf, ref uint cchPathBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiInstallMissingComponent(string szProduct, string szComponent, int eInstallState);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiInstallMissingFile(string szProduct, string szFile);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiLocateComponent(string szComponent, StringBuilder lpPathBuf, ref uint cchBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetProductProperty(int hProduct, string szProperty, StringBuilder lpValueBuf, ref uint cchValueBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetFeatureInfo(int hProduct, string szFeature, out uint lpAttributes, StringBuilder lpTitleBuf, ref uint cchTitleBuf, StringBuilder lpHelpBuf, ref uint cchHelpBuf);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiVerifyDiskSpace(int hInstall);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumComponentCosts(int hInstall, string szComponent, uint dwIndex, int iState, StringBuilder lpDriveBuf, ref uint cchDriveBuf, out int iCost, out int iTempCost);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetFeatureAttributes(int hInstall, string szFeature, uint dwAttributes);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiRemovePatches(string szPatchList, string szProductCode, int eUninstallType, string szPropertyList);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDetermineApplicablePatches(string szProductPackagePath, uint cPatchInfo, [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] MsiPatchSequenceData[] pPatchInfo);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiDeterminePatchSequence(string szProductCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint cPatchInfo, [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] MsiPatchSequenceData[] pPatchInfo);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiApplyMultiplePatches(string szPatchPackages, string szProductCode, string szPropertiesList);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumPatchesEx(string szProductCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwFilter, uint dwIndex, StringBuilder szPatchCode, StringBuilder szTargetProductCode, [MarshalAs(UnmanagedType.I4)] out UserContexts pdwTargetProductContext, StringBuilder szTargetUserSid, ref uint pcchTargetUserSid);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetPatchInfoEx(string szPatchCode, string szProductCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, string szProperty, StringBuilder lpValue, ref uint pcchValue);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEnumProductsEx(string szProductCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwIndex, StringBuilder szInstalledProductCode, [MarshalAs(UnmanagedType.I4)] out UserContexts pdwInstalledContext, StringBuilder szSid, ref uint pcchSid);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetProductInfoEx(string szProductCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, string szProperty, StringBuilder lpValue, ref uint pcchValue);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiQueryFeatureStateEx(string szProductCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, string szFeature, out int pdwState);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiQueryComponentState(string szProductCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, string szComponent, out int pdwState);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiExtractPatchXMLData(string szPatchPath, uint dwReserved, StringBuilder szXMLData, ref uint pcchXMLData);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListEnumSources(string szProductCodeOrPatchCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions, uint dwIndex, StringBuilder szSource, ref uint pcchSource);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListAddSourceEx(string szProductCodeOrPatchCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions, string szSource, uint dwIndex);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListClearSource(string szProductCodeOrPatchCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions, string szSource);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListClearAllEx(string szProductCodeOrPatchCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListForceResolutionEx(string szProductCodeOrPatchCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListGetInfo(string szProductCodeOrPatchCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions, string szProperty, StringBuilder szValue, ref uint pcchValue);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListSetInfo(string szProductCodeOrPatchCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions, string szProperty, string szValue);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListEnumMediaDisks(string szProductCodeOrPatchCode, string szUserSID, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions, uint dwIndex, out uint pdwDiskId, StringBuilder szVolumeLabel, ref uint pcchVolumeLabel, StringBuilder szDiskPrompt, ref uint pcchDiskPrompt);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListAddMediaDisk(string szProductCodeOrPatchCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions, uint dwDiskId, string szVolumeLabel, string szDiskPrompt);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSourceListClearMediaDisk(string szProductCodeOrPatchCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] UserContexts dwContext, uint dwOptions, uint dwDiskID);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiNotifySidChange(string szOldSid, string szNewSid);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiSetExternalUIRecord([MarshalAs(UnmanagedType.FunctionPtr)] NativeExternalUIRecordHandler puiHandler, uint dwMessageFilter, IntPtr pvContext, out NativeExternalUIRecordHandler ppuiPrevHandler);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiGetPatchFileList(string szProductCode, string szPatchList, out uint cFiles, out IntPtr phFileRecords);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiBeginTransaction(string szTransactionName, int dwTransactionAttributes, out int hTransaction, out IntPtr phChangeOfOwnerEvent);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiEndTransaction(int dwTransactionState);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint MsiJoinTransaction(int hTransaction, int dwTransactionAttributes, out IntPtr phChangeOfOwnerEvent);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryExW", SetLastError = true)]
		internal static extern IntPtr LoadLibraryEx(string fileName, IntPtr hFile, uint flags);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr type, IntPtr name, ushort langId);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr LoadResource(IntPtr hModule, IntPtr lpResourceInfo);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr LockResource(IntPtr resourceData);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW", SetLastError = true)]
		internal static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, StringBuilder lpBuffer, uint nSize, IntPtr Arguments);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int WaitForSingleObject(IntPtr handle, int milliseconds);

		[DllImport("ole32.dll")]
		internal static extern int StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)] string wcsName, IntPtr stgPriority, uint grfMode, IntPtr snbExclude, uint reserved, [MarshalAs(UnmanagedType.Interface)] out IStorage stgOpen);

		[DllImport("ole32.dll")]
		internal static extern int StgCreateDocfile([MarshalAs(UnmanagedType.LPWStr)] string wcsName, uint grfMode, uint reserved, [MarshalAs(UnmanagedType.Interface)] out IStorage stgOpen);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MessageBoxW")]
		internal static extern MessageResult MessageBox(IntPtr hWnd, string lpText, string lpCaption, [MarshalAs(UnmanagedType.U4)] int uType);
	}
}
