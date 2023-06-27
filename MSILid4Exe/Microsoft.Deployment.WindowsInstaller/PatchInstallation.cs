using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class PatchInstallation : Installation
	{
		private string productCode;

		public static IEnumerable<PatchInstallation> AllPatches => GetPatches(null, null, null, UserContexts.All, PatchStates.All);

		public string PatchCode => base.InstallationCode;

		public string ProductCode => productCode;

		public override bool IsInstalled => (State & PatchStates.Applied) != 0;

		public bool IsObsoleted => (State & PatchStates.Obsoleted) != 0;

		public bool IsSuperseded => (State & PatchStates.Superseded) != 0;

		internal override int InstallationType => 1073741824;

		public PatchStates State => (PatchStates)int.Parse(this["State"], CultureInfo.InvariantCulture.NumberFormat);

		public string LocalPackage => this["LocalPackage"];

		public string Transforms => this["Transforms"];

		public DateTime InstallDate
		{
			get
			{
				try
				{
					return DateTime.ParseExact(this["InstallDate"], "yyyyMMdd", CultureInfo.InvariantCulture);
				}
				catch (FormatException)
				{
					return DateTime.MinValue;
				}
			}
		}

		public bool Uninstallable => this["Uninstallable"] == "1";

		public string DisplayName => this["DisplayName"];

		public Uri MoreInfoUrl
		{
			get
			{
				string text = this["MoreInfoURL"];
				if (!string.IsNullOrEmpty(text))
				{
					try
					{
						return new Uri(text);
					}
					catch (UriFormatException)
					{
					}
				}
				return null;
			}
		}

		public override string this[string propertyName]
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("");
				uint pcchValueBuf = 0u;
				checked
				{
					uint num;
					if (base.Context == UserContexts.UserManaged || base.Context == UserContexts.UserUnmanaged || base.Context == UserContexts.Machine)
					{
						num = NativeMethods.MsiGetPatchInfoEx(PatchCode, ProductCode, base.UserSid, base.Context, propertyName, stringBuilder, ref pcchValueBuf);
						if (num == 234)
						{
							stringBuilder.Capacity = (int)(++pcchValueBuf);
							num = NativeMethods.MsiGetPatchInfoEx(PatchCode, ProductCode, base.UserSid, base.Context, propertyName, stringBuilder, ref pcchValueBuf);
						}
					}
					else
					{
						num = NativeMethods.MsiGetPatchInfo(PatchCode, propertyName, stringBuilder, ref pcchValueBuf);
						if (num == 234)
						{
							stringBuilder.Capacity = (int)(++pcchValueBuf);
							num = NativeMethods.MsiGetPatchInfo(PatchCode, propertyName, stringBuilder, ref pcchValueBuf);
						}
					}
					if (num != 0)
					{
						return null;
					}
					return stringBuilder.ToString();
				}
			}
		}

		public static IEnumerable<PatchInstallation> GetPatches(string patchCode, string targetProductCode, string userSid, UserContexts context, PatchStates states)
		{
			StringBuilder buf = new StringBuilder(40);
			StringBuilder targetProductBuf = new StringBuilder(40);
			StringBuilder targetSidBuf = new StringBuilder(40);
			uint i = 0u;
			checked
			{
				while (true)
				{
					uint pcchTargetUserSid = (uint)targetSidBuf.Capacity;
					UserContexts pdwTargetProductContext;
					uint num = NativeMethods.MsiEnumPatchesEx(targetProductCode, userSid, context, (uint)states, i, buf, targetProductBuf, out pdwTargetProductContext, targetSidBuf, ref pcchTargetUserSid);
					if (num == 234)
					{
						targetSidBuf.Capacity = (int)(++pcchTargetUserSid);
						num = NativeMethods.MsiEnumPatchesEx(targetProductCode, userSid, context, (uint)states, i, buf, targetProductBuf, out pdwTargetProductContext, targetSidBuf, ref pcchTargetUserSid);
					}
					switch (num)
					{
					default:
						throw InstallerException.ExceptionFromReturnCode(num);
					case 0u:
					{
						string text = buf.ToString();
						if (patchCode == null || patchCode == text)
						{
							yield return new PatchInstallation(buf.ToString(), targetProductBuf.ToString(), targetSidBuf.ToString(), pdwTargetProductContext);
						}
						i++;
						break;
					}
					case 259u:
						yield break;
					}
				}
			}
		}

		public PatchInstallation(string patchCode, string productCode)
			: this(patchCode, productCode, null, UserContexts.All)
		{
		}

		public PatchInstallation(string patchCode, string productCode, string userSid, UserContexts context)
			: base(patchCode, userSid, context)
		{
			if (string.IsNullOrEmpty(patchCode))
			{
				throw new ArgumentNullException("patchCode");
			}
			this.productCode = productCode;
		}
	}
}
