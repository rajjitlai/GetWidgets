namespace Vetra.Models;

public sealed class WidgetSettings
{
    public double Left { get; set; } = 80;
    public double Top { get; set; } = 80;
    public double Width { get; set; } = 420;
    public double Height { get; set; } = 154;
    public double Opacity { get; set; } = 1;
    public WidgetPlacementMode PlacementMode { get; set; } = WidgetPlacementMode.Overlay;
    public bool IsClickThrough { get; set; }
}

