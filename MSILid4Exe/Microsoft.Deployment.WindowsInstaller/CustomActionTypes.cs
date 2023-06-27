using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum CustomActionTypes
	{
		None = 0,
		Dll = 1,
		Exe = 2,
		TextData = 3,
		JScript = 5,
		VBScript = 6,
		Install = 7,
		SourceFile = 0x10,
		Directory = 0x20,
		Property = 0x30,
		Continue = 0x40,
		Async = 0x80,
		FirstSequence = 0x100,
		OncePerProcess = 0x200,
		ClientRepeat = 0x300,
		InScript = 0x400,
		Rollback = 0x100,
		Commit = 0x200,
		NoImpersonate = 0x800,
		TSAware = 0x4000,
		SixtyFourBitScript = 0x1000,
		HideTarget = 0x2000,
		PatchUninstall = 0x8000
	}
}
