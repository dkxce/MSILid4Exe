namespace Microsoft.Deployment.WindowsInstaller
{
	public enum InstallState
	{
		NotUsed = -7,
		BadConfig = -6,
		Incomplete = -5,
		SourceAbsent = -4,
		MoreData = -3,
		InvalidArgument = -2,
		Unknown = -1,
		Broken = 0,
		Advertised = 1,
		Removed = 1,
		Absent = 2,
		Local = 3,
		Source = 4,
		Default = 5
	}
}
