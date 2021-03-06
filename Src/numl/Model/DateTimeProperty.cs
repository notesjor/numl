using System;
using System.Collections.Generic;

namespace numl.Model
{
  /// <summary>DateTime Property. Used as a feature expansion mechanism.</summary>
  public class DateTimeProperty : Property
  {
    /// <summary>The length.</summary>
    private int _length = -1;

    /// <summary>Default constructor.</summary>
    public DateTimeProperty() { Initialize(DatePortion.Date | DatePortion.DateExtended); }

    /// <summary>Constructor.</summary>
    /// <param name="portion">The portion.</param>
    public DateTimeProperty(DatePortion portion) { Initialize(portion); }

    /// <summary>Constructor.</summary>
    /// <param name="features">The features.</param>
    public DateTimeProperty(DateTimeFeature features)
    {
      Type = typeof(DateTime);
      Features = features;
    }

    /// <summary>Gets or sets the features.</summary>
    /// <value>The features.</value>
    public DateTimeFeature Features { get; set; }

    /// <summary>Length of property.</summary>
    /// <value>The length.</value>
    public override int Length
    {
      get
      {
        if (_length == -1)
        {
          _length = 0;
          if (Features.HasFlag(DateTimeFeature.Year))
            _length++;
          if (Features.HasFlag(DateTimeFeature.DayOfYear))
            _length++;
          if (Features.HasFlag(DateTimeFeature.Month))
            _length++;
          if (Features.HasFlag(DateTimeFeature.Day))
            _length++;
          if (Features.HasFlag(DateTimeFeature.DayOfWeek))
            _length++;
          if (Features.HasFlag(DateTimeFeature.Hour))
            _length++;
          if (Features.HasFlag(DateTimeFeature.Minute))
            _length++;
          if (Features.HasFlag(DateTimeFeature.Second))
            _length++;
          if (Features.HasFlag(DateTimeFeature.Millisecond))
            _length++;
        }

        return _length;
      }
    }

    /// <summary>Convert the numeric representation back to the original type.</summary>
    /// <param name="val">.</param>
    /// <returns>An object.</returns>
    public override object Convert(double val) { return val; }

    /// <summary>Convert an object to a list of numbers.</summary>
    /// <exception cref="InvalidCastException">
    ///   Thrown when an object cannot be cast to a required
    ///   type.
    /// </exception>
    /// <param name="o">Object.</param>
    /// <returns>Lazy list of doubles.</returns>
    public override IEnumerable<double> Convert(object o)
    {
      if (o.GetType() == typeof(DateTime))
      {
        // tedious I know...
        // be thankful I wrote it for you...
        var d = (DateTime) o;
        if (Features.HasFlag(DateTimeFeature.Year))
          yield return d.Year;
        if (Features.HasFlag(DateTimeFeature.DayOfYear))
          yield return d.DayOfYear;
        if (Features.HasFlag(DateTimeFeature.Month))
          yield return d.Month;
        if (Features.HasFlag(DateTimeFeature.Day))
          yield return d.Day;
        if (Features.HasFlag(DateTimeFeature.DayOfWeek))
          yield return (int) d.DayOfWeek;
        if (Features.HasFlag(DateTimeFeature.Hour))
          yield return d.Hour;
        if (Features.HasFlag(DateTimeFeature.Minute))
          yield return d.Minute;
        if (Features.HasFlag(DateTimeFeature.Second))
          yield return d.Second;
        if (Features.HasFlag(DateTimeFeature.Millisecond))
          yield return d.Millisecond;
      }
      else
      {
        throw new InvalidCastException("Object is not a date");
      }
    }

    /// <summary>
    ///   Equality test
    /// </summary>
    /// <param name="obj">object to compare</param>
    /// <returns>equality</returns>
    public override bool Equals(object obj)
    {
      if (base.Equals(obj) && obj is DateTimeProperty)
        return Features == ((DateTimeProperty) obj).Features;
      return false;
    }

    /// <summary>
    ///   Retrieve the list of expanded columns. If there is a one-to-one correspondence between the
    ///   type and its expansion it will return a single value/.
    /// </summary>
    /// <returns>
    ///   An enumerator that allows foreach to be used to process the columns in this collection.
    /// </returns>
    public override IEnumerable<string> GetColumns() { return GetColumns(Features); }

    /// <summary>
    ///   Retrieve the list of expanded columns. If there is a one-to-one correspondence between the
    ///   type and its expansion it will return a single value/.
    /// </summary>
    /// <param name="features">Features</param>
    /// <returns></returns>
    public static IEnumerable<string> GetColumns(DateTimeFeature features)
    {
      Func<DateTimeFeature, string> c = d => Enum.GetName(typeof(DateTimeFeature), d);
      if (features.HasFlag(DateTimeFeature.Year))
        yield return c(DateTimeFeature.Year);
      if (features.HasFlag(DateTimeFeature.DayOfYear))
        yield return "DayOfYear";
      if (features.HasFlag(DateTimeFeature.Month))
        yield return "Month";
      if (features.HasFlag(DateTimeFeature.Day))
        yield return "Day";
      if (features.HasFlag(DateTimeFeature.DayOfWeek))
        yield return "DayOfWeek";
      if (features.HasFlag(DateTimeFeature.Hour))
        yield return "Hour";
      if (features.HasFlag(DateTimeFeature.Minute))
        yield return "Minute";
      if (features.HasFlag(DateTimeFeature.Second))
        yield return "Second";
      if (features.HasFlag(DateTimeFeature.Millisecond))
        yield return "Millisecond";
    }

    /// <summary>
    ///   Gets DateTimeFeature enum from enumerated
    ///   values
    /// </summary>
    /// <param name="features">string collection of enum values</param>
    /// <returns>DateTimeFeatures</returns>
    public static DateTimeFeature GetFeatures(string[] features)
    {
      DateTimeFeature feature = 0;
      for (var i = 0; i < features.Length; i++)
        feature |= (DateTimeFeature) Enum.Parse(typeof(DateTimeFeature), features[i]);
      return feature;
    }

    /// <summary>
    ///   Return hash
    /// </summary>
    /// <returns>hash</returns>
    public override int GetHashCode() { return base.GetHashCode(); }

    /// <summary>Initializes this object.</summary>
    /// <param name="portion">The portion.</param>
    private void Initialize(DatePortion portion)
    {
      Type = typeof(DateTime);
      Features = 0;
      if (portion.HasFlag(DatePortion.Date))
        Features |= DateTimeFeature.Year | DateTimeFeature.Month |
                    DateTimeFeature.Day;

      if (portion.HasFlag(DatePortion.DateExtended))
        Features |= DateTimeFeature.DayOfYear | DateTimeFeature.DayOfWeek;

      if (portion.HasFlag(DatePortion.Time))
        Features |= DateTimeFeature.Hour | DateTimeFeature.Minute;

      if (portion.HasFlag(DatePortion.TimeExtended))
        Features |= DateTimeFeature.Second | DateTimeFeature.Millisecond;
    }
  }
}