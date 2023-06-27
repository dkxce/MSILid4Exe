namespace Microsoft.Deployment.WindowsInstaller
{
	public struct MediaDisk
	{
		private int diskId;

		private string volumeLabel;

		private string diskPrompt;

		public int DiskId
		{
			get
			{
				return diskId;
			}
			set
			{
				diskId = value;
			}
		}

		public string VolumeLabel
		{
			get
			{
				return volumeLabel;
			}
			set
			{
				volumeLabel = value;
			}
		}

		public string DiskPrompt
		{
			get
			{
				return diskPrompt;
			}
			set
			{
				diskPrompt = value;
			}
		}

		public MediaDisk(int diskId, string volumeLabel, string diskPrompt)
		{
			this.diskId = diskId;
			this.volumeLabel = volumeLabel;
			this.diskPrompt = diskPrompt;
		}
	}
}
