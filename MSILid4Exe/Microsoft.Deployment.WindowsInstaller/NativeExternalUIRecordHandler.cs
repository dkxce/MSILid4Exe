using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal delegate int NativeExternalUIRecordHandler(IntPtr context, int messageType, int recordHandle);
}
