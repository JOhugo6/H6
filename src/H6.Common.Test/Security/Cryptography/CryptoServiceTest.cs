using H6.Common.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using H6.Extensions;

namespace H6.Common.Test.Security.Cryptography
{
  public class CryptoServiceTest
  {
    private readonly CryptoServiceProviderOptions CryptoServiceProviderOptions;

    public CryptoServiceTest()
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
    public void Encrypt()
    {
      var data = "abcd";
      var cryptoService = new CryptoService(CryptoServiceProviderOptions);

      var dataArr = data.ToByteArray(Enums.StringFormat.UTF8Encoder);
      var enc = cryptoService.Encrypt(dataArr);


      var data2 = cryptoService.DecryptAsString(enc);

      Assert.True(string.Compare(data,data2,false) == 0);
    }
  }
}
