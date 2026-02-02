using System.Windows;

namespace WPF_APP;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Global exception handling
        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show($"Unhandled exception: {args.Exception.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };
    }
}
