namespace Unosquare.FFME.Windows.Sample.Converters;
using System;
using System.Windows.Data;
using System.Windows;

/// <summary>
/// TimeSpan to Thickness converter.
/// </summary>
public class TimeSpanToThicknessConverter : IMultiValueConverter
{
    /// <summary>
    /// Converts the TimeSpan to a Thickness depending on the length of a timeline and max Total duration. I guess.
    /// </summary>
    /// <param name="values">Value to convert.</param>
    /// <param name="targetType">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Sie tickness.</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        //TimeSpan timelineDuration = (TimeSpan)values[0];
        //TimeSpan relativeTime = (TimeSpan)values[1];
        //double containerWidth = (double)values[2];
        //double factor = relativeTime.TotalSeconds / timelineDuration.TotalSeconds;
        //double rval = factor * containerWidth;
        var rval = ConvertTimeSpanToWidth((double)values[2], (TimeSpan)values[1], (TimeSpan)values[0]);

        if (targetType == typeof(Thickness))
        {
            return new Thickness(rval, 0, 0, 0);
        }
        else
        {
            return rval;
        }
    }
    /// <summary>
    /// Converts TimeSpan to Width, based on the container's Width, max timeline Duration and relative time from the timeline.
    /// </summary>
    /// <param name="containerWidth"></param>
    /// <param name="relativeTime"></param>
    /// <param name="timelineDuration"></param>
    /// <returns></returns>
    public static double ConvertTimeSpanToWidth(double containerWidth, TimeSpan relativeTime, TimeSpan timelineDuration)
    {
        //TimeSpan timelineDuration = (TimeSpan)values[0];
        //TimeSpan relativeTime = (TimeSpan)values[1];
        //double containerWidth = (double)values[2];
        double factor = relativeTime.TotalSeconds / timelineDuration.TotalSeconds;
        return factor * containerWidth;
    }
    /// <summary>
    ///  Convert the given width based on the container width and max timeline duration.
    /// </summary>
    /// <param name="containerWidth"></param>
    /// <param name="position"></param>
    /// <param name="timelineDuration"></param>
    /// <returns></returns>
    public static TimeSpan ConvertWidthToTimeSpan(double containerWidth, double position, TimeSpan timelineDuration)
    {
        double factor = position / containerWidth;
        var end = TimeSpan.FromMicroseconds(timelineDuration.TotalMicroseconds * factor);
        if (end > timelineDuration) end = timelineDuration;
        return end;
    }
    /// <summary>
    /// Converts back, but it's not implemented.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="targetTypes">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotImplementedException">It's no implemented.</exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
