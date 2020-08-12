using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using H6.Common.Enums;
using H6.Extensions;

namespace H6.Common.Security.Cryptography
{
  

  public class CryptoService
  {
    private CryptoServiceProvider _cryptoServiceProvider;

		public CryptoService(CryptoServiceProviderOptions options)
    {
			_cryptoServiceProvider = new CryptoServiceProvider(options);
    }

		private byte[] Process(ICryptoTransform cryptoTransform, byte[] data)
    {
			var memoryStream = new MemoryStream();
			var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();
			cryptoStream.Close();
			return memoryStream.ToArray();
		}


		public byte[] Encrypt(byte[] data)
		{
			return Process(_cryptoServiceProvider.GetEncryptorProvider(), data);
		}

		public byte[] Encrypt(string data)
    {
			var arr = data.ToByteArray(StringFormat.UTF8Encoder);
			return Encrypt(arr);
    }

		public byte[] Decrypt(byte[] data)
		{
			return Process(_cryptoServiceProvider.GetDencryptorProvider(), data);
		}

		public string DecryptAsString(byte[] data)
    {
			var result = Decrypt(data).ToString(StringFormat.UTF8Encoder);
			return result;
    }
	}
}
