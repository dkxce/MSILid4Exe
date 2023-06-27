using System;
using System.Runtime.InteropServices;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal delegate int NativeExternalUIHandler(IntPtr context, int messageType, [MarshalAs(UnmanagedType.LPWStr)] string message);
}
