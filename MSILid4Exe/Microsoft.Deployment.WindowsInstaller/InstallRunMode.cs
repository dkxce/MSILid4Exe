namespace Microsoft.Deployment.WindowsInstaller
{
	public enum InstallRunMode
	{
		Admin = 0,
		Advertise = 1,
		Maintenance = 2,
		RollbackEnabled = 3,
		LogEnabled = 4,
		Operations = 5,
		RebootAtEnd = 6,
		RebootNow = 7,
		Cabinet = 8,
		SourceShortNames = 9,
		TargetShortNames = 10,
		Windows9x = 12,
		ZawEnabled = 13,
		Scheduled = 16,
		Rollback = 17,
		Commit = 18
	}
}
