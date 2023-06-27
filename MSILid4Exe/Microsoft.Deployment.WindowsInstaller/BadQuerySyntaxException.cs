using System;
using System.Runtime.Serialization;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Serializable]
	public class BadQuerySyntaxException : InstallerException
	{
		public BadQuerySyntaxException(string msg, Exception innerException)
			: base(1615, msg, innerException)
		{
		}

		public BadQuerySyntaxException(string msg)
			: this(msg, null)
		{
		}

		public BadQuerySyntaxException()
			: this(null, null)
		{
		}

		protected BadQuerySyntaxException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
