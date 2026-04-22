using DenonRemote.Services;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace DenonRemote.Clients;

public sealed class TelnetClient : ITelnetClient
{
    private readonly SettingsService _settings;
    private TcpClient? _tcp;

    public TelnetClient(SettingsService settings)
    {
        _settings = settings;
    }

    public async Task ConnectAsync(CancellationToken ct)
    {
        _tcp?.Dispose();
        _tcp = new TcpClient();
        var host = new Uri(_settings.Current.BaseUrl).Host;
        var port = _settings.Current.TelnetPort;
        await _tcp.ConnectAsync(host, port, ct);
    }

    public async Task SendAsync(string command, CancellationToken ct)
    {
        var bytes = Encoding.ASCII.GetBytes(command + "\r");
        await _tcp!.GetStream().WriteAsync(bytes, ct);
    }

    // Denon uses bare \r as the line terminator.
    public async IAsyncEnumerable<string> ReadLinesAsync(
        [EnumeratorCancellation] CancellationToken ct)
    {
        var stream = _tcp!.GetStream();
        var oneByte = new byte[1];
        var line = new List<byte>(64);

        while (!ct.IsCancellationRequested)
        {
            var read = await stream.ReadAsync(oneByte.AsMemory(), ct);
            if (read == 0) yield break;

            if (oneByte[0] == '\r')
            {
                if (line.Count > 0)
                {
                    yield return Encoding.ASCII.GetString(line.ToArray());
                    line.Clear();
                }
            }
            else
            {
                line.Add(oneByte[0]);
            }
        }
    }
}
