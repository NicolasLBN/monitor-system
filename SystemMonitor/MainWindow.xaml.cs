using System.Windows;
using SystemMonitor.ViewModels;

namespace SystemMonitor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the MainWindow class
    /// </summary>
    /// <param name="viewModel">The ViewModel for this window</param>
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        
        // Format CPU gauge to show 2 decimal places
        CpuGauge.LabelFormatter = value => value.ToString("F2");
    }

    /// <summary>
    /// Called when the window is closed
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        // Dispose the ViewModel
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.Dispose();
        }
        
        base.OnClosed(e);
    }
}
