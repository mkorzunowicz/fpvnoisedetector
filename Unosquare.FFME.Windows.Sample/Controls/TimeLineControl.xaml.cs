namespace Unosquare.FFME.Windows.Sample.Controls;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Unosquare.FFME.Windows.Sample.Converters;
using Unosquare.FFME.Windows.Sample.Foundation;

/// <summary>
/// Interaction logic for TimeLine.xaml.
/// </summary>
public partial class TimeLineControl : UserControl
{
    #region TimeLineConfirmed
    /// <summary>
    /// Command to fire on timeline manipulation end
    /// </summary>
    public static readonly DependencyProperty TimeLineConfirmedProperty =
        DependencyProperty.Register(
            "TimeLineConfirmed",
            typeof(ICommand),
            typeof(TimeLineControl),
            new UIPropertyMetadata(null));

    /// <summary>
    /// The TimeLineConfirmed command
    /// </summary>
    public ICommand TimeLineConfirmed
    {
        get { return (ICommand)GetValue(TimeLineConfirmedProperty); }
        set { SetValue(TimeLineConfirmedProperty, value); }
    }
    #endregion

    #region MergeEndFile
    /// <summary>
    /// Adds a file this timeline should merge to
    /// </summary>
    public static readonly DependencyProperty MergeEndFileProperty =
        DependencyProperty.Register(
            "MergeEndFile",
            typeof(ICommand),
            typeof(TimeLineControl),
            new UIPropertyMetadata(null));
    /// <summary>
    /// The AddBeginFile command
    /// </summary>
    public ICommand MergeEndFile
    {
        get { return (ICommand)GetValue(MergeEndFileProperty); }
        set { SetValue(MergeEndFileProperty, value); }
    }
    #endregion
    #region MergeBeginFile
    /// <summary>
    /// Adds a file this timeline should merge to
    /// </summary>
    public static readonly DependencyProperty MergeBeginFileProperty =
        DependencyProperty.Register(
            "MergeBeginFile",
            typeof(ICommand),
            typeof(TimeLineControl),
            new UIPropertyMetadata(null));
    /// <summary>
    /// The MergeBeginFile command
    /// </summary>
    public ICommand MergeBeginFile
    {
        get { return (ICommand)GetValue(MergeBeginFileProperty); }
        set { SetValue(MergeBeginFileProperty, value); }
    }
    #endregion

    #region ProgressPegVisibility
    /// <summary>
    /// TimeLinesSourceProperty regsitered on the Control.
    /// </summary>
    public static readonly DependencyProperty ProgressPegVisibilityProperty = DependencyProperty.Register("ProgressPegVisibility", typeof(Visibility), typeof(TimeLineControl));
    /// <summary>
    /// Position of the currently scrubbed timeline
    /// </summary>
    public Visibility ProgressPegVisibility
    {
        get => (Visibility)GetValue(ProgressPegVisibilityProperty);
        set => SetValue(ProgressPegVisibilityProperty, value);
    }
    #endregion
    #region ProgressPosition
    /// <summary>
    /// TimeLinesSourceProperty regsitered on the Control.
    /// </summary>
    public static readonly DependencyProperty ProgressPositionProperty = DependencyProperty.Register("ProgressPosition", typeof(TimeSpan?), typeof(TimeLineControl));
    /// <summary>
    /// Position of the currently scrubbed timeline
    /// </summary>
    public TimeSpan? ProgressPosition
    {
        get => (TimeSpan?)GetValue(ProgressPositionProperty);
        set
        {
            SetValue(ProgressPositionProperty, value);
            if (value == TimeSpan.Zero) 
                ProgressPegVisibility = Visibility.Collapsed;
            else 
                ProgressPegVisibility = Visibility.Visible;
        }
    }
    #endregion

