using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Database.Entities
{
  public class MetadataAttribute : Attribute
  {
    public MetadataAttribute()
    {

    }
    public int Id { get; set; }
  }
}
