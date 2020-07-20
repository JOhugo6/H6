using H6.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Database
{
  public class DBServiceBase<TDBContext> : ServiceBase where TDBContext : DBModelContext
  {
    protected readonly IDBContextFactory<TDBContext> DBContextFactory;

    public DBServiceBase(ILogger logger, IDBContextFactory<TDBContext> dBContextFactory) : base(logger)
    {
      DBContextFactory = dBContextFactory;
    }

    /// <summary>
    /// Call method in try catch block with creating db context
    /// </summary>
    /// <param name="func"></param>
    /// <param name="methodInfo">For better information in log error.</param>
    /// <returns></returns>
    protected IMethodResult TryReturnDBContext(Func<TDBContext, IMethodResult> func, string methodInfo = null)
    {
      return TryReturn(() =>
      {
        using (var dbContext = DBContextFactory.EnsureDBContext())
        {
          return func(dbContext);
        }
      });
    }

    /// <summary>
    /// Call method in try catch block with creating db context
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func"></param>
    /// <param name="methodInfo">For better information in log error.</param>
    /// <returns></returns>
    protected IMethodResult<TResult> TryReturnDBContext<TResult>(Func<TDBContext, IMethodResult<TResult>> func, string methodInfo = null)
    {
      return TryReturn(() =>
      {
        using (var dbContext = DBContextFactory.EnsureDBContext())
        {
          return func(dbContext);
        }
      }, methodInfo);
    }

    /// <summary>
    /// Call method in try catch block with creating db context
    /// </summary>
    /// <param name="func"></param>
    /// <param name="methodInfo">For better information in log error.</param>
    /// <returns></returns>
    protected IMethodResult TryReturnDBContextWithTransaction(Func<TDBContext, IMethodResult> func, string methodInfo = null)
    {
      return TryReturn(() =>
      {
        using (var dbContext = DBContextFactory.EnsureDBContext())
        {
          using (var transaction = dbContext.Database.CurrentTransaction == null ? dbContext.Database.BeginTransaction() : null)
          {
            var result = func(dbContext);
            if (result.IsSuccess) transaction?.Commit();
            return result;
          }
        }
      }, methodInfo);
    }

    /// <summary>
    /// Call method in try catch block with creating db context
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func"></param>
    /// <param name="methodInfo">For better information in log error.</param>
    /// <returns></returns>
    protected IMethodResult<TResult> TryReturnDbContextWithTransaction<TResult>(Func<TDBContext, IMethodResult<TResult>> func, string methodInfo = null)
    {
      return TryReturn(() =>
      {
        using (var dbContext = DBContextFactory.EnsureDBContext())
        {
          using (var transaction = dbContext.Database.CurrentTransaction == null ? dbContext.Database.BeginTransaction() : null)
          {
            var result = func(dbContext);
            if (result.IsSuccess) transaction?.Commit();
            return result;
          }
        }
      }, methodInfo);
    }
  }
}
