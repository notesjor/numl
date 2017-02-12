using numl.Model;

namespace numl.Tests.Data
{
  public class Tennis
  {
    [Feature]
    public Humidity Humidity { get; set; }

    [Feature]
    public Outlook Outlook { get; set; }

    [Label]
    public bool Play { get; set; }

    [Feature]
    public Temperature Temperature { get; set; }

    [Feature]
    public bool Windy { get; set; }

    public static Tennis[] GetData()
    {
      return new[]
      {
        Make(Outlook.Sunny, Temperature.Hot, Humidity.High, false, false),
        Make(Outlook.Sunny, Temperature.Hot, Humidity.High, true, false),
        Make(Outlook.Overcast, Temperature.Hot, Humidity.High, false, true),
        Make(Outlook.Rainy, Temperature.Mild, Humidity.High, false, true),
        Make(Outlook.Rainy, Temperature.Cool, Humidity.Normal, false, true),
        Make(Outlook.Rainy, Temperature.Cool, Humidity.Normal, true, false),
        Make(Outlook.Overcast, Temperature.Cool, Humidity.Normal, true, true),
        Make(Outlook.Sunny, Temperature.Mild, Humidity.High, false, false),
        Make(Outlook.Sunny, Temperature.Cool, Humidity.Normal, false, true),
        Make(Outlook.Rainy, Temperature.Mild, Humidity.Normal, false, true),
        Make(Outlook.Sunny, Temperature.Mild, Humidity.Normal, true, true),
        Make(Outlook.Overcast, Temperature.Mild, Humidity.High, true, true),
        Make(Outlook.Overcast, Temperature.Hot, Humidity.Normal, false, true),
        Make(Outlook.Rainy, Temperature.Mild, Humidity.High, true, false)
      };
    }

    public static Tennis Make(Outlook outlook, Temperature temperature, Humidity humidity, bool windy, bool play)
    {
      return new Tennis
      {
        Outlook = outlook,
        Temperature = temperature,
        Humidity = humidity,
        Windy = windy,
        Play = play
      };
    }
  }
}