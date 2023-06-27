using System;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class FeatureInstallation : InstallationPart
	{
		public struct UsageData
		{
			private int useCount;

			private DateTime lastUsedDate;

			public int UseCount => useCount;

			public DateTime LastUsedDate => lastUsedDate;

			internal UsageData(int useCount, DateTime lastUsedDate)
			{
				this.useCount = useCount;
				this.lastUsedDate = lastUsedDate;
			}
		}

		public string FeatureName => base.Id;

		public override InstallState State => (InstallState)NativeMethods.MsiQueryFeatureState(base.ProductCode, FeatureName);

		public FeatureInstallation Parent
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(256);
				StringBuilder stringBuilder2 = new StringBuilder(256);
				for (uint num = 0u; NativeMethods.MsiEnumFeatures(base.ProductCode, num, stringBuilder, stringBuilder2) == 0; num = checked(num + 1u))
				{
					if (stringBuilder.ToString() == FeatureName)
					{
						if (stringBuilder2.Length > 0)
						{
							return new FeatureInstallation(stringBuilder2.ToString(), base.ProductCode);
						}
						return null;
					}
				}
				return null;
			}
		}

		public UsageData Usage
		{
			get
			{
				uint dwUseCount;
				ushort dwDateUsed;
				uint num = NativeMethods.MsiGetFeatureUsage(base.ProductCode, FeatureName, out dwUseCount, out dwDateUsed);
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
				return checked(new UsageData(lastUsedDate: (dwUseCount != 0) ? new DateTime(1980 + (dwDateUsed >> 9), (dwDateUsed & 0x1FF) >> 5, dwDateUsed & 0x1F) : DateTime.MinValue, useCount: (int)dwUseCount));
			}
		}

		public FeatureInstallation(string featureName, string productCode)
			: base(featureName, productCode)
		{
			if (string.IsNullOrEmpty(featureName))
			{
				throw new ArgumentNullException("featureName");
			}
		}
	}
}
