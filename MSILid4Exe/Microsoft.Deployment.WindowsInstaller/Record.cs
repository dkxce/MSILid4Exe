using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class Record : InstallerHandle
	{
		private View view;

		private bool isFormatStringInvalid;

		protected internal bool IsFormatStringInvalid
		{
			get
			{
				return isFormatStringInvalid;
			}
			set
			{
				isFormatStringInvalid = value;
			}
		}

		public int FieldCount => checked((int)RemotableNativeMethods.MsiRecordGetFieldCount((int)base.Handle));

		public string FormatString
		{
			get
			{
				return GetString(0);
			}
			set
			{
				SetString(0, value);
			}
		}

		public object this[string fieldName]
		{
			get
			{
				int field = FindColumn(fieldName);
				return this[field];
			}
			set
			{
				int field = FindColumn(fieldName);
				this[field] = value;
			}
		}

		public object this[int field]
		{
			get
			{
				if (field == 0)
				{
					return GetString(0);
				}
				Type type = null;
				if (view != null)
				{
					CheckRange(field);
					type = view.Columns[checked(field - 1)].Type;
				}
				if (type == null || type == typeof(string))
				{
					return GetString(field);
				}
				if (type == typeof(Stream))
				{
					if (!IsNull(field))
					{
						return new RecordStream(this, field);
					}
					return null;
				}
				int? nullableInteger = GetNullableInteger(field);
				if (!nullableInteger.HasValue)
				{
					return null;
				}
				return nullableInteger.Value;
			}
			set
			{
				if (field == 0)
				{
					if (value == null)
					{
						value = string.Empty;
					}
					SetString(0, value.ToString());
					return;
				}
				if (value == null)
				{
					SetNullableInteger(field, null);
					return;
				}
				Type type = value.GetType();
				if (type == typeof(int) || type == typeof(short))
				{
					SetInteger(field, (int)value);
				}
				else if (type.IsSubclassOf(typeof(Stream)))
				{
					SetStream(field, (Stream)value);
				}
				else
				{
					SetString(field, value.ToString());
				}
			}
		}

		public Record(int fieldCount)
			: this((IntPtr)RemotableNativeMethods.MsiCreateRecord(checked((uint)fieldCount), 0), ownsHandle: true, null)
		{
		}

		public Record(params object[] fields)
			: this(fields.Length)
		{
			checked
			{
				for (int i = 0; i < fields.Length; i++)
				{
					this[i + 1] = fields[i];
				}
			}
		}

		internal Record(IntPtr handle, bool ownsHandle, View view)
			: base(handle, ownsHandle)
		{
			this.view = view;
		}

		public static Record FromHandle(IntPtr handle, bool ownsHandle)
		{
			return new Record(handle, ownsHandle, null);
		}

		public void Clear()
		{
			uint num = RemotableNativeMethods.MsiRecordClearData((int)base.Handle);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public bool IsNull(int field)
		{
			CheckRange(field);
			return RemotableNativeMethods.MsiRecordIsNull((int)base.Handle, checked((uint)field));
		}

		public bool IsNull(string fieldName)
		{
			int field = FindColumn(fieldName);
			return IsNull(field);
		}

		public int GetDataSize(int field)
		{
			CheckRange(field);
			return checked((int)RemotableNativeMethods.MsiRecordDataSize((int)base.Handle, (uint)field));
		}

		public int GetDataSize(string fieldName)
		{
			int field = FindColumn(fieldName);
			return GetDataSize(field);
		}

		public int GetInteger(int field)
		{
			CheckRange(field);
			int num = RemotableNativeMethods.MsiRecordGetInteger((int)base.Handle, checked((uint)field));
			if (num == int.MinValue)
			{
				return 0;
			}
			return num;
		}

		public int GetInteger(string fieldName)
		{
			int field = FindColumn(fieldName);
			return GetInteger(field);
		}

		public int? GetNullableInteger(int field)
		{
			CheckRange(field);
			int num = RemotableNativeMethods.MsiRecordGetInteger((int)base.Handle, checked((uint)field));
			if (num == int.MinValue)
			{
				return null;
			}
			return num;
		}

		public int? GetNullableInteger(string fieldName)
		{
			int field = FindColumn(fieldName);
			return GetInteger(field);
		}

		public void SetInteger(int field, int value)
		{
			CheckRange(field);
			uint num = RemotableNativeMethods.MsiRecordSetInteger((int)base.Handle, checked((uint)field), value);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void SetInteger(string fieldName, int value)
		{
			int field = FindColumn(fieldName);
			SetInteger(field, value);
		}

		public void SetNullableInteger(int field, int? value)
		{
			CheckRange(field);
			uint num = RemotableNativeMethods.MsiRecordSetInteger((int)base.Handle, checked((uint)field), value.HasValue ? value.Value : int.MinValue);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void SetNullableInteger(string fieldName, int? value)
		{
			int field = FindColumn(fieldName);
			SetNullableInteger(field, value);
		}

		public string GetString(int field)
		{
			CheckRange(field);
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			uint cchValueBuf = 0u;
			checked
			{
				uint num = RemotableNativeMethods.MsiRecordGetString((int)base.Handle, (uint)field, stringBuilder, ref cchValueBuf);
				if (num == 234)
				{
					stringBuilder.Capacity = (int)(++cchValueBuf);
					num = RemotableNativeMethods.MsiRecordGetString((int)base.Handle, (uint)field, stringBuilder, ref cchValueBuf);
				}
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
				return stringBuilder.ToString();
			}
		}

		public string GetString(string fieldName)
		{
			int field = FindColumn(fieldName);
			return GetString(field);
		}

		public void SetString(int field, string value)
		{
			CheckRange(field);
			if (value == null)
			{
				value = string.Empty;
			}
			uint num = RemotableNativeMethods.MsiRecordSetString((int)base.Handle, checked((uint)field), value);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			if (field == 0)
			{
				IsFormatStringInvalid = false;
			}
		}

		public void SetString(string fieldName, string value)
		{
			int field = FindColumn(fieldName);
			SetString(field, value);
		}

		public void GetStream(int field, string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			IList<TableInfo> list = ((view != null) ? view.Tables : null);
			if (list != null && list.Count == 1 && list[0].Name == "_Storages" && field == FindColumn("Data"))
			{
				if (!view.Database.IsReadOnly)
				{
					throw new NotSupportedException("Database must be opened read-only to support substorage extraction.");
				}
				if (view.Database.FilePath == null)
				{
					throw new NotSupportedException("Database must have an associated file path to support substorage extraction.");
				}
				if (FindColumn("Name") <= 0)
				{
					throw new NotSupportedException("Name column must be part of the Record in order to extract substorage.");
				}
				ExtractSubStorage(view.Database.FilePath, GetString("Name"), filePath);
			}
			else
			{
				if (IsNull(field))
				{
					return;
				}
				Stream stream = null;
				Stream stream2 = null;
				try
				{
					stream = new RecordStream(this, field);
					stream2 = new FileStream(filePath, FileMode.Create, FileAccess.Write);
					int num = 512;
					byte[] array = new byte[num];
					while (num == array.Length)
					{
						if ((num = stream.Read(array, 0, array.Length)) > 0)
						{
							stream2.Write(array, 0, num);
						}
					}
				}
				finally
				{
					stream?.Close();
					stream2?.Close();
				}
			}
		}

		public void GetStream(string fieldName, string filePath)
		{
			int field = FindColumn(fieldName);
			GetStream(field, filePath);
		}

		public Stream GetStream(int field)
		{
			CheckRange(field);
			if (!IsNull(field))
			{
				return new RecordStream(this, field);
			}
			return null;
		}

		public Stream GetStream(string fieldName)
		{
			int field = FindColumn(fieldName);
			return GetStream(field);
		}

		public void SetStream(int field, string filePath)
		{
			CheckRange(field);
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			uint num = RemotableNativeMethods.MsiRecordSetStream((int)base.Handle, checked((uint)field), filePath);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void SetStream(string fieldName, string filePath)
		{
			int field = FindColumn(fieldName);
			SetStream(field, filePath);
		}

		public void SetStream(int field, Stream stream)
		{
			CheckRange(field);
			checked
			{
				if (stream == null)
				{
					uint num = RemotableNativeMethods.MsiRecordSetStream((int)base.Handle, (uint)field, null);
					if (num != 0)
					{
						throw InstallerException.ExceptionFromReturnCode(num);
					}
					return;
				}
				Stream stream2 = null;
				string tempFileName = Path.GetTempFileName();
				try
				{
					stream2 = new FileStream(tempFileName, FileMode.Truncate, FileAccess.Write);
					byte[] array = new byte[512];
					int count;
					while ((count = stream.Read(array, 0, array.Length)) > 0)
					{
						stream2.Write(array, 0, count);
					}
					stream2.Close();
					stream2 = null;
					uint num2 = RemotableNativeMethods.MsiRecordSetStream((int)base.Handle, (uint)field, tempFileName);
					if (num2 != 0)
					{
						throw InstallerException.ExceptionFromReturnCode(num2);
					}
				}
				finally
				{
					stream2?.Close();
					if (File.Exists(tempFileName))
					{
						try
						{
							File.Delete(tempFileName);
						}
						catch (IOException)
						{
							if (view != null)
							{
								view.Database.DeleteOnClose(tempFileName);
							}
						}
					}
				}
			}
		}

		public void SetStream(string fieldName, Stream stream)
		{
			int field = FindColumn(fieldName);
			SetStream(field, stream);
		}

		public override string ToString()
		{
			return ToString((IFormatProvider)null);
		}

		public string ToString(IFormatProvider provider)
		{
			if (IsFormatStringInvalid)
			{
				return string.Empty;
			}
			int hInstall = ((provider is InstallerHandle installerHandle) ? ((int)installerHandle.Handle) : 0);
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			uint cchResultBuf = 1u;
			uint num = RemotableNativeMethods.MsiFormatRecord(hInstall, (int)base.Handle, stringBuilder, ref cchResultBuf);
			checked
			{
				if (num == 234)
				{
					cchResultBuf++;
					stringBuilder = new StringBuilder((int)cchResultBuf);
					num = RemotableNativeMethods.MsiFormatRecord(hInstall, (int)base.Handle, stringBuilder, ref cchResultBuf);
				}
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
				return stringBuilder.ToString();
			}
		}

		[Obsolete("This method is obsolete because it has undesirable side-effects. As an alternative, set the FormatString property separately before calling the ToString() override that takes no parameters.")]
		public string ToString(string format)
		{
			return ToString(format, null);
		}

		[Obsolete("This method is obsolete because it has undesirable side-effects. As an alternative, set the FormatString property separately before calling the ToString() override that takes just a format provider.")]
		public string ToString(string format, IFormatProvider provider)
		{
			if (format == null)
			{
				return ToString(provider);
			}
			if (format.Length == 0)
			{
				return string.Empty;
			}
			string formatString = (string)this[0];
			try
			{
				FormatString = format;
				return ToString(provider);
			}
			finally
			{
				FormatString = formatString;
			}
		}

		private static void ExtractSubStorage(string databaseFile, string storageName, string extractFile)
		{
			NativeMethods.STGM grfMode = NativeMethods.STGM.SHARE_DENY_WRITE;
			int num = NativeMethods.StgOpenStorage(databaseFile, IntPtr.Zero, (uint)grfMode, IntPtr.Zero, 0u, out var stgOpen);
			if (num != 0)
			{
				Marshal.ThrowExceptionForHR(num);
			}
			try
			{
				grfMode = NativeMethods.STGM.SHARE_EXCLUSIVE;
				IStorage storage = stgOpen.OpenStorage(storageName, IntPtr.Zero, (uint)grfMode, IntPtr.Zero, 0u);
				try
				{
					grfMode = NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE | NativeMethods.STGM.CREATE;
					num = NativeMethods.StgCreateDocfile(extractFile, (uint)grfMode, 0u, out var stgOpen2);
					if (num != 0)
					{
						Marshal.ThrowExceptionForHR(num);
					}
					try
					{
						storage.CopyTo(0u, IntPtr.Zero, IntPtr.Zero, stgOpen2);
						stgOpen2.Commit(0u);
					}
					finally
					{
						Marshal.ReleaseComObject(stgOpen2);
					}
				}
				finally
				{
					Marshal.ReleaseComObject(storage);
				}
			}
			finally
			{
				Marshal.ReleaseComObject(stgOpen);
			}
		}

		private int FindColumn(string fieldName)
		{
			if (view == null)
			{
				throw new InvalidOperationException();
			}
			ColumnCollection columns = view.Columns;
			checked
			{
				for (int i = 0; i < columns.Count; i++)
				{
					if (columns[i].Name == fieldName)
					{
						return i + 1;
					}
				}
				throw new ArgumentOutOfRangeException("fieldName");
			}
		}

		private void CheckRange(int field)
		{
			if (field < 0 || field > FieldCount)
			{
				throw new ArgumentOutOfRangeException("field");
			}
		}
	}
}
