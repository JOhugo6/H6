using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace H6.Common.Security.Cryptography
{
  public class CryptoServiceProviderOptions
  {
    public EncryptionAlgorithm EncryptionAlgorithm { get; set; }

    public byte[] InitializationVector { get; set; }

    public CipherMode CipherMode { get; set; } = CipherMode.CBC;

    public byte[] Key { get; set; }
  }
}
