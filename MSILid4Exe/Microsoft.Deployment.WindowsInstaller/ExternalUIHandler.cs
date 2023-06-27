namespace Microsoft.Deployment.WindowsInstaller
{
	public delegate MessageResult ExternalUIHandler(InstallMessage messageType, string message, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton);
}
