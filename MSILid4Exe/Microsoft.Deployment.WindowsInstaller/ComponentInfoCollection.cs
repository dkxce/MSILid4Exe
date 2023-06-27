using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Deployment.WindowsInstaller
{
	public sealed class ComponentInfoCollection : ICollection<ComponentInfo>, IEnumerable<ComponentInfo>, IEnumerable
	{
		private Session session;

		public ComponentInfo this[string component] => new ComponentInfo(session, component);

		public int Count => session.Database.CountRows("Component");

		bool ICollection<ComponentInfo>.IsReadOnly => true;

		internal ComponentInfoCollection(Session session)
		{
			this.session = session;
		}

		void ICollection<ComponentInfo>.Add(ComponentInfo item)
		{
			throw new InvalidOperationException();
		}

		void ICollection<ComponentInfo>.Clear()
		{
			throw new InvalidOperationException();
		}

		public bool Contains(string component)
		{
			return session.Database.CountRows("Component", "`Component` = '" + component + "'") == 1;
		}

		bool ICollection<ComponentInfo>.Contains(ComponentInfo item)
		{
			if (item != null)
			{
				return Contains(item.Name);
			}
			return false;
		}

		public void CopyTo(ComponentInfo[] array, int arrayIndex)
		{
			using (IEnumerator<ComponentInfo> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ComponentInfo current = enumerator.Current;
					array[checked(arrayIndex++)] = current;
				}
			}
		}

		bool ICollection<ComponentInfo>.Remove(ComponentInfo item)
		{
			throw new InvalidOperationException();
		}

		public IEnumerator<ComponentInfo> GetEnumerator()
		{
			using (View compView = session.Database.OpenView("SELECT `Component` FROM `Component`"))
			{
				compView.Execute();
				foreach (Record item in compView)
				{
					using (item)
					{
						string @string = item.GetString(1);
						yield return new ComponentInfo(session, @string);
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
