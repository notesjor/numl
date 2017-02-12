using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using numl.Serialization;

namespace numl.Tests.SerializationTests
{
  public class BaseSerialization
  {
    public BaseSerialization()
    {
      // Need to register external assemblies
      Register.Assembly(GetType().GetTypeInfo().Assembly);
    }

    internal T Deserialize<T>()
    {
      var caller = GetCaller();
      var file = Path.Combine(GetPath(GetType()), $"{caller}.json");

      using (var fs = new FileStream(file, FileMode.Open))
      {
        using (var f = new StreamReader(fs))
        {
          var val = new JsonReader(f).Read();
          return (T) val;
        }
      }
    }

    internal object Deserialize()
    {
      var caller = GetCaller();
      var file = Path.Combine(GetPath(GetType()), $"{caller}.json");

      using (var fs = new FileStream(file, FileMode.Open))
      {
        using (var f = new StreamReader(fs))
        {
          return new JsonReader(f).Read();
        }
      }
    }


    internal string GetCaller()
    {
      var stack = Environment.StackTrace.Split('\n')
                             .Select(s => s.Trim())
                             .SkipWhile(s => !s.Contains(GetType().GetTypeInfo().Name))
                             .ToArray();

      var regex = new Regex(@".\.(.*)\(");
      var match = regex.Match(stack[0]);
      var method = match.Groups[1].Value.Split('.').Last();
      return method;
    }

    internal static string GetPath(Type t)
    {
      var basePath = Path.Combine(
        new[]
        {
          Directory.GetCurrentDirectory(),
          "TestResults",
          t.Name
        });

      if (!Directory.Exists(basePath))
        Directory.CreateDirectory(basePath);

      return basePath;
    }

    internal JsonReader GetReader()
    {
      var caller = GetCaller();
      var file = Path.Combine(GetPath(GetType()), $"{caller}.json");

      var fs = new FileStream(file, FileMode.Open);
      var f = new StreamReader(fs);
      return new JsonReader(f);
    }

    internal JsonWriter GetWriter()
    {
      var caller = GetCaller();
      var file = Path.Combine(GetPath(GetType()), $"{caller}.json");
      if (File.Exists(file))
        File.Delete(file);

      var fs = new FileStream(file, FileMode.CreateNew);
      var f = new StreamWriter(fs);
      return new JsonWriter(f);
    }

    internal void Serialize(object o)
    {
      var caller = GetCaller();
      var file = Path.Combine(GetPath(GetType()), $"{caller}.json");

      if (File.Exists(file))
        File.Delete(file);

      using (var fs = new FileStream(file, FileMode.CreateNew))
      {
        using (var f = new StreamWriter(fs))
        {
          new JsonWriter(f).Write(o);
        }
      }
    }
  }
}