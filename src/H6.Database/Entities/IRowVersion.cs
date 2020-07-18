using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Database.Entities
{
  public interface IRowVersion
  {
    byte[] RowVersion { get; set; }
  }
}
