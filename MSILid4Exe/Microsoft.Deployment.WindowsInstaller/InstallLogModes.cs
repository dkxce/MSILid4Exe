using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum InstallLogModes
	{
		None = 0,
		FatalExit = 1,
		Error = 2,
		Warning = 4,
		User = 8,
		Info = 0x10,
		ResolveSource = 0x40,
		OutOfDiskSpace = 0x80,
		ActionStart = 0x100,
		ActionData = 0x200,
		CommonData = 0x800,
		PropertyDump = 0x400,
		Verbose = 0x1000,
		ExtraDebug = 0x2000,
		LogOnlyOnError = 0x4000,
		Progress = 0x400,
		Initialize = 0x1000,
		Terminate = 0x2000,
		ShowDialog = 0x4000,
		FilesInUse = 0x20,
		RMFilesInUse = 0x2000000
	}
}
