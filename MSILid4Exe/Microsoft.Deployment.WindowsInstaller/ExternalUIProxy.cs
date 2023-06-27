using System;
using System.Runtime.InteropServices;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal class ExternalUIProxy
	{
		private ExternalUIHandler handler;

		public ExternalUIHandler Handler => handler;

		internal ExternalUIProxy(ExternalUIHandler handler)
		{
			this.handler = handler;
		}

		public int ProxyHandler(IntPtr contextPtr, int messageType, [MarshalAs(UnmanagedType.LPWStr)] string message)
		{
			try
			{
				int messageType2 = messageType & 0x7F000000;
				int buttons = messageType & 0xF;
				int icon = messageType & 0xF0;
				int defaultButton = messageType & 0xF00;
				return (int)handler((InstallMessage)messageType2, message, (MessageButtons)buttons, (MessageIcon)icon, (MessageDefaultButton)defaultButton);
			}
			catch
			{
				return -1;
			}
		}
	}
}
