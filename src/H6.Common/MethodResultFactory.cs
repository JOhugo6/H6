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

    public static MethodResult CreateError(string errorCode, string errorMessage = null)
    {
      return new MethodResult() { ErrorCode = errorCode, ErrorMessage = errorMessage };
    }

    public static MethodResult<TResult> CreateError<TResult>(string errorCode, string errorMessage = null)
    {
      return new MethodResult<TResult>() { ErrorCode = errorCode, ErrorMessage = errorMessage };
    }

    public static IMethodResult CreateInternalError()
    {
      return new MethodResult() { ErrorCode = InternalError_CODE };
    }

    public static IMethodResult<TResult> CreateInternalError<TResult>()
    {
      return new MethodResult<TResult>() { ErrorCode = InternalError_CODE };
    }

    public static IMethodResult CreateFrom(IMethodResult source)
    {
      return new MethodResult(source);
    }

    public static IMethodResult<TResult> CreateFrom<TResult>(IMethodResult source, TResult data)
    {
      var result = new MethodResult<TResult>(source);
      result.ResultObject = data;
      return result;
    }
  }
}
