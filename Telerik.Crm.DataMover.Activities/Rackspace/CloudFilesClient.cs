using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;
using Telerik.Crm.DataMover.Activities.Model;
using Telerik.Crm.DataMover.Activities.Model.Configuration;

namespace Telerik.Crm.DataMover.Activities.Rackspace
{
	public class CloudFilesClient : ICloudFilesClient
	{
		private readonly RackspaceConfig config;

		private readonly CloudIdentity cloudIdentity;

		private readonly string environmentName;

		public CloudFilesClient(RackspaceConfig config, string environmentName)
		{
			this.config = config;
			this.cloudIdentity = new CloudIdentity()
			{
				Username = config.Username,
				APIKey = config.ApiKey
			};
			this.environmentName = environmentName.ToUpperInvariant(); ;
		}

		public void UpsertDocument(string uniqueName, string content)
		{
			using (MemoryStream contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
			{
				this.UpsertDocument(uniqueName, contentStream, "text/plain");
			}
		}

		public void UpsertDocument(string uniqueName, System.IO.Stream contentStream, string contentType = null)
		{
			CloudFilesProvider cloudFilesProvider = new CloudFilesProvider(cloudIdentity);
			cloudFilesProvider.CreateObject(this.config.ContainerName, contentStream, string.Concat(this.environmentName, "/", uniqueName), contentType);
		}
	}
}
