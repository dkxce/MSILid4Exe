using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class SummaryInfo : InstallerHandle
	{
		internal const int MAX_PROPERTIES = 20;

		public string Title
		{
			get
			{
				return this[2u];
			}
			set
			{
				this[2u] = value;
			}
		}

		public string Subject
		{
			get
			{
				return this[3u];
			}
			set
			{
				this[3u] = value;
			}
		}

		public string Author
		{
			get
			{
				return this[4u];
			}
			set
			{
				this[4u] = value;
			}
		}

		public string Keywords
		{
			get
			{
				return this[5u];
			}
			set
			{
				this[5u] = value;
			}
		}

		public string Comments
		{
			get
			{
				return this[6u];
			}
			set
			{
				this[6u] = value;
			}
		}

		public string Template
		{
			get
			{
				return this[7u];
			}
			set
			{
				this[7u] = value;
			}
		}

		public string LastSavedBy
		{
			get
			{
				return this[8u];
			}
			set
			{
				this[8u] = value;
			}
		}

		public string RevisionNumber
		{
			get
			{
				return this[9u];
			}
			set
			{
				this[9u] = value;
			}
		}

		public string CreatingApp
		{
			get
			{
				return this[18u];
			}
			set
			{
				this[18u] = value;
			}
		}

		public DateTime LastPrintTime
		{
			get
			{
				return (DateTime)this[11u, typeof(DateTime)];
			}
			set
			{
				this[11u, typeof(DateTime)] = value;
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return (DateTime)this[12u, typeof(DateTime)];
			}
			set
			{
				this[12u, typeof(DateTime)] = value;
			}
		}

		public DateTime LastSaveTime
		{
			get
			{
				return (DateTime)this[13u, typeof(DateTime)];
			}
			set
			{
				this[13u, typeof(DateTime)] = value;
			}
		}

		public short CodePage
		{
			get
			{
				return (short)this[1u, typeof(short)];
			}
			set
			{
				this[1u, typeof(short)] = value;
			}
		}

		public int PageCount
		{
			get
			{
				return (int)this[14u, typeof(int)];
			}
			set
			{
				this[14u, typeof(int)] = value;
			}
		}

		public int WordCount
		{
			get
			{
				return (int)this[15u, typeof(int)];
			}
			set
			{
				this[15u, typeof(int)] = value;
			}
		}

		public int CharacterCount
		{
			get
			{
				return (int)this[16u, typeof(int)];
			}
			set
			{
				this[16u, typeof(int)] = value;
			}
		}

		public int Security
		{
			get
			{
				return (int)this[19u, typeof(int)];
			}
			set
			{
				this[19u, typeof(int)] = value;
			}
		}

		private object this[uint property, Type type]
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("");
				uint cchValueBuf = 0u;
				long ftValue = 0L;
				uint uiDataType;
				int iValue;
				uint num = RemotableNativeMethods.MsiSummaryInfoGetProperty((int)base.Handle, property, out uiDataType, out iValue, ref ftValue, stringBuilder, ref cchValueBuf);
				if (num != 0 && uiDataType != 30)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
				checked
				{
					switch ((int)uiDataType)
					{
					case 0:
						if (type == typeof(DateTime))
						{
							return DateTime.MinValue;
						}
						if (type == typeof(string))
						{
							return string.Empty;
						}
						if (type == typeof(short))
						{
							return (short)0;
						}
						return 0;
					case 30:
						if (num == 234)
						{
							stringBuilder.Capacity = (int)(++cchValueBuf);
							num = RemotableNativeMethods.MsiSummaryInfoGetProperty((int)base.Handle, property, out uiDataType, out iValue, ref ftValue, stringBuilder, ref cchValueBuf);
						}
						if (num != 0)
						{
							throw InstallerException.ExceptionFromReturnCode(num);
						}
						return stringBuilder.ToString();
					case 2:
					case 3:
						if (type == typeof(string))
						{
							return iValue.ToString(CultureInfo.InvariantCulture);
						}
						if (type == typeof(short))
						{
							return (short)iValue;
						}
						return iValue;
					case 64:
						if (type == typeof(string))
						{
							return DateTime.FromFileTime(ftValue).ToString(CultureInfo.InvariantCulture);
						}
						return DateTime.FromFileTime(ftValue);
					default:
						throw new InstallerException();
					}
				}
			}
			set
			{
				uint num = 1u;
				string szValue = "";
				int iValue = 0;
				long ftValue = 0L;
				if (type == typeof(short))
				{
					num = 2u;
					iValue = (short)value;
				}
				else if (type == typeof(int))
				{
					num = 3u;
					iValue = (int)value;
				}
				else if (type == typeof(string))
				{
					num = 30u;
					szValue = (string)value;
				}
				else
				{
					num = 64u;
					ftValue = ((DateTime)value).ToFileTime();
				}
				uint num2 = NativeMethods.MsiSummaryInfoSetProperty((int)base.Handle, property, num, iValue, ref ftValue, szValue);
				if (num2 != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num2);
				}
			}
		}

		private string this[uint property]
		{
			get
			{
				return (string)this[property, typeof(string)];
			}
			set
			{
				this[property, typeof(string)] = value;
			}
		}

		public SummaryInfo(string packagePath, bool enableWrite)
			: base((IntPtr)OpenSummaryInfo(packagePath, enableWrite), ownsHandle: true)
		{
		}

		internal SummaryInfo(IntPtr handle, bool ownsHandle)
			: base(handle, ownsHandle)
		{
		}

		public void Persist()
		{
			uint num = NativeMethods.MsiSummaryInfoPersist((int)base.Handle);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		private static int OpenSummaryInfo(string packagePath, bool enableWrite)
		{
			int num = (enableWrite ? 20 : 0);
			int hSummaryInfo;
			uint num2 = RemotableNativeMethods.MsiGetSummaryInformation(0, packagePath, checked((uint)num), out hSummaryInfo);
			switch (num2)
			{
			case 2u:
			case 5u:
				throw new FileNotFoundException(null, packagePath);
			default:
				throw InstallerException.ExceptionFromReturnCode(num2);
			case 0u:
				return hSummaryInfo;
			}
		}
	}
}
