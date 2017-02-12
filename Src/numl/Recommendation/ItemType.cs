namespace numl.Recommendation
{
  /// <summary>
  /// Type of the Item to be recommended
  /// </summary>
  public enum ItemType
  {
    /// <summary>
    /// A reference item (i.e. a User Rating)
    /// </summary>
    References,
    /// <summary>
    /// An entity item (i.e. a book or movie)
    /// </summary>
    Entities
  }
}