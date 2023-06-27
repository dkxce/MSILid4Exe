namespace Microsoft.Deployment.WindowsInstaller
{
	public abstract class InstallationPart
	{
		private string id;

		private string productCode;

		private string userSid;

		private UserContexts context;

		internal string Id => id;

		internal string ProductCode => productCode;

		internal string UserSid => userSid;

		internal UserContexts Context => context;

		public ProductInstallation Product
		{
			get
			{
				if (productCode == null)
				{
					return null;
				}
				return new ProductInstallation(productCode, userSid, context);
			}
		}

		public abstract InstallState State { get; }

		internal InstallationPart(string id, string productCode)
			: this(id, productCode, null, UserContexts.None)
		{
		}

		internal InstallationPart(string id, string productCode, string userSid, UserContexts context)
		{
			this.id = id;
			this.productCode = productCode;
			this.userSid = userSid;
			this.context = context;
		}
	}
}
