// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackerManipulator.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides a plot manipulator for tracker functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using OxyPlot;
using OxyPlot.Series;

namespace OxyplotToolTipsForAllLines;

/// <summary>
/// Provides a plot manipulator for tracker functionality.
/// </summary>
public class AllLinesTrackerManipulator : MouseManipulator
{
    /// <summary>
    /// The current series.
    /// </summary>
    private Series currentSeries;

    /// <summary>
    /// Initializes a new instance of the <see cref="AllLinesTrackerManipulator" /> class.
    /// </summary>
    /// <param name="plotView">The plot view.</param>
    public AllLinesTrackerManipulator(IPlotView plotView)
        : base(plotView)
    {
        Snap = true;
        PointsOnly = false;
        LockToInitialSeries = true;
        FiresDistance = 20.0;
        CheckDistanceBetweenPoints = false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show tracker on points only (not interpolating).
    /// </summary>
    public bool PointsOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to snap to the nearest point.
    /// </summary>
    public bool Snap { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to lock the tracker to the initial series.
    /// </summary>
    /// <value><c>true</c> if the tracker should be locked; otherwise, <c>false</c>.</value>
    public bool LockToInitialSeries { get; set; }

    /// <summary>
    /// Gets or sets the distance from the series at which the tracker fires.
    /// </summary>
    public double FiresDistance { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to check distance when showing tracker between data points.
    /// </summary>
    /// <remarks>This parameter is ignored if <see cref="PointsOnly"/> is equal to <c>False</c>.</remarks>
    public bool CheckDistanceBetweenPoints { get; set; }

    /// <summary>
    /// Occurs when a manipulation is complete.
    /// </summary>
    /// <param name="e">The <see cref="OxyPlot.OxyMouseEventArgs" /> instance containing the event data.</param>
    public override void Completed(OxyMouseEventArgs e)
    {
        base.Completed(e);
        e.Handled = true;

        currentSeries = null;
        PlotView.HideTracker();
        if (PlotView.ActualModel != null)
        {
            PlotView.ActualModel.RaiseTrackerChanged(null);
        }
    }

    /// <summary>
    /// Occurs when the input device changes position during a manipulation.
    /// </summary>
    /// <param name="e">The <see cref="OxyPlot.OxyMouseEventArgs" /> instance containing the event data.</param>
    public override void Delta(OxyMouseEventArgs e)
    {
        base.Delta(e);
        e.Handled = true;

        if (currentSeries == null || !LockToInitialSeries)
        {
            currentSeries = PlotView.ActualModel?.GetSeriesFromPoint(e.Position, FiresDistance);
        }

        if (currentSeries == null)
        {
            if (!LockToInitialSeries)
            {
                PlotView.HideTracker();
            }

            return;
        }

        var actualModel = PlotView.ActualModel;
        if (actualModel == null)
        {
            return;
        }

        if (!actualModel.PlotArea.Contains(e.Position.X, e.Position.Y))
        {
            return;
        }

        var result = TrackerHelper.GetNearestHit(
            currentSeries, e.Position, Snap, PointsOnly, FiresDistance, CheckDistanceBetweenPoints);

        if (result == null)
        {
            return;
        }

        result.PlotModel = actualModel;
        var resWrapper = new AllLinesTrackerHitResult(result);
        var seriesTrackerHitResults = new List<TrackerHitResult>();
        foreach (var s in actualModel.Series.Where(x => x.IsVisible))
        {
            if (s == currentSeries)
            {
                seriesTrackerHitResults.Add(result);
                continue;
            }

            var thr = TrackerHelper.GetNearestHit(s, e.Position, Snap, PointsOnly, FiresDistance, CheckDistanceBetweenPoints);
            if (thr == null) continue;
            seriesTrackerHitResults.Add(thr);
        }

        resWrapper.SeriesTrackerHitResults = seriesTrackerHitResults.ToArray();
        PlotView.ShowTracker(resWrapper);
        actualModel.RaiseTrackerChanged(result);
    }

    /// <summary>
    /// Occurs when an input device begins a manipulation on the plot.
    /// </summary>
    /// <param name="e">The <see cref="OxyPlot.OxyMouseEventArgs" /> instance containing the event data.</param>
    public override void Started(OxyMouseEventArgs e)
    {
        base.Started(e);
        currentSeries = PlotView.ActualModel?.GetSeriesFromPoint(e.Position, FiresDistance);
        Delta(e);
    }
}

public class AllLinesTrackerHitResult : TrackerHitResult
{
    public TrackerHitResult[] SeriesTrackerHitResults { get; set; }

    public AllLinesTrackerHitResult(TrackerHitResult thr)
    {
        DataPoint = thr.DataPoint;
        Index = thr.Index;
        Item = thr.Item;
        PlotModel = thr.PlotModel;
        Position = thr.Position;
        Series = thr.Series;
        Text = thr.Text;
        LineExtents = thr.LineExtents;
    }
}