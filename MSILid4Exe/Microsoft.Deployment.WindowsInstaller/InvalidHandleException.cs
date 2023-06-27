using System;
using System.Runtime.Serialization;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Serializable]
	public class InvalidHandleException : InstallerException
	{
		public InvalidHandleException(string msg, Exception innerException)
			: base(6, msg, innerException)
		{
		}

		public InvalidHandleException(string msg)
			: this(msg, null)
		{
		}

		public InvalidHandleException()
			: this(null, null)
		{
		}

		protected InvalidHandleException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
