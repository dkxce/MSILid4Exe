using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class TableInfo
	{
		private string name;

		private ColumnCollection columns;

		private ReadOnlyCollection<string> primaryKeys;

		public string Name => name;

		public ColumnCollection Columns => columns;

		public IList<string> PrimaryKeys => primaryKeys;

		public string SqlCreateString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("CREATE TABLE `");
				stringBuilder.Append(name);
				stringBuilder.Append("` (");
				int num = 0;
				checked
				{
					foreach (ColumnInfo column in Columns)
					{
						if (num > 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(column.SqlCreateString);
						num++;
					}
					stringBuilder.Append("  PRIMARY KEY ");
					num = 0;
					foreach (string primaryKey in PrimaryKeys)
					{
						if (num > 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.AppendFormat("`{0}`", primaryKey);
						num++;
					}
					stringBuilder.Append(')');
					return stringBuilder.ToString();
				}
			}
		}

		public string SqlInsertString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("INSERT INTO `");
				stringBuilder.Append(name);
				stringBuilder.Append("` (");
				int num = 0;
				checked
				{
					foreach (ColumnInfo column in Columns)
					{
						if (num > 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.AppendFormat("`{0}`", column.Name);
						num++;
					}
					stringBuilder.Append(") VALUES (");
					while (num > 0)
					{
						num--;
						stringBuilder.Append("?");
						if (num > 0)
						{
							stringBuilder.Append(", ");
						}
					}
					stringBuilder.Append(')');
					return stringBuilder.ToString();
				}
			}
		}

		public string SqlSelectString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("SELECT ");
				int num = 0;
				foreach (ColumnInfo column in Columns)
				{
					if (num > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.AppendFormat("`{0}`", column.Name);
					num = checked(num + 1);
				}
				stringBuilder.AppendFormat(" FROM `{0}`", Name);
				return stringBuilder.ToString();
			}
		}

		public TableInfo(string name, ICollection<ColumnInfo> columns, IList<string> primaryKeys)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (columns == null || columns.Count == 0)
			{
				throw new ArgumentNullException("columns");
			}
			if (primaryKeys == null || primaryKeys.Count == 0)
			{
				throw new ArgumentNullException("primaryKeys");
			}
			this.name = name;
			this.columns = new ColumnCollection(columns);
			this.primaryKeys = new List<string>(primaryKeys).AsReadOnly();
			foreach (string primaryKey in this.primaryKeys)
			{
				if (!this.columns.Contains(primaryKey))
				{
					throw new ArgumentOutOfRangeException("primaryKeys");
				}
			}
		}

		internal TableInfo(Database db, string name)
		{
			if (db == null)
			{
				throw new ArgumentNullException("db");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			using (View view = db.OpenView("SELECT * FROM `{0}`", name))
			{
				columns = new ColumnCollection(view);
			}
			primaryKeys = new ReadOnlyCollection<string>(GetTablePrimaryKeys(db, name));
		}

		public override string ToString()
		{
			return name;
		}

		private static IList<string> GetTablePrimaryKeys(Database db, string table)
		{
			checked
			{
				switch (table)
				{
				case "_Tables":
					return new string[1] { "Name" };
				case "_Columns":
					return new string[2] { "Table", "Number" };
				case "_Storages":
					return new string[1] { "Name" };
				case "_Streams":
					return new string[1] { "Name" };
				default:
				{
					int hRecord;
					uint num = RemotableNativeMethods.MsiDatabaseGetPrimaryKeys((int)db.Handle, table, out hRecord);
					if (num != 0)
					{
						throw InstallerException.ExceptionFromReturnCode(num);
					}
					using (Record record = new Record((IntPtr)hRecord, ownsHandle: true, null))
					{
						string[] array = new string[record.FieldCount];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = record.GetString(i + 1);
						}
						return array;
					}
				}
				}
			}
		}
	}
}
