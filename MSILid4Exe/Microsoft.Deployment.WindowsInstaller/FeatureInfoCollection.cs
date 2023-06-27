using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Deployment.WindowsInstaller
{
	public sealed class FeatureInfoCollection : ICollection<FeatureInfo>, IEnumerable<FeatureInfo>, IEnumerable
	{
		private Session session;

		public FeatureInfo this[string feature] => new FeatureInfo(session, feature);

		public int Count => session.Database.CountRows("Feature");

		bool ICollection<FeatureInfo>.IsReadOnly => true;

		internal FeatureInfoCollection(Session session)
		{
			this.session = session;
		}

		void ICollection<FeatureInfo>.Add(FeatureInfo item)
		{
			throw new InvalidOperationException();
		}

		void ICollection<FeatureInfo>.Clear()
		{
			throw new InvalidOperationException();
		}

		public bool Contains(string feature)
		{
			return session.Database.CountRows("Feature", "`Feature` = '" + feature + "'") == 1;
		}

		bool ICollection<FeatureInfo>.Contains(FeatureInfo item)
		{
			if (item != null)
			{
				return Contains(item.Name);
			}
			return false;
		}

		public void CopyTo(FeatureInfo[] array, int arrayIndex)
		{
			using (IEnumerator<FeatureInfo> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FeatureInfo current = enumerator.Current;
					array[checked(arrayIndex++)] = current;
				}
			}
		}

		bool ICollection<FeatureInfo>.Remove(FeatureInfo item)
		{
			throw new InvalidOperationException();
		}

		public IEnumerator<FeatureInfo> GetEnumerator()
		{
			using (View featureView = session.Database.OpenView("SELECT `Feature` FROM `Feature`"))
			{
				featureView.Execute();
				foreach (Record item in featureView)
				{
					using (item)
					{
						string @string = item.GetString(1);
						yield return new FeatureInfo(session, @string);
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
