using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal static class CustomActionProxy
	{
		public static int InvokeCustomAction32(int sessionHandle, string entryPoint, int remotingDelegatePtr)
		{
			return InvokeCustomAction(sessionHandle, entryPoint, new IntPtr(remotingDelegatePtr));
		}

		public static int InvokeCustomAction64(int sessionHandle, string entryPoint, long remotingDelegatePtr)
		{
			return InvokeCustomAction(sessionHandle, entryPoint, new IntPtr(remotingDelegatePtr));
		}

		public static int InvokeCustomAction(int sessionHandle, string entryPoint, IntPtr remotingDelegatePtr)
		{
			Session session = null;
			string methodName;
			MethodInfo customActionMethod;
			try
			{
				RemotableNativeMethods.RemotingDelegate = (MsiRemoteInvoke)Marshal.GetDelegateForFunctionPointer(remotingDelegatePtr, typeof(MsiRemoteInvoke));
				sessionHandle = RemotableNativeMethods.MakeRemoteHandle(sessionHandle);
				session = new Session((IntPtr)sessionHandle, ownsHandle: false);
				if (string.IsNullOrEmpty(entryPoint))
				{
					throw new ArgumentNullException("entryPoint");
				}
				if (!FindEntryPoint(session, entryPoint, out var assemblyName, out var className, out methodName))
				{
					return 1603;
				}
				session.Log("Calling custom action {0}!{1}.{2}", assemblyName, className, methodName);
				customActionMethod = GetCustomActionMethod(session, assemblyName, className, methodName);
				if (customActionMethod == null)
				{
					return 1603;
				}
			}
			catch (Exception ex)
			{
				if (session != null)
				{
					try
					{
						session.Log("Exception while loading custom action:");
						session.Log(ex.ToString());
					}
					catch (Exception)
					{
					}
				}
				return 1603;
			}
			try
			{
				Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
				object[] parameters = new object[1] { session };
				if (DebugBreakEnabled(new string[2] { entryPoint, methodName }))
				{
					string lpText = string.Format("To debug your custom action, attach to process ID {0} (0x{0:x}) and click OK; otherwise, click Cancel to fail the custom action.", Process.GetCurrentProcess().Id);
					MessageResult messageResult = NativeMethods.MessageBox(IntPtr.Zero, lpText, "Custom Action Breakpoint", 2359361);
					if (MessageResult.Cancel == messageResult)
					{
						return 1602;
					}
				}
				ActionResult result = (ActionResult)customActionMethod.Invoke(null, parameters);
				session.Close();
				return (int)result;
			}
			catch (InstallCanceledException)
			{
				return 1602;
			}
			catch (Exception ex4)
			{
				session.Log("Exception thrown by custom action:");
				session.Log(ex4.ToString());
				return 1603;
			}
		}

		internal static bool DebugBreakEnabled(string[] names)
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MMsiBreak");
			if (environmentVariable != null)
			{
				string[] array = environmentVariable.Split(',', ';');
				foreach (string text in array)
				{
					foreach (string text2 in names)
					{
						if (text == text2)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool FindEntryPoint(Session session, string entryPoint, out string assemblyName, out string className, out string methodName)
		{
			assemblyName = null;
			className = null;
			methodName = null;
			string text;
			if (entryPoint.IndexOf('!') > 0)
			{
				text = entryPoint;
			}
			else
			{
				IDictionary dictionary;
				try
				{
					dictionary = (IDictionary)ConfigurationManager.GetSection("customActions");
				}
				catch (ConfigurationException ex)
				{
					session.Log("Error: missing or invalid customActions config section.");
					session.Log(ex.ToString());
					return false;
				}
				text = (string)dictionary[entryPoint];
				if (text == null)
				{
					session.Log("Error: custom action entry point '{0}' not found in customActions config section.", entryPoint);
					return false;
				}
			}
			int num = text.IndexOf('!');
			int num2 = text.LastIndexOf('.');
			if (num < 0 || num2 < 0 || num2 < num)
			{
				session.Log("Error: invalid custom action entry point:" + entryPoint);
				return false;
			}
			assemblyName = text.Substring(0, num);
			checked
			{
				className = text.Substring(num + 1, num2 - num - 1);
				methodName = text.Substring(num2 + 1);
				return true;
			}
		}

		private static MethodInfo GetCustomActionMethod(Session session, string assemblyName, string className, string methodName)
		{
			Type type = null;
			Exception ex = null;
			try
			{
				type = AppDomain.CurrentDomain.Load(assemblyName).GetType(className, throwOnError: true, ignoreCase: true);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (BadImageFormatException ex3)
			{
				ex = ex3;
			}
			catch (TypeLoadException ex4)
			{
				ex = ex4;
			}
			catch (ReflectionTypeLoadException ex5)
			{
				ex = ex5;
			}
			catch (SecurityException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				session.Log("Error: could not load custom action class " + className + " from assembly: " + assemblyName);
				session.Log(ex.ToString());
				return null;
			}
			MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.Name == methodName && MethodHasCustomActionSignature(methodInfo))
				{
					return methodInfo;
				}
			}
			session.Log("Error: custom action method \"" + methodName + "\" is missing or has the wrong signature.");
			return null;
		}

		private static bool MethodHasCustomActionSignature(MethodInfo method)
		{
			if (method.ReturnType == typeof(ActionResult) && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(Session))
			{
				object[] customAttributes = method.GetCustomAttributes(inherit: false);
				for (int i = 0; i < customAttributes.Length; i++)
				{
					if (customAttributes[i] is CustomActionAttribute)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
