namespace DenonRemote.Services.Interfaces;

public enum Theme { Modern, Subtle }

public interface IThemeService
{
    Theme Current { get; }
    string CssName { get; }
    event Action? ThemeChanged;
    void Set(Theme theme);
    void Toggle();
}
