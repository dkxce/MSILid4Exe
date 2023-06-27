using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	public delegate void InapplicablePatchHandler(string patch, Exception exception);
}
