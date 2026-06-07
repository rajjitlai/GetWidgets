using System;
using System.Runtime.InteropServices;

namespace Vetra.Native;

public static class WindowInteraction
{
    private const int GwlExStyle = -20;
    private const int WsExTransparent = 0x00000020;
    private const int WsExLayered = 0x00080000;

    public static void SetClickThrough(IntPtr windowHandle, bool enabled)
    {
        var style = GetWindowLong(windowHandle, GwlExStyle);
        var nextStyle = enabled
            ? style | WsExTransparent | WsExLayered
            : style & ~WsExTransparent;

        SetWindowLong(windowHandle, GwlExStyle, nextStyle);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr windowHandle, int index);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr windowHandle, int index, int newLong);
}

