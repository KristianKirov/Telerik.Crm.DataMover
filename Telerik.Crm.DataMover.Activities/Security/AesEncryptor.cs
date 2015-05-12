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

		private static Encoding encoding = Encoding.ASCII;

		public AesEncryptor(byte[] initialisationVector, byte[] key)
		{
			this.initialisationVector = initialisationVector;
			this.key = key;
		}

		public string Encrypt(string rawData)
		{
			string encryptedString;

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

					encryptedString = Convert.ToBase64String(memoryStream.ToArray());
				}
			}

			return encryptedString;
		}

		public string Decrypt(string encryptedData)
		{
			string rawData;

			using (AesManaged aesAlgorithm = new AesManaged())
			{
				aesAlgorithm.IV = this.initialisationVector;
				aesAlgorithm.Key = this.key;

				ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor(aesAlgorithm.Key, aesAlgorithm.IV);

				byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
				using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader(cryptoStream))
						{
							rawData = streamReader.ReadToEnd();
						}
					}
				}
			}

			return rawData;
		}
	}
}
