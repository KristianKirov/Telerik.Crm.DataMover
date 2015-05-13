using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.Model;

namespace Telerik.Crm.DataMover.Activities.Security
{
	public class AesEncryptor : IEncryptor
	{
		private readonly byte[] initialisationVector;

		private readonly byte[] key;

		public AesEncryptor(byte[] initialisationVector, byte[] key)
		{
			this.initialisationVector = initialisationVector;
			this.key = key;
		}

		public Stream Encrypt(string rawData)
		{
			Stream encryptedStream;

			using (AesManaged aesAlgorithm = new AesManaged())
			{
				aesAlgorithm.IV = this.initialisationVector;
				aesAlgorithm.Key = this.key;
				ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV);

				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
						{
							streamWriter.Write(rawData);
						}
					}

					encryptedStream = new MemoryStream(memoryStream.ToArray());
				}
			}

			return encryptedStream;
		}
	}
}
