namespace Microsoft.Deployment.WindowsInstaller
{
	public struct InstallCost
	{
		private string driveName;

		private long cost;

		private long tempCost;

		public string DriveName => driveName;

		public long Cost => cost;

		public long TempCost => tempCost;

		internal InstallCost(string driveName, long cost, long tempCost)
		{
			this.driveName = driveName;
			this.cost = cost;
			this.tempCost = tempCost;
		}
	}
}
