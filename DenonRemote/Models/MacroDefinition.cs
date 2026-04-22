namespace DenonRemote.Models;

public record MacroStep
{
    public string? Command { get; init; }
    public int? Delay { get; init; }
}

public record MacroDefinition
{
    public string Name { get; init; } = "";
    public string Icon { get; init; } = "▶";
    public List<MacroStep> Steps { get; init; } = [];
}
