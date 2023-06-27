using System.Collections.Generic;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class ComponentInfo
	{
		private Session session;

		private string name;

		public string Name => name;

		public InstallState CurrentState
		{
			get
			{
				int iInstalled;
				int iAction;
				uint num = RemotableNativeMethods.MsiGetComponentState((int)session.Handle, name, out iInstalled, out iAction);
				switch (num)
				{
				case 1607u:
					throw InstallerException.ExceptionFromReturnCode(num, name);
				default:
					throw InstallerException.ExceptionFromReturnCode(num);
				case 0u:
					return (InstallState)iInstalled;
				}
			}
		}

		public InstallState RequestState
		{
			get
			{
				int iInstalled;
				int iAction;
				uint num = RemotableNativeMethods.MsiGetComponentState((int)session.Handle, name, out iInstalled, out iAction);
				switch (num)
				{
				case 1607u:
					throw InstallerException.ExceptionFromReturnCode(num, name);
				default:
					throw InstallerException.ExceptionFromReturnCode(num);
				case 0u:
					return (InstallState)iAction;
				}
			}
			set
			{
				uint num = RemotableNativeMethods.MsiSetComponentState((int)session.Handle, name, (int)value);
				switch (num)
				{
				case 1607u:
					throw InstallerException.ExceptionFromReturnCode(num, name);
				default:
					throw InstallerException.ExceptionFromReturnCode(num);
				case 0u:
					break;
				}
			}
		}

		internal ComponentInfo(Session session, string name)
		{
			this.session = session;
			this.name = name;
		}

		public IList<InstallCost> GetCost(InstallState installState)
		{
			IList<InstallCost> list = new List<InstallCost>();
			StringBuilder stringBuilder = new StringBuilder(20);
			uint num = 0u;
			checked
			{
				while (true)
				{
					uint cchDriveBuf = (uint)stringBuilder.Capacity;
					int iCost;
					int iTempCost;
					uint num2 = RemotableNativeMethods.MsiEnumComponentCosts((int)session.Handle, name, num, unchecked((int)installState), stringBuilder, ref cchDriveBuf, out iCost, out iTempCost);
					if (num2 == 259)
					{
						break;
					}
					if (num2 == 234)
					{
						stringBuilder.Capacity = (int)(++cchDriveBuf);
						num2 = RemotableNativeMethods.MsiEnumComponentCosts((int)session.Handle, name, num, unchecked((int)installState), stringBuilder, ref cchDriveBuf, out iCost, out iTempCost);
					}
					if (num2 != 0)
					{
						throw InstallerException.ExceptionFromReturnCode(num2);
					}
					list.Add(new InstallCost(stringBuilder.ToString(), unchecked((long)iCost) * 512L, unchecked((long)iTempCost) * 512L));
					num++;
				}
				return list;
			}
		}
	}
}
