using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class Database : InstallerHandle
	{
		private string filePath;

		private DatabaseOpenMode openMode;

		private SummaryInfo summaryInfo;

		private TableCollection tables;

		private IList<string> deleteOnClose;

		public string FilePath => filePath;

		public DatabaseOpenMode OpenMode => openMode;

		public bool IsReadOnly
		{
			get
			{
				if (RemotableNativeMethods.RemotingEnabled)
				{
					return true;
				}
				return NativeMethods.MsiGetDatabaseState((int)base.Handle) != 1;
			}
		}

		public TableCollection Tables
		{
			get
			{
				if (tables == null)
				{
					tables = new TableCollection(this);
				}
				return tables;
			}
		}

		public int CodePage
		{
			get
			{
				string tempFileName = Path.GetTempFileName();
				StreamReader streamReader = null;
				try
				{
					Export("_ForceCodepage", tempFileName);
					streamReader = File.OpenText(tempFileName);
					streamReader.ReadLine();
					streamReader.ReadLine();
					return int.Parse(streamReader.ReadLine().Split('\t')[0], CultureInfo.InvariantCulture.NumberFormat);
				}
				finally
				{
					streamReader?.Close();
					File.Delete(tempFileName);
				}
			}
			set
			{
				string tempFileName = Path.GetTempFileName();
				StreamWriter streamWriter = null;
				try
				{
					streamWriter = File.AppendText(tempFileName);
					streamWriter.WriteLine("");
					streamWriter.WriteLine("");
					streamWriter.WriteLine("{0}\t_ForceCodepage", value);
					streamWriter.Close();
					streamWriter = null;
					Import(tempFileName);
				}
				finally
				{
					streamWriter?.Close();
					File.Delete(tempFileName);
				}
			}
		}

		public SummaryInfo SummaryInfo
		{
			get
			{
				if (summaryInfo == null || summaryInfo.IsClosed)
				{
					lock (base.Sync)
					{
						if (summaryInfo == null || summaryInfo.IsClosed)
						{
							int num = ((!IsReadOnly) ? 20 : 0);
							int hSummaryInfo;
							uint num2 = RemotableNativeMethods.MsiGetSummaryInformation((int)base.Handle, null, checked((uint)num), out hSummaryInfo);
							if (num2 != 0)
							{
								throw InstallerException.ExceptionFromReturnCode(num2);
							}
							summaryInfo = new SummaryInfo((IntPtr)hSummaryInfo, ownsHandle: true);
						}
					}
				}
				return summaryInfo;
			}
		}

		public Database(string filePath)
			: this(filePath, DatabaseOpenMode.ReadOnly)
		{
		}

		public Database(string filePath, string outputPath)
			: this((IntPtr)Open(filePath, outputPath), ownsHandle: true, outputPath, DatabaseOpenMode.CreateDirect)
		{
		}

		public Database(string filePath, DatabaseOpenMode mode)
			: this((IntPtr)Open(filePath, mode), ownsHandle: true, filePath, mode)
		{
		}

		protected internal Database(IntPtr handle, bool ownsHandle, string filePath, DatabaseOpenMode openMode)
			: base(handle, ownsHandle)
		{
			this.filePath = filePath;
			this.openMode = openMode;
		}

		public static Database FromHandle(IntPtr handle, bool ownsHandle)
		{
			return new Database(handle, ownsHandle, null, (NativeMethods.MsiGetDatabaseState((int)handle) == 1) ? DatabaseOpenMode.Direct : DatabaseOpenMode.ReadOnly);
		}

		public void DeleteOnClose(string path)
		{
			if (deleteOnClose == null)
			{
				deleteOnClose = new List<string>();
			}
			deleteOnClose.Add(path);
		}

		public void Merge(Database otherDatabase, string errorTable)
		{
			if (otherDatabase == null)
			{
				throw new ArgumentNullException("otherDatabase");
			}
			uint num = NativeMethods.MsiDatabaseMerge((int)base.Handle, (int)otherDatabase.Handle, errorTable);
			switch (num)
			{
			case 1627u:
				throw new MergeException(this, errorTable);
			case 1629u:
				throw new MergeException("Schema difference between the two databases.");
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				break;
			}
		}

		public void Merge(Database otherDatabase)
		{
			Merge(otherDatabase, null);
		}

		public bool IsTablePersistent(string table)
		{
			if (string.IsNullOrEmpty(table))
			{
				throw new ArgumentNullException("table");
			}
			uint num = RemotableNativeMethods.MsiDatabaseIsTablePersistent((int)base.Handle, table);
			if (num == 3)
			{
				throw new InstallerException();
			}
			return num == 1;
		}

		public bool IsColumnPersistent(string table, string column)
		{
			if (string.IsNullOrEmpty(table))
			{
				throw new ArgumentNullException("table");
			}
			if (string.IsNullOrEmpty(column))
			{
				throw new ArgumentNullException("column");
			}
			using (View view = OpenView("SELECT `Number` FROM `_Columns` WHERE `Table` = '{0}' AND `Name` = '{1}'", table, column))
			{
				view.Execute();
				using (Record record = view.Fetch())
				{
					return record != null;
				}
			}
		}

		public int CountRows(string table)
		{
			return CountRows(table, null);
		}

		public int CountRows(string table, string where)
		{
			if (string.IsNullOrEmpty(table))
			{
				throw new ArgumentNullException("table");
			}
			TableInfo tableInfo = Tables[table];
			string text = ((tableInfo == null) ? "*" : ("`" + tableInfo.PrimaryKeys[0] + "`"));
			try
			{
				using (View view = OpenView("SELECT {0} FROM `{1}`{2}", text, table, (where != null && where.Length != 0) ? (" WHERE " + where) : ""))
				{
					view.Execute();
					int num = 0;
					while (true)
					{
						int hRecord;
						uint num2 = RemotableNativeMethods.MsiViewFetch((int)view.Handle, out hRecord);
						switch (num2)
						{
						default:
							throw InstallerException.ExceptionFromReturnCode(num2);
						case 0u:
							break;
						case 259u:
							return num;
						}
						RemotableNativeMethods.MsiCloseHandle(hRecord);
						num = checked(num + 1);
					}
				}
			}
			catch (BadQuerySyntaxException)
			{
				return 0;
			}
		}

		public void Commit()
		{
			if (summaryInfo != null && !summaryInfo.IsClosed)
			{
				summaryInfo.Persist();
				summaryInfo.Close();
				summaryInfo = null;
			}
			uint num = NativeMethods.MsiDatabaseCommit((int)base.Handle);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void Export(string table, string exportFilePath)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			FileInfo fileInfo = new FileInfo(exportFilePath);
			uint num = NativeMethods.MsiDatabaseExport((int)base.Handle, table, fileInfo.DirectoryName, fileInfo.Name);
			switch (num)
			{
			case 161u:
				throw new FileNotFoundException(null, exportFilePath);
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				break;
			}
		}

		public void Import(string importFilePath)
		{
			if (string.IsNullOrEmpty(importFilePath))
			{
				throw new ArgumentNullException("importFilePath");
			}
			FileInfo fileInfo = new FileInfo(importFilePath);
			uint num = NativeMethods.MsiDatabaseImport((int)base.Handle, fileInfo.DirectoryName, fileInfo.Name);
			switch (num)
			{
			case 161u:
				throw new FileNotFoundException(null, importFilePath);
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				break;
			}
		}

		public void ExportAll(string directoryPath)
		{
			if (string.IsNullOrEmpty(directoryPath))
			{
				throw new ArgumentNullException("directoryPath");
			}
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
			Export("_SummaryInformation", Path.Combine(directoryPath, "_SummaryInformation.idt"));
			using (View view = OpenView("SELECT `Name` FROM `_Tables`"))
			{
				view.Execute();
				foreach (Record item in view)
				{
					using (item)
					{
						string text = (string)item[1];
						Export(text, Path.Combine(directoryPath, text + ".idt"));
					}
				}
			}
			if (!Directory.Exists(Path.Combine(directoryPath, "_Streams")))
			{
				Directory.CreateDirectory(Path.Combine(directoryPath, "_Streams"));
			}
			using (View view2 = OpenView("SELECT `Name`, `Data` FROM `_Streams`"))
			{
				view2.Execute();
				foreach (Record item2 in view2)
				{
					using (item2)
					{
						string text2 = (string)item2[1];
						if (!text2.EndsWith("SummaryInformation", StringComparison.Ordinal))
						{
							int num = text2.IndexOf('.');
							if (num < 0 || !File.Exists(Path.Combine(directoryPath, Path.Combine(text2.Substring(0, num), text2.Substring(checked(num + 1)) + ".ibd"))))
							{
								item2.GetStream(2, Path.Combine(directoryPath, Path.Combine("_Streams", text2)));
							}
						}
					}
				}
			}
		}

		public void ImportAll(string directoryPath)
		{
			if (string.IsNullOrEmpty(directoryPath))
			{
				throw new ArgumentNullException("directoryPath");
			}
			if (File.Exists(Path.Combine(directoryPath, "_SummaryInformation.idt")))
			{
				Import(Path.Combine(directoryPath, "_SummaryInformation.idt"));
			}
			string[] files = Directory.GetFiles(directoryPath, "*.idt");
			foreach (string text in files)
			{
				if (Path.GetFileName(text) != "_SummaryInformation.idt")
				{
					Import(text);
				}
			}
			if (!Directory.Exists(Path.Combine(directoryPath, "_Streams")))
			{
				return;
			}
			View view = OpenView("SELECT `Name`, `Data` FROM `_Streams`");
			Record record = null;
			try
			{
				view.Execute();
				files = Directory.GetFiles(Path.Combine(directoryPath, "_Streams"));
				foreach (string path in files)
				{
					record = CreateRecord(2);
					record[1] = Path.GetFileName(path);
					record.SetStream(2, path);
					view.Insert(record);
					record.Close();
					record = null;
				}
			}
			finally
			{
				record?.Close();
				view.Close();
			}
		}

		public Record CreateRecord(int fieldCount)
		{
			return new Record((IntPtr)RemotableNativeMethods.MsiCreateRecord(checked((uint)fieldCount), (int)base.Handle), ownsHandle: true, null);
		}

		public override string ToString()
		{
			if (FilePath != null)
			{
				return FilePath;
			}
			return "#" + ((int)base.Handle).ToString(CultureInfo.InvariantCulture);
		}

		protected override void Dispose(bool disposing)
		{
			if (!base.IsClosed && (OpenMode == DatabaseOpenMode.CreateDirect || OpenMode == DatabaseOpenMode.Direct))
			{
				Commit();
			}
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			if (summaryInfo != null)
			{
				summaryInfo.Close();
				summaryInfo = null;
			}
			if (deleteOnClose == null)
			{
				return;
			}
			foreach (string item in deleteOnClose)
			{
				try
				{
					if (Directory.Exists(item))
					{
						Directory.Delete(item, recursive: true);
					}
					else if (File.Exists(item))
					{
						File.Delete(item);
					}
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}
			}
			deleteOnClose = null;
		}

		private static int Open(string filePath, string outputPath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			if (string.IsNullOrEmpty(outputPath))
			{
				throw new ArgumentNullException("outputPath");
			}
			int hDatabase;
			uint num = NativeMethods.MsiOpenDatabase(filePath, outputPath, out hDatabase);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return hDatabase;
		}

		private static int Open(string filePath, DatabaseOpenMode mode)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			if (Path.GetExtension(filePath).Equals(".msp", StringComparison.Ordinal))
			{
				mode |= (DatabaseOpenMode)32;
			}
			int hDatabase;
			uint num = NativeMethods.MsiOpenDatabase(filePath, (IntPtr)(int)mode, out hDatabase);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num, string.Format(CultureInfo.InvariantCulture, "Database=\"{0}\"", filePath));
			}
			return hDatabase;
		}

		public string ExecutePropertyQuery(string property)
		{
			IList<string> list = ExecuteStringQuery("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", property);
			if (list.Count <= 0)
			{
				return null;
			}
			return list[0];
		}

		public View OpenView(string sqlFormat, params object[] args)
		{
			if (string.IsNullOrEmpty(sqlFormat))
			{
				throw new ArgumentNullException("sqlFormat");
			}
			string text = ((args == null || args.Length == 0) ? sqlFormat : string.Format(CultureInfo.InvariantCulture, sqlFormat, args));
			int hView;
			uint num = RemotableNativeMethods.MsiDatabaseOpenView((int)base.Handle, text, out hView);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return new View((IntPtr)hView, text, this);
		}

		public void Execute(string sqlFormat, params object[] args)
		{
			if (string.IsNullOrEmpty(sqlFormat))
			{
				throw new ArgumentNullException("sqlFormat");
			}
			Execute((args == null || args.Length == 0) ? sqlFormat : string.Format(CultureInfo.InvariantCulture, sqlFormat, args), (Record)null);
		}

		public void Execute(string sql, Record record)
		{
			if (string.IsNullOrEmpty(sql))
			{
				throw new ArgumentNullException("sql");
			}
			using (View view = OpenView(sql))
			{
				view.Execute(record);
			}
		}

		public IList ExecuteQuery(string sqlFormat, params object[] args)
		{
			if (string.IsNullOrEmpty(sqlFormat))
			{
				throw new ArgumentNullException("sqlFormat");
			}
			return ExecuteQuery((args == null || args.Length == 0) ? sqlFormat : string.Format(CultureInfo.InvariantCulture, sqlFormat, args), (Record)null);
		}

		public IList ExecuteQuery(string sql, Record record)
		{
			if (string.IsNullOrEmpty(sql))
			{
				throw new ArgumentNullException("sql");
			}
			using (View view = OpenView(sql))
			{
				view.Execute(record);
				IList list = new ArrayList();
				int num = 0;
				foreach (Record item in view)
				{
					using (item)
					{
						if (num == 0)
						{
							num = item.FieldCount;
						}
						for (int i = 1; i <= num; i = checked(i + 1))
						{
							list.Add(item[i]);
						}
					}
				}
				return list;
			}
		}

		public IList<int> ExecuteIntegerQuery(string sqlFormat, params object[] args)
		{
			if (string.IsNullOrEmpty(sqlFormat))
			{
				throw new ArgumentNullException("sqlFormat");
			}
			return ExecuteIntegerQuery((args == null || args.Length == 0) ? sqlFormat : string.Format(CultureInfo.InvariantCulture, sqlFormat, args), (Record)null);
		}

		public IList<int> ExecuteIntegerQuery(string sql, Record record)
		{
			if (string.IsNullOrEmpty(sql))
			{
				throw new ArgumentNullException("sql");
			}
			using (View view = OpenView(sql))
			{
				view.Execute(record);
				IList<int> list = new List<int>();
				int num = 0;
				foreach (Record item in view)
				{
					using (item)
					{
						if (num == 0)
						{
							num = item.FieldCount;
						}
						for (int i = 1; i <= num; i = checked(i + 1))
						{
							list.Add(item.GetInteger(i));
						}
					}
				}
				return list;
			}
		}

		public IList<string> ExecuteStringQuery(string sqlFormat, params object[] args)
		{
			if (string.IsNullOrEmpty(sqlFormat))
			{
				throw new ArgumentNullException("sqlFormat");
			}
			return ExecuteStringQuery((args == null || args.Length == 0) ? sqlFormat : string.Format(CultureInfo.InvariantCulture, sqlFormat, args), (Record)null);
		}

		public IList<string> ExecuteStringQuery(string sql, Record record)
		{
			if (string.IsNullOrEmpty(sql))
			{
				throw new ArgumentNullException("sql");
			}
			using (View view = OpenView(sql))
			{
				view.Execute(record);
				IList<string> list = new List<string>();
				int num = 0;
				foreach (Record item in view)
				{
					using (item)
					{
						if (num == 0)
						{
							num = item.FieldCount;
						}
						for (int i = 1; i <= num; i = checked(i + 1))
						{
							list.Add(item.GetString(i));
						}
					}
				}
				return list;
			}
		}

		public object ExecuteScalar(string sqlFormat, params object[] args)
		{
			if (string.IsNullOrEmpty(sqlFormat))
			{
				throw new ArgumentNullException("sqlFormat");
			}
			return ExecuteScalar((args == null || args.Length == 0) ? sqlFormat : string.Format(CultureInfo.InvariantCulture, sqlFormat, args), (Record)null);
		}

		public object ExecuteScalar(string sql, Record record)
		{
			if (string.IsNullOrEmpty(sql))
			{
				throw new ArgumentNullException("sql");
			}
			View view = OpenView(sql);
			Record record2 = null;
			try
			{
				view.Execute(record);
				record2 = view.Fetch();
				if (record2 == null)
				{
					throw InstallerException.ExceptionFromReturnCode(259u);
				}
				return record2[1];
			}
			finally
			{
				record2?.Close();
				view.Close();
			}
		}

		public bool GenerateTransform(Database referenceDatabase, string transformFile)
		{
			if (referenceDatabase == null)
			{
				throw new ArgumentNullException("referenceDatabase");
			}
			if (string.IsNullOrEmpty(transformFile))
			{
				throw new ArgumentNullException("transformFile");
			}
			uint num = NativeMethods.MsiDatabaseGenerateTransform((int)base.Handle, (int)referenceDatabase.Handle, transformFile, 0, 0);
			switch (num)
			{
			case 232u:
				return false;
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return true;
			}
		}

		public void CreateTransformSummaryInfo(Database referenceDatabase, string transformFile, TransformErrors errors, TransformValidations validations)
		{
			if (referenceDatabase == null)
			{
				throw new ArgumentNullException("referenceDatabase");
			}
			if (string.IsNullOrEmpty(transformFile))
			{
				throw new ArgumentNullException("transformFile");
			}
			uint num = NativeMethods.MsiCreateTransformSummaryInfo((int)base.Handle, (int)referenceDatabase.Handle, transformFile, (int)errors, (int)validations);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void ViewTransform(string transformFile)
		{
			TransformErrors errorConditionsToSuppress = TransformErrors.AddExistingRow | TransformErrors.DelMissingRow | TransformErrors.AddExistingTable | TransformErrors.DelMissingTable | TransformErrors.UpdateMissingRow | TransformErrors.ChangeCodePage | TransformErrors.ViewTransform;
			ApplyTransform(transformFile, errorConditionsToSuppress);
		}

		public void ApplyTransform(string transformFile)
		{
			if (string.IsNullOrEmpty(transformFile))
			{
				throw new ArgumentNullException("transformFile");
			}
			TransformErrors errorConditionsToSuppress;
			using (SummaryInfo summaryInfo = new SummaryInfo(transformFile, enableWrite: false))
			{
				errorConditionsToSuppress = (TransformErrors)(summaryInfo.CharacterCount & 0xFFFF);
			}
			ApplyTransform(transformFile, errorConditionsToSuppress);
		}

		public void ApplyTransform(string transformFile, TransformErrors errorConditionsToSuppress)
		{
			if (string.IsNullOrEmpty(transformFile))
			{
				throw new ArgumentNullException("transformFile");
			}
			uint num = NativeMethods.MsiDatabaseApplyTransform((int)base.Handle, transformFile, (int)errorConditionsToSuppress);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public bool IsTransformValid(string transformFile)
		{
			if (string.IsNullOrEmpty(transformFile))
			{
				throw new ArgumentNullException("transformFile");
			}
			using (SummaryInfo transformSummaryInfo = new SummaryInfo(transformFile, enableWrite: false))
			{
				return IsTransformValid(transformSummaryInfo);
			}
		}

		public bool IsTransformValid(SummaryInfo transformSummaryInfo)
		{
			if (transformSummaryInfo == null)
			{
				throw new ArgumentNullException("transformSummaryInfo");
			}
			string[] array = transformSummaryInfo.RevisionNumber.Split(new char[1] { ';' }, 3);
			string text = array[0].Substring(0, 38);
			string version = array[0].Substring(38);
			string text2 = array[2];
			string[] array2 = transformSummaryInfo.Template.Split(new char[1] { ';' }, 2);
			int num = 0;
			if (array2.Length >= 2 && array2[1].Length > 0)
			{
				num = int.Parse(array2[1], CultureInfo.InvariantCulture.NumberFormat);
			}
			int num2 = transformSummaryInfo.CharacterCount >> 16;
			string text3 = ExecutePropertyQuery("ProductCode");
			string version2 = ExecutePropertyQuery("ProductVersion");
			string text4 = ExecutePropertyQuery("UpgradeCode");
			string text5 = ExecutePropertyQuery("ProductLanguage");
			int num3 = 0;
			if (!string.IsNullOrEmpty(text5))
			{
				num3 = int.Parse(text5, CultureInfo.InvariantCulture.NumberFormat);
			}
			if (((uint)num2 & 2u) != 0 && text3 != text)
			{
				return false;
			}
			if (((uint)num2 & 0x800u) != 0 && text4 != text2)
			{
				return false;
			}
			if (((uint)num2 & (true ? 1u : 0u)) != 0 && num != 0 && num3 != num)
			{
				return false;
			}
			Version version3 = new Version(version2);
			Version version4 = new Version(version);
			if (((uint)num2 & 0x20u) != 0)
			{
				if (version3.Major != version4.Major)
				{
					return false;
				}
				if (version3.Minor != version4.Minor)
				{
					return false;
				}
				if (version3.Build != version4.Build)
				{
					return false;
				}
			}
			else if (((uint)num2 & 0x10u) != 0)
			{
				if (version3.Major != version4.Major)
				{
					return false;
				}
				if (version3.Minor != version4.Minor)
				{
					return false;
				}
			}
			else if (((uint)num2 & 8u) != 0 && version3.Major != version4.Major)
			{
				return false;
			}
			return true;
		}
	}
}
