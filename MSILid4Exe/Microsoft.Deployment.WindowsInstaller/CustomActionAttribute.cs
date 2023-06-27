using System;

namespace Microsoft.Deployment.WindowsInstaller
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class CustomActionAttribute : Attribute
	{
		private string name;

		public string Name => name;

		public CustomActionAttribute()
			: this(null)
		{
		}

		public CustomActionAttribute(string name)
		{
			this.name = name;
		}
	}
}