    #region Position
    /// <summary>
    /// TimeLinesSourceProperty regsitered on the Control.
    /// </summary>
    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(TimeSpan?), typeof(TimeLineControl));
    /// <summary>
    /// Position of the currently scrubbed timeline
    /// </summary>
    public TimeSpan? Position
    {
        get => (TimeSpan?)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }
    #endregion
    #region TimeLineSource
    /// <summary>
    /// TimeLinesSourceProperty regsitered on the Control.
    /// </summary>
    public static readonly DependencyProperty TimeLineSourceProperty = DependencyProperty.Register("TimeLineSource", typeof(TimeLine), typeof(TimeLineControl));

    /// <summary>
    /// TimeLines to display.
    /// </summary>
    public TimeLine TimeLineSource
    {
        get => (TimeLine)GetValue(TimeLineSourceProperty);
        set => SetValue(TimeLineSourceProperty, value);
    }
    #endregion
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeLineControl"/> class.
    /// </summary>
    public TimeLineControl()
    {
        InitializeComponent();
    }


    private Double prevX = 0;
    private System.Windows.Shapes.Rectangle movedSliderPeg;
    private bool shouldDelete = false;
    private bool isLeftPeg = false;
    private TimeSpan lastStartTime;
    private TimeSpan lastEndTime;
    private TimeLineEvent lastEvent;

    private void TimeLineEnd_MouseLeftButtonDown(object sender, MouseEventArgs e)
    {
        isLeftPeg = false;
        ManipulationBegins(sender);
    }

    private void TimeLineStart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        isLeftPeg = true;
        ManipulationBegins(sender);
    }
    private void ManipulationBegins(object sender)
    {
        var ctrl = sender as System.Windows.Shapes.Rectangle;
        if (ctrl == null)
            return;
        if (movedSliderPeg == null)
        {
            Application.Current.MainWindow.MouseMove += MainWindow_MouseMove;
            Application.Current.MainWindow.MouseUp += ManipulationEnds;
        }
        movedSliderPeg = ctrl;
        lastEvent = ctrl.DataContext as TimeLineEvent;
        prevX = Mouse.GetPosition(null).X;
    }

    private void MainWindow_MouseMove(object sender, MouseEventArgs e)
    {
        if (movedSliderPeg == null || Mouse.LeftButton != MouseButtonState.Pressed) return;

        try
        {
            Double mouseX = Mouse.GetPosition(null).X;
            var eventGrid = movedSliderPeg.Parent as Grid;
            var deltaX = mouseX - prevX;
            var containerWidth = this.TimeLine.ActualWidth; // not sure why the this.Margin isnt 5 but 15?
            //var containerWidth = this.TimeLine.ActualWidth - 10; // not sure why the this.Margin isnt 5 but 15?
            shouldDelete = false;

            if (isLeftPeg)
            {
                var earlierEvents = TimeLineSource.Events.Where(ev => ev.End <= lastEvent.Start);
                var leftTimeBoundry = TimeSpan.Zero;
                if (earlierEvents.Any())
                    leftTimeBoundry = earlierEvents.Max(ev => ev.End);
                var startTime = TimeSpanToThicknessConverter.ConvertWidthToTimeSpan(containerWidth, eventGrid.Margin.Left + deltaX, TimeLineSource.Duration);
                var rightTimeBoundry = lastEvent.End;

                var widthWouldBe = eventGrid.Width - deltaX;
                if (startTime <= leftTimeBoundry) // hit the end of a neighbouring event OR the beginning of the timeline
                {
                    startTime = leftTimeBoundry;
                    var pos = TimeSpanToThicknessConverter.ConvertTimeSpanToWidth(containerWidth, startTime, TimeLineSource.Duration);
                    eventGrid.Margin = new Thickness(pos, 0, 0, 0);
                    var width = TimeSpanToThicknessConverter.ConvertTimeSpanToWidth(containerWidth, rightTimeBoundry - startTime, TimeLineSource.Duration);

                    eventGrid.Width = width;
                    movedSliderPeg.Fill = new SolidColorBrush(Colors.Purple);
                }
                else if (startTime >= rightTimeBoundry || widthWouldBe <= eventGrid.MinWidth) // the right side of the event itself!
                {
                    startTime = rightTimeBoundry;
                    var pos = TimeSpanToThicknessConverter.ConvertTimeSpanToWidth(containerWidth, startTime, TimeLineSource.Duration);
                    eventGrid.Margin = new Thickness(pos - eventGrid.MinWidth, 0, 0, 0);
                    eventGrid.Width = eventGrid.MinWidth;
                    shouldDelete = true;
                    movedSliderPeg.Fill = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    eventGrid.Width -= deltaX;
                    eventGrid.Margin = new Thickness(eventGrid.Margin.Left + deltaX, 0, 0, 0);
                    movedSliderPeg.Fill = new SolidColorBrush(Colors.Blue);
                }
                lastStartTime = startTime;
                Position = startTime;
            }
            else // rightPeg
            {
                if (eventGrid.Width + deltaX < eventGrid.MinWidth) // hit the left side of the event itself
                {
                    eventGrid.Width = eventGrid.MinWidth;
                    shouldDelete = true;
                    movedSliderPeg.Fill = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    var laterEvents = TimeLineSource.Events.Where(ev => ev.Start >= lastEvent.End);
                    var rightTimeBoundry = TimeLineSource.Duration;
                    if (laterEvents.Any())
                        rightTimeBoundry = laterEvents.Min(ev => ev.Start);
                    movedSliderPeg.Fill = new SolidColorBrush(Colors.Blue);

                    var widthWouldBe = eventGrid.Width += deltaX; // this makes it shorter already!
                    var endTime = TimeSpanToThicknessConverter.ConvertWidthToTimeSpan(containerWidth, eventGrid.Margin.Left + widthWouldBe, TimeLineSource.Duration);
                    var startTime = TimeSpanToThicknessConverter.ConvertWidthToTimeSpan(containerWidth, eventGrid.Margin.Left, TimeLineSource.Duration);

                    if (endTime >= rightTimeBoundry) // hit the start of a neighbouring event OR the end of the timeline
                    {
                        endTime = rightTimeBoundry;
                        var pos = TimeSpanToThicknessConverter.ConvertTimeSpanToWidth(containerWidth, endTime - startTime, TimeLineSource.Duration);
                        eventGrid.Width = pos;
                        movedSliderPeg.Fill = new SolidColorBrush(Colors.Purple);
                    }
                    lastEndTime = endTime;
                    Position = endTime;
                }
            }

            prevX = mouseX;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                       Application.Current.MainWindow,
                       $"Media Failed: {ex.GetType()}\r\n{ex.Message}",
                       $"{nameof(MediaElement)} Error",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error,
                       MessageBoxResult.OK);
        }
    }

    private void ManipulationEnds(object sender, MouseButtonEventArgs e)
    {
        Application.Current.MainWindow.MouseMove -= MainWindow_MouseMove;
        Application.Current.MainWindow.MouseUp -= ManipulationEnds;
        movedSliderPeg.Fill = new SolidColorBrush(Colors.Blue);
        if (shouldDelete)
            TimeLineSource.Events.Remove(movedSliderPeg.DataContext as TimeLineEvent);
        movedSliderPeg = null;

        if (lastEndTime != default(TimeSpan))
        {
            lastEvent.End = lastEndTime;
            lastEndTime = default(TimeSpan);
        }
        if (lastStartTime != default(TimeSpan))
        {
            lastEvent.Start = lastStartTime;
            lastStartTime = default(TimeSpan);
        }
        TimeLineConfirmed?.Execute(this);
    }
    private void EventContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            TimeSpan timelineDuration = TimeLineSource.Duration;
            var containerWidth = this.ActualWidth; // not deducting anything cause we take global mouse position, or?

            Double mouseX = Mouse.GetPosition(this).X;
            var startTime = TimeSpanToThicknessConverter.ConvertWidthToTimeSpan(containerWidth, mouseX, timelineDuration);
            var endTime = TimeSpanToThicknessConverter.ConvertWidthToTimeSpan(containerWidth, mouseX + 10, timelineDuration);

            var conflictingEvents = TimeLineSource.Events.Where(ev => ev.Start <= startTime && ev.End >= startTime);
            if (conflictingEvents.Any()) return;

            TimeLineSource.Events.Add(new TimeLineEvent
            {
                Start = startTime,
                Duration = endTime - startTime
            });
        }
    }
}