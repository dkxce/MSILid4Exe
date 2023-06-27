using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum UpgradeAttributes
	{
		MigrateFeatures = 1,
		OnlyDetect = 2,
		IgnoreRemoveFailure = 4,
		VersionMinInclusive = 0x100,
		VersionMaxInclusive = 0x200,
		LanguagesExclusive = 0x400
	}
}
