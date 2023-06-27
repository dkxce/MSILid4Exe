namespace Microsoft.Deployment.WindowsInstaller
{
	public struct ShortcutTarget
	{
		private string productCode;

		private string feature;

		private string componentCode;

		public string ProductCode => productCode;

		public string Feature => feature;

		public string ComponentCode => componentCode;

		internal ShortcutTarget(string productCode, string feature, string componentCode)
		{
			this.productCode = productCode;
			this.feature = feature;
			this.componentCode = componentCode;
		}

		public static bool operator ==(ShortcutTarget st1, ShortcutTarget st2)
		{
			return st1.Equals(st2);
		}

		public static bool operator !=(ShortcutTarget st1, ShortcutTarget st2)
		{
			return !st1.Equals(st2);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != typeof(ShortcutTarget))
			{
				return false;
			}
			ShortcutTarget shortcutTarget = (ShortcutTarget)obj;
			if (productCode == shortcutTarget.productCode && feature == shortcutTarget.feature)
			{
				return componentCode == shortcutTarget.componentCode;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((productCode != null) ? productCode.GetHashCode() : 0) ^ ((feature != null) ? feature.GetHashCode() : 0) ^ ((componentCode != null) ? componentCode.GetHashCode() : 0);
		}
	}
}
