using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum DialogAttributes
	{
		Visible = 1,
		Modal = 2,
		Minimize = 4,
		SysModal = 8,
		KeepModeless = 0x10,
		TrackDiskSpace = 0x20,
		UseCustomPalette = 0x40,
		RightToLeftReadingOrder = 0x80,
		RightAligned = 0x100,
		LeftScroll = 0x200,
		Bidirectional = 0x380,
		Error = 0x10000
	}
}
