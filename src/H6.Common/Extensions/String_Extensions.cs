using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Extensions
{
  public static class String_Extensions
  {
    /// <summary>
    /// short string
    /// </summary>
    /// <param name="item"></param>
    /// <param name="maxLenght"></param>
    /// <param name="replaceValue"></param>
    /// <returns></returns>
    public static string ShortText(this string item, int maxLenght, string replaceValue = "...")
    {
      if (string.IsNullOrWhiteSpace(item)) return item;
      if (item.Length <= maxLenght) return item;
      if (string.IsNullOrWhiteSpace(replaceValue) || replaceValue.Length >= maxLenght) return item.Substring(0, maxLenght);
      return item.Substring(0, maxLenght - replaceValue.Length) + replaceValue;
    }

    /// <summary>
    /// Convert hexa string to array, for uppercase A-F letters
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static byte[] HexStringToByteArray(this string hex)
    {
      if (string.IsNullOrWhiteSpace(hex)) return null;

      if (hex.Length % 2 == 1) throw new Exception("The binary key cannot have an odd number of digits");

      byte[] arr = new byte[hex.Length >> 1];
      for (int i = 0; i < (hex.Length >> 1); ++i)
      {
        arr[i] = (byte)((hex[i << 1].GetHexVal() << 4) + (hex[(i << 1) + 1].GetHexVal()));
      }

      return arr;

    }
  }
}
