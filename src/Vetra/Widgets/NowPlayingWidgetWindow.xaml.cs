using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Vetra.Models;
using Vetra.Native;
using Vetra.Services;

namespace Vetra.Widgets;

public partial class NowPlayingWidgetWindow : Window
{
    private readonly MediaSessionService _mediaSessionService;
    private readonly SettingsStoreService _settingsStore;
    private bool _isRestoringSettings;
    private WidgetPlacementMode _placementMode = WidgetPlacementMode.Overlay;
    private bool _isClickThrough;

    public WidgetPlacementMode PlacementMode
    {
        get => _placementMode;
        set
        {
            _placementMode = value;
            ApplyPlacementMode();
            SaveWidgetSettings();
        }
    }

    public double WidgetOpacity
    {
        get => Frame?.Opacity ?? 1;
        set
        {
            if (Frame is not null)
            {
                Frame.Opacity = value;
                SaveWidgetSettings();
            }
        }
    }

    public bool IsClickThrough
    {
        get => _isClickThrough;
        set
        {
            _isClickThrough = value;
            ApplyClickThrough();
            SaveWidgetSettings();
        }
    }

    public NowPlayingWidgetWindow(MediaSessionService mediaSessionService, SettingsStoreService settingsStore)
    {
        InitializeComponent();
        _mediaSessionService = mediaSessionService;
        _settingsStore = settingsStore;
        _mediaSessionService.TrackChanged += MediaSessionService_TrackChanged;

        RestoreWidgetSettings();
        LocationChanged += (_, _) => SaveWidgetSettings();
        SizeChanged += (_, _) => SaveWidgetSettings();

        Loaded += (_, _) =>
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source?.AddHook(WindowMessageHook);

            WindowEffects.ApplyRoundedCorners(this);
            ApplyPlacementMode();
            ApplyClickThrough();
            RenderTrack(_mediaSessionService.CurrentTrack);
        };
    }

    private void MediaSessionService_TrackChanged(object? sender, MediaTrack track)
    {
        Dispatcher.Invoke(() => RenderTrack(track));
    }

    private void RenderTrack(MediaTrack track)
    {
        TitleText.Text = track.Title;
        ArtistText.Text = track.Artist;
        SourceText.Text = FriendlySourceName(track.SourceApp);

        ThumbnailImage.Source = track.Thumbnail;
        NoArtText.Visibility = track.Thumbnail is null ? Visibility.Visible : Visibility.Collapsed;

        PreviousButton.IsEnabled = track.CanGoPrevious;
        PlayPauseButton.IsEnabled = track.CanPlayPause;
        NextButton.IsEnabled = track.CanGoNext;
        PlayIcon.Visibility = track.IsPlaying ? Visibility.Collapsed : Visibility.Visible;
        PauseIcon.Visibility = track.IsPlaying ? Visibility.Visible : Visibility.Collapsed;
    }

    private static string FriendlySourceName(string sourceApp)
    {
        if (string.IsNullOrWhiteSpace(sourceApp))
        {
            return "Unknown source";
        }

        if (sourceApp.Contains("spotify", StringComparison.OrdinalIgnoreCase))
        {
            return "Spotify";
        }

        if (sourceApp.Contains("chrome", StringComparison.OrdinalIgnoreCase))
        {
            return "Chrome";
        }

        if (sourceApp.Contains("edge", StringComparison.OrdinalIgnoreCase))
        {
            return "Edge";
        }

        return sourceApp;
    }

    private void Frame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private async void PreviousButton_Click(object sender, RoutedEventArgs e)
    {
        await _mediaSessionService.GoPreviousAsync();
    }

    private async void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {
        await _mediaSessionService.TogglePlayPauseAsync();
    }

    private async void NextButton_Click(object sender, RoutedEventArgs e)
    {
        await _mediaSessionService.GoNextAsync();
    }

    private void ApplyPlacementMode()
    {
        var handle = new WindowInteropHelper(this).Handle;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        if (_placementMode == WidgetPlacementMode.Desktop)
        {
            Topmost = false;
            DesktopWindowHost.PlaceAboveDesktopIcons(handle);
            return;
        }

        DesktopWindowHost.RestoreOverlay(handle);
        Topmost = true;
    }

    private void ApplyClickThrough()
    {
        var handle = new WindowInteropHelper(this).Handle;
        if (handle != IntPtr.Zero)
        {
            WindowInteraction.SetClickThrough(handle, _isClickThrough);
        }
    }

    private void RestoreWidgetSettings()
    {
        _isRestoringSettings = true;

        var settings = _settingsStore.Current.NowPlaying;
        Left = settings.Left;
        Top = settings.Top;
        Width = settings.Width;
        Height = settings.Height;
        _placementMode = settings.PlacementMode;
        _isClickThrough = settings.IsClickThrough;

        Loaded += (_, _) => WidgetOpacity = settings.Opacity;

        _isRestoringSettings = false;
    }

    private void SaveWidgetSettings()
    {
        if (_isRestoringSettings || !IsLoaded)
        {
            return;
        }

        var settings = _settingsStore.Current.NowPlaying;
        settings.Left = Left;
        settings.Top = Top;
        settings.Width = Width;
        settings.Height = Height;
        settings.Opacity = WidgetOpacity;
        settings.PlacementMode = _placementMode;
        settings.IsClickThrough = _isClickThrough;

        _settingsStore.Save();
    }

    private IntPtr WindowMessageHook(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int wmNchittest = 0x0084;
        const int htLeft = 10;
        const int htRight = 11;
        const int htTop = 12;
        const int htTopLeft = 13;
        const int htTopRight = 14;
        const int htBottom = 15;
        const int htBottomLeft = 16;
        const int htBottomRight = 17;

        if (message != wmNchittest)
        {
            return IntPtr.Zero;
        }

        var screenPoint = new System.Windows.Point(
            unchecked((short)(long)lParam),
            unchecked((short)((long)lParam >> 16)));
        var point = PointFromScreen(screenPoint);
        const double resizeBorder = 8;

        var onLeft = point.X <= resizeBorder;
        var onRight = point.X >= ActualWidth - resizeBorder;
        var onTop = point.Y <= resizeBorder;
        var onBottom = point.Y >= ActualHeight - resizeBorder;

        handled = true;

        if (onTop && onLeft) return new IntPtr(htTopLeft);
        if (onTop && onRight) return new IntPtr(htTopRight);
        if (onBottom && onLeft) return new IntPtr(htBottomLeft);
        if (onBottom && onRight) return new IntPtr(htBottomRight);
        if (onLeft) return new IntPtr(htLeft);
        if (onRight) return new IntPtr(htRight);
        if (onTop) return new IntPtr(htTop);
        if (onBottom) return new IntPtr(htBottom);

        handled = false;
        return IntPtr.Zero;
    }
}
