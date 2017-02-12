using System;
using numl.Model;

namespace numl.Tests.Data
{
  using numl.Model;

  public class FakeDate
  {
    // First four to test feature conversion
    [DateFeature(DateTimeFeature.Day)]
    public DateTime Date1 { get; set; }

    [DateFeature(DateTimeFeature.Day | DateTimeFeature.Year)]
    public DateTime Date2 { get; set; }

    [DateFeature(DateTimeFeature.Day | DateTimeFeature.DayOfYear | DateTimeFeature.Millisecond)]
    public DateTime Date3 { get; set; }

    [DateFeature(DateTimeFeature.Month | DateTimeFeature.Year | DateTimeFeature.Second | DateTimeFeature.Hour)]
    public DateTime Date4 { get; set; }

    // last four test date portion
    [DateFeature(DatePortion.Date)]
    public DateTime Date5 { get; set; }

    [DateFeature(DatePortion.Date | DatePortion.TimeExtended)]
    public DateTime Date6 { get; set; }

    [DateFeature(DatePortion.Time)]
    public DateTime Date7 { get; set; }

    [DateFeature(DatePortion.Date | DatePortion.DateExtended)]
    public DateTime Date8 { get; set; }
  }
}