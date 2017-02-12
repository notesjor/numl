using System;

namespace numl.Model
{
  /// <summary>Date portions available for the DateTime property.</summary>
  [Flags]
  public enum DatePortion
  {
    /// <summary>
    /// Date (Jan. 1, 2000) -> [1, 1, 2000]
    /// </summary>
    Date = 0x0001,
    /// <summary>
    /// Extended Date (Jan. 1, 2000) -> [1, 6] (DayOfYear, DayOfWeek)
    /// </summary>
    DateExtended = 0x0002,
    /// <summary>
    /// Time 4:45pm -> [16, 45] (Hour, Minute)
    /// </summary>
    Time = 0x0008,
    /// <summary>
    /// Extended Time 4:45pm -> [0, 0] (Second, Millisecond)
    /// </summary>
    TimeExtended = 0x0010
  }
}