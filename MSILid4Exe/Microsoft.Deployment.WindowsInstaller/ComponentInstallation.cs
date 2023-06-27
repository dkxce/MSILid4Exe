using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class ComponentInstallation : InstallationPart
	{
		public struct Qualifier
		{
			private string qualifierCode;

			private string data;

			public string QualifierCode => qualifierCode;

			public string Data => data;

			internal Qualifier(string qualifierCode, string data)
			{
				this.qualifierCode = qualifierCode;
				this.data = data;
			}
		}

		public static IEnumerable<ComponentInstallation> AllComponents
		{
			get
			{
				StringBuilder buf = new StringBuilder(40);
				uint i = 0u;
				while (true)
				{
					uint num = NativeMethods.MsiEnumComponents(i, buf);
					switch (num)
					{
					default:
						throw InstallerException.ExceptionFromReturnCode(num);
					case 0u:
						yield return new ComponentInstallation(buf.ToString());
						i = checked(i + 1u);
						break;
					case 259u:
						yield break;
					}
				}
			}
		}

		public string ComponentCode => base.Id;

		public IEnumerable<ProductInstallation> ClientProducts
		{
			get
			{
				StringBuilder buf = new StringBuilder(40);
				uint i = 0u;
				while (true)
				{
					uint pcchSid = 0u;
					UserContexts pdwInstalledContext;
					uint num = ((base.Context == UserContexts.None) ? NativeMethods.MsiEnumClients(ComponentCode, i, buf) : NativeMethods.MsiEnumClientsEx(ComponentCode, base.UserSid, base.Context, i, buf, out pdwInstalledContext, null, ref pcchSid));
					switch (num)
					{
					default:
						throw InstallerException.ExceptionFromReturnCode(num);
					case 0u:
						yield return new ProductInstallation(buf.ToString());
						i = checked(i + 1u);
						break;
					case 259u:
					case 1607u:
						yield break;
					}
				}
			}
		}

		public override InstallState State
		{
			get
			{
				if (base.ProductCode != null)
				{
					uint pcchBuf = 0u;
					if (base.Context != 0)
					{
						return (InstallState)NativeMethods.MsiGetComponentPathEx(base.ProductCode, ComponentCode, base.UserSid, base.Context, null, ref pcchBuf);
					}
					return (InstallState)NativeMethods.MsiGetComponentPath(base.ProductCode, ComponentCode, null, ref pcchBuf);
				}
				return InstallState.Unknown;
			}
		}

		public string Path
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(256);
				uint pcchBuf = checked((uint)stringBuilder.Capacity);
				InstallState installState;
				if (base.ProductCode != null)
				{
					installState = (InstallState)((base.Context == UserContexts.None) ? NativeMethods.MsiGetComponentPath(base.ProductCode, ComponentCode, stringBuilder, ref pcchBuf) : NativeMethods.MsiGetComponentPathEx(base.ProductCode, ComponentCode, base.UserSid, base.Context, stringBuilder, ref pcchBuf));
					if (installState == InstallState.MoreData)
					{
						stringBuilder.Capacity = checked((int)(++pcchBuf));
						installState = (InstallState)((base.Context == UserContexts.None) ? NativeMethods.MsiGetComponentPath(base.ProductCode, ComponentCode, stringBuilder, ref pcchBuf) : NativeMethods.MsiGetComponentPathEx(base.ProductCode, ComponentCode, base.UserSid, base.Context, stringBuilder, ref pcchBuf));
					}
				}
				else
				{
					installState = (InstallState)checked((int)NativeMethods.MsiLocateComponent(ComponentCode, stringBuilder, ref pcchBuf));
					if (installState == InstallState.MoreData)
					{
						stringBuilder.Capacity = checked((int)(++pcchBuf));
						installState = (InstallState)checked((int)NativeMethods.MsiLocateComponent(ComponentCode, stringBuilder, ref pcchBuf));
					}
				}
				if (installState == InstallState.SourceAbsent || (uint)(installState - 3) <= 1u)
				{
					return stringBuilder.ToString();
				}
				return null;
			}
		}

		public IEnumerable<Qualifier> Qualifiers
		{
			get
			{
				StringBuilder qualBuf = new StringBuilder(255);
				StringBuilder dataBuf = new StringBuilder(255);
				uint i = 0u;
				checked
				{
					while (true)
					{
						uint pcchQualifierBuf = (uint)qualBuf.Capacity;
						uint pcchApplicationDataBuf = (uint)dataBuf.Capacity;
						uint num = NativeMethods.MsiEnumComponentQualifiers(ComponentCode, i, qualBuf, ref pcchQualifierBuf, dataBuf, ref pcchApplicationDataBuf);
						if (num == 234)
						{
							qualBuf.Capacity = (int)(++pcchQualifierBuf);
							dataBuf.Capacity = (int)(++pcchApplicationDataBuf);
							num = NativeMethods.MsiEnumComponentQualifiers(ComponentCode, i, qualBuf, ref pcchQualifierBuf, dataBuf, ref pcchApplicationDataBuf);
						}
						switch (num)
						{
						default:
							throw InstallerException.ExceptionFromReturnCode(num);
						case 0u:
							yield return new Qualifier(qualBuf.ToString(), dataBuf.ToString());
							i++;
							break;
						case 259u:
						case 1607u:
							yield break;
						}
					}
				}
			}
		}

		public static IEnumerable<ComponentInstallation> Components(string szUserSid, UserContexts dwContext)
		{
			uint pcchSid = 32u;
			checked
			{
				StringBuilder szSid = new StringBuilder((int)pcchSid);
				StringBuilder buf = new StringBuilder(40);
				uint i = 0u;
				while (true)
				{
					UserContexts pdwInstalledContext;
					uint num = NativeMethods.MsiEnumComponentsEx(szUserSid, dwContext, i, buf, out pdwInstalledContext, szSid, ref pcchSid);
					if (num == 234)
					{
						uint num2 = pcchSid + 1u;
						pcchSid = num2;
						szSid.EnsureCapacity((int)num2);
						num = NativeMethods.MsiEnumComponentsEx(szUserSid, dwContext, i, buf, out pdwInstalledContext, szSid, ref pcchSid);
					}
					switch (num)
					{
					default:
						throw InstallerException.ExceptionFromReturnCode(num);
					case 0u:
						yield return new ComponentInstallation(buf.ToString(), szSid.ToString(), pdwInstalledContext);
						i++;
						break;
					case 259u:
						yield break;
					}
				}
			}
		}

		private static string GetProductCode(string component)
		{
			StringBuilder stringBuilder = new StringBuilder(40);
			if (NativeMethods.MsiGetProductCode(component, stringBuilder) != 0)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		private static string GetProductCode(string component, string szUserSid, UserContexts dwContext)
		{
			return GetProductCode(component);
		}

		public ComponentInstallation(string componentCode)
			: this(componentCode, GetProductCode(componentCode))
		{
		}

		public ComponentInstallation(string componentCode, string szUserSid, UserContexts dwContext)
			: this(componentCode, GetProductCode(componentCode, szUserSid, dwContext), szUserSid, dwContext)
		{
		}

		public ComponentInstallation(string componentCode, string productCode)
			: this(componentCode, productCode, null, UserContexts.None)
		{
		}

		public ComponentInstallation(string componentCode, string productCode, string szUserSid, UserContexts dwContext)
			: base(componentCode, productCode, szUserSid, dwContext)
		{
			if (string.IsNullOrEmpty(componentCode))
			{
				throw new ArgumentNullException("componentCode");
			}
		}
	}
}
