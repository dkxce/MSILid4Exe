using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum ReinstallModes
	{
		FileMissing = 2,
		FileOlderVersion = 4,
		FileEqualVersion = 8,
		FileExact = 0x10,
		FileVerify = 0x20,
		FileReplace = 0x40,
		MachineData = 0x80,
		UserData = 0x100,
		Shortcut = 0x200,
		Package = 0x400
	}
}
