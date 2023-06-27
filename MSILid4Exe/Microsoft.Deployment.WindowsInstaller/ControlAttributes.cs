using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum ControlAttributes
	{
		Visible = 1,
		Enabled = 2,
		Sunken = 4,
		Indirect = 8,
		Integer = 0x10,
		RightToLeftReadingOrder = 0x20,
		RightAligned = 0x40,
		LeftScroll = 0x80,
		Bidirectional = 0xE0,
		Transparent = 0x10000,
		NoPrefix = 0x20000,
		NoWrap = 0x40000,
		FormatSize = 0x80000,
		UsersLanguage = 0x100000,
		Multiline = 0x10000,
		PasswordInput = 0x200000,
		Progress95 = 0x10000,
		RemovableVolume = 0x10000,
		FixedVolume = 0x20000,
		RemoteVolume = 0x40000,
		CdromVolume = 0x80000,
		RamDiskVolume = 0x100000,
		FloppyVolume = 0x200000,
		ShowRollbackCost = 0x400000,
		Sorted = 0x10000,
		ComboList = 0x20000,
		PushLike = 0x20000,
		Bitmap = 0x40000,
		Icon = 0x80000,
		FixedSize = 0x100000,
		IconSize16 = 0x200000,
		IconSize32 = 0x400000,
		IconSize48 = 0x600000,
		ElevationShield = 0x800000,
		HasBorder = 0x1000000
	}
}
