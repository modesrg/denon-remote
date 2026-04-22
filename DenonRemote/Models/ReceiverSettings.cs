namespace DenonRemote.Models;

public record ReceiverSettings
{
    public string BaseUrl { get; init; } = "http://192.168.1.177";
    public int TelnetPort { get; init; } = 23;
}
