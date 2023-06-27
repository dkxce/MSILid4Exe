using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public sealed class ColumnCollection : ICollection<ColumnInfo>, IEnumerable<ColumnInfo>, IEnumerable
	{
		private IList<ColumnInfo> columns;

		private string formatString;

		public int Count => columns.Count;

		public bool IsReadOnly => true;

		public ColumnInfo this[int columnIndex]
		{
			get
			{
				if (columnIndex >= 0 && columnIndex < columns.Count)
				{
					return columns[columnIndex];
				}
				throw new ArgumentOutOfRangeException("columnIndex");
			}
		}

		public ColumnInfo this[string columnName]
		{
			get
			{
				if (string.IsNullOrEmpty(columnName))
				{
					throw new ArgumentNullException("columnName");
				}
				foreach (ColumnInfo column in columns)
				{
					if (column.Name == columnName)
					{
						return column;
					}
				}
				throw new ArgumentOutOfRangeException("columnName");
			}
		}

		public string FormatString
		{
			get
			{
				if (formatString == null)
				{
					formatString = CreateFormatString(columns);
				}
				return formatString;
			}
		}

		public ColumnCollection(ICollection<ColumnInfo> columns)
		{
			if (columns == null)
			{
				throw new ArgumentNullException("columns");
			}
			this.columns = new List<ColumnInfo>(columns);
		}

		internal ColumnCollection(View view)
		{
			if (view == null)
			{
				throw new ArgumentNullException("view");
			}
			columns = GetViewColumns(view);
		}

		public void Add(ColumnInfo item)
		{
			throw new InvalidOperationException();
		}

		public void Clear()
		{
			throw new InvalidOperationException();
		}

		public bool Contains(string columnName)
		{
			return IndexOf(columnName) >= 0;
		}

		bool ICollection<ColumnInfo>.Contains(ColumnInfo column)
		{
			return Contains(column.Name);
		}

		public int IndexOf(string columnName)
		{
			if (string.IsNullOrEmpty(columnName))
			{
				throw new ArgumentNullException("columnName");
			}
			for (int i = 0; i < columns.Count; i = checked(i + 1))
			{
				if (columns[i].Name == columnName)
				{
					return i;
				}
			}
			return -1;
		}

		public void CopyTo(ColumnInfo[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			columns.CopyTo(array, arrayIndex);
		}

		bool ICollection<ColumnInfo>.Remove(ColumnInfo column)
		{
			throw new InvalidOperationException();
		}

		public IEnumerator<ColumnInfo> GetEnumerator()
		{
			return columns.GetEnumerator();
		}

		private static string CreateFormatString(IList<ColumnInfo> columns)
		{
			StringBuilder stringBuilder = new StringBuilder();
			checked
			{
				for (int i = 0; i < columns.Count; i++)
				{
					if (columns[i].Type == typeof(Stream))
					{
						stringBuilder.AppendFormat("{0} = [Binary Data]", columns[i].Name);
					}
					else
					{
						stringBuilder.AppendFormat("{0} = [{1}]", columns[i].Name, i + 1);
					}
					if (i < columns.Count - 1)
					{
						stringBuilder.Append(", ");
					}
				}
				return stringBuilder.ToString();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static IList<ColumnInfo> GetViewColumns(View view)
		{
			IList<string> viewColumns = GetViewColumns(view, types: false);
			IList<string> viewColumns2 = GetViewColumns(view, types: true);
			int num = viewColumns.Count;
			checked
			{
				if (viewColumns2[num - 1] == "O0")
				{
					num--;
				}
				IList<ColumnInfo> list = new List<ColumnInfo>(num);
				for (int i = 0; i < num; i++)
				{
					list.Add(new ColumnInfo(viewColumns[i], viewColumns2[i]));
				}
				return list;
			}
		}

		private static IList<string> GetViewColumns(View view, bool types)
		{
			int num = (types ? 1 : 0);
			checked
			{
				uint num2 = RemotableNativeMethods.MsiViewGetColumnInfo((int)view.Handle, (uint)num, out var hRecord);
				if (num2 != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num2);
				}
				using (Record record = new Record((IntPtr)hRecord, ownsHandle: true, null))
				{
					int fieldCount = record.FieldCount;
					IList<string> list = new List<string>(fieldCount);
					for (int i = 1; i <= fieldCount; i++)
					{
						uint cchValueBuf = 256u;
						StringBuilder stringBuilder = new StringBuilder((int)cchValueBuf);
						num2 = RemotableNativeMethods.MsiRecordGetString((int)record.Handle, (uint)i, stringBuilder, ref cchValueBuf);
						if (num2 != 0)
						{
							throw InstallerException.ExceptionFromReturnCode(num2);
						}
						list.Add(stringBuilder.ToString());
					}
					return list;
				}
			}
		}
	}
}
