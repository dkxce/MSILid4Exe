namespace Microsoft.Deployment.WindowsInstaller
{
	public enum InstallMessage
	{
		FatalExit = 0,
		Error = 16777216,
		Warning = 33554432,
		User = 50331648,
		Info = 67108864,
		FilesInUse = 83886080,
		ResolveSource = 100663296,
		OutOfDiskSpace = 117440512,
		ActionStart = 134217728,
		ActionData = 150994944,
		Progress = 167772160,
		CommonData = 184549376,
		Initialize = 201326592,
		Terminate = 218103808,
		ShowDialog = 234881024,
		RMFilesInUse = 419430400,
		InstallStart = 436207616,
		InstallEnd = 452984832
	}
}
