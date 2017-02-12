using System;

namespace numl.Model
{
  /// <summary>Features available for the DateTime property.</summary>
  [Flags]
  public enum DateTimeFeature
  {
    /// <summary>
    ///   Year
    /// </summary>
    Year = 0x0001,

    /// <summary>
    ///   Day of the year (1, 366)
    /// </summary>
    DayOfYear = 0x0002,

    /// <summary>
    ///   Month
    /// </summary>
    Month = 0x0008,

    /// <summary>
    ///   Day
    /// </summary>
    Day = 0x0010,

    /// <summary>
    ///   Day of the week (0, 6)
    /// </summary>
    DayOfWeek = 0x0020,

    /// <summary>
    ///   Hour
    /// </summary>
    Hour = 0x0040,

    /// <summary>
    ///   Minute
    /// </summary>
    Minute = 0x0080,

    /// <summary>
    ///   Second
    /// </summary>
    Second = 0x0100,

    /// <summary>
    ///   Millisecond
    /// </summary>
    Millisecond = 0x0200
  }
}