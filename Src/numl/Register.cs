using System.Reflection;
using numl.Utils;

namespace numl
{
  public static class Register
  {
    /// <summary>
    ///   Registration for numl to understand all of
    ///   your types
    /// </summary>
    /// <param name="assemblies">The assembly.</param>
    public static void Assembly(params Assembly[] assemblies)
    {
      // register assemblies
      foreach (var a in assemblies)
        Ject.AddAssembly(a);
    }
  }
}