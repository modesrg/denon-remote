using DenonRemote.Constants;
using DenonRemote.Models;

namespace DenonRemote.Services;

// Mutable accumulator — updated field-by-field as Telnet lines arrive.
internal sealed class ReceiverStateAccumulator
{
    public string ModelName { get; set; } = "";
    public bool IsConnected { get; set; }
    public bool PowerOn { get; set; }
    public double Volume { get; set; }   // stored as dB (Denon scale − 80)
    public bool Muted { get; set; }
    public string Input { get; set; } = "";
    public string SoundMode { get; set; } = "";
    public bool   Zone2On    { get; set; }
    public double Zone2Volume { get; set; }
    public string Zone2Input  { get; set; } = "";
    public Dictionary<string, string> InputNames { get; } = new(StringComparer.OrdinalIgnoreCase);
    public string SignalFormat   { get; set; } = "";
    public string SignalChannels { get; set; } = "";
    public string OutputChannels { get; set; } = "";

    public ReceiverState ToSnapshot() => new()
    {
        ModelName = ModelName,
        IsConnected = IsConnected,
        PowerOn = PowerOn,
        Volume = Volume,
        Muted = Muted,
        Input = Input,
        SoundMode = SoundMode,
        Zone2On     = Zone2On,
        Zone2Volume = Zone2Volume,
        Zone2Input  = Zone2Input,
        InputNames = new Dictionary<string, string>(InputNames, StringComparer.OrdinalIgnoreCase),
        SignalFormat   = SignalFormat,
        SignalChannels = SignalChannels,
        OutputChannels = OutputChannels,
    };
}

internal static class TelnetStateParser
{
    /// Returns true when the line produced a state change worth broadcasting.
    public static bool TryApply(string line, ReceiverStateAccumulator state)
    {
        if (line.StartsWith("PW", StringComparison.Ordinal))
        {
            state.PowerOn = line == "PWON";
            return true;
        }

        if (line.StartsWith("MV", StringComparison.Ordinal))
        {
            if (line.StartsWith("MVMAX")) return false;  // ignore max-volume notification
            state.Volume = ParseVolumeToDb(line[2..]);
            return true;
        }

        if (line.StartsWith("MU", StringComparison.Ordinal))
        {
            state.Muted = line == "MUON";
            return true;
        }

        if (line.StartsWith("SI", StringComparison.Ordinal))
        {
            state.Input = line; // keep full "SIHDMI1" to match Commands.Input.All keys
            return true;
        }

        if (line.StartsWith("MS", StringComparison.Ordinal))
        {
            state.SoundMode = line[2..];
            return true;
        }

        if (line.StartsWith("Z2", StringComparison.Ordinal))
        {
            var rest = line[2..];
            if (rest == "ON")  { state.Zone2On = true;  return true; }
            if (rest == "OFF") { state.Zone2On = false; return true; }
            if (rest.Length >= 2 && int.TryParse(rest, out _))
            {
                state.Zone2Volume = ParseVolumeToDb(rest);
                return true;
            }
            if (Commands.Input.All.ContainsKey("SI" + rest))
            {
                state.Zone2Input = "SI" + rest; // store as full SI key to match Commands.Input.All
                return true;
            }
            return false;
        }

        if (line.StartsWith("SSINFA", StringComparison.Ordinal))
        {
            var rest = line[6..];
            if (rest.StartsWith("ISF", StringComparison.Ordinal))      // input sampling freq — ignore
                return false;
            if (rest.StartsWith("ISB", StringComparison.Ordinal))      // input bitstream (format)
            {
                state.SignalFormat = rest[3..].Trim();
                return true;
            }
            if (rest.StartsWith("ICN", StringComparison.Ordinal))      // input channel count
            {
                state.SignalChannels = rest[3..].Trim();
                return true;
            }
            if (rest.StartsWith("OAN", StringComparison.Ordinal))      // output active channel count
            {
                state.OutputChannels = rest[3..].Trim();
                return true;
            }
            return false;
        }

        if (line.StartsWith("SSFUN", StringComparison.Ordinal))
        {
            var body = line[5..];
            var space = body.IndexOf(' ');
            if (space > 0)
                state.InputNames["SI" + body[..space]] = body[(space + 1)..].Trim();
            return true;
        }

        return false;
    }

    // Denon encodes volume as 2-digit whole (e.g. "45") or 3-digit half-step (e.g. "455" = 45.5).
    // Converts to dB by subtracting 80 so ReceiverState.Volume stays consistent with the HTTP baseline.
    private static double ParseVolumeToDb(string raw)
    {
        if (!int.TryParse(raw, out var value)) return 0;

        var denonScale = raw.Length == 3
            ? value / 10.0   // "455" → 45.5
            : (double)value; // "45"  → 45.0

        return denonScale - 80.0;
    }
}
