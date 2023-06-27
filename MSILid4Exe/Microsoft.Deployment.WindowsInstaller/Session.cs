using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public sealed class Session : InstallerHandle, IFormatProvider
	{
		private Database database;

		private CustomActionData customActionData;

		private bool sessionAccessValidated;

		public Database Database
		{
			get
			{
				if (database == null || database.IsClosed)
				{
					lock (base.Sync)
					{
						if (database == null || database.IsClosed)
						{
							ValidateSessionAccess();
							int num = RemotableNativeMethods.MsiGetActiveDatabase((int)base.Handle);
							if (num == 0)
							{
								throw new InstallerException();
							}
							database = new Database((IntPtr)num, ownsHandle: true, "", DatabaseOpenMode.ReadOnly);
						}
					}
				}
				return database;
			}
		}

		public int Language => RemotableNativeMethods.MsiGetLanguage((int)base.Handle);

		public string this[string property]
		{
			get
			{
				if (string.IsNullOrEmpty(property))
				{
					throw new ArgumentNullException("property");
				}
				if (!sessionAccessValidated && !NonImmediatePropertyNames.Contains(property))
				{
					ValidateSessionAccess();
				}
				StringBuilder stringBuilder = new StringBuilder();
				uint cchValueBuf = 0u;
				uint num = RemotableNativeMethods.MsiGetProperty((int)base.Handle, property, stringBuilder, ref cchValueBuf);
				if (num == 234)
				{
					stringBuilder.Capacity = checked((int)(++cchValueBuf));
					num = RemotableNativeMethods.MsiGetProperty((int)base.Handle, property, stringBuilder, ref cchValueBuf);
				}
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
				return stringBuilder.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(property))
				{
					throw new ArgumentNullException("property");
				}
				ValidateSessionAccess();
				if (value == null)
				{
					value = string.Empty;
				}
				uint num = RemotableNativeMethods.MsiSetProperty((int)base.Handle, property, value);
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
			}
		}

		public ComponentInfoCollection Components
		{
			get
			{
				ValidateSessionAccess();
				return new ComponentInfoCollection(this);
			}
		}

		public FeatureInfoCollection Features
		{
			get
			{
				ValidateSessionAccess();
				return new FeatureInfoCollection(this);
			}
		}

		public CustomActionData CustomActionData
		{
			get
			{
				if (customActionData == null)
				{
					customActionData = new CustomActionData(this["CustomActionData"]);
				}
				return customActionData;
			}
		}

		private static IList<string> NonImmediatePropertyNames => new string[3] { "CustomActionData", "ProductCode", "UserSID" };

		internal Session(IntPtr handle, bool ownsHandle)
			: base(handle, ownsHandle)
		{
		}

		public static Session FromHandle(IntPtr handle, bool ownsHandle)
		{
			return new Session(handle, ownsHandle);
		}

		public MessageResult Message(InstallMessage messageType, Record record)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			int num = RemotableNativeMethods.MsiProcessMessage((int)base.Handle, checked((uint)messageType), (int)record.Handle);
			if (num < 0)
			{
				throw new InstallerException();
			}
			if (num == 2)
			{
				throw new InstallCanceledException();
			}
			return (MessageResult)num;
		}

		public void Log(string msg)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			using (Record record = new Record(0))
			{
				record.FormatString = msg;
				Message(InstallMessage.Info, record);
			}
		}

		public void Log(string format, params object[] args)
		{
			Log(string.Format(CultureInfo.InvariantCulture, format, args));
		}

		public bool EvaluateCondition(string condition)
		{
			if (string.IsNullOrEmpty(condition))
			{
				throw new ArgumentNullException("condition");
			}
			switch (RemotableNativeMethods.MsiEvaluateCondition((int)base.Handle, condition))
			{
			case 0u:
				return false;
			case 1u:
				return true;
			default:
				throw new InvalidOperationException();
			}
		}

		public bool EvaluateCondition(string condition, bool defaultValue)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			if (condition.Length == 0)
			{
				return defaultValue;
			}
			ValidateSessionAccess();
			return EvaluateCondition(condition);
		}

		public string Format(string format)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			using (Record record = new Record(0))
			{
				record.FormatString = format;
				return record.ToString(this);
			}
		}

		public string FormatRecord(Record record)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			return record.ToString(this);
		}

		[Obsolete("This method is obsolete because it has undesirable side-effects. As an alternative, set the Record's FormatString property separately before calling the FormatRecord() override that takes only the Record parameter.")]
		public string FormatRecord(Record record, string format)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			return record.ToString(format, this);
		}

		public string GetProductProperty(string property)
		{
			if (string.IsNullOrEmpty(property))
			{
				throw new ArgumentNullException("property");
			}
			ValidateSessionAccess();
			StringBuilder stringBuilder = new StringBuilder();
			checked
			{
				uint cchValueBuf = (uint)stringBuilder.Capacity;
				uint num = NativeMethods.MsiGetProductProperty((int)base.Handle, property, stringBuilder, ref cchValueBuf);
				if (num == 234)
				{
					stringBuilder.Capacity = (int)(++cchValueBuf);
					num = NativeMethods.MsiGetProductProperty((int)base.Handle, property, stringBuilder, ref cchValueBuf);
				}
				if (num != 0)
				{
					throw InstallerException.ExceptionFromReturnCode(num);
				}
				return stringBuilder.ToString();
			}
		}

		public bool VerifyDiskSpace()
		{
			ValidateSessionAccess();
			uint num = RemotableNativeMethods.MsiVerifyDiskSpace((int)base.Handle);
			switch (num)
			{
			case 112u:
				return false;
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return true;
			}
		}

		public IList<InstallCost> GetTotalCost()
		{
			ValidateSessionAccess();
			IList<InstallCost> list = new List<InstallCost>();
			StringBuilder stringBuilder = new StringBuilder(20);
			uint num = 0u;
			checked
			{
				while (true)
				{
					uint cchDriveBuf = (uint)stringBuilder.Capacity;
					int iCost;
					int iTempCost;
					uint num2 = RemotableNativeMethods.MsiEnumComponentCosts((int)base.Handle, null, num, 5, stringBuilder, ref cchDriveBuf, out iCost, out iTempCost);
					if (num2 == 259)
					{
						break;
					}
					if (num2 == 234)
					{
						stringBuilder.Capacity = (int)(++cchDriveBuf);
						num2 = RemotableNativeMethods.MsiEnumComponentCosts((int)base.Handle, null, num, 5, stringBuilder, ref cchDriveBuf, out iCost, out iTempCost);
					}
					if (num2 != 0)
					{
						throw InstallerException.ExceptionFromReturnCode(num2);
					}
					list.Add(new InstallCost(stringBuilder.ToString(), unchecked((long)iCost) * 512L, unchecked((long)iTempCost) * 512L));
					num++;
				}
				return list;
			}
		}

		public bool GetMode(InstallRunMode mode)
		{
			return RemotableNativeMethods.MsiGetMode((int)base.Handle, checked((uint)mode));
		}

		public void SetMode(InstallRunMode mode, bool value)
		{
			ValidateSessionAccess();
			uint num = RemotableNativeMethods.MsiSetMode((int)base.Handle, checked((uint)mode), value);
			switch (num)
			{
			case 5u:
				throw new InvalidOperationException();
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				break;
			}
		}

		public string GetSourcePath(string directory)
		{
			if (string.IsNullOrEmpty(directory))
			{
				throw new ArgumentNullException("directory");
			}
			ValidateSessionAccess();
			StringBuilder stringBuilder = new StringBuilder();
			uint cchPathBuf = 0u;
			uint num = RemotableNativeMethods.MsiGetSourcePath((int)base.Handle, directory, stringBuilder, ref cchPathBuf);
			if (num == 234)
			{
				stringBuilder.Capacity = checked((int)(++cchPathBuf));
				num = (num = RemotableNativeMethods.MsiGetSourcePath((int)base.Handle, directory, stringBuilder, ref cchPathBuf));
			}
			switch (num)
			{
			case 267u:
				throw InstallerException.ExceptionFromReturnCode(num, directory);
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return stringBuilder.ToString();
			}
		}

		public string GetTargetPath(string directory)
		{
			if (string.IsNullOrEmpty(directory))
			{
				throw new ArgumentNullException("directory");
			}
			ValidateSessionAccess();
			StringBuilder stringBuilder = new StringBuilder();
			uint cchPathBuf = 0u;
			uint num = RemotableNativeMethods.MsiGetTargetPath((int)base.Handle, directory, stringBuilder, ref cchPathBuf);
			if (num == 234)
			{
				stringBuilder.Capacity = checked((int)(++cchPathBuf));
				num = (num = RemotableNativeMethods.MsiGetTargetPath((int)base.Handle, directory, stringBuilder, ref cchPathBuf));
			}
			switch (num)
			{
			case 267u:
				throw InstallerException.ExceptionFromReturnCode(num, directory);
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				return stringBuilder.ToString();
			}
		}

		public void SetTargetPath(string directory, string value)
		{
			if (string.IsNullOrEmpty(directory))
			{
				throw new ArgumentNullException("directory");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			ValidateSessionAccess();
			uint num = RemotableNativeMethods.MsiSetTargetPath((int)base.Handle, directory, value);
			switch (num)
			{
			case 267u:
				throw InstallerException.ExceptionFromReturnCode(num, directory);
			default:
				throw InstallerException.ExceptionFromReturnCode(num);
			case 0u:
				break;
			}
		}

		public void SetInstallLevel(int installLevel)
		{
			ValidateSessionAccess();
			uint num = RemotableNativeMethods.MsiSetInstallLevel((int)base.Handle, installLevel);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void DoAction(string action)
		{
			DoAction(action, null);
		}

		public void DoAction(string action, CustomActionData actionData)
		{
			if (string.IsNullOrEmpty(action))
			{
				throw new ArgumentNullException("action");
			}
			ValidateSessionAccess();
			if (actionData != null)
			{
				this[action] = actionData.ToString();
			}
			uint num = RemotableNativeMethods.MsiDoAction((int)base.Handle, action);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		public void DoActionSequence(string sequenceTable)
		{
			if (string.IsNullOrEmpty(sequenceTable))
			{
				throw new ArgumentNullException("sequenceTable");
			}
			ValidateSessionAccess();
			uint num = RemotableNativeMethods.MsiSequence((int)base.Handle, sequenceTable, 0);
			if (num != 0)
			{
				throw InstallerException.ExceptionFromReturnCode(num);
			}
		}

		object IFormatProvider.GetFormat(Type formatType)
		{
			if (formatType != typeof(Session))
			{
				return null;
			}
			return this;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && database != null)
				{
					database.Dispose();
					database = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void ValidateSessionAccess()
		{
			if (!sessionAccessValidated)
			{
				if (GetMode(InstallRunMode.Scheduled) || GetMode(InstallRunMode.Rollback) || GetMode(InstallRunMode.Commit))
				{
					throw new InstallerException("Cannot access session details from a non-immediate custom action");
				}
				sessionAccessValidated = true;
			}
		}
	}
}
