namespace FPVNoiseDetector.Converters;
using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using System.Windows.Media;

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
        double rval;
        if (values[0] == DependencyProperty.UnsetValue)
            rval = 0;
        else
            rval = ConvertTimeSpanToWidth((double)values[2], (TimeSpan)values[1], (TimeSpan)values[0]);
        if (rval<=0) rval = 0;
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

/// <summary>
/// Zero TimeSpan to Visibility converter.
/// </summary>
public class ZeroTimeSpanToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts the TimeSpan to a Thickness depending on the length of a timeline and max Total duration. I guess.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="targetType">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Sie tickness.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var ts = value as TimeSpan?;
        if (ts == null) return Visibility.Collapsed;
        if (ts == TimeSpan.Zero)
            return Visibility.Collapsed;
        else 
            return Visibility.Visible;
    }
    /// <summary>
    /// Converts back, but it's not implemented.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="targetType">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotImplementedException">It's no implemented.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// TimeSpan to boolean for IsEnabled converter.
/// </summary>
public class ZeroTimeSpanToBooleanConverter : IValueConverter
{
    /// <summary>
    /// Converts the TimeSpan to a boolean for IsEnabled, to reflect encoding progress -> if encoding (nonzero) then disable control.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="targetType">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Sie tickness.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var ts = value as TimeSpan?;
        if (ts == null) return true;
        if (ts == TimeSpan.Zero)
            return true;
        else
            return false;
    }
    /// <summary>
    /// Converts back, but it's not implemented.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="targetType">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotImplementedException">It's no implemented.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// TimeSpan to boolean for IsEnabled converter.
/// </summary>
public class BoolToBrushConverter : IValueConverter
{
    /// <summary>
    /// Converts the TimeSpan to a boolean for IsEnabled, to reflect encoding progress -> if encoding (nonzero) then disable control.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="targetType">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Sie tickness.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = value as bool?;
        if (val == null || !val.Value) return new SolidColorBrush(Colors.Gray);
        return new SolidColorBrush(Colors.Green);
    }
    /// <summary>
    /// Converts back, but it's not implemented.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="targetType">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotImplementedException">It's no implemented.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// TimeSpan to boolean for IsEnabled converter.
/// </summary>
public class NullToBrushConverter : IValueConverter
{
    /// <summary>
    /// Converts the TimeSpan to a boolean for IsEnabled, to reflect encoding progress -> if encoding (nonzero) then disable control.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="targetType">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Sie tickness.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = value as string;
        if (val == null) return new SolidColorBrush(Colors.Gray);
        return new SolidColorBrush(Colors.Green);
    }
    /// <summary>
    /// Converts back, but it's not implemented.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="targetType">Types.</param>
    /// <param name="parameter">Param.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotImplementedException">It's no implemented.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}