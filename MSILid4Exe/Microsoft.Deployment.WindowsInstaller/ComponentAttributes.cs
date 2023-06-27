using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum ComponentAttributes
	{
		None = 0,
		SourceOnly = 1,
		Optional = 2,
		RegistryKeyPath = 4,
		SharedDllRefCount = 8,
		Permanent = 0x10,
		OdbcDataSource = 0x20,
		Transitive = 0x40,
		NeverOverwrite = 0x80,
		SixtyFourBit = 0x100,
		DisableRegistryReflection = 0x200,
		UninstallOnSupersedence = 0x400,
		Shared = 0x800
	}
}
