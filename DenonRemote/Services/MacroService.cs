using DenonRemote.Clients;
using DenonRemote.Models;

namespace DenonRemote.Services;

public sealed class MacroService
{
    private readonly IDenonClient _client;
    private readonly IReadOnlyList<MacroDefinition> _macros;
    private CancellationTokenSource? _cts;

    public MacroDefinition? RunningMacro { get; private set; }
    public event Action? StateChanged;

    public MacroService(IDenonClient client, IConfiguration config)
    {
        _client = client;
        _macros = config.GetSection("Macros").Get<List<MacroDefinition>>() ?? [];
    }

    public IReadOnlyList<MacroDefinition> Macros => _macros;

    public async Task ExecuteAsync(MacroDefinition macro)
    {
        // Cancel any macro already running
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        RunningMacro = macro;
        StateChanged?.Invoke();

        try
        {
            foreach (var step in macro.Steps)
            {
                ct.ThrowIfCancellationRequested();

                if (step.Command is { } cmd)
                    await _client.SendCommandAsync(cmd);
                else if (step.Delay is { } ms)
                    await Task.Delay(ms, ct);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            RunningMacro = null;
            StateChanged?.Invoke();
        }
    }

    public void Cancel()
    {
        _cts?.Cancel();
    }
}
