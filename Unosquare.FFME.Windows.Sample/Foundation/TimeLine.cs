namespace Unosquare.FFME.Windows.Sample.Foundation;
using System;
using System.Collections.ObjectModel;

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
