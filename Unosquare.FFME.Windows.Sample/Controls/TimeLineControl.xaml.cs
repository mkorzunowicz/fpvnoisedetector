namespace Unosquare.FFME.Windows.Sample.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Unosquare.FFME.Common;

/// <summary>
/// Interaction logic for TimeLine.xaml.
/// </summary>
public partial class TimeLineControl : UserControl
{
    /// <summary>
    /// TimeLinesSourceProperty regsitered on the Control.
    /// </summary>
    public static readonly DependencyProperty TimeLinesSourceProperty = DependencyProperty.Register("TimeLinesSource", typeof(ObservableCollection<TimeLine>), typeof(TimeLineControl));

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
    public ObservableCollection<TimeLine> TimeLinesSource
    {
        get => (ObservableCollection<TimeLine>)GetValue(TimeLinesSourceProperty);
        set => SetValue(TimeLinesSourceProperty, value);
    }
}

/// <summary>
/// Main view model for the Timeline.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private ObservableCollection<TimeLine> timeLines = new ObservableCollection<TimeLine>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
    {
        TimeLine first = new()
        {
            Duration = new TimeSpan(1, 0, 0)
        };
        first.Events.Add(new TimeLineEvent() { Start = new TimeSpan(0, 15, 0), Duration = new TimeSpan(0, 15, 0) });
        first.Events.Add(new TimeLineEvent() { Start = new TimeSpan(0, 40, 0), Duration = new TimeSpan(0, 10, 0) });
        TimeLines.Add(first);
    }

    /// <summary>
    /// TimeLines.
    /// </summary>
    public ObservableCollection<TimeLine> TimeLines
    {
        get
        {
            return timeLines;
        }
        set
        {
            SetProperty(ref timeLines, value);
        }
    }
}

/// <summary>
/// An even on a TimeLine.
/// </summary>
public class TimeLineEvent : ObservableObject
{
    private TimeSpan start;
    private TimeSpan duration;

    /// <summary>
    /// Beginning of the TimeLine.
    /// </summary>
    public TimeSpan Start
    {
        get
        {
            return start;
        }
        set
        {
            Set(() => Start, ref start, value);
        }
    }

    /// <summary>
    /// Duration of the TimeLine.
    /// </summary>
    public TimeSpan Duration
    {
        get
        {
            return duration;
        }
        set
        {
            Set(() => Duration, ref duration, value);
        }
    }
}

/// <summary>
/// TimeLine.
/// </summary>
public class TimeLine : ObservableObject
{
    private TimeSpan duration;
    private ObservableCollection<TimeLineEvent> events = new ObservableCollection<TimeLineEvent>();

    /// <summary>
    /// Duration of the timeline.
    /// </summary>
    public TimeSpan Duration
    {
        get
        {
            return duration;
        }
        set
        {
            Set(() => Duration, ref duration, value);
        }
    }

    /// <summary>
    /// Events on the Timeline.
    /// </summary>
    public ObservableCollection<TimeLineEvent> Events
    {
        get
        {
            return events;
        }
        set
        {
            Set(() => Events, ref events, value);
        }
    }
}