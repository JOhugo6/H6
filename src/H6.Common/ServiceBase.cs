using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace H6.Common
{
  public class ServiceBase
  {
    protected readonly ILogger Logger;

    public ServiceBase(ILogger logger)
    {
      Logger = logger;
    }

    /// <summary>
    /// call method in try catch block
    /// </summary>
    /// <param name="getResult"></param>
    /// <param name="methodInfo">For better information in log error.</param>
    /// <returns></returns>
    protected IMethodResult TryReturn(Func<IMethodResult> getResult, string methodInfo = null)
    {
      try
      {
        return getResult();
      }
      catch (Exception ex)
      {
        var stackTrace = new System.Diagnostics.StackTrace();
        var method = stackTrace.GetFrame(1).GetMethod();
        Logger.LogError(ex, $"Error in method {method.Name}. Method Info: {methodInfo}");
        return MethodResultFactory.CreateInternalError();
      }
    }

    /// <summary>
    /// call method in try catch block
    /// </summary>
    /// <param name="getResult"></param>
    /// <param name="methodInfo">For better information in log error.</param>
    /// <returns></returns>
    protected async Task<IMethodResult> TryReturnAsync(Func<Task<IMethodResult>> getResult, string methodInfo = null)
    {
      try
      {
        return await getResult();
      }
      catch (Exception ex)
      {
        var stackTrace = new System.Diagnostics.StackTrace();
        var method = stackTrace.GetFrame(1).GetMethod();
        Logger.LogError(ex, $"Error in method {method.Name}. Method Info: {methodInfo}");
        return MethodResultFactory.CreateInternalError();
      }
    }

    /// <summary>
    /// call method in try catch block
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="getResult"></param>
    /// <param name="methodInfo">For better information in log error.</param>
    /// <returns></returns>
    protected IMethodResult<TResult> TryReturn<TResult>(Func<IMethodResult<TResult>> getResult, string methodInfo = null)
    {
      try
      {
        return getResult();
      }
      catch (Exception ex)
      {
        var stackTrace = new System.Diagnostics.StackTrace();
        var method = stackTrace.GetFrame(1).GetMethod();
        Logger.LogError(ex, $"Error in method {method.Name}. Method Info: {methodInfo}");
        return MethodResultFactory.CreateInternalError<TResult>();
      }
    }

    /// <summary>
    /// call method in try catch block
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="getResult"></param>
    /// <param name="methodInfo">For better information in log error.</param>
    /// <returns></returns>
    protected async Task<IMethodResult<TResult>> TryReturnAsync<TResult>(Func<Task<IMethodResult<TResult>>> getResult, string methodInfo = null)
    {
      try
      {
        return await getResult();
      }
      catch (Exception ex)
      {
        var stackTrace = new System.Diagnostics.StackTrace();
        var method = stackTrace.GetFrame(1).GetMethod();
        Logger.LogError(ex, $"Error in method {method.Name}. Method Info: {methodInfo}");
        return MethodResultFactory.CreateInternalError<TResult>();
      }
    }
  }
}
