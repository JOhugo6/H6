using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Common.Database
{
  /// <summary>
  /// Database entity
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  [Serializable]
  public class DBEntityBase<TEntity>
    where TEntity : class
  {

  }

  /// <summary>
  /// Database entity
  /// </summary>
  /// <typeparam name="TPrimaryKey"></typeparam>
  /// <typeparam name="TEntity"></typeparam>
  [Serializable]
  public class DBEntityBase<TPrimaryKey, TEntity> : DBEntityBase<TEntity>, IPrimaryKeyAsId<TPrimaryKey>
    where TEntity : DBEntityBase<TPrimaryKey, TEntity>
    where TPrimaryKey : IComparable
  {
    #region Properties
    /// <summary>
    /// Id.
    /// </summary>
    public TPrimaryKey Id { get; set; }
    #endregion
  }
}
