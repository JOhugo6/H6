using H6.Common.Enums;
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

		/// <summary>
		/// Text encoder
		/// </summary>
		internal static readonly Encoding TEXT_ENCODER = Encoding.UTF8;

		public static byte[] ToByteArray(this string input, StringFormat format)
		{
			if (input == null)
			{
				return null;
			}

			byte[] result = null;
			switch (format)
			{
				// string is without any format
				case StringFormat.UTF8Encoder:
					{
						result = TEXT_ENCODER.GetBytes(input);
					}
					break;
				// hexadecimal string
				case StringFormat.Hexadecimal:
					{
						result = input.HexStringToByteArray();
					}
					break;
				// base64 string
				case StringFormat.Base64:
					{
						result = Convert.FromBase64String(input);
					}
					break;
				default: throw new NotImplementedException($"Not implemented {format}");
			}

			return result;
		}
	}
}
