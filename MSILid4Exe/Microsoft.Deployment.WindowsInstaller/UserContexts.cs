using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Flags]
	public enum UserContexts
	{
		None = 0,
		UserManaged = 1,
		UserUnmanaged = 2,
		Machine = 4,
		All = 7,
		AllUserManaged = 8
	}
}
