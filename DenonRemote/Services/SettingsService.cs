using DenonRemote.Models;
using DenonRemote.Services.Interfaces;
using System.Text.Json;

namespace DenonRemote.Services;

public sealed class SettingsService : ISettingsService
{
    private readonly string _filePath;
    private readonly IConfiguration _config;

    public ReceiverSettings Current { get; private set; }
    public event Action? SettingsChanged;

    public SettingsService(IConfiguration config, IWebHostEnvironment env)
    {
        _config = config;
        _filePath = Path.Combine(env.ContentRootPath, "receiversettings.json");
        Current = Load();
    }

    private ReceiverSettings Load()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<ReceiverSettings>(json) ?? FromConfig();
            }
            catch { }
        }
        return FromConfig();
    }

    private ReceiverSettings FromConfig() => new()
    {
        BaseUrl = _config["Receiver:BaseUrl"] ?? "http://192.168.1.177",
        TelnetPort = _config.GetValue<int>("Receiver:TelnetPort", 23),
    };

    public async Task SaveAsync(ReceiverSettings settings)
    {
        var json = JsonSerializer.Serialize(settings,
            new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
        Current = settings;
        SettingsChanged?.Invoke();
    }
}
