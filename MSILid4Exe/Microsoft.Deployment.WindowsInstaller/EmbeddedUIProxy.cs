using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal static class EmbeddedUIProxy
	{
		private static IEmbeddedUI uiInstance;

		private static string uiClass;

		private static bool DebugBreakEnabled(string method)
		{
			return CustomActionProxy.DebugBreakEnabled(new string[2]
			{
				method,
				uiClass + "." + method
			});
		}

		public static int Initialize(int sessionHandle, string uiClass, int internalUILevel)
		{
			Session session = null;
			try
			{
				session = new Session((IntPtr)sessionHandle, ownsHandle: false);
				if (string.IsNullOrEmpty(uiClass))
				{
					throw new ArgumentNullException("uiClass");
				}
				uiInstance = InstantiateUI(session, uiClass);
			}
			catch (Exception ex)
			{
				if (session != null)
				{
					try
					{
						session.Log("Exception while loading embedded UI:");
						session.Log(ex.ToString());
					}
					catch (Exception)
					{
					}
				}
			}
			if (uiInstance == null)
			{
				return 1603;
			}
			try
			{
				string directoryName = Path.GetDirectoryName(uiInstance.GetType().Assembly.Location);
				InstallUIOptions internalUILevel2 = (InstallUIOptions)internalUILevel;
				if (DebugBreakEnabled("Initialize"))
				{
					Debugger.Launch();
				}
				if (uiInstance.Initialize(session, directoryName, ref internalUILevel2))
				{
					return (int)internalUILevel2 << 16;
				}
				return (int)internalUILevel2;
			}
			catch (InstallCanceledException)
			{
				return 1602;
			}
			catch (Exception ex4)
			{
				session.Log("Exception thrown by embedded UI initialization:");
				session.Log(ex4.ToString());
				return 1603;
			}
		}

		public static int ProcessMessage(int messageType, int recordHandle)
		{
			if (uiInstance != null)
			{
				try
				{
					int messageType2 = messageType & 0x7F000000;
					int buttons = messageType & 0xF;
					int icon = messageType & 0xF0;
					int defaultButton = messageType & 0xF00;
					Record record = ((recordHandle != 0) ? Record.FromHandle((IntPtr)recordHandle, ownsHandle: false) : null);
					using (record)
					{
						if (DebugBreakEnabled("ProcessMessage"))
						{
							Debugger.Launch();
						}
						return (int)uiInstance.ProcessMessage((InstallMessage)messageType2, record, (MessageButtons)buttons, (MessageIcon)icon, (MessageDefaultButton)defaultButton);
					}
				}
				catch (Exception)
				{
				}
			}
			return 0;
		}

		public static void Shutdown()
		{
			if (uiInstance == null)
			{
				return;
			}
			try
			{
				if (DebugBreakEnabled("Shutdown"))
				{
					Debugger.Launch();
				}
				uiInstance.Shutdown();
			}
			catch (Exception)
			{
			}
			uiInstance = null;
		}

		private static IEmbeddedUI InstantiateUI(Session session, string uiClass)
		{
			int num = uiClass.IndexOf('!');
			if (num < 0)
			{
				session.Log("Error: invalid embedded UI assembly and class:" + uiClass);
				return null;
			}
			string text = uiClass.Substring(0, num);
			EmbeddedUIProxy.uiClass = uiClass.Substring(checked(num + 1));
			try
			{
				Assembly assembly = AppDomain.CurrentDomain.Load(text);
				if (CustomActionProxy.DebugBreakEnabled(new string[1] { "EmbeddedUI" }))
				{
					Debugger.Launch();
				}
				return (IEmbeddedUI)assembly.CreateInstance(EmbeddedUIProxy.uiClass);
			}
			catch (Exception ex)
			{
				session.Log("Error: could not load embedded UI class " + EmbeddedUIProxy.uiClass + " from assembly: " + text);
				session.Log(ex.ToString());
				return null;
			}
		}
	}
}
