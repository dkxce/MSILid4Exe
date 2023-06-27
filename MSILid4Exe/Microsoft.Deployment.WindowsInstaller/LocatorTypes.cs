using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum LocatorTypes
	{
		Directory = 0,
		FileName = 1,
		RawValue = 2,
		SixtyFourBit = 0x10
	}
}
