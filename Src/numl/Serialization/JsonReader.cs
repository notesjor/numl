using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using numl.Math.LinearAlgebra;
using numl.Utils;

namespace numl.Serialization
{
  public class JsonReader : IDisposable
  {
    private readonly TextReader _reader;

#if DEBUG
    private readonly StringBuilder parsedText = new StringBuilder();
#endif

    public JsonReader(TextReader reader) { _reader = reader; }

    public void Dispose()
    {
      _reader?.Dispose();
    }

    private int PeekChar() { return _reader.Peek(); }

    /// <summary>
    ///   Reads from the current JSON serialized stream.
    /// </summary>
    /// <returns></returns>
    public object Read()
    {
      // A JSON value MUST be an object, array, number, or string, 
      // or one of the following three literal names
      //    false null true

      EatWhitespace();

      if (_reader.Peek() == -1)
        return null;

      EatWhitespace();

      var next = _reader.Peek();

      if (next == JsonConstants.BEGIN_OBJECT)
        return ReadObject();
      if (next == JsonConstants.BEGIN_ARRAY)
        return ReadArray();
      if (next == JsonConstants.QUOTATION)
        return ReadString();
      if (char.IsNumber((char) next) || next == '-')
        return ReadNumber();
      if (next == 'f' || next == 'n' || next == 't')
        return ReadLiteral();
      throw new InvalidOperationException("Unexpected token encountered while parsing json");
    }

    /// <summary>
    ///   Reads an array property from the JSON serialized stream.
    /// </summary>
    /// <returns></returns>
    public JsonArray ReadArrayProperty()
    {
      var name = ReadString();
      ReadToken(JsonConstants.COLON);
      var value = ReadArray();
      PeekToken(JsonConstants.COMMA);
      return new JsonArray {Name = name, Value = value};
    }

    private int ReadChar()
    {
#if DEBUG
      var c = _reader.Read();
      parsedText.Append((char) c);
      return c;
#else
            return _reader.Read();
#endif
    }

    /// <summary>
    ///   Reads a property from the JSON serialized stream.
    /// </summary>
    /// <returns></returns>
    public JsonProperty ReadProperty()
    {
      var name = ReadString();
      ReadToken(JsonConstants.COLON);
      var value = Read();
      PeekToken(JsonConstants.COMMA);

      return new JsonProperty {Name = name, Value = value};
    }

    #region Raw Methods

    internal void EatWhitespace()
    {
      while (JsonConstants.WHITESPACE.Contains((char) _reader.Peek()))
        ReadChar();
    }

    public bool IsNull()
    {
      EatWhitespace();
      return _reader.Peek() == JsonConstants.NULL[0] &&
             ReadLiteral() == null;
    }

    internal void ReadToken(int token)
    {
      EatWhitespace();
      if (ReadChar() != token)
        throw new InvalidOperationException($"Invalid token (expected \"{(char) token}\")!");
    }

    internal void PeekToken(int token)
    {
      EatWhitespace();
      if (_reader.Peek() == token)
        ReadChar();
    }

    internal string ReadString()
    {
      EatWhitespace();
      ReadToken(JsonConstants.QUOTATION);
      var sb = new StringBuilder();
      var cur = ReadChar();
      while (cur != JsonConstants.QUOTATION)
      {
        if (cur == JsonConstants.ESCAPE)
        {
          cur = ReadChar();
          switch (cur)
          {
            case '\\':
              sb.Append('\\');
              break;
            case '/':
              sb.Append('/');
              break;
            case 'b':
              sb.Append('\b');
              break;
            case 'f':
              sb.Append('\f');
              break;
            case 'n':
              sb.Append('\n');
              break;
            case 'r':
              sb.Append('\r');
              break;
            case 't':
              sb.Append('\t');
              break;
            case '"':
              sb.Append('"');
              break;
            case 'u':
              var hex = new string(new[] {(char) ReadChar(), (char) ReadChar(), (char) ReadChar(), (char) ReadChar()});
              sb.Append((char) ushort.Parse(hex, NumberStyles.HexNumber));
              break;
            default:
              throw new InvalidOperationException("Unexpected escape token encountered!");
          }
        }
        else
        {
          sb.Append((char) cur);
        }
        cur = ReadChar();
      }

      return sb.ToString();
    }

