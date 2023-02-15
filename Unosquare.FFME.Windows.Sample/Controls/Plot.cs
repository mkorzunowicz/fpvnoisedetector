namespace Unosquare.FFME.Windows.Sample.Controls;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Runtime.CompilerServices;

/// <summary>
/// Plot's ViewModel.
/// </summary>
public class PlotViewModel : INotifyPropertyChanged
{
    private double[] points = new double[100];

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotViewModel"/> class.
    /// </summary>
    public PlotViewModel()
    {
    }

    /// <summary>
    /// Property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Points of the plot.
    /// </summary>
    public double[] Points
    {
        get => points; set
        {
            points = value;
            OnPropertyChanged(nameof(Points));
        }
    }

    // public PlotViewModel() => GeneratePoints();

    // private async void GeneratePoints()
    // {
    //    Random rnd = new Random();
    //    do
    //    {
    //        double[] db = new double[100];
    //        for (int i = 0; i < Points.GetUpperBound(0); i++) db[i] = Points[i + 1];
    //        db[99] = rnd.Next(20, 200);
    //        Points = db;
    //        OnPropertyChanged(nameof(Points));
    //        await Task.Delay(50);
    //    }
    //    while (true);
    // }
    private void OnPropertyChanged([CallerMemberName] string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}

/// <summary>
/// Draws a simple LineChart Plot.
/// </summary>
public class Plot : FrameworkElement
{
    /// <summary>
    /// Data property to bind to.
    /// </summary>
    public static readonly DependencyProperty DataProperty = DependencyProperty.RegisterAttached("Data", typeof(double[]), typeof(Plot), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(DataChanged)));
    private VisualCollection children;
    private Canvas plotArea;
    private Polyline poly;
    private Line xAxis;
    private Line yAxis;
    private Point Origin = new Point(10, 10);

    /// <summary>
    /// Initializes a new instance of the <see cref="Plot"/> class.
    /// </summary>
    public Plot()
    {
        children = new VisualCollection(this);
        InitAxis();
        InitPlotArea();
    }

    /// <summary>
    /// X points.
    /// </summary>
    public int XPoints { get; set; } = 100;

    /// <summary>
    /// Y points.
    /// </summary>
    public int YPoints { get; set; } = 100;

    /// <summary>
    /// LineColor.
    /// </summary>
    public Brush LineColor { get; set; } = Brushes.Red;

    /// <summary>
    /// Children count.
    /// </summary>
    protected override int VisualChildrenCount => children.Count;
    private double[] Points { get; set; } = new double[0];

    /// <summary>
    /// Gets data.
    /// </summary>
    /// <param name="obj">data.</param>
    /// <returns>array of data.</returns>
    public static double[] GetData(DependencyObject obj) => (double[])obj.GetValue(DataProperty);

    /// <summary>
    /// Sets the data.
    /// </summary>
    /// <param name="obj">Dependency object.</param>
    /// <param name="data">data object.</param>
    public static void SetData(DependencyObject obj, double[] data) => obj.SetValue(DataProperty, data);

    /// <summary>
    /// Overrides the Arrange method of the Control.
    /// </summary>
    /// <param name="finalSize">the final size.</param>
    /// <returns>Size after.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        RearrangeAxis(finalSize);
        RearrangePlotArea(finalSize);
        return finalSize;
    }

    /// <summary>
    /// Visual Child.
    /// </summary>
    /// <param name="index">index of the child.</param>
    /// <returns>The Child Visual.</returns>
    protected override Visual GetVisualChild(int index) => children[index];
    private static void DataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var plot = d as Plot;
        if (plot == null) throw new InvalidOperationException("Error using Data property.");
        var data = e.NewValue as double[];
        plot.Points = data;
    }

    private void InitAxis()
    {
        xAxis = new Line() { Stroke = Brushes.LightBlue, StrokeThickness = 2 };
        yAxis = new Line() { Stroke = Brushes.LightBlue, StrokeThickness = 2 };
        children.Add(xAxis);
        children.Add(yAxis);
    }

    private void InitPlotArea()
    {
        plotArea = new Canvas()
        {
            LayoutTransform = new ScaleTransform() { ScaleY = -1 },
            RenderTransform = new TransformGroup() { Children = { new ScaleTransform(), new TranslateTransform() } }
        };
        children.Add(plotArea);
        poly = new Polyline() { Stroke = LineColor, StrokeThickness = 1 };
        plotArea.Children.Add(poly);
    }

    private void RearrangeAxis(Size size)
    {
        xAxis.X1 = 0;
        xAxis.X2 = size.Width;
        xAxis.Y1 = xAxis.Y2 = size.Height - Origin.Y;
        yAxis.X1 = yAxis.X2 = Origin.X;
        yAxis.Y1 = 0;
        yAxis.Y2 = size.Height;
        xAxis.Measure(size);
        xAxis.Arrange(new Rect(xAxis.DesiredSize));
        yAxis.Measure(size);
        yAxis.Arrange(new Rect(yAxis.DesiredSize));
    }

    private void RearrangePlotArea(Size size)
    {
        plotArea.Width = size.Width - Origin.X;
        plotArea.Height = size.Height - Origin.Y;
        var translate = (TranslateTransform)((TransformGroup)plotArea.RenderTransform).Children[1];
        translate.X = Origin.X;
        translate.Y = 0;
        plotArea.Measure(size);
        plotArea.Arrange(new Rect(plotArea.DesiredSize));
        var points = new PointCollection();
        double xStep = plotArea.Width / XPoints;
        double yFactor = plotArea.Height / YPoints;
        for (int i = 0; i < YPoints && i <= Points.GetUpperBound(0); i++)
            points.Add(new Point(i * xStep, Points[i]));
        poly.Points = points;
    }
}