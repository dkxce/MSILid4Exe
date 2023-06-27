using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class TableCollection : ICollection<TableInfo>, IEnumerable<TableInfo>, IEnumerable
	{
		private Database db;

		public int Count => GetTables().Count;

		public bool IsReadOnly => db.IsReadOnly;

		public TableInfo this[string table]
		{
			get
			{
				if (string.IsNullOrEmpty(table))
				{
					throw new ArgumentNullException("table");
				}
				if (!Contains(table))
				{
					return null;
				}
				return new TableInfo(db, table);
			}
		}

		internal TableCollection(Database db)
		{
			this.db = db;
		}

		public void Add(TableInfo item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (Contains(item.Name))
			{
				throw new InvalidOperationException();
			}
			db.Execute(item.SqlCreateString);
		}

		public void Clear()
		{
			foreach (string table in GetTables())
			{
				Remove(table);
			}
		}

		public bool Contains(string item)
		{
			if (string.IsNullOrEmpty(item))
			{
				throw new ArgumentNullException("item");
			}
			uint num = RemotableNativeMethods.MsiDatabaseIsTablePersistent((int)db.Handle, item);
			if (num == 3)
			{
				throw new InstallerException();
			}
			return num != 2;
		}

		bool ICollection<TableInfo>.Contains(TableInfo item)
		{
			return Contains(item.Name);
		}

		public void CopyTo(TableInfo[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			foreach (string table in GetTables())
			{
				array[checked(arrayIndex++)] = new TableInfo(db, table);
			}
		}

		public bool Remove(string item)
		{
			if (string.IsNullOrEmpty(item))
			{
				throw new ArgumentNullException("item");
			}
			if (!Contains(item))
			{
				return false;
			}
			db.Execute("DROP TABLE `{0}`", item);
			return true;
		}

		bool ICollection<TableInfo>.Remove(TableInfo item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return Remove(item.Name);
		}

		public IEnumerator<TableInfo> GetEnumerator()
		{
			foreach (string table in GetTables())
			{
				yield return new TableInfo(db, table);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private IList<string> GetTables()
		{
			return db.ExecuteStringQuery("SELECT `Name` FROM `_Tables`");
		}
	}
}
