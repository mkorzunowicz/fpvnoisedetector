namespace FPVNoiseDetector.Foundation;
using System;
using System.Collections.ObjectModel;

/// <summary>
/// TimeLine.
/// </summary>
public class TimeLine : ObservableObject
{
    private TimeSpan duration;
    private string endFile;
    private string beginFile;
    private ObservableCollection<TimeLineEvent> events = new ObservableCollection<TimeLineEvent>();


    /// <summary>
    /// File to merge the beginning of the timeline to.
    /// </summary>
    public string BeginFile
    {
        get
        {
            return beginFile;
        }
        set
        {
            Set(() => BeginFile, ref beginFile, value);
        }
    }
    /// <summary>
    /// File to merge the end of the timeline to.
    /// </summary>
    public string EndFile
    {
        get
        {
            return endFile;
        }
        set
        {
            Set(() => EndFile, ref endFile, value);
        }
    }
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
    private TimeSpan end;

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
            end = Start + Duration;
        }
    }

    /// <summary>
    /// End of the TimeLine.
    /// </summary>
    public TimeSpan End
    {
        get
        {
            return end;
        }
        set
        {
            Set(() => End, ref end, value);
            duration = End - Start;

        }
    }
}