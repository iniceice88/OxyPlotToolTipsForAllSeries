using OxyPlot;
using OxyPlot.Series;

namespace OxyplotToolTipsForAllLines;

/// <summary>
/// Represents the view-model for the main window.
/// </summary>
public class MainViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel" /> class.
    /// </summary>
    public MainViewModel()
    {
        // Create the plot model
        var model = new PlotModel { Title = "Tool tips example" };

        int n = 10;

        for (int i = 1; i <= n; i++)
        {
            var s = new LineSeries { Title = "Series " + i };
            model.Series.Add(s);
            for (double x = 0; x < 2 * Math.PI; x += 0.1)
                s.Points.Add(new DataPoint(x, Math.Sin(x * i) / i + i));
        }

        Model = model;
    }

    /// <summary>
    /// Gets the plot model.
    /// </summary>
    public PlotModel Model { get; private set; }
}