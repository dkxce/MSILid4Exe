using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum RemoveFileModes
	{
		None = 0,
		OnInstall = 1,
		OnRemove = 2
	}
}
