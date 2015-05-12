using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Activities.Model
{
	public interface IEncryptor
	{
		string Encrypt(string rawData);

		string Decrypt(string encryptedData);
	}
}
