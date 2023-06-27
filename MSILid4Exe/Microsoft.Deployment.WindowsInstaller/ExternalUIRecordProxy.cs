using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal class ExternalUIRecordProxy
	{
		private ExternalUIRecordHandler handler;

		public ExternalUIRecordHandler Handler => handler;

		internal ExternalUIRecordProxy(ExternalUIRecordHandler handler)
		{
			this.handler = handler;
		}

		public int ProxyHandler(IntPtr contextPtr, int messageType, int recordHandle)
		{
			try
			{
				int messageType2 = messageType & 0x7F000000;
				int buttons = messageType & 0xF;
				int icon = messageType & 0xF0;
				int defaultButton = messageType & 0xF00;
				Record record = ((recordHandle != 0) ? Record.FromHandle((IntPtr)recordHandle, ownsHandle: false) : null);
				using (record)
				{
					return (int)handler((InstallMessage)messageType2, record, (MessageButtons)buttons, (MessageIcon)icon, (MessageDefaultButton)defaultButton);
				}
			}
			catch
			{
				return -1;
			}
		}
	}
}
