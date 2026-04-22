using DenonRemote.Services;

namespace DenonRemote.Clients;

public class DenonClient : IDenonClient
{
    private readonly HttpClient _http;
    private readonly SettingsService _settings;

    public DenonClient(HttpClient http, SettingsService settings)
    {
        _http = http;
        _settings = settings;
    }

    private string Base => _settings.Current.BaseUrl.TrimEnd('/');

    public async Task SendCommandAsync(string command)
    {
        await _http.GetAsync(
            $"{Base}/goform/formiPhoneAppDirect.xml?{Uri.EscapeDataString(command)}");
    }

    public Task<string> GetStateXmlAsync() =>
        _http.GetStringAsync($"{Base}/goform/formMainZone_MainZoneXml.xml");
}
