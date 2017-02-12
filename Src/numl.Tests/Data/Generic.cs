using System;
using System.Collections.Generic;
using numl.Model;

namespace numl.Tests.Data
{
  public class Generic
  {
    // univariate types
    [Feature]
    public bool VarBoolean { get; set; }

    [Feature]
    public byte VarByte { get; set; }

    // explicit multivariate types
    [EnumerableFeature(10)]
    public byte[] VarByteArray { get; set; }

    [Feature]
    public char VarChar { get; set; }

    [DateFeature(DatePortion.Date | DatePortion.Time)]
    public DateTime VarDateTime { get; set; }

    [Feature]
    public decimal VarDecimal { get; set; }

    [Feature]
    public double VarDouble { get; set; }

    [Feature]
    public FakeEnum VarEnum { get; set; }

    [GuidFeature]
    public Guid VarGuid { get; set; }

    [Feature]
    public short VarInt16 { get; set; }

    [Feature]
    public int VarInt32 { get; set; }

    [Feature]
    public long VarInt64 { get; set; }

    [Feature]
    public sbyte VarSbyte { get; set; }

    [Feature]
    public float VarSingle { get; set; }

    // implicit multivariate types
    [Feature]
    public string VarString { get; set; }

    [Feature]
    public TimeSpan VarTimeSpan { get; set; }

    [Feature]
    public ushort VarUInt16 { get; set; }

    [Feature]
    public uint VarUInt32 { get; set; }

    [Feature]
    public ulong VarUInt64 { get; set; }

    public static IEnumerable<Generic> GetRows(int count)
    {
      while (count-- > 0)
        yield return new Generic
        {
          VarBoolean = true,
          VarByte = 0xFF,
          VarSbyte = 1,
          VarChar = 'A',
          VarDecimal = 0.4m,
          VarDouble = 0.1d,
          VarSingle = 300f,
          VarInt16 = 1,
          VarUInt16 = 2,
          VarInt32 = 3,
          VarUInt32 = 4,
          VarInt64 = 5,
          VarUInt64 = 6,
          VarEnum = FakeEnum.Item0,
          VarTimeSpan = TimeSpan.FromSeconds(100),
          VarString = "test1",
          VarDateTime = DateTime.Now,
          VarGuid = Guid.NewGuid(),
          VarByteArray =
            new byte[]
            {
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF,
              0xFF
            }
        };
    }
  }
}