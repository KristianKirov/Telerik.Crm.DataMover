using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.EventArguments;
using Telerik.Crm.DataMover.Activities.Model;
using Telerik.Crm.DataMover.Activities.Model.Configuration;

namespace Telerik.Crm.DataMover.Activities.Rackspace
{
	public class CloudFilesActivitiesWriter : BaseActivityWriter
	{
		private readonly ICloudFilesClient cloudFilesClient;

		private readonly IEncryptor encryptor;

		private readonly Task completedTask;

		public CloudFilesActivitiesWriter(ICloudFilesClient cloudFilesClient, IEncryptor encryptor)
		{
			this.cloudFilesClient = cloudFilesClient;
			this.encryptor = encryptor;

			this.completedTask = Task.FromResult(0);
		}

		public override Task WriteItems(Activity[] items)
		{
			Parallel.ForEach(items, a =>
			{
				if (!string.IsNullOrWhiteSpace(a.Description))
				{
					try
					{
						Stream fileStream = this.encryptor.Encrypt(a.Description);
						cloudFilesClient.UpsertDocument(a.Id.ToString(), fileStream, "text/plain");

						this.OnItemWritten(a);
					}
					catch (Exception ex)
					{
						this.OnItemFailed(a, ex);
					}
				}
			});

			return this.completedTask;
		}
	}
}
