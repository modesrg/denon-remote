using DenonRemote.Models;

namespace DenonRemote.Services.Interfaces;

public interface IMacroService
{
    IReadOnlyList<MacroDefinition> Macros { get; }
    MacroDefinition? RunningMacro { get; }
    event Action? StateChanged;
    Task ExecuteAsync(MacroDefinition macro);
    void Cancel();
}
