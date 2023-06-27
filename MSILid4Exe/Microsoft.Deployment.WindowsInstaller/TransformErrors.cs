using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum TransformErrors
	{
		None = 0,
		AddExistingRow = 1,
		DelMissingRow = 2,
		AddExistingTable = 4,
		DelMissingTable = 8,
		UpdateMissingRow = 0x10,
		ChangeCodePage = 0x20,
		ViewTransform = 0x100
	}
}
