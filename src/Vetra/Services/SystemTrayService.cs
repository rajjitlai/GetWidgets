using System;
using System.Windows.Forms;

namespace Vetra.Services;

public sealed class SystemTrayService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;

    public SystemTrayService(Action showNowPlaying, Action hideNowPlaying, Action openSettings, Action exit)
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Show Now Playing", null, (_, _) => showNowPlaying());
        menu.Items.Add("Hide Now Playing", null, (_, _) => hideNowPlaying());
        menu.Items.Add("Settings", null, (_, _) => openSettings());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => exit());

        _notifyIcon = new NotifyIcon
        {
            Text = "Vetra",
            Icon = System.Drawing.SystemIcons.Application,
            ContextMenuStrip = menu,
            Visible = true
        };

        _notifyIcon.DoubleClick += (_, _) => showNowPlaying();
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
