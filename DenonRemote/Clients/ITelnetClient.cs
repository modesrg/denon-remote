namespace DenonRemote.Clients;

public interface ITelnetClient
{
    Task ConnectAsync(CancellationToken ct);
    Task SendAsync(string command, CancellationToken ct);
    IAsyncEnumerable<string> ReadLinesAsync(CancellationToken ct);
}
