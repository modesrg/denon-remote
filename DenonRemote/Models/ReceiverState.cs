namespace DenonRemote.Models;

public record ReceiverState
{
    public string ModelName { get; init; } = "";
    public bool IsConnected { get; init; }
    public bool PowerOn { get; init; }
    public double Volume { get; init; }
    public bool Muted { get; init; }
    public string Input { get; init; } = "";
    public string SoundMode { get; init; } = "";
    public bool   Zone2On     { get; init; }
    public double Zone2Volume { get; init; }
    public string Zone2Input  { get; init; } = "";
    public IReadOnlyDictionary<string, string> InputNames { get; init; } = new Dictionary<string, string>();
    public string SignalFormat   { get; init; } = "";  // e.g. "PCM", "Dolby TrueHD"
    public string SignalChannels { get; init; } = "";  // e.g. "5.1ch"
    public string OutputChannels { get; init; } = ""; // e.g. "7.1ch"

    public static ReceiverState Empty => new();
}
