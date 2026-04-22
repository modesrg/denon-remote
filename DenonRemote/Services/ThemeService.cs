namespace DenonRemote.Services;

public enum Theme { Modern, Subtle }

public sealed class ThemeService
{
    public Theme Current { get; private set; } = Theme.Modern;
    public event Action? ThemeChanged;

    public void Set(Theme theme)
    {
        Current = theme;
        ThemeChanged?.Invoke();
    }

    public void Toggle() => Set(Current == Theme.Modern ? Theme.Subtle : Theme.Modern);

    public string CssName => Current == Theme.Modern ? "modern" : "subtle";
}
