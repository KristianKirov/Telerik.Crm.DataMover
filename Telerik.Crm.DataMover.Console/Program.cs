using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities;
using Telerik.Crm.DataMover.Activities.Model;
using Telerik.Crm.DataMover.Activities.Model.Configuration;
using Telerik.Crm.DataMover.Activities.Rackspace;
using Telerik.Crm.DataMover.Activities.Security;
using Telerik.Crm.DataMover.Activities.Sql;
using Telerik.Crm.DataMover.Activities.Sql.Data;
using Telerik.Crm.DataMover.Activities.Text;
using Telerik.Crm.DataMover.Console.Logging;

namespace Telerik.Crm.DataMover.Console
{
	class Program
	{
		private static int lastWrittenItemId;
		private static int lastReadStartId;
		private static int lastReadEndId;

		private static DisableTriggersRegion disableTriggersRegion;

		static void Main(string[] args)
		{
			Task t = MainAsync(args);
			t.Wait();
		}

		private static async Task MainAsync(string[] args)
		{
			System.Console.Write("Start activity id: ");
			int startActivityId = int.Parse(System.Console.ReadLine());

			CancellationTokenSource cts = new CancellationTokenSource();
			CancellationToken ct = cts.Token;

			try
			{
				SetConsoleCtrlHandler(new HandlerRoutine(controlType =>
				{
					Program.OnApplicationEnd();

					return true;
				}), true);

				ActivityDataConfig dataConfig = new ActivityDataConfig()
				{
					ConnectionString = ConfigurationManager.AppSettings["Sql.ConnectionString"],
					//GetActivitiesCountQuery = "SELECT COUNT (1) FROM [crm].[Activity]",
					//GetPagedActivitiesByIdQuery = "SELECT TOP (@Take) * FROM [crm].[Activity] where ActivityId >= @StartActivityId and ActivityId <= @EndActivityId order by ActivityId",
					GetActivitiesInRangeQuery = ConfigurationManager.AppSettings["Sql.GetActivitiesInRangeQuery"],
					SetActivityShortDescriptionQuery = ConfigurationManager.AppSettings["Sql.SetActivityShortDescriptionQuery"],
					DisableTriggersQuery = ConfigurationManager.AppSettings["Sql.DisableTriggersQuery"],
					EnableTriggersQuery = ConfigurationManager.AppSettings["Sql.EnableTriggersQuery"]
				};

				IActivityProvider activityProvider = new AdoNetActivityProvider(dataConfig);
				//IParallelReadersFactory<int, Activity> activityReadersFactory = new SqlActivitiesParallelReadersFactory(activityProvider, 4, 1000);

				SimpleSqlActivitiesReader activitiesReader = new SimpleSqlActivitiesReader(startActivityId, 1000, activityProvider);
				activitiesReader.PageRead += (s, e) =>
				{
					Program.lastReadStartId = e.From;
					Program.lastReadEndId = e.To;
				};

				System.Console.WriteLine("Job? (1 - Write to Rackspace, 2 - Generate short descriptions): ");
				int writeCommand = int.Parse(System.Console.ReadLine());

				BaseActivityWriter activityDataWriter = Program.GetWriter(writeCommand, activityProvider, dataConfig);
				activityDataWriter.ItemWritten += (s, e) =>
				{
					Program.lastWrittenItemId = e.Activity.Id;
				};
				activityDataWriter.ItemFailed += (s, e) =>
				{
					Logger.Error(string.Format("Could not store activity with id: {0}. Error: {1}", e.ActivityId, e.Exception.ToString()));
				};

				//ParallelDataMoverJob<int, Activity> movingJob = new ParallelDataMoverJob<int, Activity>(activityReadersFactory, activityDataWriter);
				DataMoverJob<int, Activity> movingJob = new DataMoverJob<int, Activity>(activitiesReader, activityDataWriter);

				Task printProgressTask = Task.Factory.StartNew((s) =>
				{
					while (!ct.IsCancellationRequested)
					{
						Thread.Sleep(1000);
						System.Console.CursorLeft = 0;
						System.Console.Write("Last activity id: ");
						System.Console.Write(Program.lastWrittenItemId);
						System.Console.Write(" Last read activities: [");
						System.Console.Write(Program.lastReadStartId);
						System.Console.Write(",");
						System.Console.Write(Program.lastReadEndId);
						System.Console.Write("]");
					}
				}, null, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

				await movingJob.StartMove().ConfigureAwait(false);

				cts.Cancel();
				await printProgressTask.ConfigureAwait(false);
			}
			catch (AggregateException ae)
			{
				ae.Handle(e =>
				{
					Logger.Log(e.ToString());

					return true;
				});
			}
			catch (Exception ex)
			{
				Logger.Log(ex.ToString());
			}
			finally
			{
				cts.Dispose();

				Program.OnApplicationEnd();
			}
		}

		private static BaseActivityWriter GetWriter(int command, IActivityProvider activityProvider, ActivityDataConfig dataConfig)
		{
			if (command == 1)
			{
				ICloudFilesClient cloudFilesClient = new CloudFilesClient(new RackspaceConfig()
				{
					Username = ConfigurationManager.AppSettings["CloudFiles.Username"],
					ApiKey = ConfigurationManager.AppSettings["CloudFiles.ApiKey"],
					ContainerName = ConfigurationManager.AppSettings["CloudFiles.ContainerName"]
				}, ConfigurationManager.AppSettings["CloudFiles.EnvironmentName"]);

				byte[] iv = ConfigurationManager.AppSettings["Security.IV"].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(b => byte.Parse(b)).ToArray();
				byte[] key = ConfigurationManager.AppSettings["Security.Key"].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(b => byte.Parse(b)).ToArray();
				IEncryptor encryptor = new AesEncryptor(iv, key);
				CloudFilesActivitiesWriter activityDataWriter = new CloudFilesActivitiesWriter(cloudFilesClient, encryptor);

				return activityDataWriter;
			}

			if (command == 2)
			{
				Program.disableTriggersRegion = new DisableTriggersRegion(dataConfig);
				Program.disableTriggersRegion.Enter();

				RelaxOptions relax = new RelaxOptions()
				{
					RelaxOnEach = 100,
					RelaxForMilliseconds = 1000
				};
				IHtmlStripper htmlStripper = new RegexHtmlStripper();
				ISummaryGenerator summaryGenerator = new FixedLengthSummaryGenerator(150);

				ActivitiesShortDescriptionWriter activityDataWriter = new ActivitiesShortDescriptionWriter(relax, htmlStripper, summaryGenerator, activityProvider);

				return activityDataWriter;
			}

			throw new NotSupportedException(string.Format("Unknown command: {0}", command));
		}

		private static void OnApplicationEnd()
		{
			Logger.Log(string.Format("Last activity id: {0} | Last read activities: [{1}, {2}]", Program.lastWrittenItemId, Program.lastReadStartId, Program.lastReadEndId));

			if (Program.disableTriggersRegion != null)
			{
				Program.disableTriggersRegion.Exit();
			}
		}

		[DllImport("Kernel32")]
		public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

		public delegate bool HandlerRoutine(CtrlType ctrlType);

		public enum CtrlType
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT
		}
	}
}
