using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Common
{
  public interface IMethodResult
  {
    /// <summary>
    /// The error code from method
    /// </summary>
    string ErrorCode { get; }

    /// <summary>
    /// The error message from method
    /// </summary>
    string ErrorMessage { get; }

    /// <summary>
    /// Is method result success?
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Whether the error Code equals given code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    bool Is(string code);
  }

  public interface IMethodResult<T> : IMethodResult
  {
    /// <summary>
    /// Method result object
    /// </summary>
    T ResultObject { get; }
  }
}
