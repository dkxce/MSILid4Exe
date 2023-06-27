using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum TransactionAttributes
	{
		None = 0,
		ChainEmbeddedUI = 1,
		JoinExistingEmbeddedUI = 2
	}
}
