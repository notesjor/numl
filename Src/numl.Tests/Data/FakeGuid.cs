using System;
using numl.Model;

namespace numl.Tests.Data
{
  public class FakeGuid
  {
    [GuidFeature]
    public Guid Guid1 { get; set; }
  }
}