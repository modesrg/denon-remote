using DenonRemote.Clients;
using DenonRemote.Models;
using DenonRemote.Services.Interfaces;
using System.Xml.Linq;

namespace DenonRemote.Services;

public sealed class TelnetService : BackgroundService, IReceiverStateService
{
    private readonly ITelnetClient _telnet;
    private readonly IDenonClient _http;
    private readonly ILogger<TelnetService> _logger;
    private readonly ReceiverStateAccumulator _accumulator = new();
    private CancellationTokenSource _reconnectCts = new();

    public ReceiverState CurrentState { get; private set; } = ReceiverState.Empty;
    public event Action<ReceiverState>? StateChanged;

    public TelnetService(ITelnetClient telnet, IDenonClient http,
                         ILogger<TelnetService> logger, SettingsService settings)
    {
        _telnet = telnet;
        _http = http;
        _logger = logger;
        settings.SettingsChanged += OnSettingsChanged;
    }

    private void OnSettingsChanged()
    {
        _reconnectCts.Cancel();
        _reconnectCts = new CancellationTokenSource();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var reconnectCts = _reconnectCts;
            using var linked = CancellationTokenSource
                .CreateLinkedTokenSource(stoppingToken, reconnectCts.Token);
            var ct = linked.Token;
            var settingsTriggered = false;

            try
            {
                await _telnet.ConnectAsync(ct);

                _accumulator.IsConnected = true;
                if (string.IsNullOrEmpty(_accumulator.ModelName))
                    _accumulator.ModelName = await FetchModelNameAsync();
                CurrentState = _accumulator.ToSnapshot();
                StateChanged?.Invoke(CurrentState);
                _logger.LogInformation("Telnet connected to receiver.");

                await RequestInitialStateAsync(ct);

                await foreach (var line in _telnet.ReadLinesAsync(ct))
                {
                    if (TelnetStateParser.TryApply(line, _accumulator))
                    {
                        CurrentState = _accumulator.ToSnapshot();
                        StateChanged?.Invoke(CurrentState);
                    }
                }

                _logger.LogWarning("Telnet stream ended unexpectedly.");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (OperationCanceledException)
            {
                settingsTriggered = true;
                _logger.LogInformation("Settings changed — reconnecting.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Telnet connection lost, reconnecting in 5s.");
            }
            finally
            {
                _accumulator.IsConnected = false;
                CurrentState = _accumulator.ToSnapshot();
                StateChanged?.Invoke(CurrentState);
            }

            if (!settingsTriggered && !stoppingToken.IsCancellationRequested)
            {
                try { await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }
    }

    private async Task RequestInitialStateAsync(CancellationToken ct)
    {
        string[] queries = ["PW?", "MV?", "MU?", "SI?", "MS?", "Z2?", "SSFUN?", "SSINFA ?"];
        foreach (var query in queries)
        {
            await _telnet.SendAsync(query, ct);
            await Task.Delay(100, ct);
        }
    }

    private async Task<string> FetchModelNameAsync()
    {
        try
        {
            var xml = await _http.GetStateXmlAsync();
            var doc = XDocument.Parse(xml);
            return doc.Root?.Element("FriendlyName")?.Element("value")?.Value ?? "";
        }
        catch
        {
            return "";
        }
    }
}
