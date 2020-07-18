using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Database.Entities
{
  public interface IModifiedOn
  {
    DateTime? ModifiedOn { get; set; }
  }
}
