namespace numl.Serialization
{
  /// <summary>
  ///   JsonProperty structure.
  /// </summary>
  public struct JsonProperty
  {
    /// <summary>
    ///   Name of the property.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///   Value.
    /// </summary>
    public object Value { get; set; }
  }
}