using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Common
{
  public class MethodResult : IMethodResult
  {
    private string _errorCode;
    private string _errorMessage;

    internal MethodResult()
    {
    }

    internal MethodResult(IMethodResult methodResult)
    {
      _errorCode = methodResult.ErrorCode;
      _errorMessage = methodResult.ErrorMessage;
    }

    /// <summary>
    /// Is method result success?
    /// </summary>
    public bool IsSuccess
    {
      get => string.IsNullOrEmpty(_errorCode);
    }

    /// <summary>
    /// The error code from method
    /// </summary>
    public string ErrorCode
    {
      get => _errorCode;
      internal set
      {
        if (!string.IsNullOrEmpty(value)) _errorCode = value.Trim();
        else _errorCode = null;
      }
    }

    /// <summary>
    /// Error nessage from method.
    /// </summary>
    public string ErrorMessage
    {
      get => _errorMessage;
      internal set
      {
        if (!string.IsNullOrEmpty(value)) _errorMessage = value.Trim();
        else _errorMessage = null;
      }
    }

    /// <summary>
    /// Whether the error Code equals given code (using OrdinalIgnoreCase comparison)
    /// </summary>
    public bool Is(string code)
    {
      if (string.IsNullOrEmpty(code))
        throw new ArgumentException("Code is empty.");

      return string.Equals(code.Trim(), _errorCode, StringComparison.OrdinalIgnoreCase);
    }
  }

  public class MethodResult<T> : MethodResult, IMethodResult<T>,IMethodResult
  {
    internal MethodResult()
    {
    }

    internal MethodResult(IMethodResult methodResult) : base(methodResult)
    {
    }

    /// <summary>
    /// Method result object
    /// </summary>
    public T ResultObject { get; internal set; }
  }
}
