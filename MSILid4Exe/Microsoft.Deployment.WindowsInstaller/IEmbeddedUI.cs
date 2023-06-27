namespace Microsoft.Deployment.WindowsInstaller
{
	public interface IEmbeddedUI
	{
		bool Initialize(Session session, string resourcePath, ref InstallUIOptions internalUILevel);

		MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton);

		void Shutdown();
	}
}
