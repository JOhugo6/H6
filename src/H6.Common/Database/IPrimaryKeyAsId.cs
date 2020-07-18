using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Common.Database
{
  public interface IPrimaryKeyAsId<TPrimaryKey> where TPrimaryKey : IComparable
  {
    #region Properties
    /// <summary>
    /// Id.
    /// </summary>
    TPrimaryKey Id { get; set; }
    #endregion
  }
}
