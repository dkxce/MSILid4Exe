using System.Collections.Generic;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class FeatureInfo
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
				uint num = RemotableNativeMethods.MsiGetFeatureState((int)session.Handle, name, out iInstalled, out iAction);
				switch (num)
				{
				case 1606u:
					throw InstallerException.ExceptionFromReturnCode(num, name);
				default:
					throw InstallerException.ExceptionFromReturnCode(num);
				case 0u:
					if (iInstalled == 1)
					{
						return InstallState.Advertised;
					}
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
				uint num = RemotableNativeMethods.MsiGetFeatureState((int)session.Handle, name, out iInstalled, out iAction);
				switch (num)
				{
				case 1606u:
					throw InstallerException.ExceptionFromReturnCode(num, name);
				default:
					throw InstallerException.ExceptionFromReturnCode(num);
				case 0u:
					return (InstallState)iAction;
				}
			}
			set
			{
				uint num = RemotableNativeMethods.MsiSetFeatureState((int)session.Handle, name, (int)value);
				switch (num)
				{
				case 1606u:
					throw InstallerException.ExceptionFromReturnCode(num, name);
				default:
					throw InstallerException.ExceptionFromReturnCode(num);
				case 0u:
					break;
				}
			}
		}

		public ICollection<InstallState> ValidStates
		{
			get
			{
				List<InstallState> list = new List<InstallState>();
				uint dwInstalledState;
				uint num = RemotableNativeMethods.MsiGetFeatureValidStates((int)session.Handle, name, out dwInstalledState);
				switch (num)
				{
				case 1606u:
					throw InstallerException.ExceptionFromReturnCode(num, name);
				default:
					throw InstallerException.ExceptionFromReturnCode(num);
				case 0u:
				{
					for (int i = 1; i <= 5; i = checked(i + 1))
					{
						if ((checked((int)dwInstalledState) & (1 << i)) != 0)
						{
							list.Add((InstallState)i);
						}
					}
					return list.AsReadOnly();
				}
				}
			}
		}

		public FeatureAttributes Attributes
		{
			get
			{
				uint cchTitleBuf = 0u;
				uint cchHelpBuf = 0u;
				uint lpAttributes;
				uint num = NativeMethods.MsiGetFeatureInfo((int)session.Handle, name, out lpAttributes, null, ref cchTitleBuf, null, ref cchHelpBuf);
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
				FeatureAttributes featureAttributes = (FeatureAttributes)checked((int)(lpAttributes >> 1));
				if ((featureAttributes & FeatureAttributes.UIDisallowAbsent) == FeatureAttributes.UIDisallowAbsent)
				{
					featureAttributes &= ~FeatureAttributes.UIDisallowAbsent;
					featureAttributes |= FeatureAttributes.NoUnsupportedAdvertise;
				}
				return featureAttributes;
			}
			set
			{
				uint num = checked((uint)(value & ~FeatureAttributes.UIDisallowAbsent)) << 1;
				uint num2 = 64u;
				if ((num & num2) == num2)
				{
					num &= ~num2;
					num |= 0x20u;
				}
				uint num3 = RemotableNativeMethods.MsiSetFeatureAttributes((int)session.Handle, name, num);
				switch (num3)
				{
				case 1606u:
					throw InstallerException.ExceptionFromReturnCode(num3, name);
				default:
					throw InstallerException.ExceptionFromReturnCode(num3);
				case 0u:
					break;
				}
			}
		}

		public string Title
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(80);
				checked
				{
					uint cchTitleBuf = (uint)stringBuilder.Capacity;
					uint cchHelpBuf = 0u;
					uint lpAttributes;
					uint num = NativeMethods.MsiGetFeatureInfo((int)session.Handle, name, out lpAttributes, stringBuilder, ref cchTitleBuf, null, ref cchHelpBuf);
					if (num == 234)
					{
						stringBuilder.Capacity = (int)(++cchTitleBuf);
						num = NativeMethods.MsiGetFeatureInfo((int)session.Handle, name, out lpAttributes, stringBuilder, ref cchTitleBuf, null, ref cchHelpBuf);
					}
					if (num != 0)
					{
						throw InstallerException.ExceptionFromReturnCode(num);
					}
					return stringBuilder.ToString();
				}
			}
		}

		public string Description
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(256);
				uint cchTitleBuf = 0u;
				checked
				{
					uint cchHelpBuf = (uint)stringBuilder.Capacity;
					uint lpAttributes;
					uint num = NativeMethods.MsiGetFeatureInfo((int)session.Handle, name, out lpAttributes, null, ref cchTitleBuf, stringBuilder, ref cchHelpBuf);
					if (num == 234)
					{
						stringBuilder.Capacity = (int)(++cchHelpBuf);
						num = NativeMethods.MsiGetFeatureInfo((int)session.Handle, name, out lpAttributes, null, ref cchTitleBuf, stringBuilder, ref cchHelpBuf);
					}
					if (num != 0)
					{
						throw InstallerException.ExceptionFromReturnCode(num);
					}
					return stringBuilder.ToString();
				}
			}
		}

		internal FeatureInfo(Session session, string name)
		{
			this.session = session;
			this.name = name;
		}

		public long GetCost(bool includeParents, bool includeChildren, InstallState installState)
		{
			int iCost;
			uint num = RemotableNativeMethods.MsiGetFeatureCost((int)session.Handle, name, (includeParents ? 2 : 0) | (includeChildren ? 1 : 0), (int)installState, out iCost);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			checked
			{
				return unchecked((long)iCost) * 512L;
			}
		}
	}
}
