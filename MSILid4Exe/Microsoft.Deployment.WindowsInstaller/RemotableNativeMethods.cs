using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal static class RemotableNativeMethods
	{
		private const int MAX_REQUEST_FIELDS = 4;

		private static int requestFieldDataOffset;

		private static int requestFieldSize;

		private static IntPtr requestBuf;

		private static MsiRemoteInvoke remotingDelegate;

		internal static bool RemotingEnabled => remotingDelegate != null;

		internal static MsiRemoteInvoke RemotingDelegate
		{
			set
			{
				remotingDelegate = value;
				checked
				{
					if (value != null && requestBuf == IntPtr.Zero)
					{
						requestFieldDataOffset = Marshal.SizeOf(typeof(IntPtr));
						requestFieldSize = 2 * Marshal.SizeOf(typeof(IntPtr));
						requestBuf = Marshal.AllocHGlobal(requestFieldSize * 4);
					}
				}
			}
		}

		internal static bool IsRemoteHandle(int handle)
		{
			return (handle & int.MinValue) != 0;
		}

		internal static int MakeRemoteHandle(int handle)
		{
			if (handle == 0)
			{
				return handle;
			}
			if (IsRemoteHandle(handle))
			{
				throw new InvalidOperationException("Handle already has the remote bit set.");
			}
			return handle ^ int.MinValue;
		}

		internal static int GetRemoteHandle(int handle)
		{
			if (handle == 0)
			{
				return handle;
			}
			if (!IsRemoteHandle(handle))
			{
				throw new InvalidOperationException("Handle does not have the remote bit set.");
			}
			return handle ^ int.MinValue;
		}

		private static void ClearData(IntPtr buf)
		{
			checked
			{
				for (int i = 0; i < 4; i++)
				{
					Marshal.WriteInt32(buf, i * requestFieldSize, 1);
					Marshal.WriteIntPtr(buf, i * requestFieldSize + requestFieldDataOffset, IntPtr.Zero);
				}
			}
		}

		private static void WriteInt(IntPtr buf, int field, int value)
		{
			checked
			{
				Marshal.WriteInt32(buf, field * requestFieldSize, 3);
				Marshal.WriteInt32(buf, field * requestFieldSize + requestFieldDataOffset, value);
			}
		}

		private static void WriteString(IntPtr buf, int field, string value)
		{
			checked
			{
				if (value == null)
				{
					Marshal.WriteInt32(buf, field * requestFieldSize, 1);
					Marshal.WriteIntPtr(buf, field * requestFieldSize + requestFieldDataOffset, IntPtr.Zero);
				}
				else
				{
					IntPtr val = Marshal.StringToHGlobalUni(value);
					Marshal.WriteInt32(buf, field * requestFieldSize, 31);
					Marshal.WriteIntPtr(buf, field * requestFieldSize + requestFieldDataOffset, val);
				}
			}
		}

		private static int ReadInt(IntPtr buf, int field)
		{
			checked
			{
				switch (Marshal.ReadInt32(buf, field * requestFieldSize))
				{
				case 0:
					return 0;
				default:
					throw new InstallerException("Invalid data received from remote MSI function invocation.");
				case 3:
				case 19:
					return Marshal.ReadInt32(buf, field * requestFieldSize + requestFieldDataOffset);
				}
			}
		}

		private static void ReadString(IntPtr buf, int field, StringBuilder szBuf, ref uint cchBuf)
		{
			checked
			{
				switch (Marshal.ReadInt32(buf, field * requestFieldSize))
				{
				case 1:
					szBuf.Remove(0, szBuf.Length);
					break;
				default:
					throw new InstallerException("Invalid data received from remote MSI function invocation.");
				case 31:
				{
					szBuf.Remove(0, szBuf.Length);
					string text = Marshal.PtrToStringUni(Marshal.ReadIntPtr(buf, field * requestFieldSize + requestFieldDataOffset));
					if (text != null)
					{
						szBuf.Append(text);
					}
					cchBuf = (uint)szBuf.Length;
					break;
				}
				}
			}
		}

		private static void FreeString(IntPtr buf, int field)
		{
			IntPtr intPtr = Marshal.ReadIntPtr(buf, checked(field * requestFieldSize + requestFieldDataOffset));
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		private static void ReadStream(IntPtr buf, int field, byte[] sBuf, int count)
		{
			checked
			{
				if (Marshal.ReadInt32(buf, field * requestFieldSize) != 66)
				{
					throw new InstallerException("Invalid data received from remote MSI function invocation.");
				}
				Marshal.Copy(Marshal.ReadIntPtr(buf, field * requestFieldSize + requestFieldDataOffset), sBuf, 0, count);
			}
		}

		private static uint MsiFunc_III(RemoteMsiFunctionId id, int in1, int in2, int in3)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteInt(requestBuf, 1, in2);
				WriteInt(requestBuf, 2, in3);
				remotingDelegate(id, requestBuf, out var response);
				return (uint)ReadInt(response, 0);
			}
		}

		private static uint MsiFunc_IIS(RemoteMsiFunctionId id, int in1, int in2, string in3)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteInt(requestBuf, 1, in2);
				WriteString(requestBuf, 2, in3);
				remotingDelegate(id, requestBuf, out var response);
				FreeString(requestBuf, 2);
				return (uint)ReadInt(response, 0);
			}
		}

		private static uint MsiFunc_ISI(RemoteMsiFunctionId id, int in1, string in2, int in3)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteString(requestBuf, 1, in2);
				WriteInt(requestBuf, 2, in3);
				remotingDelegate(id, requestBuf, out var response);
				FreeString(requestBuf, 2);
				return (uint)ReadInt(response, 0);
			}
		}

		private static uint MsiFunc_ISS(RemoteMsiFunctionId id, int in1, string in2, string in3)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteString(requestBuf, 1, in2);
				WriteString(requestBuf, 2, in3);
				remotingDelegate(id, requestBuf, out var response);
				FreeString(requestBuf, 1);
				FreeString(requestBuf, 2);
				return (uint)ReadInt(response, 0);
			}
		}

		private static uint MsiFunc_II_I(RemoteMsiFunctionId id, int in1, int in2, out int out1)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteInt(requestBuf, 1, in2);
				remotingDelegate(id, requestBuf, out var response);
				int result = ReadInt(response, 0);
				out1 = ReadInt(response, 1);
				return (uint)result;
			}
		}

		private static uint MsiFunc_ISII_I(RemoteMsiFunctionId id, int in1, string in2, int in3, int in4, out int out1)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteString(requestBuf, 1, in2);
				WriteInt(requestBuf, 2, in3);
				WriteInt(requestBuf, 3, in4);
				remotingDelegate(id, requestBuf, out var response);
				FreeString(requestBuf, 1);
				int result = ReadInt(response, 0);
				out1 = ReadInt(response, 1);
				return (uint)result;
			}
		}

		private static uint MsiFunc_IS_II(RemoteMsiFunctionId id, int in1, string in2, out int out1, out int out2)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteString(requestBuf, 1, in2);
				remotingDelegate(id, requestBuf, out var response);
				FreeString(requestBuf, 1);
				int result = ReadInt(response, 0);
				out1 = ReadInt(response, 1);
				out2 = ReadInt(response, 2);
				return (uint)result;
			}
		}

		private static uint MsiFunc_II_S(RemoteMsiFunctionId id, int in1, int in2, StringBuilder out1, ref uint cchOut1)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteInt(requestBuf, 1, in2);
				remotingDelegate(id, requestBuf, out var response);
				int num = ReadInt(response, 0);
				if (num == 0)
				{
					ReadString(response, 1, out1, ref cchOut1);
				}
				return (uint)num;
			}
		}

		private static uint MsiFunc_IS_S(RemoteMsiFunctionId id, int in1, string in2, StringBuilder out1, ref uint cchOut1)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteString(requestBuf, 1, in2);
				remotingDelegate(id, requestBuf, out var response);
				FreeString(requestBuf, 1);
				int num = ReadInt(response, 0);
				if (num == 0)
				{
					ReadString(response, 1, out1, ref cchOut1);
				}
				return (uint)num;
			}
		}

		private static uint MsiFunc_ISII_SII(RemoteMsiFunctionId id, int in1, string in2, int in3, int in4, StringBuilder out1, ref uint cchOut1, out int out2, out int out3)
		{
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, in1);
				WriteString(requestBuf, 1, in2);
				WriteInt(requestBuf, 2, in3);
				WriteInt(requestBuf, 3, in4);
				remotingDelegate(id, requestBuf, out var response);
				FreeString(requestBuf, 1);
				int num = ReadInt(response, 0);
				if (num == 0)
				{
					ReadString(response, 1, out1, ref cchOut1);
				}
				out2 = ReadInt(response, 2);
				out3 = ReadInt(response, 3);
				return (uint)num;
			}
		}

		internal static int MsiProcessMessage(int hInstall, uint eMessageType, int hRecord)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiProcessMessage(hInstall, eMessageType, hRecord);
			}
			lock (remotingDelegate)
			{
				IntPtr intPtr = Marshal.AllocHGlobal(checked(requestFieldSize * 4));
				ClearData(intPtr);
				WriteInt(intPtr, 0, GetRemoteHandle(hInstall));
				WriteInt(intPtr, 1, (int)eMessageType);
				WriteInt(intPtr, 2, GetRemoteHandle(hRecord));
				remotingDelegate(RemoteMsiFunctionId.MsiProcessMessage, intPtr, out var response);
				Marshal.FreeHGlobal(intPtr);
				return ReadInt(response, 0);
			}
		}

		internal static uint MsiCloseHandle(int hAny)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hAny))
			{
				return NativeMethods.MsiCloseHandle(hAny);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiCloseHandle, GetRemoteHandle(hAny), 0, 0);
		}

		internal static uint MsiGetProperty(int hInstall, string szName, StringBuilder szValueBuf, ref uint cchValueBuf)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetProperty(hInstall, szName, szValueBuf, ref cchValueBuf);
			}
			return MsiFunc_IS_S(RemoteMsiFunctionId.MsiGetProperty, GetRemoteHandle(hInstall), szName, szValueBuf, ref cchValueBuf);
		}

		internal static uint MsiSetProperty(int hInstall, string szName, string szValue)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiSetProperty(hInstall, szName, szValue);
			}
			return MsiFunc_ISS(RemoteMsiFunctionId.MsiSetProperty, GetRemoteHandle(hInstall), szName, szValue);
		}

		internal static int MsiCreateRecord(uint cParams, int hAny)
		{
			if (!RemotingEnabled || (hAny != 0 && !IsRemoteHandle(hAny)))
			{
				return NativeMethods.MsiCreateRecord(cParams);
			}
			return MakeRemoteHandle((int)MsiFunc_III(RemoteMsiFunctionId.MsiCreateRecord, (int)cParams, 0, 0));
		}

		internal static uint MsiRecordGetFieldCount(int hRecord)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordGetFieldCount(hRecord);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiRecordGetFieldCount, GetRemoteHandle(hRecord), 0, 0);
		}

		internal static int MsiRecordGetInteger(int hRecord, uint iField)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordGetInteger(hRecord, iField);
			}
			return (int)MsiFunc_III(RemoteMsiFunctionId.MsiRecordGetInteger, GetRemoteHandle(hRecord), (int)iField, 0);
		}

		internal static uint MsiRecordGetString(int hRecord, uint iField, StringBuilder szValueBuf, ref uint cchValueBuf)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordGetString(hRecord, iField, szValueBuf, ref cchValueBuf);
			}
			return MsiFunc_II_S(RemoteMsiFunctionId.MsiRecordGetString, GetRemoteHandle(hRecord), checked((int)iField), szValueBuf, ref cchValueBuf);
		}

		internal static uint MsiRecordSetInteger(int hRecord, uint iField, int iValue)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordSetInteger(hRecord, iField, iValue);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiRecordSetInteger, GetRemoteHandle(hRecord), checked((int)iField), iValue);
		}

		internal static uint MsiRecordSetString(int hRecord, uint iField, string szValue)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordSetString(hRecord, iField, szValue);
			}
			return MsiFunc_IIS(RemoteMsiFunctionId.MsiRecordSetString, GetRemoteHandle(hRecord), checked((int)iField), szValue);
		}

		internal static int MsiGetActiveDatabase(int hInstall)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetActiveDatabase(hInstall);
			}
			return MakeRemoteHandle(checked((int)MsiFunc_III(RemoteMsiFunctionId.MsiGetActiveDatabase, GetRemoteHandle(hInstall), 0, 0)));
		}

		internal static uint MsiDatabaseOpenView(int hDatabase, string szQuery, out int hView)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hDatabase))
			{
				return NativeMethods.MsiDatabaseOpenView(hDatabase, szQuery, out hView);
			}
			uint result = MsiFunc_ISII_I(RemoteMsiFunctionId.MsiDatabaseOpenView, GetRemoteHandle(hDatabase), szQuery, 0, 0, out hView);
			hView = MakeRemoteHandle(hView);
			return result;
		}

		internal static uint MsiViewExecute(int hView, int hRecord)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hView))
			{
				return NativeMethods.MsiViewExecute(hView, hRecord);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiViewExecute, GetRemoteHandle(hView), GetRemoteHandle(hRecord), 0);
		}

		internal static uint MsiViewFetch(int hView, out int hRecord)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hView))
			{
				return NativeMethods.MsiViewFetch(hView, out hRecord);
			}
			uint result = MsiFunc_II_I(RemoteMsiFunctionId.MsiViewFetch, GetRemoteHandle(hView), 0, out hRecord);
			hRecord = MakeRemoteHandle(hRecord);
			return result;
		}

		internal static uint MsiViewModify(int hView, int iModifyMode, int hRecord)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hView))
			{
				return NativeMethods.MsiViewModify(hView, iModifyMode, hRecord);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiViewModify, GetRemoteHandle(hView), iModifyMode, GetRemoteHandle(hRecord));
		}

		internal static int MsiViewGetError(int hView, StringBuilder szColumnNameBuffer, ref uint cchBuf)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hView))
			{
				return NativeMethods.MsiViewGetError(hView, szColumnNameBuffer, ref cchBuf);
			}
			return (int)MsiFunc_II_S(RemoteMsiFunctionId.MsiViewGetError, GetRemoteHandle(hView), 0, szColumnNameBuffer, ref cchBuf);
		}

		internal static uint MsiViewGetColumnInfo(int hView, uint eColumnInfo, out int hRecord)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hView))
			{
				return NativeMethods.MsiViewGetColumnInfo(hView, eColumnInfo, out hRecord);
			}
			uint result = MsiFunc_II_I(RemoteMsiFunctionId.MsiViewGetColumnInfo, GetRemoteHandle(hView), checked((int)eColumnInfo), out hRecord);
			hRecord = MakeRemoteHandle(hRecord);
			return result;
		}

		internal static uint MsiFormatRecord(int hInstall, int hRecord, StringBuilder szResultBuf, ref uint cchResultBuf)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiFormatRecord(hInstall, hRecord, szResultBuf, ref cchResultBuf);
			}
			return MsiFunc_II_S(RemoteMsiFunctionId.MsiFormatRecord, GetRemoteHandle(hInstall), GetRemoteHandle(hRecord), szResultBuf, ref cchResultBuf);
		}

		internal static uint MsiRecordClearData(int hRecord)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordClearData(hRecord);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiRecordClearData, GetRemoteHandle(hRecord), 0, 0);
		}

		internal static bool MsiRecordIsNull(int hRecord, uint iField)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordIsNull(hRecord, iField);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiRecordIsNull, GetRemoteHandle(hRecord), checked((int)iField), 0) != 0;
		}

		internal static uint MsiDatabaseGetPrimaryKeys(int hDatabase, string szTableName, out int hRecord)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hDatabase))
			{
				return NativeMethods.MsiDatabaseGetPrimaryKeys(hDatabase, szTableName, out hRecord);
			}
			uint result = MsiFunc_ISII_I(RemoteMsiFunctionId.MsiDatabaseGetPrimaryKeys, GetRemoteHandle(hDatabase), szTableName, 0, 0, out hRecord);
			hRecord = MakeRemoteHandle(hRecord);
			return result;
		}

		internal static uint MsiDatabaseIsTablePersistent(int hDatabase, string szTableName)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hDatabase))
			{
				return NativeMethods.MsiDatabaseIsTablePersistent(hDatabase, szTableName);
			}
			return MsiFunc_ISI(RemoteMsiFunctionId.MsiDatabaseIsTablePersistent, GetRemoteHandle(hDatabase), szTableName, 0);
		}

		internal static uint MsiDoAction(int hInstall, string szAction)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiDoAction(hInstall, szAction);
			}
			return MsiFunc_ISI(RemoteMsiFunctionId.MsiDoAction, GetRemoteHandle(hInstall), szAction, 0);
		}

		internal static uint MsiEnumComponentCosts(int hInstall, string szComponent, uint dwIndex, int iState, StringBuilder lpDriveBuf, ref uint cchDriveBuf, out int iCost, out int iTempCost)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiEnumComponentCosts(hInstall, szComponent, dwIndex, iState, lpDriveBuf, ref cchDriveBuf, out iCost, out iTempCost);
			}
			return MsiFunc_ISII_SII(RemoteMsiFunctionId.MsiEvaluateCondition, GetRemoteHandle(hInstall), szComponent, checked((int)dwIndex), iState, lpDriveBuf, ref cchDriveBuf, out iCost, out iTempCost);
		}

		internal static uint MsiEvaluateCondition(int hInstall, string szCondition)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiEvaluateCondition(hInstall, szCondition);
			}
			return MsiFunc_ISI(RemoteMsiFunctionId.MsiEvaluateCondition, GetRemoteHandle(hInstall), szCondition, 0);
		}

		internal static uint MsiGetComponentState(int hInstall, string szComponent, out int iInstalled, out int iAction)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetComponentState(hInstall, szComponent, out iInstalled, out iAction);
			}
			return MsiFunc_IS_II(RemoteMsiFunctionId.MsiGetComponentState, GetRemoteHandle(hInstall), szComponent, out iInstalled, out iAction);
		}

		internal static uint MsiGetFeatureCost(int hInstall, string szFeature, int iCostTree, int iState, out int iCost)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetFeatureCost(hInstall, szFeature, iCostTree, iState, out iCost);
			}
			return MsiFunc_ISII_I(RemoteMsiFunctionId.MsiGetFeatureCost, GetRemoteHandle(hInstall), szFeature, iCostTree, iState, out iCost);
		}

		internal static uint MsiGetFeatureState(int hInstall, string szFeature, out int iInstalled, out int iAction)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetFeatureState(hInstall, szFeature, out iInstalled, out iAction);
			}
			return MsiFunc_IS_II(RemoteMsiFunctionId.MsiGetFeatureState, GetRemoteHandle(hInstall), szFeature, out iInstalled, out iAction);
		}

		internal static uint MsiGetFeatureValidStates(int hInstall, string szFeature, out uint dwInstalledState)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetFeatureValidStates(hInstall, szFeature, out dwInstalledState);
			}
			int @out;
			uint result = MsiFunc_ISII_I(RemoteMsiFunctionId.MsiGetFeatureValidStates, GetRemoteHandle(hInstall), szFeature, 0, 0, out @out);
			dwInstalledState = checked((uint)@out);
			return result;
		}

		internal static int MsiGetLanguage(int hInstall)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetLanguage(hInstall);
			}
			return (int)MsiFunc_III(RemoteMsiFunctionId.MsiGetLanguage, GetRemoteHandle(hInstall), 0, 0);
		}

		internal static int MsiGetLastErrorRecord(int hAny)
		{
			if (!RemotingEnabled || (hAny != 0 && !IsRemoteHandle(hAny)))
			{
				return NativeMethods.MsiGetLastErrorRecord();
			}
			return MakeRemoteHandle((int)MsiFunc_III(RemoteMsiFunctionId.MsiGetLastErrorRecord, 0, 0, 0));
		}

		internal static bool MsiGetMode(int hInstall, uint iRunMode)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetMode(hInstall, iRunMode);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiGetMode, GetRemoteHandle(hInstall), checked((int)iRunMode), 0) != 0;
		}

		internal static uint MsiGetSourcePath(int hInstall, string szFolder, StringBuilder szPathBuf, ref uint cchPathBuf)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetSourcePath(hInstall, szFolder, szPathBuf, ref cchPathBuf);
			}
			return MsiFunc_IS_S(RemoteMsiFunctionId.MsiGetSourcePath, GetRemoteHandle(hInstall), szFolder, szPathBuf, ref cchPathBuf);
		}

		internal static uint MsiGetSummaryInformation(int hDatabase, string szDatabasePath, uint uiUpdateCount, out int hSummaryInfo)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hDatabase))
			{
				return NativeMethods.MsiGetSummaryInformation(hDatabase, szDatabasePath, uiUpdateCount, out hSummaryInfo);
			}
			uint result = MsiFunc_ISII_I(RemoteMsiFunctionId.MsiGetSummaryInformation, GetRemoteHandle(hDatabase), szDatabasePath, checked((int)uiUpdateCount), 0, out hSummaryInfo);
			hSummaryInfo = MakeRemoteHandle(hSummaryInfo);
			return result;
		}

		internal static uint MsiGetTargetPath(int hInstall, string szFolder, StringBuilder szPathBuf, ref uint cchPathBuf)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiGetTargetPath(hInstall, szFolder, szPathBuf, ref cchPathBuf);
			}
			return MsiFunc_IS_S(RemoteMsiFunctionId.MsiGetTargetPath, GetRemoteHandle(hInstall), szFolder, szPathBuf, ref cchPathBuf);
		}

		internal static uint MsiRecordDataSize(int hRecord, uint iField)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordDataSize(hRecord, iField);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiRecordDataSize, GetRemoteHandle(hRecord), checked((int)iField), 0);
		}

		internal static uint MsiRecordReadStream(int hRecord, uint iField, byte[] szDataBuf, ref uint cbDataBuf)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordReadStream(hRecord, iField, szDataBuf, ref cbDataBuf);
			}
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, GetRemoteHandle(hRecord));
				WriteInt(requestBuf, 1, (int)iField);
				WriteInt(requestBuf, 2, (int)cbDataBuf);
				remotingDelegate(RemoteMsiFunctionId.MsiRecordReadStream, requestBuf, out var response);
				int num = ReadInt(response, 0);
				if (num == 0)
				{
					cbDataBuf = (uint)ReadInt(response, 2);
					if (cbDataBuf != 0)
					{
						ReadStream(response, 1, szDataBuf, (int)cbDataBuf);
					}
				}
				return (uint)num;
			}
		}

		internal static uint MsiRecordSetStream(int hRecord, uint iField, string szFilePath)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hRecord))
			{
				return NativeMethods.MsiRecordSetStream(hRecord, iField, szFilePath);
			}
			return MsiFunc_IIS(RemoteMsiFunctionId.MsiRecordSetStream, GetRemoteHandle(hRecord), checked((int)iField), szFilePath);
		}

		internal static uint MsiSequence(int hInstall, string szTable, int iSequenceMode)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiSequence(hInstall, szTable, iSequenceMode);
			}
			return MsiFunc_ISI(RemoteMsiFunctionId.MsiSequence, GetRemoteHandle(hInstall), szTable, iSequenceMode);
		}

		internal static uint MsiSetComponentState(int hInstall, string szComponent, int iState)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiSetComponentState(hInstall, szComponent, iState);
			}
			return MsiFunc_ISI(RemoteMsiFunctionId.MsiSetComponentState, GetRemoteHandle(hInstall), szComponent, iState);
		}

		internal static uint MsiSetFeatureAttributes(int hInstall, string szFeature, uint dwAttributes)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiSetFeatureAttributes(hInstall, szFeature, dwAttributes);
			}
			return MsiFunc_ISI(RemoteMsiFunctionId.MsiSetFeatureAttributes, GetRemoteHandle(hInstall), szFeature, checked((int)dwAttributes));
		}

		internal static uint MsiSetFeatureState(int hInstall, string szFeature, int iState)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiSetFeatureState(hInstall, szFeature, iState);
			}
			return MsiFunc_ISI(RemoteMsiFunctionId.MsiSetFeatureState, GetRemoteHandle(hInstall), szFeature, iState);
		}

		internal static uint MsiSetInstallLevel(int hInstall, int iInstallLevel)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiSetInstallLevel(hInstall, iInstallLevel);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiSetInstallLevel, GetRemoteHandle(hInstall), iInstallLevel, 0);
		}

		internal static uint MsiSetMode(int hInstall, uint iRunMode, bool fState)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiSetMode(hInstall, iRunMode, fState);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiSetMode, GetRemoteHandle(hInstall), checked((int)iRunMode), fState ? 1 : 0);
		}

		internal static uint MsiSetTargetPath(int hInstall, string szFolder, string szFolderPath)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiSetTargetPath(hInstall, szFolder, szFolderPath);
			}
			return MsiFunc_ISS(RemoteMsiFunctionId.MsiSetTargetPath, GetRemoteHandle(hInstall), szFolder, szFolderPath);
		}

		internal static uint MsiSummaryInfoGetProperty(int hSummaryInfo, uint uiProperty, out uint uiDataType, out int iValue, ref long ftValue, StringBuilder szValueBuf, ref uint cchValueBuf)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hSummaryInfo))
			{
				return NativeMethods.MsiSummaryInfoGetProperty(hSummaryInfo, uiProperty, out uiDataType, out iValue, ref ftValue, szValueBuf, ref cchValueBuf);
			}
			lock (remotingDelegate)
			{
				ClearData(requestBuf);
				WriteInt(requestBuf, 0, GetRemoteHandle(hSummaryInfo));
				WriteInt(requestBuf, 1, checked((int)uiProperty));
				remotingDelegate(RemoteMsiFunctionId.MsiSummaryInfoGetProperty, requestBuf, out var response);
				uint num = (uint)ReadInt(response, 0);
				if (num == 0)
				{
					uiDataType = (uint)ReadInt(response, 1);
					switch (uiDataType)
					{
					case 2u:
					case 3u:
						iValue = ReadInt(response, 2);
						break;
					case 64u:
					{
						uint num2 = (uint)ReadInt(response, 2);
						uint num3 = (uint)ReadInt(response, 3);
						ftValue = (long)(((ulong)num2 << 32) | num3);
						iValue = 0;
						break;
					}
					case 30u:
						ReadString(response, 2, szValueBuf, ref cchValueBuf);
						iValue = 0;
						break;
					default:
						iValue = 0;
						break;
					}
				}
				else
				{
					uiDataType = 0u;
					iValue = 0;
				}
				return num;
			}
		}

		internal static uint MsiVerifyDiskSpace(int hInstall)
		{
			if (!RemotingEnabled || !IsRemoteHandle(hInstall))
			{
				return NativeMethods.MsiVerifyDiskSpace(hInstall);
			}
			return MsiFunc_III(RemoteMsiFunctionId.MsiVerifyDiskSpace, GetRemoteHandle(hInstall), 0, 0);
		}
	}
}
