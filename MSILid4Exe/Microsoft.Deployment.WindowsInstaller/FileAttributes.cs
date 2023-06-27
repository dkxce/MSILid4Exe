using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum FileAttributes
	{
		None = 0,
		ReadOnly = 1,
		Hidden = 2,
		System = 4,
		Vital = 0x200,
		Checksum = 0x400,
		PatchAdded = 0x1000,
		NonCompressed = 0x2000,
		Compressed = 0x4000
	}
}
