using System;
using Xunit;

namespace numl.Tests
{
  public static class Almost
  {
    public static void Equal(double expected, double actual, double tolerance, string message = "")
    {
      Assert.InRange(actual, expected - tolerance, expected + tolerance);
      if (message.Length > 0)
        Console.WriteLine(message);
    }
  }
}