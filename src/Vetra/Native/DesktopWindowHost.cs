using System;
using System.Runtime.InteropServices;

namespace Vetra.Native;

public static class DesktopWindowHost
{
    private static readonly IntPtr HwndBottom = new(1);
    private const uint SwpNoMove = 0x0002;
    private const uint SwpNoSize = 0x0001;
    private const uint SwpNoActivate = 0x0010;

    public static void PlaceAboveDesktopIcons(IntPtr widgetWindow)
    {
        SetParent(widgetWindow, IntPtr.Zero);
        SetWindowPos(widgetWindow, HwndBottom, 0, 0, 0, 0, SwpNoMove | SwpNoSize | SwpNoActivate);
    }

    public static void RestoreOverlay(IntPtr widgetWindow)
    {
        SetParent(widgetWindow, IntPtr.Zero);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetParent(IntPtr childWindow, IntPtr newParent);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr windowHandle,
        IntPtr insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);
}
