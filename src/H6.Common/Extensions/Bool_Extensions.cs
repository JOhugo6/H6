using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Extensions
{
  /// <summary>
  /// Extension for data type bool
  /// </summary>
  public static class Bool_Extensions
  {
    /// <summary>
    /// Convert bool to int
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static byte ToInt(this bool p) => p ? (byte)1 : (byte)0;

    /// <summary>
    /// Contert bool to number as string
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static string ToNumAsString(this bool p) => p ? "1" : "0";
  }
}
