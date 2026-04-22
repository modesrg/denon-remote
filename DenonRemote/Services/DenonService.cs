using DenonRemote.Clients;
using DenonRemote.Constants;
using DenonRemote.Models;
using System.Xml.Linq;

namespace DenonRemote.Services;

public class DenonService
{
    private readonly IDenonClient _client;

    public DenonService(IDenonClient client)
    {
        _client = client;
    }

    // ── Power ────────────────────────────────────────────────────────────────

    public Task PowerOnAsync() => _client.SendCommandAsync(Commands.Power.On);
    public Task PowerStandbyAsync() => _client.SendCommandAsync(Commands.Power.Standby);

    // ── Volume ───────────────────────────────────────────────────────────────

    public Task VolumeUpAsync() => _client.SendCommandAsync(Commands.Volume.Up);
    public Task VolumeDownAsync() => _client.SendCommandAsync(Commands.Volume.Down);
    public Task SetVolumeAsync(int level) => _client.SendCommandAsync(Commands.Volume.Set(level));

    // ── Mute ─────────────────────────────────────────────────────────────────

    public Task MuteOnAsync() => _client.SendCommandAsync(Commands.Mute.On);
    public Task MuteOffAsync() => _client.SendCommandAsync(Commands.Mute.Off);

    // ── Input ────────────────────────────────────────────────────────────────

    public Task SetInputAsync(string input) => _client.SendCommandAsync(Commands.Input.Select(input));

    // ── Sound mode ───────────────────────────────────────────────────────────

    public Task SetSoundModeAsync(string suffix) => _client.SendCommandAsync("MS" + suffix);

    // ── Zone 2 ───────────────────────────────────────────────────────────────

    public Task Zone2SetInputAsync(string siKey) => _client.SendCommandAsync(Commands.Zone2.SetInput(siKey));
    public Task Zone2OnAsync() => _client.SendCommandAsync(Commands.Zone2.On);
    public Task Zone2OffAsync() => _client.SendCommandAsync(Commands.Zone2.Off);
    public Task Zone2VolumeUpAsync() => _client.SendCommandAsync(Commands.Zone2.Up);
    public Task Zone2VolumeDownAsync() => _client.SendCommandAsync(Commands.Zone2.Down);
    public Task Zone2SetVolumeAsync(int lvl) => _client.SendCommandAsync(Commands.Zone2.Set(lvl));

    // ── State ────────────────────────────────────────────────────────────────

    public async Task<ReceiverState> GetStateAsync()
    {
        try
        {
            var xml = await _client.GetStateXmlAsync();
            return ParseState(xml);
        }
        catch
        {
            return ReceiverState.Empty;
        }
    }

    private static ReceiverState ParseState(string xml)
    {
        var doc = XDocument.Parse(xml);
        var root = doc.Root!;

        return new ReceiverState
        {
            PowerOn = GetValue(root, "Power").Equals("ON", StringComparison.OrdinalIgnoreCase),
            Volume = double.TryParse(GetValue(root, "MasterVolume"), out var v) ? v : 0,
            Muted = GetValue(root, "Mute").Equals("on", StringComparison.OrdinalIgnoreCase),
            Input = GetValue(root, "InputFuncSelect"),
            SoundMode = GetValue(root, "selectSurround"),
            Zone2On = GetValue(root, "Zone2Power").Equals("ON", StringComparison.OrdinalIgnoreCase),
        };
    }

    private static string GetValue(XElement root, string elementName) =>
        root.Element(elementName)?.Element("value")?.Value ?? "";
}
