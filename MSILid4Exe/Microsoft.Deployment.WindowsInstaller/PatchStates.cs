using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum PatchStates
	{
		None = 0,
		Applied = 1,
		Superseded = 2,
		Obsoleted = 4,
		Registered = 8,
		All = 0xF
	}
}
