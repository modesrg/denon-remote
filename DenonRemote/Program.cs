using System.Net;
using DenonRemote;
using DenonRemote.Clients;
using DenonRemote.Services;
using DenonRemote.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var appPort = builder.Configuration.GetValue<int>("App:Port", 5544);
builder.WebHost.ConfigureKestrel(options =>
    options.Listen(IPAddress.Any, appPort));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddHttpClient<IDenonClient, DenonClient>();

builder.Services.AddSingleton<IDenonService, DenonService>();
builder.Services.AddSingleton<IThemeService, ThemeService>();
builder.Services.AddSingleton<IMacroService, MacroService>();
builder.Services.AddSingleton<ITelnetClient, TelnetClient>();
builder.Services.AddSingleton<TelnetService>();
builder.Services.AddSingleton<IReceiverStateService>(sp => sp.GetRequiredService<TelnetService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<TelnetService>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
