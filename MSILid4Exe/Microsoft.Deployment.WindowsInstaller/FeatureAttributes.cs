using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum FeatureAttributes
	{
		None = 0,
		FavorSource = 1,
		FollowParent = 2,
		FavorAdvertise = 4,
		DisallowAdvertise = 8,
		UIDisallowAbsent = 0x10,
		NoUnsupportedAdvertise = 0x20
	}
}
