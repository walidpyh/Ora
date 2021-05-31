using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Nen
{
	public class Nen
	{
		private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
		private static void SafeWriter(string path, string content)
		{
			_readWriteLock.EnterWriteLock();
			try
			{
				using (StreamWriter sw = File.AppendText(path))
				{
					sw.WriteLine(content);
					sw.Close();
				}
			}
			finally
			{
				_readWriteLock.ExitWriteLock();
			}
		}

		private static void DebugView(MethodBase obj, object obj2, object[] Parameters, object result)
		{
			try
			{
				string text;
				if (obj2 == null)
				{
					text = string.Concat(new object[]
					{
							"MethodDeclaringType: [ ",
							obj.DeclaringType,
							" ]\nMethodName: ( ",
							obj.Name.ToString(),
							" ) MethodType: ",
							obj.GetType(),
							"\nObject Value: ( null ) Type: null\nParameters: ",
							Parameters.ToString()
					});
				}
				else
				{
					text = string.Concat(new object[]
					{
							"MethodDeclaringType: [ ",
							obj.DeclaringType,
							" ]\nMethodName: ( ",
							obj.Name.ToString(),
							" ) MethodType: ",
							obj.GetType(),
							"\nObject Value: ( ",
							obj2.ToString(),
							" ) Type: ",
							obj2.GetType(),
							"\nParameters: ",
							Parameters.ToString()
					});
				}
				int num = 0;
				foreach (object par in Parameters)
				{
					text = string.Concat(new object[]
					{
							text,
							"\nParameter[",
							num,
							"] Value: ( ",
							par,
							" ) Type: ",
							par.GetType()
					});
					num++;
				}
				if (!string.IsNullOrEmpty(result.ToString()))
				{
					if (obj.Name.ToString().Contains("FromBase64String"))
					{
						text = string.Concat(new object[]
						{
							text,
							"\nReturns: Decrypted-BASE64= ",
							Encoding.UTF8.GetString(Convert.FromBase64String(result.ToString())) + "(" + result.GetType() + ")"
						});
					}
					else
					{
						text = string.Concat(new object[]
						{
							text,
							"\nReturns: ",
							result.ToString() + "(" + result.GetType() + ")"
						});
					}

				}
				else
				{
					text += "\nFailed to grab returns";
				}
				SafeWriter("Tracer.txt", text + Environment.NewLine + Environment.NewLine + Environment.NewLine);
			}
			catch (Exception e) { SafeWriter("TracerErrors.txt", e.Message + Environment.NewLine + Environment.NewLine + Environment.NewLine); }
		}

		private static bool logQueue = true;
		private static bool bypassQueue = false;

		public static object Queue(MethodBase obj, object obj2, object[] Parameters)
		{
			object result;
			try
			{
				result = obj.Invoke(obj2, Parameters);
				if (logQueue)
					DebugView(obj, obj2, Parameters, result);

				//Code your bypass/patches here

				if (bypassQueue)
				{
					//do something after the bypass

				}

			}
			catch (Exception e)
			{
				if (logQueue)
					SafeWriter("invokeErrors.txt", e.Message + Environment.NewLine + Environment.NewLine + Environment.NewLine);

				result = null;
			}
			return result;
		}

	}
}
