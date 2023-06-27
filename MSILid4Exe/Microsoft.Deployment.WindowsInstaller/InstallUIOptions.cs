using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum InstallUIOptions
	{
		NoChange = 0,
		Default = 1,
		Silent = 2,
		Basic = 3,
		Reduced = 4,
		Full = 5,
		HideCancel = 0x20,
		ProgressOnly = 0x40,
		EndDialog = 0x80,
		SourceResolutionOnly = 0x100,
		UacOnly = 0x200
	}
}
