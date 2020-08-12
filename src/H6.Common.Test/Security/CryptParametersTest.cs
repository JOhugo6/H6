using H6.Common.Security;
using H6.Common.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Xunit;

namespace H6.Common.Test.Security
{
  public class CryptParametersTest
  {
    private readonly CryptoServiceProviderOptions CryptoServiceProviderOptions;

    public CryptParametersTest()
    {
      CryptoServiceProviderOptions = new CryptoServiceProviderOptions()
      {
        CipherMode = System.Security.Cryptography.CipherMode.CBC,
        EncryptionAlgorithm = EncryptionAlgorithm.TripleDes,
        InitializationVector = new byte[] { 24, 164, 24, 145, 19, 159, 235, 249, },
        Key = new byte[] { 170, 178, 52, 231, 240, 148, 177, 89, 92, 122, 132, 24, 20, 198, 15, 180, 137, 23, 146, 167, 12, 95, 206, 12 }
      };


    }

    [Fact]
    public byte[] Encrypt()
    {
      var crypt = new H6.Common.Security.CryptParameters(CryptoServiceProviderOptions);

      crypt.AddParam("mid", "1");
      crypt.AddParam("mame", "as");

      var result = crypt.Encrypt();

      Assert.True(result.Length > 0);

      return result;
    }

    [Fact]
    public void Decrypt()
    {
      var data = Encrypt();
      var crypt = new H6.Common.Security.CryptParameters(data, CryptoServiceProviderOptions);

      Assert.True(crypt.Decrypt());

      var mid = crypt.GetValue("mid");
      Assert.True(mid=="1");

    }
  }
}
