using System.Windows;
using Vetra.Models;
using Vetra.Widgets;

namespace Vetra;

public partial class SettingsWindow : Window
{
    private readonly NowPlayingWidgetWindow _nowPlayingWidget;

    public SettingsWindow(NowPlayingWidgetWindow nowPlayingWidget)
    {
        InitializeComponent();
        _nowPlayingWidget = nowPlayingWidget;
        ClickThroughCheckBox.IsChecked = _nowPlayingWidget.IsClickThrough;
        OpacitySlider.Value = _nowPlayingWidget.WidgetOpacity;
        PlacementComboBox.SelectedIndex = _nowPlayingWidget.PlacementMode == WidgetPlacementMode.Desktop ? 1 : 0;
    }

    private void ClickThroughCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (_nowPlayingWidget is not null)
        {
            _nowPlayingWidget.IsClickThrough = ClickThroughCheckBox.IsChecked == true;
        }
    }

    private void PlacementComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (_nowPlayingWidget is not null)
        {
            _nowPlayingWidget.PlacementMode = PlacementComboBox.SelectedIndex == 1
                ? WidgetPlacementMode.Desktop
                : WidgetPlacementMode.Overlay;
        }
    }

    private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_nowPlayingWidget is not null)
        {
            _nowPlayingWidget.WidgetOpacity = e.NewValue;
        }
    }
}
