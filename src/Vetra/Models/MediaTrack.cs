using System.Windows.Media.Imaging;

namespace Vetra.Models;

public sealed record MediaTrack(
    string Title,
    string Artist,
    string SourceApp,
    BitmapImage? Thumbnail,
    bool CanPlayPause,
    bool CanGoPrevious,
    bool CanGoNext,
    bool IsPlaying)
{
    public static MediaTrack Empty { get; } = new(
        "Nothing playing",
        "Start media in Spotify, YouTube, or another supported player",
        "No active session",
        null,
        false,
        false,
        false,
        false);
}
