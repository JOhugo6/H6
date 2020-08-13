using H6.Common;
using H6.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace H6.Database
{
  public class DBService<TDBEntity, TDBContext> : DBServiceBase<TDBContext>
    where TDBEntity : DBEntityBase<TDBEntity>, new()
    where TDBContext : DBModelContext
  {

    public DBService(ILogger logger, IDBContextFactory<TDBContext> dBContextFactory) : base(logger, dBContextFactory)
    {
    }

    protected IQueryable<TResult> ListOfIQueryableEx<TResult>(TDBContext dbContext,
      Expression<Func<TDBEntity, TResult>> selector,
      Expression<Func<TDBEntity, bool>> wherePredicate = null,
      bool tracking = true
      )
    {
      var result = tracking
                            ? dbContext.Set<TDBEntity>().AsTracking().AsQueryable()
                            : dbContext.Set<TDBEntity>().AsNoTracking().AsQueryable();
      if (wherePredicate != null) result = result.Where(wherePredicate);
      return result.Select(selector);
    }

    public virtual IMethodResult<List<TResult>> ListOfEx<TResult>(
      Expression<Func<TDBEntity, TResult>> selector,
      Expression<Func<TDBEntity, bool>> wherePredicate = null)
    {
      return base.TryReturnDBContext((dBContext) =>
      {
        var items = ListOfIQueryableEx(dBContext, selector, wherePredicate,false).ToList();
        return MethodResultFactory.CreateSuccess(items);
      });
    }
  }

  public class DBService<TPrimaryKey, TDBEntity, TDBContext> : DBService<TDBEntity,TDBContext>
    where TDBEntity : DBEntityBase<TPrimaryKey,TDBEntity>, new()
    where TDBContext : DBModelContext
    where TPrimaryKey : IComparable
  {
    public DBService(ILogger logger, IDBContextFactory<TDBContext> dBContextFactory) : base(logger, dBContextFactory)
    {
    }

    internal virtual TResult LoadByIdEx<TResult>(TPrimaryKey id
      , TDBContext dBContext
      , Expression<Func<TDBEntity, TResult>> selector
      , bool tracking = true)
    {
      var item = (tracking ? dBContext.Set<TDBEntity>().AsTracking() : dBContext.Set<TDBEntity>().AsNoTracking()).Where(s => s.Id.Equals(id)).Select(selector).SingleOrDefault();
      return item;
    }

    public virtual IMethodResult<TResult> LoadByIdEx<TResult>(TPrimaryKey id, Expression<Func<TDBEntity, TResult>> selector)
    {
      return base.TryReturnDBContext((dBContext) =>
      {
        var item = LoadByIdEx(id, dBContext, selector,false);
        return MethodResultFactory.CreateSuccess(item);
      });
    }
  }

}
