using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum ServiceControlEvents
	{
		None = 0,
		Start = 1,
		Stop = 2,
		Delete = 8,
		UninstallStart = 0x10,
		UninstallStop = 0x20,
		UninstallDelete = 0x80
	}
}
