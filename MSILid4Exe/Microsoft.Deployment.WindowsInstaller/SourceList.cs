using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class SourceList : ICollection<string>, IEnumerable<string>, IEnumerable
	{
		private Installation installation;

		private SourceMediaList mediaList;

		public SourceMediaList MediaList
		{
			get
			{
				if (mediaList == null)
				{
					mediaList = new SourceMediaList(installation);
				}
				return mediaList;
			}
		}

		public int Count
		{
			get
			{
				int num = 0;
				IEnumerator<string> enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					num = checked(num + 1);
				}
				return num;
			}
		}

		public bool IsReadOnly => false;

		public string MediaPackagePath
		{
			get
			{
				return this["MediaPackagePath"];
			}
			set
			{
				this["MediaPackagePath"] = value;
			}
		}

		public string DiskPrompt
		{
			get
			{
				return this["DiskPrompt"];
			}
			set
			{
				this["DiskPrompt"] = value;
			}
		}

		public string LastUsedSource
		{
			get
			{
				return this["LastUsedSource"];
			}
			set
			{
				this["LastUsedSource"] = value;
			}
		}

		public string PackageName
		{
			get
			{
				return this["PackageName"];
			}
			set
			{
				this["PackageName"] = value;
			}
		}

		public string LastUsedType => this["LastUsedType"];

		public string this[string property]
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("");
				uint pcchValue = 0u;
				checked
				{
					uint num = NativeMethods.MsiSourceListGetInfo(installation.InstallationCode, installation.UserSid, installation.Context, (uint)installation.InstallationType, property, stringBuilder, ref pcchValue);
					if (num != 0)
					{
						if (num == 234)
						{
							stringBuilder.Capacity = (int)(++pcchValue);
							num = NativeMethods.MsiSourceListGetInfo(installation.InstallationCode, installation.UserSid, installation.Context, (uint)installation.InstallationType, property, stringBuilder, ref pcchValue);
						}
						switch (num)
						{
						case 1605u:
						case 1608u:
							throw new ArgumentOutOfRangeException("property");
						default:
							throw InstallerException.ExceptionFromReturnCode(num);
						case 0u:
							break;
						}
					}
					return stringBuilder.ToString();
				}
			}
			set
			{
				uint num = NativeMethods.MsiSourceListSetInfo(installation.InstallationCode, installation.UserSid, installation.Context, checked((uint)installation.InstallationType), property, value);
				switch (num)
				{
				case 1605u:
				case 1608u:
					throw new ArgumentOutOfRangeException("property");
				default:
					throw InstallerException.ExceptionFromReturnCode(num);
				case 0u:
					break;
				}
			}
		}

		internal SourceList(Installation installation)
		{
			this.installation = installation;
		}

		public void Add(string item)
		{
			if (!Contains(item))
			{
				Insert(item, 0);
			}
		}

		public void Insert(string item, int index)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			NativeMethods.SourceType sourceType = ((!item.Contains("://")) ? NativeMethods.SourceType.Network : NativeMethods.SourceType.Url);
			uint num = checked(NativeMethods.MsiSourceListAddSourceEx(installation.InstallationCode, installation.UserSid, installation.Context, (uint)sourceType | (uint)installation.InstallationType, item, (uint)index));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void Clear()
		{
			ClearSourceType(NativeMethods.SourceType.Url);
			ClearSourceType(NativeMethods.SourceType.Network);
			MediaList.Clear();
		}

		public void ClearNetworkSources()
		{
			ClearSourceType(NativeMethods.SourceType.Network);
		}

		public void ClearUrlSources()
		{
			ClearSourceType(NativeMethods.SourceType.Url);
		}

		public bool Contains(string item)
		{
			if (string.IsNullOrEmpty(item))
			{
				throw new ArgumentNullException("item");
			}
			using (IEnumerator<string> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Equals(item, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void CopyTo(string[] array, int arrayIndex)
		{
			using (IEnumerator<string> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					array[checked(arrayIndex++)] = current;
				}
			}
		}

		public bool Remove(string item)
		{
			if (string.IsNullOrEmpty(item))
			{
				throw new ArgumentNullException("item");
			}
			NativeMethods.SourceType sourceType = ((!item.Contains("://")) ? NativeMethods.SourceType.Network : NativeMethods.SourceType.Url);
			uint num = NativeMethods.MsiSourceListClearSource(installation.InstallationCode, installation.UserSid, installation.Context, checked((uint)sourceType | (uint)installation.InstallationType), item);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
			return true;
		}

		public IEnumerator<string> GetEnumerator()
		{
			StringBuilder sourceBuf = new StringBuilder(256);
			checked
			{
				_ = (uint)sourceBuf.Capacity;
				uint j = 0u;
				while (true)
				{
					uint num = EnumSources(sourceBuf, j, NativeMethods.SourceType.Network);
					switch (num)
					{
					default:
						throw InstallerException.ExceptionFromReturnCode(num);
					case 0u:
						yield return sourceBuf.ToString();
						break;
					case 259u:
						j = 0u;
						while (true)
						{
							uint num2 = EnumSources(sourceBuf, j, NativeMethods.SourceType.Url);
							switch (num2)
							{
							default:
								throw InstallerException.ExceptionFromReturnCode(num2);
							case 0u:
								yield return sourceBuf.ToString();
								j++;
								break;
							case 259u:
								yield break;
							}
						}
					}
					j++;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void ForceResolution()
		{
			uint num = NativeMethods.MsiSourceListForceResolutionEx(installation.InstallationCode, installation.UserSid, installation.Context, checked((uint)installation.InstallationType));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		private void ClearSourceType(NativeMethods.SourceType type)
		{
			uint num = NativeMethods.MsiSourceListClearAllEx(installation.InstallationCode, installation.UserSid, installation.Context, checked((uint)type | (uint)installation.InstallationType));
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		private uint EnumSources(StringBuilder sourceBuf, uint i, NativeMethods.SourceType sourceType)
		{
			int num = installation.InstallationType | (int)sourceType;
			checked
			{
				uint pcchSource = (uint)sourceBuf.Capacity;
				uint num2 = NativeMethods.MsiSourceListEnumSources(installation.InstallationCode, installation.UserSid, installation.Context, (uint)num, i, sourceBuf, ref pcchSource);
				if (num2 == 234)
				{
					sourceBuf.Capacity = (int)(++pcchSource);
					num2 = NativeMethods.MsiSourceListEnumSources(installation.InstallationCode, installation.UserSid, installation.Context, (uint)num, i, sourceBuf, ref pcchSource);
				}
				return num2;
			}
		}
	}
}
