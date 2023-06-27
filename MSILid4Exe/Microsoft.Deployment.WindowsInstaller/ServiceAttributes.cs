using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum ServiceAttributes
	{
		None = 0,
		OwnProcess = 0x10,
		ShareProcess = 0x20,
		Interactive = 0x100,
		AutoStart = 2,
		DemandStart = 3,
		Disabled = 4,
		ErrorMessage = 1,
		ErrorCritical = 3,
		ErrorControlVital = 0x8000
	}
}
