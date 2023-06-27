using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum TransformValidations
	{
		None = 0,
		Language = 1,
		Product = 2,
		MajorVersion = 8,
		MinorVersion = 0x10,
		UpdateVersion = 0x20,
		NewLessBaseVersion = 0x40,
		NewLessEqualBaseVersion = 0x80,
		NewEqualBaseVersion = 0x100,
		NewGreaterEqualBaseVersion = 0x200,
		NewGreaterBaseVersion = 0x400,
		UpgradeCode = 0x800
	}
}
