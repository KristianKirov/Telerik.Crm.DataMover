using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.Model.Configuration;

namespace Telerik.Crm.DataMover.Activities.Sql
{
	public class DisableTriggersRegion
	{
		private readonly ActivityDataConfig dataConfig;

		public DisableTriggersRegion(ActivityDataConfig dataConfig)
		{
			this.dataConfig = dataConfig;
		}

		public void Enter()
		{
			this.StopTriggers();
		}

		public void Exit()
		{
			this.StartTriggers();
		}

		private void StopTriggers()
		{
			this.ExecuteCommand(this.dataConfig.DisableTriggersQuery);
		}

		private void StartTriggers()
		{
			this.ExecuteCommand(this.dataConfig.EnableTriggersQuery);
		}

		private void ExecuteCommand(string commandText)
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.dataConfig.ConnectionString))
			{
				sqlConnection.Open();
				using (SqlCommand selectActivitiesCommand = sqlConnection.CreateCommand())
				{
					selectActivitiesCommand.CommandText = commandText;

					selectActivitiesCommand.ExecuteNonQuery();
				}
			}
		}
	}
}
