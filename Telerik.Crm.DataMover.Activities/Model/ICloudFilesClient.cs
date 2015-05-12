using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Activities.Model
{
	public interface ICloudFilesClient
	{
		void UpsertDocument(string uniqueName, string content);

		void UpsertDocument(string uniqueName, Stream contentStream, string contentType);
	}
}
