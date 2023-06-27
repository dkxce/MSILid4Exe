using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class View : InstallerHandle, IEnumerable<Record>, IEnumerable
	{
		private Database database;

		private string sql;

		private IList<TableInfo> tables;

		private ColumnCollection columns;

		public Database Database => database;

		public string QueryString => sql;

		public IList<TableInfo> Tables
		{
			get
			{
				checked
				{
					if (tables == null)
					{
						if (sql == null)
						{
							return null;
						}
						string text = sql.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
						string text2 = text.ToUpper(CultureInfo.InvariantCulture);
						string[] array = new string[3] { " FROM ", " INTO ", " TABLE " };
						string[] array2 = new string[5] { " WHERE ", " ORDER ", " SET ", " (", " ADD " };
						for (int i = 0; i < array.Length; i++)
						{
							int num;
							if ((num = text2.IndexOf(array[i], StringComparison.Ordinal)) > 0)
							{
								text = text.Substring(num + array[i].Length);
								text2 = text2.Substring(num + array[i].Length);
							}
						}
						if (text2.StartsWith("UPDATE ", StringComparison.Ordinal))
						{
							text = text.Substring(7);
							text2 = text2.Substring(7);
						}
						for (int j = 0; j < array2.Length; j++)
						{
							int num;
							if ((num = text2.IndexOf(array2[j], StringComparison.Ordinal)) > 0)
							{
								text = text.Substring(0, num);
								text2 = text2.Substring(0, num);
							}
						}
						if (text2.EndsWith(" HOLD", StringComparison.Ordinal) || text2.EndsWith(" FREE", StringComparison.Ordinal))
						{
							text = text.Substring(0, text.Length - 5);
							text2 = text2.Substring(0, text2.Length - 5);
						}
						string[] array3 = text.Split(',');
						IList<TableInfo> list = new List<TableInfo>(array3.Length);
						for (int k = 0; k < array3.Length; k++)
						{
							string name = array3[k].Trim(' ', '`');
							list.Add(new TableInfo(database, name));
						}
						tables = list;
					}
					return new List<TableInfo>(tables);
				}
			}
		}

		public ColumnCollection Columns
		{
			get
			{
				if (columns == null)
				{
					columns = new ColumnCollection(this);
				}
				return columns;
			}
		}

		internal View(IntPtr handle, string sql, Database database)
			: base(handle, ownsHandle: true)
		{
			this.sql = sql;
			this.database = database;
		}

		public void Execute(Record executeParams)
		{
			uint num = RemotableNativeMethods.MsiViewExecute((int)base.Handle, (executeParams != null) ? ((int)executeParams.Handle) : 0);
			switch (num)
			{
			case 1615u:
				throw new BadQuerySyntaxException(sql);
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				break;
			}
		}

		public void Execute()
		{
			Execute(null);
		}

		public Record Fetch()
		{
			int hRecord;
			uint num = RemotableNativeMethods.MsiViewFetch((int)base.Handle, out hRecord);
			switch (num)
			{
			case 259u:
				return null;
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return new Record((IntPtr)hRecord, ownsHandle: true, this)
				{
					IsFormatStringInvalid = true
				};
			}
		}

		public void Modify(ViewModifyMode mode, Record record)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			uint num = RemotableNativeMethods.MsiViewModify((int)base.Handle, (int)mode, (int)record.Handle);
			if (mode == ViewModifyMode.Insert || mode == ViewModifyMode.InsertTemporary)
			{
				record.IsFormatStringInvalid = true;
			}
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void Refresh(Record record)
		{
			Modify(ViewModifyMode.Refresh, record);
		}

		public void Insert(Record record)
		{
			Modify(ViewModifyMode.Insert, record);
		}

		public void Update(Record record)
		{
			Modify(ViewModifyMode.Update, record);
		}

		public void Assign(Record record)
		{
			Modify(ViewModifyMode.Assign, record);
		}

		public void Replace(Record record)
		{
			Modify(ViewModifyMode.Replace, record);
		}

		public void Delete(Record record)
		{
			Modify(ViewModifyMode.Delete, record);
		}

		public void InsertTemporary(Record record)
		{
			Modify(ViewModifyMode.InsertTemporary, record);
		}

		public bool Seek(Record record)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			uint num = RemotableNativeMethods.MsiViewModify((int)base.Handle, -1, (int)record.Handle);
			record.IsFormatStringInvalid = true;
			switch (num)
			{
			case 1627u:
				return false;
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return true;
			}
		}

		public bool Merge(Record record)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			uint num = RemotableNativeMethods.MsiViewModify((int)base.Handle, 5, (int)record.Handle);
			switch (num)
			{
			case 1627u:
				return false;
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return true;
			}
		}

		public ICollection<ValidationErrorInfo> Validate(Record record)
		{
			return InternalValidate(ViewModifyMode.Validate, record);
		}

		public ICollection<ValidationErrorInfo> ValidateNew(Record record)
		{
			return InternalValidate(ViewModifyMode.ValidateNew, record);
		}

		public ICollection<ValidationErrorInfo> ValidateFields(Record record)
		{
			return InternalValidate(ViewModifyMode.ValidateField, record);
		}

		public ICollection<ValidationErrorInfo> ValidateDelete(Record record)
		{
			return InternalValidate(ViewModifyMode.ValidateDelete, record);
		}

		public IEnumerator<Record> GetEnumerator()
		{
			Record record;
			while ((record = Fetch()) != null)
			{
				yield return record;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private ICollection<ValidationErrorInfo> InternalValidate(ViewModifyMode mode, Record record)
		{
			uint num = RemotableNativeMethods.MsiViewModify((int)base.Handle, (int)mode, (int)record.Handle);
			switch (num)
			{
			case 13u:
			{
				ICollection<ValidationErrorInfo> collection = new List<ValidationErrorInfo>();
				while (true)
				{
					uint cchBuf = 40u;
					StringBuilder stringBuilder;
					int num2;
					checked
					{
						stringBuilder = new StringBuilder("", (int)cchBuf);
						num2 = RemotableNativeMethods.MsiViewGetError((int)base.Handle, stringBuilder, ref cchBuf);
						if (num2 == -2)
						{
							stringBuilder.Capacity = (int)(++cchBuf);
							num2 = RemotableNativeMethods.MsiViewGetError((int)base.Handle, stringBuilder, ref cchBuf);
						}
						switch (num2)
						{
						case -3:
							throw InstallerException.ExceptionFromReturnCode(87u);
						case 0:
							return collection;
						}
					}
					collection.Add(new ValidationErrorInfo((ValidationError)num2, stringBuilder.ToString()));
				}
			}
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return null;
			}
		}
	}
}
