using H6.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H6.Extensions
{
  public static class ByteArray_Extensions
  {


		public static string ToString(this byte[] input, StringFormat format)
		{
			if (input == null) return null;
			if (input.Length == 0) return string.Empty;

			string result = null;
			switch (format)
			{
				case StringFormat.UTF8Encoder:
					{
						result = String_Extensions.TEXT_ENCODER.GetString(input);
					}
					break;

				case StringFormat.Hexadecimal:
					{
						var d = new StringBuilder();
						for (int i = 0; i < input.Length; i++) d.Append(input[i].ToString("x2"));
						result = d.ToString();
					}
					break;

				case StringFormat.Base64:
					{
						result = Convert.ToBase64String(input);
					}
					break;
				default: throw new NotImplementedException($"Not implemented {format}");
			}

			return result;
		}
	}
}
