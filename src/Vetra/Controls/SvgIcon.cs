using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Vetra.Controls;

public sealed class SvgIcon : Viewbox
{
    private readonly Path _path = new() { Stretch = Stretch.Uniform };

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(string),
        typeof(SvgIcon),
        new PropertyMetadata(string.Empty, (_, args) => ((SvgIcon)_).LoadSvg((string)args.NewValue)));

    public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
        nameof(Fill),
        typeof(System.Windows.Media.Brush),
        typeof(SvgIcon),
        new PropertyMetadata(System.Windows.Media.Brushes.White, (_, args) => ((SvgIcon)_)._path.Fill = (System.Windows.Media.Brush)args.NewValue));

    public SvgIcon()
    {
        Width = 14;
        Height = 14;
        Child = _path;
        _path.Fill = Fill;
    }

    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public System.Windows.Media.Brush Fill
    {
        get => (System.Windows.Media.Brush)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    private void LoadSvg(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return;
        }

        var streamInfo = System.Windows.Application.GetResourceStream(new Uri(source, UriKind.Relative));
        if (streamInfo is null)
        {
            return;
        }

        using var stream = streamInfo.Stream;
        var document = XDocument.Load(stream);
        var geometries = document
            .Descendants()
            .Where(element => element.Name.LocalName == "path")
            .Select(element => element.Attribute("d")?.Value)
            .Where(data => !string.IsNullOrWhiteSpace(data))
            .Select(data => Geometry.Parse(data!))
            .ToArray();

        _path.Data = geometries.Length switch
        {
            0 => null,
            1 => geometries[0],
            _ => new GeometryGroup { Children = new GeometryCollection(geometries) }
        };
    }
}
