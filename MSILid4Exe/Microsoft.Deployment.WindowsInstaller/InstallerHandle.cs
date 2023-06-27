using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	public abstract class InstallerHandle : MarshalByRefObject, IDisposable
	{
		private NativeMethods.MsiHandle handle;

		public IntPtr Handle
		{
			get
			{
				if (IsClosed)
				{
					throw new InvalidHandleException();
				}
				return handle;
			}
		}

		public bool IsClosed => handle.IsClosed;

		internal object Sync => handle;

		protected InstallerHandle(IntPtr handle, bool ownsHandle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new InvalidHandleException();
			}
			this.handle = new NativeMethods.MsiHandle(handle, ownsHandle);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Close()
		{
			Dispose();
		}

		public override bool Equals(object obj)
		{
			if (obj != null && GetType() == obj.GetType())
			{
				return Handle == ((InstallerHandle)obj).Handle;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Handle.GetHashCode();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				handle.Dispose();
			}
		}
	}
}
