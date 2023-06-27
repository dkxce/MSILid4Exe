using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Serializable]
	public class MergeException : InstallerException
	{
		private IList<string> conflictTables;

		private IList<int> conflictCounts;

		public IList<int> ConflictCounts => new List<int>(conflictCounts);

		public IList<string> ConflictTables => new List<string>(conflictTables);

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(base.Message);
				if (conflictTables != null)
				{
					for (int i = 0; i < conflictTables.Count; i = checked(i + 1))
					{
						stringBuilder.Append((i == 0) ? "  Conflicts: " : ", ");
						stringBuilder.Append(conflictTables[i]);
						stringBuilder.Append('(');
						stringBuilder.Append(conflictCounts[i]);
						stringBuilder.Append(')');
					}
				}
				return stringBuilder.ToString();
			}
		}

		public MergeException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public MergeException(string msg)
			: base(msg)
		{
		}

		public MergeException()
		{
		}

		internal MergeException(Database db, string conflictsTableName)
			: base("Merge failed.")
		{
			if (conflictsTableName == null)
			{
				return;
			}
			IList<string> list = new List<string>();
			IList<int> list2 = new List<int>();
			using (View view = db.OpenView("SELECT `Table`, `NumRowMergeConflicts` FROM `" + conflictsTableName + "`"))
			{
				view.Execute();
				foreach (Record item in view)
				{
					using (item)
					{
						list.Add(item.GetString(1));
						list2.Add(item.GetInteger(2));
					}
				}
			}
			conflictTables = list;
			conflictCounts = list2;
		}

		protected MergeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			conflictTables = (string[])info.GetValue("mergeConflictTables", typeof(string[]));
			conflictCounts = (int[])info.GetValue("mergeConflictCounts", typeof(int[]));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("mergeConflictTables", conflictTables);
			info.AddValue("mergeConflictCounts", conflictCounts);
			base.GetObjectData(info, context);
		}
	}
}
