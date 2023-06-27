using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	internal enum MessageBoxStyles
	{
		TopMost = 0x40000,
		ServiceNotification = 0x200000
	}
}
