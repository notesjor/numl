namespace numl.Tests.Data
{
  public class FakeDateWithError
  {
    [DateFeature(DatePortion.Date)]
    public int NotADate { get; set; }
  }
}