using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.Model;
using Telerik.Crm.DataMover.Activities.Model.Configuration;

namespace Telerik.Crm.DataMover.Activities.Sql.Data
{
	public class AdoNetActivityProvider : IActivityProvider
	{
		private readonly ActivityDataConfig config;

		public AdoNetActivityProvider(ActivityDataConfig config)
		{
			this.config = config;
		}

		public async Task<Activity[]> GetOrderedById(int startKey, int endKey, int pageSize)
		{
			Activity[] allActivities;

			using (SqlConnection sqlConnection = new SqlConnection(config.ConnectionString))
			{
				await sqlConnection.OpenAsync();
				using (SqlCommand selectActivitiesCommand = sqlConnection.CreateCommand())
				{
					selectActivitiesCommand.CommandText = config.GetPagedActivitiesByIdQuery;

					selectActivitiesCommand.Parameters.AddWithValue("@Take", pageSize);
					selectActivitiesCommand.Parameters.AddWithValue("@StartActivityId", startKey);
					selectActivitiesCommand.Parameters.AddWithValue("@EndActivityId", endKey);

					allActivities = await this.GetActivitiesFromCommand(selectActivitiesCommand).ConfigureAwait(false);
				}
			}

			return allActivities.ToArray();
		}

		public async Task<int> GetTotalItemsCount()
		{
			int itemsCount = 0;
			using (SqlConnection sqlConnection = new SqlConnection(config.ConnectionString))
			{
				await sqlConnection.OpenAsync();
				using (SqlCommand getActivitiesCountCommand = sqlConnection.CreateCommand())
				{
					getActivitiesCountCommand.CommandText = config.GetActivitiesCountQuery;

					itemsCount = (int)(await getActivitiesCountCommand.ExecuteScalarAsync());
				}
			}

			return itemsCount;
		}

		public async Task<int> GetMaxActivityId()
		{
			int maxActivityId = 0;
			using (SqlConnection sqlConnection = new SqlConnection(config.ConnectionString))
			{
				await sqlConnection.OpenAsync();
				using (SqlCommand getMaxActivityIdCommand = sqlConnection.CreateCommand())
				{
					getMaxActivityIdCommand.CommandText = config.GetMaxActivityIdQuery;

					maxActivityId = (int)(await getMaxActivityIdCommand.ExecuteScalarAsync());
				}
			}

			return maxActivityId;
		}

		public async Task<Activity[]> GetInRange(int fromInclusive, int toInclusive)
		{
			Activity[] allActivities;

			using (SqlConnection sqlConnection = new SqlConnection(config.ConnectionString))
			{
				await sqlConnection.OpenAsync();
				using (SqlCommand selectActivitiesCommand = sqlConnection.CreateCommand())
				{
					selectActivitiesCommand.CommandText = config.GetActivitiesInRangeQuery;

					selectActivitiesCommand.Parameters.AddWithValue("@StartActivityId", fromInclusive);
					selectActivitiesCommand.Parameters.AddWithValue("@EndActivityId", toInclusive);

					allActivities = await this.GetActivitiesFromCommand(selectActivitiesCommand).ConfigureAwait(false);
				}
			}

			return allActivities;
		}

		private async Task<Activity[]> GetActivitiesFromCommand(SqlCommand selectActivitiesCommand)
		{
			List<Activity> allActivities = new List<Activity>();

			using (SqlDataReader activitiesReader = await selectActivitiesCommand.ExecuteReaderAsync().ConfigureAwait(false))
			{
				while (activitiesReader.Read())
				{
					allActivities.Add(new Activity()
					{
						Id = (int)activitiesReader["ActivityId"],
						Description = activitiesReader["Description"] == DBNull.Value ? null : (string)activitiesReader["Description"]
					});
				}
			}

			return allActivities.ToArray();
		}


		public async Task SetShortDescription(int activityId, string shortDescription)
		{
			using (SqlConnection sqlConnection = new SqlConnection(config.ConnectionString))
			{
				await sqlConnection.OpenAsync();
				using (SqlCommand selectActivitiesCommand = sqlConnection.CreateCommand())
				{
					selectActivitiesCommand.CommandText = config.SetActivityShortDescriptionQuery;

					selectActivitiesCommand.Parameters.AddWithValue("@ShortDescription", shortDescription);
					selectActivitiesCommand.Parameters.AddWithValue("@ActivityId", activityId);

					await selectActivitiesCommand.ExecuteNonQueryAsync();
				}
			}
		}
	}
}