    internal double ReadNumber()
    {
      EatWhitespace();
      var sb = new StringBuilder();
      while (_reader.Peek() > -1 && JsonConstants.NUMBER.Contains((char) _reader.Peek()))
        sb.Append((char) ReadChar());
      return double.Parse(sb.ToString(), CultureInfo.InvariantCulture);
    }

    internal object ReadLiteral()
    {
      EatWhitespace();
      var next = _reader.Peek();
      switch (next)
      {
        case 'f':
          for (var i = 0; i < JsonConstants.FALSE.Length; i++)
            if (ReadChar() != JsonConstants.FALSE[i])
              throw new InvalidOperationException(
                $"Unexpected token parsing \"false\", exected {JsonConstants.FALSE[i]}!");
          return false;
        case 'n':
          for (var i = 0; i < JsonConstants.NULL.Length; i++)
            if (ReadChar() != JsonConstants.NULL[i])
              throw new InvalidOperationException(
                $"Unexpected token parsing \"null\", exected {JsonConstants.NULL[i]}!");
          return null;
        case 't':
          for (var i = 0; i < JsonConstants.TRUE.Length; i++)
            if (ReadChar() != JsonConstants.TRUE[i])
              throw new InvalidOperationException(
                $"Unexpected token parsing \"true\", exected {JsonConstants.TRUE[i]}!");
          return true;
        default:
          throw new InvalidOperationException("Unexpected token when parsing literal!");
      }
    }

    /// <summary>
    ///   Reads a Vector from the underlying Json stream.
    /// </summary>
    /// <returns></returns>
    public Vector ReadVector()
    {
      return new Vector(ReadArray().Select(i => (double) i).ToArray());
    }

    /// <summary>
    ///   Reads the vector property from the underlying Json stream..
    /// </summary>
    /// <returns>JsonProperty.</returns>
    public JsonProperty ReadVectorProperty()
    {
      var name = ReadString();
      ReadToken(JsonConstants.COLON);
      var value = ReadVector();
      PeekToken(JsonConstants.COMMA);

      return new JsonProperty {Name = name, Value = value};
    }

    /// <summary>
    ///   Reads a Matrix from the underlying Json stream.
    /// </summary>
    /// <returns></returns>
    public Matrix ReadMatrix()
    {
      return new Matrix(
        ReadArray().Select(i => ((object[]) i).Select(j => (double) j).ToArray()).ToArray()
      );
    }

    /// <summary>
    ///   Reads the matrix property.
    /// </summary>
    /// <returns>JsonProperty.</returns>
    public JsonProperty ReadMatrixProperty()
    {
      var name = ReadString();
      ReadToken(JsonConstants.COLON);
      var value = ReadMatrix();
      PeekToken(JsonConstants.COMMA);

      return new JsonProperty {Name = name, Value = value};
    }

    internal object[] ReadArray()
    {
      EatWhitespace();
      ReadToken(JsonConstants.BEGIN_ARRAY);
      var array = new List<object>();
      int token;
      do
      {
        EatWhitespace();

        // empty array situation
        if (_reader.Peek() == JsonConstants.END_ARRAY)
        {
          ReadChar();
          break;
        }

        array.Add(Read());

        EatWhitespace();

        token = ReadChar();
        if (token != JsonConstants.COMMA && token != JsonConstants.END_ARRAY)
          throw new InvalidOperationException("Unexpected token while parsing array!");
      }
      while (token != JsonConstants.END_ARRAY);

      return array.ToArray();
    }

    internal void ReadStartObject()
    {
      EatWhitespace();
      ReadToken(JsonConstants.BEGIN_OBJECT);
    }

    internal void ReadEndObject()
    {
      EatWhitespace();
      ReadToken(JsonConstants.END_OBJECT);
    }

    internal object ReadObject()
    {
      ReadStartObject();
      var obj = new Dictionary<string, object>();
      while (_reader.Peek() != JsonConstants.END_OBJECT)
      {
        var p = ReadProperty();

        if (p.Name == JsonSerializer.SERIALIZER_ATTRIBUTE)
        {
          var s = Ject.FindType(p.Value.ToString()).GetSerializer();
          var o = s.Read(this);
          s.PostRead(this);
          return o;
        }

        if (obj.ContainsKey(p.Name))
          throw new InvalidOperationException("Key already exists");

        obj[p.Name] = p.Value;
      }
      ReadEndObject();
      return obj;
    }

    #endregion
  }
}