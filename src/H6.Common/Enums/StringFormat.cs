using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Common.Enums
{
	public enum StringFormat
	{
		/// <summary>
		/// String without any format, pure text is converted by UTF8 encoder
		/// </summary>
		UTF8Encoder,
		/// <summary>
		/// Base 64 encoding
		/// </summary>
		Base64,
		/// <summary>
		/// Hexadecimal encoding
		/// </summary>
		Hexadecimal,
		
	}
}
