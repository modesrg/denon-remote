using DenonRemote.Models;

namespace DenonRemote.Services.Interfaces;

public interface IDenonService
{
    // Power
    Task PowerOnAsync();
    Task PowerStandbyAsync();

    // Volume
    Task VolumeUpAsync();
    Task VolumeDownAsync();
    Task SetVolumeAsync(int level);

    // Mute
    Task MuteOnAsync();
    Task MuteOffAsync();

    // Input
    Task SetInputAsync(string input);

    // Sound mode
    Task SetSoundModeAsync(string suffix);

    // Zone 2
    Task Zone2OnAsync();
    Task Zone2OffAsync();
    Task Zone2VolumeUpAsync();
    Task Zone2VolumeDownAsync();
    Task Zone2SetVolumeAsync(int level);
    Task Zone2SetInputAsync(string siKey);

    // State
    Task<ReceiverState> GetStateAsync();
}
