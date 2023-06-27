using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Guid("0000000b-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IStorage
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		IStream CreateStream([MarshalAs(UnmanagedType.LPWStr)] string wcsName, uint grfMode, uint reserved1, uint reserved2);

		[return: MarshalAs(UnmanagedType.Interface)]
		IStream OpenStream([MarshalAs(UnmanagedType.LPWStr)] string wcsName, IntPtr reserved1, uint grfMode, uint reserved2);

		[return: MarshalAs(UnmanagedType.Interface)]
		IStorage CreateStorage([MarshalAs(UnmanagedType.LPWStr)] string wcsName, uint grfMode, uint reserved1, uint reserved2);

		[return: MarshalAs(UnmanagedType.Interface)]
		IStorage OpenStorage([MarshalAs(UnmanagedType.LPWStr)] string wcsName, IntPtr stgPriority, uint grfMode, IntPtr snbExclude, uint reserved);

		void CopyTo(uint ciidExclude, IntPtr rgiidExclude, IntPtr snbExclude, [MarshalAs(UnmanagedType.Interface)] IStorage stgDest);

		void MoveElementTo([MarshalAs(UnmanagedType.LPWStr)] string wcsName, [MarshalAs(UnmanagedType.Interface)] IStorage stgDest, [MarshalAs(UnmanagedType.LPWStr)] string wcsNewName, uint grfFlags);

		void Commit(uint grfCommitFlags);

		void Revert();

		IntPtr EnumElements(uint reserved1, IntPtr reserved2, uint reserved3);

		void DestroyElement([MarshalAs(UnmanagedType.LPWStr)] string wcsName);

		void RenameElement([MarshalAs(UnmanagedType.LPWStr)] string wcsOldName, [MarshalAs(UnmanagedType.LPWStr)] string wcsNewName);

		void SetElementTimes([MarshalAs(UnmanagedType.LPWStr)] string wcsName, ref System.Runtime.InteropServices.ComTypes.FILETIME ctime, ref System.Runtime.InteropServices.ComTypes.FILETIME atime, ref System.Runtime.InteropServices.ComTypes.FILETIME mtime);

		void SetClass(ref Guid clsid);

		void SetStateBits(uint grfStateBits, uint grfMask);

		void Stat(ref System.Runtime.InteropServices.ComTypes.STATSTG statstg, uint grfStatFlag);
	}
}
