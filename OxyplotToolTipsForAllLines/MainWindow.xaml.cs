using OxyPlot;

namespace OxyplotToolTipsForAllLines;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        PlotView.ActualController.UnbindMouseDown(OxyMouseButton.Left);

        var snapTrack = new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
            controller.AddMouseManipulator(view, new AllLinesTrackerManipulator(view) { Snap = true, PointsOnly = false }, args));
        PlotView.ActualController.BindMouseDown(OxyMouseButton.Left, snapTrack);
    }
}