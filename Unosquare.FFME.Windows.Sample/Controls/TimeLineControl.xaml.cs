namespace Unosquare.FFME.Windows.Sample.Controls;
using System.Windows;
using System.Windows.Controls;
using Unosquare.FFME.Windows.Sample.Foundation;

/// <summary>
/// Interaction logic for TimeLine.xaml.
/// </summary>
public partial class TimeLineControl : UserControl
{
    /// <summary>
    /// TimeLinesSourceProperty regsitered on the Control.
    /// </summary>
    public static readonly DependencyProperty TimeLineSourceProperty = DependencyProperty.Register("TimeLineSource", typeof(TimeLine), typeof(TimeLineControl));

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeLineControl"/> class.
    /// </summary>
    public TimeLineControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// TimeLines to display.
    /// </summary>
    public TimeLine TimeLineSource
    {
        get => (TimeLine)GetValue(TimeLineSourceProperty);
        set => SetValue(TimeLineSourceProperty, value);
    }
}