using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Serializable]
	public class InstallerException : SystemException
	{
		private int errorCode;

		private object[] errorData;

		public int ErrorCode => errorCode;

		public override string Message
		{
			get
			{
				string message = base.Message;
				using (Record record = GetErrorRecord())
				{
					if (record != null)
					{
						string errorMessage = Installer.GetErrorMessage(record, CultureInfo.InvariantCulture);
						return Combine(message, errorMessage);
					}
					return message;
				}
			}
		}

		public InstallerException(string msg, Exception innerException)
			: this(0, msg, innerException)
		{
		}

		public InstallerException(string msg)
			: this(0, msg)
		{
		}

		public InstallerException()
			: this(0, null)
		{
		}

		internal InstallerException(int errorCode, string msg, Exception innerException)
			: base(msg, innerException)
		{
			this.errorCode = errorCode;
			SaveErrorRecord();
		}

		internal InstallerException(int errorCode, string msg)
			: this(errorCode, msg, null)
		{
		}

		protected InstallerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			errorCode = info.GetInt32("msiErrorCode");
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("msiErrorCode", errorCode);
			base.GetObjectData(info, context);
		}

		public Record GetErrorRecord()
		{
			if (errorData == null)
			{
				return null;
			}
			return new Record(errorData);
		}

		internal static Exception ExceptionFromReturnCode(uint errorCode)
		{
			return ExceptionFromReturnCode(errorCode, null);
		}

		internal static Exception ExceptionFromReturnCode(uint errorCode, string msg)
		{
			msg = Combine(GetSystemMessage(errorCode), msg);
			checked
			{
				switch (errorCode)
				{
				case 2u:
				case 3u:
					return new FileNotFoundException(msg);
				case 87u:
				case 267u:
				case 1605u:
				case 1606u:
				case 1607u:
				case 1608u:
					return new ArgumentException(msg);
				case 1615u:
					return new BadQuerySyntaxException(msg);
				case 6u:
				case 1609u:
					return new InvalidHandleException(msg)
					{
						errorCode = (int)errorCode
					};
				case 1602u:
					return new InstallCanceledException(msg);
				case 120u:
					return new NotImplementedException(msg);
				default:
					return new InstallerException((int)errorCode, msg);
				}
			}
		}

		internal static string GetSystemMessage(uint errorCode)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (NativeMethods.FormatMessage(4608u, IntPtr.Zero, errorCode, 0u, stringBuilder, checked((uint)stringBuilder.Capacity), IntPtr.Zero) != 0)
			{
				return stringBuilder.ToString().Trim();
			}
			return null;
		}

		internal void SaveErrorRecord()
		{
			int num = RemotableNativeMethods.MsiGetLastErrorRecord(0);
			checked
			{
				if (num != 0)
				{
					using (Record record = new Record((IntPtr)num, ownsHandle: true, null))
					{
						errorData = new object[record.FieldCount];
						for (int i = 0; i < errorData.Length; i++)
						{
							errorData[i] = record[i + 1];
						}
						return;
					}
				}
				errorData = null;
			}
		}

		private static string Combine(string msg1, string msg2)
		{
			if (msg1 == null)
			{
				return msg2;
			}
			if (msg2 == null)
			{
				return msg1;
			}
			return msg1 + " " + msg2;
		}
	}
}
