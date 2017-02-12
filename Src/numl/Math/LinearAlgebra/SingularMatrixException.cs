using System;

namespace numl.Math.LinearAlgebra
{
  /// <summary>Exception for signalling singular matrix errors.</summary>
  public class SingularMatrixException : Exception
  {
    /// <summary>Default constructor.</summary>
    public SingularMatrixException() { }

    /// <summary>Constructor.</summary>
    /// <param name="message">The message.</param>
    public SingularMatrixException(string message)
      : base(message) { }
  }
}