namespace Microsoft.Deployment.WindowsInstaller
{
	public struct ValidationErrorInfo
	{
		private ValidationError error;

		private string column;

		public ValidationError Error => error;

		public string Column => column;

		internal ValidationErrorInfo(ValidationError error, string column)
		{
			this.error = error;
			this.column = column;
		}
	}
}
