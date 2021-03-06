﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace H6.Common.Security.Cryptography
{
  public class CryptoServiceProvider
  {

		private CryptoServiceProviderOptions _options;

    public CryptoServiceProvider(CryptoServiceProviderOptions options)
    {
			if (options==null) throw new ArgumentNullException("options");

			if (options.InitializationVector == null) throw new ArgumentNullException("options.InitializationVector");
			if (options.InitializationVector.Length == 0) throw new ArgumentOutOfRangeException("options.InitializationVector");

			if (options.Key == null) throw new ArgumentNullException("options.Key");
			if (options.Key.Length == 0) throw new ArgumentOutOfRangeException("options.Key");

			_options = options;
    }

		public ICryptoTransform GetEncryptorProvider()
    {
			return GetSymetricProvider().CreateEncryptor();
		}

		public ICryptoTransform GetDencryptorProvider()
		{
			return GetSymetricProvider().CreateDecryptor();
		}

		private SymmetricAlgorithm _symmetricAlgorithm;

		private SymmetricAlgorithm GetSymetricProvider()
		{
			if (_symmetricAlgorithm != null) return _symmetricAlgorithm;

			// Pick the provider
			switch (_options.EncryptionAlgorithm)
			{
				case EncryptionAlgorithm.Rijndael:
					{
						var rijndael = new RijndaelManaged();
						rijndael.Mode = CipherMode.CBC;
						rijndael.Key = _options.Key;
						rijndael.IV = _options.InitializationVector;
						_symmetricAlgorithm = rijndael;
						break;
					}
				case EncryptionAlgorithm.TripleDes:
					TripleDES des3 = new TripleDESCryptoServiceProvider();
					des3.Mode = CipherMode.CBC;
					des3.Key = _options.Key;
					des3.IV = _options.InitializationVector;
					_symmetricAlgorithm = des3;
					break;
				default:
					throw new CryptographicException($"EncryptionAlgorithm: {_options.EncryptionAlgorithm} not supported.");
			}

			return _symmetricAlgorithm;
		}
	}
}
