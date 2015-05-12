using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Console.Logging
{
	public static class Logger
	{
		private const string LoggingDirectoryName = "logs";

		private const string LogFileName = "logs\\log.log";

		private const string ErrorFileName = "logs\\error.log";

		static Logger()
		{
			if (!Directory.Exists(Logger.LoggingDirectoryName))
			{
				Directory.CreateDirectory(Logger.LoggingDirectoryName);
			}
		}

		public static void Log(string message)
		{
			Logger.Write(Logger.LogFileName, message);
		}

		public static void Error(string errorMessage)
		{
			Logger.Write(Logger.ErrorFileName, errorMessage);
		}

		private static void Write(string fileName, string message)
		{
			string logMessage = string.Format("{0:G} {1}{2}", DateTime.Now, message, Environment.NewLine);

			try
			{
				File.AppendAllText(fileName, logMessage);
			}
			catch (Exception)
			{
				File.AppendAllText(string.Format("{0:N}_{1}", Guid.NewGuid(), fileName), logMessage);
			}
		}
	}
}
