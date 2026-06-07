using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Vetra.Models;
using Windows.Media.Control;
using Windows.Storage.Streams;

namespace Vetra.Services;

public sealed class MediaSessionService : IDisposable
{
    private readonly System.Threading.Timer _pollTimer;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private GlobalSystemMediaTransportControlsSessionManager? _manager;
    private GlobalSystemMediaTransportControlsSession? _currentSession;

    public event EventHandler<MediaTrack>? TrackChanged;

    public MediaTrack CurrentTrack { get; private set; } = MediaTrack.Empty;

    public MediaSessionService()
    {
        _pollTimer = new System.Threading.Timer(async _ => await RefreshAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public async Task TogglePlayPauseAsync()
    {
        if (_currentSession?.GetPlaybackInfo().Controls.IsPlayPauseToggleEnabled == true)
        {
            await _currentSession.TryTogglePlayPauseAsync();
        }
    }

    public async Task GoPreviousAsync()
    {
        if (_currentSession?.GetPlaybackInfo().Controls.IsPreviousEnabled == true)
        {
            await _currentSession.TrySkipPreviousAsync();
        }
    }

    public async Task GoNextAsync()
    {
        if (_currentSession?.GetPlaybackInfo().Controls.IsNextEnabled == true)
        {
            await _currentSession.TrySkipNextAsync();
        }
    }

    private async Task RefreshAsync()
    {
        if (!await _refreshLock.WaitAsync(0))
        {
            return;
        }

        try
        {
            _manager ??= await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            _currentSession = _manager.GetCurrentSession();

            if (_currentSession is null)
            {
                Publish(MediaTrack.Empty);
                return;
            }

            var properties = await _currentSession.TryGetMediaPropertiesAsync();
            var playback = _currentSession.GetPlaybackInfo();
            var controls = playback.Controls;
            var thumbnail = await LoadThumbnailAsync(properties.Thumbnail);

            var next = new MediaTrack(
                string.IsNullOrWhiteSpace(properties.Title) ? "Untitled media" : properties.Title,
                string.IsNullOrWhiteSpace(properties.Artist) ? _currentSession.SourceAppUserModelId : properties.Artist,
                _currentSession.SourceAppUserModelId,
                thumbnail,
                controls.IsPlayPauseToggleEnabled,
                controls.IsPreviousEnabled,
                controls.IsNextEnabled,
                playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing);

            Publish(next);
        }
        catch
        {
            Publish(MediaTrack.Empty);
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private void Publish(MediaTrack track)
    {
        if (track.Title == CurrentTrack.Title &&
            track.Artist == CurrentTrack.Artist &&
            track.SourceApp == CurrentTrack.SourceApp &&
            track.CanPlayPause == CurrentTrack.CanPlayPause &&
            track.CanGoPrevious == CurrentTrack.CanGoPrevious &&
            track.CanGoNext == CurrentTrack.CanGoNext &&
            (track.Thumbnail is null) == (CurrentTrack.Thumbnail is null) &&
            track.IsPlaying == CurrentTrack.IsPlaying)
        {
            return;
        }

        CurrentTrack = track;
        TrackChanged?.Invoke(this, track);
    }

    private static async Task<BitmapImage?> LoadThumbnailAsync(IRandomAccessStreamReference? thumbnailReference)
    {
        if (thumbnailReference is null)
        {
            return null;
        }

        using var stream = await thumbnailReference.OpenReadAsync();
        var buffer = new Windows.Storage.Streams.Buffer((uint)stream.Size);
        await stream.ReadAsync(buffer, (uint)stream.Size, InputStreamOptions.None);

        using var reader = DataReader.FromBuffer(buffer);
        var bytes = new byte[buffer.Length];
        reader.ReadBytes(bytes);

        var image = new BitmapImage();
        using var memory = new MemoryStream(bytes);
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = memory;
        image.EndInit();
        image.Freeze();

        return image;
    }

    public void Dispose()
    {
        _pollTimer.Dispose();
        _refreshLock.Dispose();
    }
}
