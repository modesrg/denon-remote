using DenonRemote.Models;

namespace DenonRemote.Services.Interfaces;

public interface ISettingsService
{
    ReceiverSettings Current { get; }
    event Action? SettingsChanged;
    Task SaveAsync(ReceiverSettings settings);
}
