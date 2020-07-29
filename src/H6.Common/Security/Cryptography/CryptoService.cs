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
    private ICryptoTransform _cryptoTransform;

    public CryptoService(CryptoServiceProviderOptions options)
    {
      _cryptoTransform = new CryptoServiceProvider(options).GetProvider();
    }

		public byte[] Encrypt(byte[] data)
		{
			var memoryStream = new MemoryStream();
			var cryptoStream = new CryptoStream(memoryStream, _cryptoTransform, CryptoStreamMode.Write);
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		public byte[] Encrypt(string data)
    {
			var arr = data.ToByteArray(StringFormat.UTF8Encoder);
			return Encrypt(arr);
    }

		public byte[] Decrypt(byte[] data)
		{
			var memoryStream = new MemoryStream();
			var cryptoStream = new CryptoStream(memoryStream, _cryptoTransform, CryptoStreamMode.Write);
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		public string DecryptAsString(byte[] data)
    {
			var result = Decrypt(data).ToString(StringFormat.UTF8Encoder);
			return result;
    }
	}
}
