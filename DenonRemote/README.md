# Denon Remote

A local-network web remote for **Denon AVR receivers** that support the Telnet control protocol, built with .NET 9 Blazor Server. The receiver model name is fetched from the device on connect and displayed in the UI automatically.

## Features

- **Power** — on / standby
- **Volume** — slider, nudge buttons (+ / −), mute bar
- **Input selection** — with custom names pulled from the receiver
- **Sound modes** — Stereo / Surround family tabs with sub-mode grid and per-mode descriptions; Auto mode
- **Zone 2** — on/off toggle with independent volume control
- **Macros / Actions** — configurable one-tap command sequences (defined in `appsettings.json`)
- **Settings page** — change receiver IP without editing config files; reconnects automatically
- **Theme toggle** — Modern (lime/teal on dark) and Subtle (monochrome) themes, persisted in `localStorage`
- **Real-time state** — push updates via Telnet; no polling

## Stack

- .NET 9 Blazor Server (Interactive Server rendering)
- Custom CSS — no UI framework
- Telnet (port 23) for real-time state + commands
- HTTP for model name fetch on connect

## Getting Started

### Prerequisites

- .NET 9 SDK
- Denon AVR on the same local network with a static IP

### Run

```bash
dotnet run
```

Then open `http://localhost:5000` in a browser.

### Configure the receiver IP

Either edit `appsettings.json` before first run:

```json
"Receiver": {
  "BaseUrl": "http://192.168.1.x",
  "TelnetPort": 23
}
```

Or use the in-app **Settings** page (⚙ in the header) to change the IP at runtime — the app reconnects immediately without a restart. Settings are persisted to `receiversettings.json` in the app directory (takes precedence over `appsettings.json`).

> `receiversettings.json` contains your local network IP — keep it out of source control.

## Macros

Macros are sequences of commands defined in `appsettings.json`. Each step is either a command string or a millisecond delay:

```json
"Macros": [
  {
    "Name": "Movie Night",
    "Icon": "🎬",
    "Steps": [
      { "Command": "SIHDMI1" },
      { "Delay": 300 },
      { "Command": "MSDOLBY DIGITAL" },
      { "Delay": 300 },
      { "Command": "MV45" }
    ]
  }
]
```

Tapping a running macro cancels it. No rebuild needed to add or change macros — just edit the file and restart.

## Project Structure

```
Clients/        ITelnetClient, TelnetClient, IDenonClient, DenonClient
Components/     Blazor components (PowerButton, VolumeControl, InputSelector, …)
Constants/      Commands — all Denon command strings and input/sound mode metadata
Models/         ReceiverState, ReceiverSettings, MacroDefinition
Pages/          Index.razor (main remote), Settings.razor
Services/       TelnetService, DenonService, MacroService, SettingsService, ThemeService
wwwroot/        app.css, theme.js
```

## Architecture Notes

- `TelnetService` is a `BackgroundService` that maintains a persistent Telnet connection, parses push notifications from the receiver, and broadcasts state changes via `IReceiverStateService`.
- All Blazor components subscribe to `IReceiverStateService.StateChanged` and re-render on updates — they never call the receiver directly.
- `SettingsService` owns the receiver address. Both `TelnetClient` and `DenonClient` read from it per connection/request, so an IP change takes effect on the next reconnect without restarting the process.
- The two UI themes are implemented entirely via CSS custom property overrides on `[data-theme="subtle"]` — no component changes needed when switching.
