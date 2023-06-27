using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class SourceMediaList : ICollection<MediaDisk>, IEnumerable<MediaDisk>, IEnumerable
	{
		private Installation installation;

		public int Count
		{
			get
			{
				int num = 0;
				IEnumerator<MediaDisk> enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					num = checked(num + 1);
				}
				return num;
			}
		}

		public bool IsReadOnly => false;

		internal SourceMediaList(Installation installation)
		{
			this.installation = installation;
		}

		public void Add(MediaDisk item)
		{
			uint num = checked(NativeMethods.MsiSourceListAddMediaDisk(installation.InstallationCode, installation.UserSid, installation.Context, (uint)installation.InstallationType, (uint)item.DiskId, item.VolumeLabel, item.DiskPrompt));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void Clear()
		{
			uint num = NativeMethods.MsiSourceListClearAllEx(installation.InstallationCode, installation.UserSid, installation.Context, 3u | checked((uint)installation.InstallationType));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public bool Contains(int diskId)
		{
			using (IEnumerator<MediaDisk> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.DiskId == diskId)
					{
						return true;
					}
				}
			}
			return false;
		}

		bool ICollection<MediaDisk>.Contains(MediaDisk mediaDisk)
		{
			return Contains(mediaDisk.DiskId);
		}

		public void CopyTo(MediaDisk[] array, int arrayIndex)
		{
			using (IEnumerator<MediaDisk> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MediaDisk current = enumerator.Current;
					array[checked(arrayIndex++)] = current;
				}
			}
		}

		public bool Remove(int diskId)
		{
			uint num = checked(NativeMethods.MsiSourceListClearMediaDisk(installation.InstallationCode, installation.UserSid, installation.Context, (uint)installation.InstallationType, (uint)diskId));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return true;
		}

		bool ICollection<MediaDisk>.Remove(MediaDisk mediaDisk)
		{
			return Remove(mediaDisk.DiskId);
		}

		public IEnumerator<MediaDisk> GetEnumerator()
		{
			StringBuilder volumeBuf = new StringBuilder(40);
			checked
			{
				uint volumeBufSize = (uint)volumeBuf.Capacity;
				StringBuilder promptBuf = new StringBuilder(80);
				uint promptBufSize = (uint)promptBuf.Capacity;
				uint i = 0u;
				while (true)
				{
					uint pdwDiskId;
					uint num = NativeMethods.MsiSourceListEnumMediaDisks(installation.InstallationCode, installation.UserSid, installation.Context, (uint)installation.InstallationType, i, out pdwDiskId, volumeBuf, ref volumeBufSize, promptBuf, ref promptBufSize);
					if (num == 234)
					{
						uint num2 = volumeBufSize + 1u;
						volumeBufSize = num2;
						volumeBuf.Capacity = (int)num2;
						num2 = promptBufSize + 1u;
						promptBufSize = num2;
						promptBuf.Capacity = (int)num2;
						num = NativeMethods.MsiSourceListEnumMediaDisks(installation.InstallationCode, installation.UserSid, installation.Context, (uint)installation.InstallationType, i, out pdwDiskId, volumeBuf, ref volumeBufSize, promptBuf, ref promptBufSize);
					}
					switch (num)
					{
					default:
						throw InstallerException.ExceptionFromReturnCode(num);
					case 0u:
						yield return new MediaDisk((int)pdwDiskId, volumeBuf.ToString(), promptBuf.ToString());
						i++;
						break;
					case 259u:
						yield break;
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
