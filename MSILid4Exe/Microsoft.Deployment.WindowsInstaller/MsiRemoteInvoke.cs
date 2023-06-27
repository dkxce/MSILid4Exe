using System;
using System.Runtime.InteropServices;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal delegate void MsiRemoteInvoke(RemoteMsiFunctionId id, [MarshalAs(UnmanagedType.SysInt)] IntPtr request, [MarshalAs(UnmanagedType.SysInt)] out IntPtr response);
}
