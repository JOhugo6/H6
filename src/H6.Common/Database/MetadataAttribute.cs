using System;

namespace H6.Common.Database
{
  public class MetadataAttribute : Attribute
  {
    public MetadataAttribute()
    {

    }
    public int Id { get; set; }
  }
}
