using System.Collections.Generic;
using System.Linq;

namespace numl.AI.Collections
{
  /// <summary>
  ///   A sorted data table.
  /// </summary>
  /// <typeparam name="TKey1">Parent key type.</typeparam>
  /// <typeparam name="TKey2">Child key type.</typeparam>
  /// <typeparam name="TValue">Value type.</typeparam>
  public class SortedTable<TKey1, TKey2, TValue>
  {
    private readonly SortedDictionary<TKey1, SortedDictionary<TKey2, TValue>> _Table;

    /// <summary>
    ///   Default constructor.
    /// </summary>
    public SortedTable()
    {
      _Table = new SortedDictionary<TKey1, SortedDictionary<TKey2, TValue>>();
      DefaultValue = default(TValue);
    }

    /// <summary>
    ///   Gets or sets the value for the key-key pair.  Returns the default value if one is not found at the specified
    ///   location.
    /// </summary>
    /// <param name="key1">Parent key.</param>
    /// <param name="key2">Child key.</param>
    /// <returns><typeparamref name="TValue" />.</returns>
    public TValue this[TKey1 key1, TKey2 key2]
    {
      get
      {
        if (key1 == null || key2 == null || !_Table.ContainsKey(key1) || _Table[key1] == null)
          return DefaultValue;
        return _Table[key1][key2];
      }
      set
      {
        if (_Table[key1] == null)
          _Table[key1] = new SortedDictionary<TKey2, TValue>();

        _Table[key1][key2] = value;
      }
    }

    /// <summary>
    ///   Gets or sets the default table value.
    /// </summary>
    public TValue DefaultValue { get; set; }

    /// <summary>
    ///   Returns all the keys in the current collection.
    /// </summary>
    public IEnumerable<TKey1> Keys { get { return _Table.Keys; } }

    /// <summary>
    ///   Adds the specified parent key to the collection.
    /// </summary>
    /// <param name="key">Parent key to add.</param>
    public void AddKey(TKey1 key)
    {
      if (!_Table.ContainsKey(key))
        _Table.Add(key, new SortedDictionary<TKey2, TValue>());
    }

    /// <summary>
    ///   Adds or updates the paired value in the collection.
    /// </summary>
    /// <param name="key">Parent key.</param>
    /// <param name="childKey">Child key.</param>
    /// <param name="value">Value to add.</param>
    public void AddOrUpdate(TKey1 key, TKey2 childKey, TValue value)
    {
      if (!_Table.ContainsKey(key))
        _Table.Add(key, new SortedDictionary<TKey2, TValue>());

      if (!_Table[key].ContainsKey(childKey))
        _Table[key].Add(childKey, value);
      else
        _Table[key][childKey] = value;
    }

    /// <summary>
    ///   Returns <c>True</c> if the specified key exists in the collection, otherwise <c>False</c>.
    /// </summary>
    /// <param name="key">Parent key.</param>
    /// <returns>Boolean.</returns>
    public bool ContainsKey(TKey1 key) { return _Table.ContainsKey(key); }

    /// <summary>
    ///   Returns <c>True</c> if the specified keys exist in the collection, otherwise <c>False</c>.
    /// </summary>
    /// <param name="key">Parent key.</param>
    /// <param name="childKey">Child key.</param>
    /// <returns>Boolean.</returns>
    public bool ContainsKey(TKey1 key, TKey2 childKey) { return ContainsKey(key) && _Table[key].ContainsKey(childKey); }

    /// <summary>
    ///   Returns all associated child keys for the parent key.
    /// </summary>
    /// <param name="key">Parent key.</param>
    /// <returns>IEnumerable&lt;<typeparamref name="TValue" />&gt;</returns>
    public IEnumerable<TKey2> GetKeys(TKey1 key) { return _Table[key]?.Select(s => s.Key); }

    /// <summary>
    ///   Returns all child key-value pairs for the specified parent key.
    /// </summary>
    /// <param name="key">Parent key.</param>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<TKey2, TValue>> GetPairs(TKey1 key) { return _Table[key]; }

    /// <summary>
    ///   Returns all values for the given parent key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IEnumerable<TValue> GetValues(TKey1 key) { return _Table[key]?.Select(s => s.Value); }

    /// <summary>
    ///   Removes all child elements, including the parent, by the specified parent key.
    /// </summary>
    /// <param name="key">Parent key.</param>
    public void Remove(TKey1 key) { _Table.Remove(key); }

    /// <summary>
    ///   Removes only the value at the specified location.
    /// </summary>
    /// <param name="key">Parent key.</param>
    /// <param name="childKey">Child key.</param>
    public void Remove(TKey1 key, TKey2 childKey)
    {
      if (_Table.ContainsKey(key))
        _Table[key].Remove(childKey);
    }
  }
}