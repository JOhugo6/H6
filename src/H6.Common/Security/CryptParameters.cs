using H6.Common.Security.Cryptography;
using H6.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace H6.Common.Security
{
  public class CryptParameters
  {
    private const string _createdOn_Key = "createdOn";
    private Dictionary<string, string> _values = new Dictionary<string, string>();
    private byte[] _dataForEncrypt = null;

    private CryptoService _cryptoService;


    public CryptParameters(CryptoServiceProviderOptions options)
    {
      _values[_createdOn_Key] = DateTime.UtcNow.ToString();
      _cryptoService = new CryptoService(options);
    }

    public CryptParameters(byte[] data, CryptoServiceProviderOptions options)
    {
      _cryptoService = new CryptoService(options);
      _dataForEncrypt = data;
    }

    public string CreatedOn
    {
      get
      {
        return _values[_createdOn_Key];
      }
    }

    public void AddParam(string key, string value)
    {
      if (string.IsNullOrWhiteSpace(key))
        throw new Exception("Key is empty.");

      if (_createdOn_Key.ToLower() == key.Trim().ToLower())
        throw new Exception("Is internal key.");

      _values[key.Trim()] = value;
    }

    public string GetValue(string key)
    {
      _values.TryGetValue(key, out string value);
      return value;
    }

    public byte[] Encrypt()
    {
      string stringSerialize = SerializeJSON();

      byte[] encrData = _cryptoService.Encrypt(stringSerialize);
      return encrData;
    }

    public bool Decrypt()
    {
      if (_dataForEncrypt == null || _dataForEncrypt.Length == 0)
        return false;

      try
      {
        var decryptValues = _cryptoService.DecryptAsString(_dataForEncrypt);
        _values = DeserializeJSON(decryptValues);
      }
      catch (Exception)
      {
        return false;
      }
      return true;
    }

    private string SerializeJSON()
    {
      var result = System.Text.Json.JsonSerializer.Serialize(_values);
      return result;
    }

    private Dictionary<string, string> DeserializeJSON(string data)
    {
      var self = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(data);
      return self;
    }
  }
}
