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
        TimeLine.Loaded += TimeLineLoaded;
    }

    void TimeLineLoaded(object sender, RoutedEventArgs e)
    {
        var x = (ItemsControl)sender;

        var presenter = x.Template.FindName("TimeLineItem", x);
        //var stackPanel = x.ItemsPanel.FindName("EventContainer", presenter);
        //var presenter = (ItemsPresenter)this.Template.FindName("PART_Presenter", this);
        //var stackPanel = (StackPanel)this.ItemsPanel.FindName("PART_StackPanel", presenter);

        //var presenter = (ItemsPresenter)x.ItemsPanel.FindName("EventContainer", x);
        //var presenter = (ItemsPresenter)this.Template.FindName("EventContainer", this);
        //var stackPanel = (StackPanel)this.ItemsPanel.FindName("PART_StackPanel", presenter);
    }

    /// <summary>
    /// TimeLines to display.
    /// </summary>
    public TimeLine TimeLineSource
    {
        get => (TimeLine)GetValue(TimeLineSourceProperty);
        set => SetValue(TimeLineSourceProperty, value);
    }

    private Double prevX = 0;
    private System.Windows.Shapes.Rectangle movedSliderPeg;
    private bool shouldDelete = false;
    private bool isStart = false;
    private TimeSpan lastStartTime;
    private TimeSpan lastEndTime;
    private TimeLineEvent lastEvent;

    private void TimeLineEnd_MouseLeftButtonDown(object sender, MouseEventArgs e)
    {
        isStart = false;
        ManipulationBegins(sender);
    }

    private void TimeLineStart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        isStart = true;
        ManipulationBegins(sender);

    }
    private void ManipulationBegins(object sender)
    {
        var ctrl = sender as System.Windows.Shapes.Rectangle;
        if (ctrl == null)
            return;
        movedSliderPeg = ctrl;
        lastEvent = ctrl.DataContext as TimeLineEvent;
        prevX = Mouse.GetPosition(null).X;
        Application.Current.MainWindow.MouseMove += MainWindow_MouseMove;
        Application.Current.MainWindow.MouseUp += ManipulationEnds;
    }

    private void MainWindow_MouseMove(object sender, MouseEventArgs e)
    {
        if (movedSliderPeg == null || Mouse.LeftButton != MouseButtonState.Pressed) return;

        try
        {
            var tlEvent = lastEvent;

            Double mouseX = Mouse.GetPosition(null).X;
            var eventGrid = movedSliderPeg.Parent as Grid;
            var deltaX = mouseX - prevX;
            var containerWidth = this.ActualWidth - 10; // not sure why the margin isnt 5 but 15?

            var otherEvents = TimeLineSource.Events.Where(ev => ev != tlEvent);

            if (isStart)
            {
                var earlierEvents = TimeLineSource.Events.Where(ev => ev.End <= tlEvent.Start);
                var leftTimeBoundry = TimeSpan.Zero;
                if (earlierEvents.Any())
                    leftTimeBoundry = earlierEvents.Max(ev => ev.End);
                var startTime = TimeSpanToThicknessConverter.ConvertWidthToTimeSpan(containerWidth, eventGrid.Margin.Left + deltaX, TimeLineSource.Duration);
                var rightTimeBoundry = lastEvent.End;

                shouldDelete = false;
                var widthWouldBe = eventGrid.Width - deltaX;
                var marginWouldBe = eventGrid.Margin.Left + deltaX;
                if (startTime <= leftTimeBoundry || marginWouldBe <= 0)
                {
                    startTime = leftTimeBoundry;
                    var pos = TimeSpanToThicknessConverter.ConvertTimeSpanToWidth(containerWidth, startTime, TimeLineSource.Duration);
                    eventGrid.Margin = new Thickness(pos, 0, 0, 0);
                    var width = TimeSpanToThicknessConverter.ConvertTimeSpanToWidth(containerWidth, rightTimeBoundry - startTime, TimeLineSource.Duration);

                    eventGrid.Width = width;
                    movedSliderPeg.Fill = new SolidColorBrush(Colors.Purple);
                }
                else if (startTime >= rightTimeBoundry || widthWouldBe <= eventGrid.MinWidth)
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
                    eventGrid.Width -= deltaX;  // the width of the bar should be made longer, while the margin left and the other way around
                    eventGrid.Margin = new Thickness(eventGrid.Margin.Left + deltaX, 0, 0, 0);
                    movedSliderPeg.Fill = new SolidColorBrush(Colors.Blue);
                }
                lastStartTime = startTime;
            }
            else
            {
                if (eventGrid.Width + deltaX < eventGrid.MinWidth)
                {
                    eventGrid.Width = eventGrid.MinWidth;
                    shouldDelete = true;
                    movedSliderPeg.Fill = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    shouldDelete = false;

                    var laterEvents = TimeLineSource.Events.Where(ev => ev.Start >= tlEvent.End);
                    var rightTimeBoundry = TimeLineSource.Duration;
                    if (laterEvents.Any())
                        rightTimeBoundry = laterEvents.Min(ev => ev.Start);
                    var widthWouldBe = eventGrid.Width += deltaX;

                    var endTime = TimeSpanToThicknessConverter.ConvertWidthToTimeSpan(containerWidth, eventGrid.Margin.Left + widthWouldBe, TimeLineSource.Duration);
                    var startTime = TimeSpanToThicknessConverter.ConvertWidthToTimeSpan(containerWidth, eventGrid.Margin.Left, TimeLineSource.Duration);

                    if (endTime >= rightTimeBoundry)
                    {
                        endTime = rightTimeBoundry;
                        var pos = TimeSpanToThicknessConverter.ConvertTimeSpanToWidth(containerWidth, endTime - startTime, TimeLineSource.Duration);
                        eventGrid.Width = pos;
                        movedSliderPeg.Fill = new SolidColorBrush(Colors.Purple);
                    }
                    else
                    {
                        eventGrid.Width = widthWouldBe;
                        if (eventGrid.Width > containerWidth - eventGrid.Margin.Left)
                        {
                            // hit the right timeline bound
                            eventGrid.Width = containerWidth - eventGrid.Margin.Left;
                            movedSliderPeg.Fill = new SolidColorBrush(Colors.Purple);
                        }
                        else
                            movedSliderPeg.Fill = new SolidColorBrush(Colors.Blue);
                    }
                    lastEndTime = endTime;
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
            TimeLineSource.Events.Add(new TimeLineEvent
            {
                Start = startTime,
                Duration = endTime - startTime
            });
        }
    }
}