# Vetra

Vetra is a Windows 11-focused desktop widget app for personal use and open-source development.

The first target is a separate, glassy Now Playing widget that reads Windows media sessions from apps and browsers such as Spotify, YouTube in Edge/Chrome, and other players that publish metadata to Windows media controls.

## Direction

- Separate floating widgets, each draggable and resizable.
- Per-widget placement mode: overlay or desktop-attached.
- Minimal glass UI with opacity/effects controls.
- Quiet background operation through the system tray.
- Media first, then system usage, then a broader widget framework.

## Requirements

- Windows 11.
- .NET 8 SDK or newer with Windows Desktop workload support.

## Build

```powershell
dotnet build .\src\Vetra\Vetra.csproj
dotnet run --project .\src\Vetra\Vetra.csproj
```

## Platform Notes

Media detection uses Windows `GlobalSystemMediaTransportControlsSessionManager`. This is the correct integration point for Spotify, YouTube in modern browsers, and other apps that participate in Windows media controls. Apps that do not expose metadata to Windows cannot be reliably detected without special integrations.

## License

MIT License. See [LICENSE](LICENSE).
