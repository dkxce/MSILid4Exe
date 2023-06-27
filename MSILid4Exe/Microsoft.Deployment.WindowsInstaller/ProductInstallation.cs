using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class ProductInstallation : Installation
	{
		private IDictionary<string, string> properties;

		public static IEnumerable<ProductInstallation> AllProducts => GetProducts(null, null, UserContexts.All);

		public IEnumerable<FeatureInstallation> Features
		{
			get
			{
				StringBuilder buf = new StringBuilder(256);
				for (uint i = 0u; NativeMethods.MsiEnumFeatures(ProductCode, i, buf, null) == 0; i = checked(i + 1u))
				{
					yield return new FeatureInstallation(buf.ToString(), ProductCode);
				}
			}
		}

		public string ProductCode => base.InstallationCode;

		public override bool IsInstalled => State == InstallState.Default;

		public bool IsAdvertised => State == InstallState.Advertised;

		public bool IsElevated
		{
			get
			{
				bool fElevated;
				uint num = NativeMethods.MsiIsProductElevated(ProductCode, out fElevated);
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
				return fElevated;
			}
		}

		public override SourceList SourceList
		{
			get
			{
				if (properties != null)
				{
					return null;
				}
				return base.SourceList;
			}
		}

		internal InstallState State
		{
			get
			{
				if (properties != null)
				{
					return InstallState.Unknown;
				}
				return (InstallState)NativeMethods.MsiQueryProductState(ProductCode);
			}
		}

		internal override int InstallationType => 0;

		public string HelpLink => this["HelpLink"];

		public string HelpTelephone => this["HelpTelephone"];

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

		public string ProductName => this["InstalledProductName"];

		public string InstallLocation => this["InstallLocation"];

		public string InstallSource => this["InstallSource"];

		public string LocalPackage => this["LocalPackage"];

		public string Publisher => this["Publisher"];

		public Uri UrlInfoAbout
		{
			get
			{
				string text = this["URLInfoAbout"];
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

		public Uri UrlUpdateInfo
		{
			get
			{
				string text = this["URLUpdateInfo"];
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

		public Version ProductVersion => ParseVersion(this["VersionString"]);

		public string ProductId => this["ProductID"];

		public string RegCompany => this["RegCompany"];

		public string RegOwner => this["RegOwner"];

		public string AdvertisedTransforms => this["Transforms"];

		public string AdvertisedLanguage => this["Language"];

		public string AdvertisedProductName => this["ProductName"];

		public bool AdvertisedPerMachine => this["AssignmentType"] == "1";

		public string AdvertisedPackageCode => this["PackageCode"];

		public Version AdvertisedVersion => ParseVersion(this["Version"]);

		public string AdvertisedProductIcon => this["ProductIcon"];

		public string AdvertisedPackageName => this["PackageName"];

		public bool PrivilegedPatchingAuthorized => this["AuthorizedLUAApp"] == "1";

		public override string this[string propertyName]
		{
			get
			{
				if (properties != null)
				{
					string value = null;
					properties.TryGetValue(propertyName, out value);
					return value;
				}
				StringBuilder stringBuilder = new StringBuilder(40);
				checked
				{
					uint pcchValue = (uint)stringBuilder.Capacity;
					uint num;
					if (base.Context == UserContexts.UserManaged || base.Context == UserContexts.UserUnmanaged || base.Context == UserContexts.Machine)
					{
						num = NativeMethods.MsiGetProductInfoEx(ProductCode, base.UserSid, base.Context, propertyName, stringBuilder, ref pcchValue);
						if (num == 234)
						{
							stringBuilder.Capacity = (int)(++pcchValue);
							num = NativeMethods.MsiGetProductInfoEx(ProductCode, base.UserSid, base.Context, propertyName, stringBuilder, ref pcchValue);
						}
					}
					else
					{
						num = NativeMethods.MsiGetProductInfo(ProductCode, propertyName, stringBuilder, ref pcchValue);
						if (num == 234)
						{
							stringBuilder.Capacity = (int)(++pcchValue);
							num = NativeMethods.MsiGetProductInfo(ProductCode, propertyName, stringBuilder, ref pcchValue);
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

		public static IEnumerable<ProductInstallation> GetRelatedProducts(string upgradeCode)
		{
			StringBuilder buf = new StringBuilder(40);
			uint i = 0u;
			while (true)
			{
				uint num = NativeMethods.MsiEnumRelatedProducts(upgradeCode, 0u, i, buf);
				switch (num)
				{
				default:
					throw InstallerException.ExceptionFromReturnCode(num);
				case 0u:
					yield return new ProductInstallation(buf.ToString());
					i = checked(i + 1u);
					break;
				case 259u:
					yield break;
				}
			}
		}

		public static IEnumerable<ProductInstallation> GetProducts(string productCode, string userSid, UserContexts context)
		{
			StringBuilder buf = new StringBuilder(40);
			StringBuilder targetSidBuf = new StringBuilder(40);
			uint i = 0u;
			checked
			{
				while (true)
				{
					uint pcchSid = (uint)targetSidBuf.Capacity;
					UserContexts pdwInstalledContext;
					uint num = NativeMethods.MsiEnumProductsEx(productCode, userSid, context, i, buf, out pdwInstalledContext, targetSidBuf, ref pcchSid);
					if (num == 234)
					{
						targetSidBuf.Capacity = (int)(++pcchSid);
						num = NativeMethods.MsiEnumProductsEx(productCode, userSid, context, i, buf, out pdwInstalledContext, targetSidBuf, ref pcchSid);
					}
					switch (num)
					{
					default:
						throw InstallerException.ExceptionFromReturnCode(num);
					case 0u:
						yield return new ProductInstallation(buf.ToString(), targetSidBuf.ToString(), pdwInstalledContext);
						i++;
						break;
					case 259u:
						yield break;
					}
				}
			}
		}

		public ProductInstallation(string productCode)
			: this(productCode, null, UserContexts.All)
		{
		}

		public ProductInstallation(string productCode, string userSid, UserContexts context)
			: base(productCode, userSid, context)
		{
			if (string.IsNullOrEmpty(productCode))
			{
				throw new ArgumentNullException("productCode");
			}
		}

		internal ProductInstallation(IDictionary<string, string> properties)
			: base(properties["ProductCode"], null, UserContexts.None)
		{
			this.properties = properties;
		}

		public InstallState GetFeatureState(string feature)
		{
			if (properties != null)
			{
				return InstallState.Unknown;
			}
			int pdwState;
			uint num = NativeMethods.MsiQueryFeatureStateEx(ProductCode, base.UserSid, base.Context, feature, out pdwState);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return (InstallState)pdwState;
		}

		public InstallState GetComponentState(string component)
		{
			if (properties != null)
			{
				return InstallState.Unknown;
			}
			int pdwState;
			uint num = NativeMethods.MsiQueryComponentState(ProductCode, base.UserSid, base.Context, component, out pdwState);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return (InstallState)pdwState;
		}

		public void CollectUserInfo()
		{
			if (properties == null)
			{
				uint num = NativeMethods.MsiCollectUserInfo(base.InstallationCode);
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
			}
		}

		private static Version ParseVersion(string ver)
		{
			checked
			{
				if (ver != null)
				{
					int num = 0;
					for (int i = 0; i < ver.Length; i++)
					{
						char c = ver[i];
						if (c == '.')
						{
							num++;
						}
						else if (!char.IsDigit(c))
						{
							ver = ver.Substring(0, i);
							break;
						}
					}
					if (ver.Length > 0)
					{
						if (num == 0)
						{
							ver += ".0";
						}
						else if (num > 3)
						{
							string[] value = ver.Split('.');
							ver = string.Join(".", value, 0, 4);
						}
						try
						{
							return new Version(ver);
						}
						catch (ArgumentException)
						{
						}
					}
				}
				return null;
			}
		}
	}
}
