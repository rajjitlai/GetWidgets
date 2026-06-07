using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Vetra.Native;

public static class WindowEffects
{
    public static void ApplyRoundedCorners(Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        var preference = DwmWindowCornerPreference.Round;
        DwmSetWindowAttribute(handle, DwmWindowAttribute.WindowCornerPreference, ref preference, sizeof(int));
    }

    private enum DwmWindowAttribute
    {
        WindowCornerPreference = 33
    }

    private enum DwmWindowCornerPreference
    {
        Default = 0,
        DoNotRound = 1,
        Round = 2,
        RoundSmall = 3
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr windowHandle,
        DwmWindowAttribute attribute,
        ref DwmWindowCornerPreference attributeValue,
        int attributeSize);
}
