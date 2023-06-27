namespace Microsoft.Deployment.WindowsInstaller
{
	public abstract class Installation
	{
		private string installationCode;

		private string userSid;

		private UserContexts context;

		private SourceList sourceList;

		public string UserSid => userSid;

		public UserContexts Context => context;

		public virtual SourceList SourceList
		{
			get
			{
				if (sourceList == null)
				{
					sourceList = new SourceList(this);
				}
				return sourceList;
			}
		}

		public abstract bool IsInstalled { get; }

		internal string InstallationCode => installationCode;

		internal abstract int InstallationType { get; }

		public abstract string this[string propertyName] { get; }

		internal Installation(string installationCode, string userSid, UserContexts context)
		{
			if (context == UserContexts.Machine)
			{
				userSid = null;
			}
			this.installationCode = installationCode;
			this.userSid = userSid;
			this.context = context;
		}
	}
}
