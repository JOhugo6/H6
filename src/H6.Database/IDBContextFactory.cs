using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Database
{
  public interface IDBContextFactory<T> where T: DBModelContext
  {
    T EnsureDBContext(); 
  }
}
