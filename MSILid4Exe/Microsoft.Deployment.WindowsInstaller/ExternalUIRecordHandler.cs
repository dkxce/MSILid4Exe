namespace Microsoft.Deployment.WindowsInstaller
{
	public delegate MessageResult ExternalUIRecordHandler(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton);
}
