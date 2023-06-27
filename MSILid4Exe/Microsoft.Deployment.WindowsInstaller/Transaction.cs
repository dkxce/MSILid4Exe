using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class Transaction : InstallerHandle
	{
		private string name;

		private IntPtr ownerChangeEvent;

		private IList<EventHandler<EventArgs>> ownerChangeListeners;

		public string Name => name;

		public event EventHandler<EventArgs> OwnerChanged
		{
			add
			{
				ownerChangeListeners.Add(value);
				if (ownerChangeEvent != IntPtr.Zero && ownerChangeListeners.Count == 1)
				{
					new Thread(WaitForOwnerChange).Start();
				}
			}
			remove
			{
				ownerChangeListeners.Remove(value);
			}
		}

		public Transaction(string name, TransactionAttributes attributes)
			: this(name, Begin(name, attributes), ownsHandle: true)
		{
		}

		private Transaction(string name, IntPtr[] handles, bool ownsHandle)
			: base(handles[0], ownsHandle)
		{
			this.name = name;
			ownerChangeEvent = handles[1];
			ownerChangeListeners = new List<EventHandler<EventArgs>>();
		}

		public static Transaction FromHandle(IntPtr handle, bool ownsHandle)
		{
			return new Transaction(handle.ToString(), new IntPtr[2]
			{
				handle,
				IntPtr.Zero
			}, ownsHandle);
		}

		private void OnOwnerChanged()
		{
			EventArgs e = new EventArgs();
			foreach (EventHandler<EventArgs> ownerChangeListener in ownerChangeListeners)
			{
				ownerChangeListener(this, e);
			}
		}

		private void WaitForOwnerChange()
		{
			if (NativeMethods.WaitForSingleObject(ownerChangeEvent, -1) == 0)
			{
				OnOwnerChanged();
				return;
			}
			throw new InstallerException();
		}

		public void Join(TransactionAttributes attributes)
		{
			IntPtr phChangeOfOwnerEvent;
			uint num = NativeMethods.MsiJoinTransaction((int)base.Handle, (int)attributes, out phChangeOfOwnerEvent);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			ownerChangeEvent = phChangeOfOwnerEvent;
			if (ownerChangeEvent != IntPtr.Zero && ownerChangeListeners.Count >= 1)
			{
				new Thread(WaitForOwnerChange).Start();
			}
		}

		public void Commit()
		{
			End(commit: true);
		}

		public void Rollback()
		{
			End(commit: false);
		}

		private static IntPtr[] Begin(string transactionName, TransactionAttributes attributes)
		{
			int hTransaction;
			IntPtr phChangeOfOwnerEvent;
			uint num = NativeMethods.MsiBeginTransaction(transactionName, (int)attributes, out hTransaction, out phChangeOfOwnerEvent);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return new IntPtr[2]
			{
				(IntPtr)hTransaction,
				phChangeOfOwnerEvent
			};
		}

		private void End(bool commit)
		{
			uint num = NativeMethods.MsiEndTransaction(commit ? 1 : 0);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}
	}
}
