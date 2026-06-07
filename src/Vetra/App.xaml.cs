using System.Windows;
using Vetra.Services;
using Vetra.Widgets;

namespace Vetra;

public partial class App : System.Windows.Application
{
    private SystemTrayService? _tray;
    private NowPlayingWidgetWindow? _nowPlayingWidget;
    private SettingsWindow? _settingsWindow;
    private MediaSessionService? _mediaSessionService;
    private SettingsStoreService? _settingsStore;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        _settingsStore = new SettingsStoreService();
        _mediaSessionService = new MediaSessionService();
        _nowPlayingWidget = new NowPlayingWidgetWindow(_mediaSessionService, _settingsStore);
        _nowPlayingWidget.Show();

        _tray = new SystemTrayService(
            showNowPlaying: () => ShowNowPlayingWidget(),
            hideNowPlaying: () => _nowPlayingWidget?.Hide(),
            openSettings: () => OpenSettings(),
            exit: Shutdown);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _tray?.Dispose();
        _mediaSessionService?.Dispose();
        base.OnExit(e);
    }

    private void ShowNowPlayingWidget()
    {
        if (_nowPlayingWidget is null)
        {
            return;
        }

        _nowPlayingWidget.Show();
        _nowPlayingWidget.Activate();
    }

    private void OpenSettings()
    {
        if (_nowPlayingWidget is null)
        {
            return;
        }

        _settingsWindow ??= new SettingsWindow(_nowPlayingWidget);
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;
        _settingsWindow.Show();
        _settingsWindow.Activate();
    }
}
