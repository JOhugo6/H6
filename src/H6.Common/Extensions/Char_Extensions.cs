using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Extensions
{
  public static class Char_Extensions
  {
    /// <summary>
    /// hexa char to int, for uppercase A-F letters
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static int GetHexVal(this char hex)
    {
      int val = (int)hex;
      //For uppercase A-F letters:
      return val - (val < 58 ? 48 : 55);
      //For lowercase a-f letters:
      //return val - (val < 58 ? 48 : 87);
      //Or the two combined, but a bit slower:
      //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }
  }
}
