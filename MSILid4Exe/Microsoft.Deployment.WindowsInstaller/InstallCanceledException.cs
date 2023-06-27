using System;
using System.Runtime.Serialization;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Serializable]
	public class InstallCanceledException : InstallerException
	{
		public InstallCanceledException(string msg, Exception innerException)
			: base(1602, msg, innerException)
		{
		}

		public InstallCanceledException(string msg)
			: this(msg, null)
		{
		}

		public InstallCanceledException()
			: this(null, null)
		{
		}

		protected InstallCanceledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
