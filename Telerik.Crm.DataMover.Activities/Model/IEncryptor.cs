using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Activities.Model
{
	public interface IEncryptor
	{
		Stream Encrypt(string rawData);
	}
}
