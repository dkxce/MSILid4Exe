using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public static class Installer
	{
		private static IList externalUIHandlers = ArrayList.Synchronized(new ArrayList());

		private static bool rebootRequired;

		private static bool rebootInitiated;

		private static ResourceManager errorResources;

		public static bool RebootRequired => rebootRequired;

		public static bool RebootInitiated => rebootInitiated;

		public static Version Version
		{
			get
			{
				uint[] array = new uint[5] { 20u, 0u, 0u, 0u, 0u };
				int num = NativeMethods.DllGetVersion(array);
				if (num != 0)
				{
					Marshal.ThrowExceptionForHR(num);
				}
				return checked(new Version((int)array[1], (int)array[2], (int)array[3]));
			}
		}

		internal static ResourceManager ErrorResources
		{
			get
			{
				if (errorResources == null)
				{
					errorResources = new ResourceManager(typeof(Installer).Namespace + ".Errors", typeof(Installer).Assembly);
				}
				return errorResources;
			}
		}

		public static ExternalUIHandler SetExternalUI(ExternalUIHandler uiHandler, InstallLogModes messageFilter)
		{
			NativeExternalUIHandler nativeExternalUIHandler = null;
			if (uiHandler != null)
			{
				nativeExternalUIHandler = new ExternalUIProxy(uiHandler).ProxyHandler;
				externalUIHandlers.Add(nativeExternalUIHandler);
			}
			NativeExternalUIHandler nativeExternalUIHandler2 = NativeMethods.MsiSetExternalUI(nativeExternalUIHandler, checked((uint)messageFilter), IntPtr.Zero);
			if (nativeExternalUIHandler2 != null && nativeExternalUIHandler2.Target is ExternalUIProxy)
			{
				externalUIHandlers.Remove(nativeExternalUIHandler2);
				return ((ExternalUIProxy)nativeExternalUIHandler2.Target).Handler;
			}
			return null;
		}

		public static ExternalUIRecordHandler SetExternalUI(ExternalUIRecordHandler uiHandler, InstallLogModes messageFilter)
		{
			NativeExternalUIRecordHandler nativeExternalUIRecordHandler = null;
			if (uiHandler != null)
			{
				nativeExternalUIRecordHandler = new ExternalUIRecordProxy(uiHandler).ProxyHandler;
				externalUIHandlers.Add(nativeExternalUIRecordHandler);
			}
			NativeExternalUIRecordHandler ppuiPrevHandler;
			uint num = NativeMethods.MsiSetExternalUIRecord(nativeExternalUIRecordHandler, checked((uint)messageFilter), IntPtr.Zero, out ppuiPrevHandler);
			if (num != 0)
			{
				externalUIHandlers.Remove(nativeExternalUIRecordHandler);
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			if (ppuiPrevHandler != null && ppuiPrevHandler.Target is ExternalUIRecordProxy)
			{
				externalUIHandlers.Remove(ppuiPrevHandler);
				return ((ExternalUIRecordProxy)ppuiPrevHandler.Target).Handler;
			}
			return null;
		}

		public static InstallUIOptions SetInternalUI(InstallUIOptions uiOptions, ref IntPtr windowHandle)
		{
			return (InstallUIOptions)checked((int)NativeMethods.MsiSetInternalUI((uint)uiOptions, ref windowHandle));
		}

		public static InstallUIOptions SetInternalUI(InstallUIOptions uiOptions)
		{
			return (InstallUIOptions)checked((int)NativeMethods.MsiSetInternalUI((uint)uiOptions, IntPtr.Zero));
		}

		public static void EnableLog(InstallLogModes logModes, string logFile)
		{
			EnableLog(logModes, logFile, append: false, flushEveryLine: true);
		}

		public static void EnableLog(InstallLogModes logModes, string logFile, bool append, bool flushEveryLine)
		{
			checked
			{
				uint num = NativeMethods.MsiEnableLog((uint)logModes, logFile, unchecked((uint)(append ? 1 : 0)) + unchecked((uint)(flushEveryLine ? 2 : 0)));
				if (num != 0 && num != 1006)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
			}
		}

		public static InstallState UseFeature(string productCode, string feature, InstallMode installMode)
		{
			return (InstallState)NativeMethods.MsiUseFeatureEx(productCode, feature, (uint)installMode, 0u);
		}

		public static Session OpenPackage(string packagePath, bool ignoreMachineState)
		{
			int hProduct;
			uint num = NativeMethods.MsiOpenPackageEx(packagePath, ignoreMachineState ? 1u : 0u, out hProduct);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return new Session((IntPtr)hProduct, ownsHandle: true);
		}

		public static Session OpenPackage(Database database, bool ignoreMachineState)
		{
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			return OpenPackage(string.Format(CultureInfo.InvariantCulture, "#{0}", database.Handle), ignoreMachineState);
		}

		public static Session OpenProduct(string productCode)
		{
			int hProduct;
			uint num = NativeMethods.MsiOpenProduct(productCode, out hProduct);
			switch (num)
			{
			case 1605u:
				return null;
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return new Session((IntPtr)hProduct, ownsHandle: true);
			}
		}

		public static string ProvideComponent(string product, string feature, string component, InstallMode installMode)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			uint cchPathBuf = checked((uint)stringBuilder.Capacity);
			uint num = NativeMethods.MsiProvideComponent(product, feature, component, (uint)installMode, stringBuilder, ref cchPathBuf);
			if (num == 234)
			{
				stringBuilder.Capacity = checked((int)(++cchPathBuf));
				num = NativeMethods.MsiProvideComponent(product, feature, component, (uint)installMode, stringBuilder, ref cchPathBuf);
			}
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return stringBuilder.ToString();
		}

		public static string ProvideQualifiedComponent(string component, string qualifier, InstallMode installMode, string product)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			uint cchPathBuf = checked((uint)stringBuilder.Capacity);
			uint num = NativeMethods.MsiProvideQualifiedComponentEx(component, qualifier, (uint)installMode, product, 0u, 0u, stringBuilder, ref cchPathBuf);
			if (num == 234)
			{
				stringBuilder.Capacity = checked((int)(++cchPathBuf));
				num = NativeMethods.MsiProvideQualifiedComponentEx(component, qualifier, (uint)installMode, product, 0u, 0u, stringBuilder, ref cchPathBuf);
			}
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return stringBuilder.ToString();
		}

		public static string ProvideAssembly(string assemblyName, string appContext, InstallMode installMode, bool isWin32Assembly)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			uint cchPathBuf = checked((uint)stringBuilder.Capacity);
			uint num = NativeMethods.MsiProvideAssembly(assemblyName, appContext, (uint)installMode, isWin32Assembly ? 1u : 0u, stringBuilder, ref cchPathBuf);
			if (num == 234)
			{
				stringBuilder.Capacity = checked((int)(++cchPathBuf));
				num = NativeMethods.MsiProvideAssembly(assemblyName, appContext, (uint)installMode, isWin32Assembly ? 1u : 0u, stringBuilder, ref cchPathBuf);
			}
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return stringBuilder.ToString();
		}

		public static void InstallMissingComponent(string product, string component, InstallState installState)
		{
			uint num = NativeMethods.MsiInstallMissingComponent(product, component, (int)installState);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public static void InstallMissingFile(string product, string file)
		{
			uint num = NativeMethods.MsiInstallMissingFile(product, file);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public static void ReinstallFeature(string product, string feature, ReinstallModes reinstallModes)
		{
			uint num = NativeMethods.MsiReinstallFeature(product, feature, checked((uint)reinstallModes));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public static void ReinstallProduct(string product, ReinstallModes reinstallModes)
		{
			uint num = NativeMethods.MsiReinstallProduct(product, checked((uint)reinstallModes));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public static void InstallProduct(string packagePath, string commandLine)
		{
			CheckInstallResult(NativeMethods.MsiInstallProduct(packagePath, commandLine));
		}

		public static void ConfigureProduct(string productCode, int installLevel, InstallState installState, string commandLine)
		{
			CheckInstallResult(NativeMethods.MsiConfigureProductEx(productCode, installLevel, (int)installState, commandLine));
		}

		public static void ConfigureFeature(string productCode, string feature, InstallState installState)
		{
			CheckInstallResult(NativeMethods.MsiConfigureFeature(productCode, feature, (int)installState));
		}

		public static void ApplyPatch(string patchPackage, string commandLine)
		{
			ApplyPatch(patchPackage, null, InstallType.Default, commandLine);
		}

		public static void ApplyPatch(string patchPackage, string installPackage, InstallType installType, string commandLine)
		{
			CheckInstallResult(NativeMethods.MsiApplyPatch(patchPackage, installPackage, (int)installType, commandLine));
		}

		public static void RemovePatches(IList<string> patches, string productCode, string commandLine)
		{
			if (patches == null || patches.Count == 0)
			{
				throw new ArgumentNullException("patches");
			}
			if (productCode == null)
			{
				throw new ArgumentNullException("productCode");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string patch in patches)
			{
				if (patch != null)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(';');
					}
					stringBuilder.Append(patch);
				}
			}
			if (stringBuilder.Length == 0)
			{
				throw new ArgumentNullException("patches");
			}
			CheckInstallResult(NativeMethods.MsiRemovePatches(stringBuilder.ToString(), productCode, 2, commandLine));
		}

		public static IList<string> DetermineApplicablePatches(string productPackage, string[] patches, InapplicablePatchHandler errorHandler)
		{
			return DetermineApplicablePatches(productPackage, patches, errorHandler, null, UserContexts.None);
		}

		public static IList<string> DetermineApplicablePatches(string product, string[] patches, InapplicablePatchHandler errorHandler, string userSid, UserContexts context)
		{
			if (string.IsNullOrEmpty(product))
			{
				throw new ArgumentNullException("product");
			}
			if (patches == null)
			{
				throw new ArgumentNullException("patches");
			}
			NativeMethods.MsiPatchSequenceData[] array = new NativeMethods.MsiPatchSequenceData[patches.Length];
			checked
			{
				for (int i = 0; i < patches.Length; i++)
				{
					if (string.IsNullOrEmpty(patches[i]))
					{
						throw new ArgumentNullException("patches[" + i + "]");
					}
					array[i].szPatchData = patches[i];
					array[i].ePatchDataType = GetPatchStringDataType(patches[i]);
					array[i].dwOrder = -1;
					array[i].dwStatus = 0u;
				}
				uint num = ((context != 0) ? NativeMethods.MsiDeterminePatchSequence(product, userSid, context, (uint)array.Length, array) : NativeMethods.MsiDetermineApplicablePatches(product, (uint)array.Length, array));
				if (errorHandler != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].dwOrder < 0 && array[j].dwStatus != 0)
						{
							errorHandler(array[j].szPatchData, InstallerException.ExceptionFromReturnCode(array[j].dwStatus));
						}
					}
				}
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
				IList<string> list = new List<string>(patches.Length);
				for (int k = 0; k < array.Length; k++)
				{
					for (int l = 0; l < array.Length; l++)
					{
						if (array[l].dwOrder == k)
						{
							list.Add(array[l].szPatchData);
						}
					}
				}
				return list;
			}
		}

		public static void ApplyMultiplePatches(IList<string> patchPackages, string productCode, string commandLine)
		{
			if (patchPackages == null || patchPackages.Count == 0)
			{
				throw new ArgumentNullException("patchPackages");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string patchPackage in patchPackages)
			{
				if (patchPackage != null)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(';');
					}
					stringBuilder.Append(patchPackage);
				}
			}
			if (stringBuilder.Length == 0)
			{
				throw new ArgumentNullException("patchPackages");
			}
			CheckInstallResult(NativeMethods.MsiApplyMultiplePatches(stringBuilder.ToString(), productCode, commandLine));
		}

		public static string ExtractPatchXmlData(string patchPath)
		{
			StringBuilder stringBuilder = new StringBuilder("");
			uint pcchXMLData = 0u;
			uint num = NativeMethods.MsiExtractPatchXMLData(patchPath, 0u, stringBuilder, ref pcchXMLData);
			if (num == 234)
			{
				stringBuilder.Capacity = checked((int)(++pcchXMLData));
				num = NativeMethods.MsiExtractPatchXMLData(patchPath, 0u, stringBuilder, ref pcchXMLData);
			}
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return stringBuilder.ToString();
		}

		public static void NotifySidChange(string oldSid, string newSid)
		{
			uint num = NativeMethods.MsiNotifySidChange(oldSid, newSid);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		private static void CheckInstallResult(uint ret)
		{
			switch (ret)
			{
			case 3010u:
				rebootRequired = true;
				break;
			case 1641u:
				rebootInitiated = true;
				break;
			default:
				throw InstallerException.ExceptionFromReturnCode(ret);
			case 0u:
				break;
			}
		}

		private static int GetPatchStringDataType(string patchData)
		{
			if (patchData.IndexOf("<", StringComparison.Ordinal) >= 0 && patchData.IndexOf(">", StringComparison.Ordinal) >= 0)
			{
				return 2;
			}
			if (string.Compare(Path.GetExtension(patchData), ".xml", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 1;
			}
			return 0;
		}

		public static void AdvertiseProduct(string packagePath, bool perUser, string transforms, int locale)
		{
			if (string.IsNullOrEmpty(packagePath))
			{
				throw new ArgumentNullException("packagePath");
			}
			if (!File.Exists(packagePath))
			{
				throw new FileNotFoundException(null, packagePath);
			}
			uint num = NativeMethods.MsiAdvertiseProduct(packagePath, new IntPtr(perUser ? 1 : 0), transforms, checked((ushort)locale));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public static void GenerateAdvertiseScript(string packagePath, string scriptFilePath, string transforms, int locale)
		{
			if (string.IsNullOrEmpty(packagePath))
			{
				throw new ArgumentNullException("packagePath");
			}
			if (!File.Exists(packagePath))
			{
				throw new FileNotFoundException(null, packagePath);
			}
			uint num = NativeMethods.MsiAdvertiseProduct(packagePath, scriptFilePath, transforms, checked((ushort)locale));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public static void GenerateAdvertiseScript(string packagePath, string scriptFilePath, string transforms, int locale, ProcessorArchitecture processor, bool instance)
		{
			if (string.IsNullOrEmpty(packagePath))
			{
				throw new ArgumentNullException("packagePath");
			}
			if (string.IsNullOrEmpty(scriptFilePath))
			{
				throw new ArgumentNullException("scriptFilePath");
			}
			if (!File.Exists(packagePath))
			{
				throw new FileNotFoundException(null, packagePath);
			}
			uint dwPlatform = 0u;
			switch (processor)
			{
			case ProcessorArchitecture.X86:
				dwPlatform = 1u;
				break;
			case ProcessorArchitecture.IA64:
				dwPlatform = 2u;
				break;
			case ProcessorArchitecture.Amd64:
				dwPlatform = 4u;
				break;
			}
			uint num = NativeMethods.MsiAdvertiseProductEx(packagePath, scriptFilePath, transforms, checked((ushort)locale), dwPlatform, instance ? 1u : 0u);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public static void AdvertiseScript(string scriptFile, int flags, bool removeItems)
		{
			uint num = NativeMethods.MsiAdvertiseScript(scriptFile, checked((uint)flags), IntPtr.Zero, removeItems);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public static void ProcessAdvertiseScript(string scriptFile, string iconFolder, bool shortcuts, bool removeItems)
		{
			uint num = NativeMethods.MsiProcessAdvertiseScript(scriptFile, iconFolder, IntPtr.Zero, shortcuts, removeItems);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public static ProductInstallation GetProductInfoFromScript(string scriptFile)
		{
			if (string.IsNullOrEmpty(scriptFile))
			{
				throw new ArgumentNullException("scriptFile");
			}
			StringBuilder stringBuilder = new StringBuilder(40);
			StringBuilder stringBuilder2 = new StringBuilder(100);
			StringBuilder stringBuilder3 = new StringBuilder(40);
			checked
			{
				uint num = (uint)stringBuilder.Capacity;
				uint cchNameBuf = (uint)stringBuilder2.Capacity;
				uint cchPackageBuf = (uint)stringBuilder3.Capacity;
				ushort plgidLanguage;
				uint pdwVersion;
				uint num2 = NativeMethods.MsiGetProductInfoFromScript(scriptFile, stringBuilder, out plgidLanguage, out pdwVersion, stringBuilder2, ref cchNameBuf, stringBuilder3, ref cchPackageBuf);
				if (num2 == 234)
				{
					stringBuilder.Capacity = (int)(++num);
					stringBuilder2.Capacity = (int)(++cchNameBuf);
					stringBuilder3.Capacity = (int)(++cchPackageBuf);
					num2 = NativeMethods.MsiGetProductInfoFromScript(scriptFile, stringBuilder, out plgidLanguage, out pdwVersion, stringBuilder2, ref cchNameBuf, stringBuilder3, ref cchPackageBuf);
				}
				if (num2 != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num2);
				}
				uint num3 = pdwVersion >> 24;
				uint num4 = (pdwVersion & 0xFFFFFF) >> 16;
				uint num5 = pdwVersion & 0xFFFFu;
				Version version = new Version((int)num3, (int)num4, (int)num5);
				return new ProductInstallation(new Dictionary<string, string>
				{
					["ProductCode"] = stringBuilder.ToString(),
					["Language"] = plgidLanguage.ToString(CultureInfo.InvariantCulture),
					["Version"] = version.ToString(),
					["ProductName"] = stringBuilder2.ToString(),
					["PackageName"] = stringBuilder3.ToString()
				});
			}
		}

		public static string GetErrorMessage(int errorNumber)
		{
			return GetErrorMessage(errorNumber, null);
		}

		public static string GetErrorMessage(int errorNumber, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			string text = ErrorResources.GetString(errorNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), culture);
			if (text == null)
			{
				text = GetMessageFromModule(Path.Combine(Environment.SystemDirectory, "msimsg.dll"), errorNumber, culture);
			}
			return text;
		}

		private static string GetMessageFromModule(string modulePath, int errorNumber, CultureInfo culture)
		{
			IntPtr intPtr = NativeMethods.LoadLibraryEx(modulePath, IntPtr.Zero, 2u);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			checked
			{
				try
				{
					int num = ((culture != CultureInfo.InvariantCulture) ? culture.LCID : 0);
					IntPtr intPtr2 = NativeMethods.FindResourceEx(intPtr, new IntPtr(10), new IntPtr(errorNumber), (ushort)num);
					if (intPtr2 != IntPtr.Zero)
					{
						IntPtr intPtr3 = NativeMethods.LockResource(NativeMethods.LoadResource(intPtr, intPtr2));
						if (num == 0)
						{
							return Marshal.PtrToStringAnsi(intPtr3);
						}
						int i;
						for (i = 0; Marshal.ReadByte(intPtr3, i) != 0; i++)
						{
						}
						byte[] array = new byte[i + 1];
						Marshal.Copy(intPtr3, array, 0, array.Length);
						return Encoding.GetEncoding(culture.TextInfo.ANSICodePage).GetString(array);
					}
					StringBuilder stringBuilder = new StringBuilder(1024);
					return (NativeMethods.FormatMessage(2560u, intPtr, (uint)errorNumber + 20000u, (ushort)num, stringBuilder, (uint)stringBuilder.Capacity, IntPtr.Zero) != 0) ? stringBuilder.ToString().Trim() : null;
				}
				finally
				{
					NativeMethods.FreeLibrary(intPtr);
				}
			}
		}

		public static string GetErrorMessage(Record errorRecord)
		{
			return GetErrorMessage(errorRecord, null);
		}

		public static string GetErrorMessage(Record errorRecord, CultureInfo culture)
		{
			if (errorRecord == null)
			{
				throw new ArgumentNullException("errorRecord");
			}
			int integer;
			if (errorRecord.FieldCount < 1 || (integer = errorRecord.GetInteger(1)) == 0)
			{
				throw new ArgumentOutOfRangeException("errorRecord");
			}
			string text = GetErrorMessage(integer, culture);
			if (text != null)
			{
				errorRecord.FormatString = text;
				text = errorRecord.ToString((IFormatProvider)null);
			}
			return text;
		}

		public static string GetFileVersion(string path)
		{
			StringBuilder stringBuilder = new StringBuilder(20);
			uint cchVersionBuf = 0u;
			uint cchLangBuf = 0u;
			uint num = NativeMethods.MsiGetFileVersion(path, stringBuilder, ref cchVersionBuf, null, ref cchLangBuf);
			if (num == 234)
			{
				stringBuilder.Capacity = checked((int)(++cchVersionBuf));
				num = NativeMethods.MsiGetFileVersion(path, stringBuilder, ref cchVersionBuf, null, ref cchLangBuf);
			}
			switch (num)
			{
			case 2u:
			case 5u:
				throw new FileNotFoundException(null, path);
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
			case 1006u:
				return stringBuilder.ToString();
			}
		}

		public static string GetFileLanguage(string path)
		{
			StringBuilder stringBuilder = new StringBuilder("", 10);
			uint cchVersionBuf = 0u;
			uint cchLangBuf = 0u;
			uint num = NativeMethods.MsiGetFileVersion(path, null, ref cchVersionBuf, stringBuilder, ref cchLangBuf);
			if (num == 234)
			{
				stringBuilder.Capacity = checked((int)(++cchLangBuf));
				num = NativeMethods.MsiGetFileVersion(path, null, ref cchVersionBuf, stringBuilder, ref cchLangBuf);
			}
			switch (num)
			{
			case 2u:
			case 5u:
				throw new FileNotFoundException(null, path);
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
			case 1006u:
				return stringBuilder.ToString();
			}
		}

		public static void GetFileHash(string path, int[] hash)
		{
			if (hash == null)
			{
				throw new ArgumentNullException("hash");
			}
			uint[] array = new uint[5] { 20u, 0u, 0u, 0u, 0u };
			uint num = NativeMethods.MsiGetFileHash(path, 0u, array);
			switch (num)
			{
			case 2u:
			case 5u:
				throw new FileNotFoundException(null, path);
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
			{
				for (int i = 0; i < 4; i = checked(i + 1))
				{
					hash[i] = (int)array[i + 1];
				}
				break;
			}
			}
		}

		public static ShortcutTarget GetShortcutTarget(string shortcut)
		{
			StringBuilder stringBuilder = new StringBuilder(40);
			StringBuilder stringBuilder2 = new StringBuilder(40);
			StringBuilder stringBuilder3 = new StringBuilder(40);
			uint num = NativeMethods.MsiGetShortcutTarget(shortcut, stringBuilder, stringBuilder2, stringBuilder3);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return new ShortcutTarget((stringBuilder.Length > 0) ? stringBuilder.ToString() : null, (stringBuilder2.Length > 0) ? stringBuilder2.ToString() : null, (stringBuilder3.Length > 0) ? stringBuilder3.ToString() : null);
		}

		public static bool VerifyPackage(string packagePath)
		{
			if (string.IsNullOrEmpty(packagePath))
			{
				throw new ArgumentNullException("packagePath");
			}
			if (!File.Exists(packagePath))
			{
				throw new FileNotFoundException(null, packagePath);
			}
			uint num = NativeMethods.MsiVerifyPackage(packagePath);
			switch (num)
			{
			case 1620u:
				return false;
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return true;
			}
		}

		public static IList<string> GetPatchFileList(string productCode, IList<string> patches)
		{
			if (string.IsNullOrEmpty(productCode))
			{
				throw new ArgumentNullException("productCode");
			}
			if (patches == null || patches.Count == 0)
			{
				throw new ArgumentNullException("patches");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string patch in patches)
			{
				if (patch != null)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(';');
					}
					stringBuilder.Append(patch);
				}
			}
			if (stringBuilder.Length == 0)
			{
				throw new ArgumentNullException("patches");
			}
			uint cFiles;
			IntPtr phFileRecords;
			uint num = NativeMethods.MsiGetPatchFileList(productCode, stringBuilder.ToString(), out cFiles, out phFileRecords);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			List<string> list = new List<string>();
			checked
			{
				for (uint num2 = 0u; num2 < cFiles; num2++)
				{
					int num3 = Marshal.ReadInt32(phFileRecords, (int)num2);
					using (Record record = new Record(num3, true, null))
					{
						list.Add(record.GetString(1));
					}
				}
				return list;
			}
		}
	}
}
