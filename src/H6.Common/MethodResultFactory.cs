using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Common
{
  public class MethodResultFactory
  {
    public const string InternalError_CODE = "InternalError";

    public static IMethodResult CreateSuccess()
    {
      return new MethodResult();
    }

    public static IMethodResult<TResult> CreateSuccess<TResult>(TResult result)
    {
      return new MethodResult<TResult> { ResultObject = result };
    }
    public static IMethodResult CreateInternalError()
    {
      return new MethodResult() { ErrorCode = InternalError_CODE };
    }

    public static IMethodResult<TResult> CreateInternalError<TResult>()
    {
      return new MethodResult<TResult>() { ErrorCode = InternalError_CODE };
    }

    public static IMethodResult CreateFrom(MethodResult source)
    {
      return new MethodResult(source);
    }

    public static IMethodResult<TResult> CreateFrom<TResult>(MethodResult source, TResult data)
    {
      var result = new MethodResult<TResult>(source);
      result.ResultObject = data;
      return result;
    }
  }
}
