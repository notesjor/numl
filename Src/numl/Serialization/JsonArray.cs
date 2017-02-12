namespace numl.Serialization
{
  /// <summary>
  ///   JsonArray structure.
  /// </summary>
  public struct JsonArray
  {
    /// <summary>
    ///   Name of the property.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///   Value.
    /// </summary>
    public object[] Value { get; set; }
  }
}